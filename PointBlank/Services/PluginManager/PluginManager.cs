﻿using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PointBlank.API;
using PointBlank.API.Server;
using PointBlank.API.Plugins;
using PointBlank.API.Services;
using PointBlank.API.DataManagment;
using SM = PointBlank.Framework.ServiceManager;

namespace PointBlank.Services.PluginManager
{
    internal class PluginManager : PointBlankService
    {
        #region Info
        public static readonly string ConfigurationPath = PointBlankServer.ConfigurationsPath + "/Plugins"; // Set the plugins configuration path
        public static readonly string TranslationPath = PointBlankServer.TranslationsPath + "/Plugins";
        #endregion

        #region Variables
        private static List<PluginWrapper> _plugins = new List<PluginWrapper>(); // List of plugins
        private static List<Assembly> _libraries = new List<Assembly>(); // List of libraries
        #endregion

        #region Properties
        public static PluginWrapper[] Plugins => _plugins.ToArray(); // Returns the plugins

        public static Assembly[] Libraries => _libraries.ToArray(); // Returns the libraries

        public UniversalData UniConfig { get; private set; } // The universal config data
        public JsonData JSONConfig { get; private set; } // The JSON config data

        public override int LaunchIndex => 3;
        #endregion

        #region Override Functions
        public override void Load()
        {
            if (!Directory.Exists(PointBlankServer.LibrariesPath))
                Directory.CreateDirectory(PointBlankServer.LibrariesPath); // Create libraries directory
            if (!Directory.Exists(PointBlankServer.PluginsPath))
                Directory.CreateDirectory(PointBlankServer.PluginsPath); // Create plugins directory
            if (!Directory.Exists(ConfigurationPath))
                Directory.CreateDirectory(ConfigurationPath); // Create plugins directory
            if (!Directory.Exists(TranslationPath))
                Directory.CreateDirectory(TranslationPath); // Create plugins directory

            // Setup the config
            UniConfig = new UniversalData(SM.ConfigurationPath + "\\PluginManager");
            JSONConfig = UniConfig.GetData(EDataType.JSON) as JsonData;
            LoadConfig();

            foreach (string library in Directory.GetFiles(PointBlankServer.LibrariesPath, "*.dll")) // Get all the dll files in libraries directory
                _libraries.Add(Assembly.Load(File.ReadAllBytes(library))); // Load and add the library
            foreach (string plugin in Directory.GetFiles(PointBlankServer.PluginsPath, "*.dll")) // Get all the dll files in plugins directory
            {
                try
                {
                    PluginWrapper wrapper = new PluginWrapper(plugin); // Create the plugin wrapper

                    _plugins.Add(wrapper); // Add the wrapper
                    if (!wrapper.Load() && !PluginConfiguration.ContinueOnError)
                        break;
                }
                catch (Exception ex)
                {
                    PointBlankLogging.LogError("Error initializing plugin!", ex);
                    if (!PluginConfiguration.ContinueOnError)
                        break;
                }
            }
            PointBlankPluginEvents.RunPluginsLoaded();
        }

        public override void Unload()
        {
            foreach (PluginWrapper wrapper in _plugins)
                wrapper.Unload(); // Unload the wrapper
            PointBlankPluginEvents.RunPluginsUnloaded();
            SaveConfig();
        }
        #endregion

        #region Public Functions
        public PluginWrapper GetWrapper(PointBlankPlugin plugin) => Plugins.First(a => a.PluginClass == plugin);
        #endregion

        #region Private Functions
        internal void LoadConfig()
        {
            if (UniConfig.CreatedNew)
            {
                PluginConfiguration.AutoUpdate = false;
                PluginConfiguration.ContinueOnError = true;
                PluginConfiguration.NotifyUpdates = true;
                PluginConfiguration.CheckUpdateTimeSeconds = 1800;

                JSONConfig.Document.Add("AutoUpdate", (PluginConfiguration.AutoUpdate ? "true" : "false"));
                JSONConfig.Document.Add("ContinueOnError", (PluginConfiguration.ContinueOnError ? "true" : "false"));
                JSONConfig.Document.Add("NotifyUpdates", (PluginConfiguration.NotifyUpdates ? "true" : "false"));
                JSONConfig.Document.Add("CheckUpdateTimeSeconds", PluginConfiguration.CheckUpdateTimeSeconds.ToString());
                UniConfig.Save();
            }
            else
            {
                JSONConfig.Verify(new Dictionary<string, Newtonsoft.Json.Linq.JToken>()
                {
                    { "AutoUpdate", "false" },
                    { "ContinueOnError", "true" },
                    { "NotifyUpdates", "true" },
                    { "CheckUpdateTimeSeconds", "1800" }
                });

                PluginConfiguration.AutoUpdate = ((string)JSONConfig.Document["AutoUpdate"] == "true");
                PluginConfiguration.ContinueOnError = ((string)JSONConfig.Document["ContinueOnError"] == "true");
                PluginConfiguration.NotifyUpdates = ((string)JSONConfig.Document["NotifyUpdates"] == "true");
                PluginConfiguration.CheckUpdateTimeSeconds = int.Parse((string)JSONConfig.Document["CheckUpdateTimeSeconds"]);
            }
        }

        internal void SaveConfig() => UniConfig.Save();
        #endregion
    }
}
