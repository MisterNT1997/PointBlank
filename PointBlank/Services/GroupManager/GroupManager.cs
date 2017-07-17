﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PointBlank.API.Groups;
using PointBlank.API.Server;
using PointBlank.API.Services;
using PointBlank.API.DataManagment;
using Newtonsoft.Json.Linq;
using UnityEngine;
using GM = PointBlank.API.Groups.GroupManager;

namespace PointBlank.Services.GroupManager
{
    [Service("GroupManager", true)]
    internal class GroupManager : Service
    {
        #region Info
        public static readonly string GroupPath = Server.ConfigurationsPath + "/Groups";
        #endregion

        #region Properties
        public UniversalData UniGroupConfig { get; private set; }

        public JsonData GroupConfig { get; private set; }
        #endregion

        #region Override Functions
        public override void Load()
        {
            // Setup config
            UniGroupConfig = new UniversalData(GroupPath);
            GroupConfig = UniGroupConfig.GetData(EDataType.JSON) as JsonData;

            if (!UniGroupConfig.CreatedNew)
                LoadGroups();
            else
                FirstGroups();
        }

        public override void Unload()
        {
            SaveGroups();
        }
        #endregion

        #region Private Functions
        internal void LoadGroups()
        {
            foreach (JProperty obj in GroupConfig.Document.Properties())
            {
                if (GM.Groups.Count(a => a.ID == obj.Name) > 0)
                    continue;

                if (!ColorUtility.TryParseHtmlString((string)obj.Value["Color"], out Color color))
                    color = Color.clear;

                Group g = new Group(obj.Name, (string)obj.Value["Name"], (bool)obj.Value["Default"], (int)obj.Value["Cooldown"], color);

                GM.AddGroup(g);
            }

            foreach (Group g in GM.Groups)
            {
                JObject obj = GroupConfig.Document[g.ID] as JObject;

                if (obj["Inherits"] is JArray)
                {
                    foreach (JToken token in (JArray)obj["Inherits"])
                    {
                        Group i = GM.Groups.FirstOrDefault(a => a.ID == (string)token);

                        if (i == null || g.Inherits.Contains(i) || g == i)
                            continue;
                        g.AddInherit(i);
                    }
                }
                else
                {
                    Group i = GM.Groups.FirstOrDefault(a => a.ID == (string)obj["Inherits"]);

                    if (i == null || g.Inherits.Contains(i) || g == i)
                        continue;
                    g.AddInherit(i);
                }
                if (obj["Permissions"] is JArray)
                {
                    foreach (JToken token in (JArray)obj["Permissions"])
                    {
                        if (g.Permissions.Contains((string)token))
                            continue;

                        g.AddPermission((string)token);
                    }
                }
                else
                {
                    if (g.Permissions.Contains((string)obj["Permissions"]))
                        continue;

                    g.AddPermission((string)obj["Permissions"]);
                }
                if (obj["Prefixes"] is JArray)
                {
                    foreach (JToken token in (JArray)obj["Prefixes"])
                    {
                        if (g.Prefixes.Contains((string)token))
                            continue;

                        g.AddPrefix((string)token);
                    }
                }
                else
                {
                    if (g.Prefixes.Contains((string)obj["Prefixes"]))
                        continue;

                    g.AddPrefix((string)obj["Prefixes"]);
                }
                if (obj["Suffixes"] is JArray)
                {
                    foreach (JToken token in (JArray)obj["Suffixes"])
                    {
                        if (g.Suffixes.Contains((string)token))
                            continue;

                        g.AddSuffix((string)token);
                    }
                }
                else
                {
                    if (g.Suffixes.Contains((string)obj["Suffixes"]))
                        continue;

                    g.AddSuffix((string)obj["Suffixes"]);
                }
            }
        }

        internal void FirstGroups()
        {
            // Create the groups
            Group guest = new Group("Guest", "Guest Group", true, -1, Color.clear);
            Group admin = new Group("Admin", "Admin Group", false, 0, Color.blue);

            // Configure guest group
            guest.AddPermission("unturned.commands.nonadmin.*");
            guest.AddPrefix("Guest");
            guest.AddSuffix("Guest");
            GM.AddGroup(guest);

            // Configure admin group
            admin.AddPermission("unturned.commands.admin.*");
            admin.AddPrefix("Admin");
            admin.AddSuffix("Admin");
            admin.AddInherit(guest);
            GM.AddGroup(admin);

            // Save the groups
            SaveGroups();
        }

        internal void SaveGroups()
        {
            foreach (Group g in GM.Groups)
            {
                if (GroupConfig.Document[g.ID] != null)
                {
                    JObject obj = GroupConfig.Document[g.ID] as JObject;

                    obj["Permissions"] = JToken.FromObject(g.Permissions);
                    obj["Prefixes"] = JToken.FromObject(g.Prefixes);
                    obj["Suffixes"] = JToken.FromObject(g.Suffixes);
                    obj["Inherits"] = JToken.FromObject(g.Inherits.Select(a => a.ID));
                    obj["Cooldown"] = g.Cooldown;
                    obj["Color"] = (g.Color == Color.clear ? "none" : "#" + ColorUtility.ToHtmlStringRGB(g.Color));
                }
                else
                {
                    JObject obj = new JObject
                    {
                        {"Name", g.Name},
                        {"Default", g.Default},
                        {"Permissions", JToken.FromObject(g.Permissions)},
                        {"Prefixes", JToken.FromObject(g.Prefixes)},
                        {"Suffixes", JToken.FromObject(g.Suffixes)},
                        {"Inherits", JToken.FromObject(g.Inherits.Select(a => a.ID))},
                        {"Cooldown", g.Cooldown},
                        {"Color", (g.Color == Color.clear ? "none" : "#" + ColorUtility.ToHtmlStringRGB(g.Color))}
                    };


                    GroupConfig.Document.Add(g.ID, obj);
                }
            }
            UniGroupConfig.Save();
        }
        #endregion
    }
}