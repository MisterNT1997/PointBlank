﻿using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PointBlank.API.Detour;
using PointBlank.API.Unturned.Server;
using PointBlank.API.Unturned.Structure;
using SDG.Unturned;

namespace PointBlank.Framework.Overrides
{
    internal static class _Structure
    {
        [Detour(typeof(Structure), "askDamage", BindingFlags.Public | BindingFlags.Instance)]
        public static void askDamage(this Structure stru, ushort amount)
        {
            // Run the events
            StructureEvents.RunDamageStructure(UnturnedServer.FindStructure(stru), amount);

            // Run the original function
            DetourManager.CallOriginal(typeof(Structure).GetMethod("askDamage", BindingFlags.Instance | BindingFlags.Public), stru, amount);
        }

        [Detour(typeof(Structure), "askRepair", BindingFlags.Public | BindingFlags.Instance)]
        public static void askRepair(this Structure stru, ushort amount)
        {
            // Run the events
            StructureEvents.RunRepairStructure(UnturnedServer.FindStructure(stru), amount);

            // Run the original function
            DetourManager.CallOriginal(typeof(Structure).GetMethod("askRepair", BindingFlags.Instance | BindingFlags.Public), stru, amount);
        }
    }
}