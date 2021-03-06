﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PointBlank.API.Plugins;
using PointBlank.API.Player;
using CM = PointBlank.Services.CommandManager.CommandManager;

namespace PointBlank.API.Commands
{
    /// <summary>
    /// Custom command class
    /// </summary>
    public abstract class PointBlankCommand
    {
        #region Properties
        /// <summary>
        /// The command instance
        /// </summary>
        public static PointBlankCommand Instance { get; internal set; }

        /// <summary>
        /// The commands used to execute this command
        /// </summary>
        public string[] Commands => CM.Commands.FirstOrDefault(a => a.CommandClass == this).Commands;

        /// <summary>
        /// The permissions needed to execute the command
        /// </summary>
        public string Permission => CM.Commands.FirstOrDefault(a => a.CommandClass == this).Permission;

        /// <summary>
        /// The cooldown needed to execute the command
        /// </summary>
        public int Cooldown => CM.Commands.FirstOrDefault(a => a.CommandClass == this).Cooldown;

        /// <summary>
        /// Is the command enabled
        /// </summary>
        public bool Enabled => CM.Commands.FirstOrDefault(a => a.CommandClass == this).Enabled;
        #endregion

        #region Abstract Properties
        /// <summary>
        /// If the player types any of the commands into the console it will run this command
        /// </summary>
        public abstract string[] DefaultCommands { get; }

        /// <summary>
        /// The translation key for the command help message
        /// </summary>
        public abstract string Help { get; }

        /// <summary>
        /// The translation key for the command usage message
        /// </summary>
        public abstract string Usage { get; }

        /// <summary>
        /// The permission needed to run the command
        /// </summary>
        public abstract string DefaultPermission { get; }
        #endregion

        #region Virtual Properties
        /// <summary>
        /// The minimum amount of parameters required for the command
        /// </summary>
        public virtual int MinimumParams => 0;

        /// <summary>
        /// The default cooldown(-1 to not override cooldown)
        /// </summary>
        public virtual int DefaultCooldown => -1;

        /// <summary>
        /// At what state is the command allowed to be executed
        /// </summary>
        public virtual EAllowedServerState AllowedServerState => EAllowedServerState.BOTH;

        /// <summary>
        /// Who can execute the command
        /// </summary>
        public virtual EAllowedCaller AllowedCaller => EAllowedCaller.BOTH;
        #endregion

        #region Abstract Functions
        /// <summary>
        /// Called when the player executes the command
        /// </summary>
        /// <param name="args">The arguments the player inputted</param>
        /// <param name="executor">The player executing the command</param>
        public abstract void Execute(PointBlankPlayer executor, string[] args);
        #endregion

        public PointBlankCommand()
        {
            Instance = this;
        }

        #region Functions
        /// <summary>
        /// Translates a key and data to text depending on the translation
        /// </summary>
        /// <param name="key">The key of the translation</param>
        /// <param name="data">The data to modify the translation</param>
        /// <returns>The translated text</returns>
        public string Translate(string key, params object[] data) => PointBlankPlugin.Instance.Translate(key, data);

        /// <summary>
        /// Easy to use configuration value extractor
        /// </summary>
        /// <typeparam name="T">The configuration value type</typeparam>
        /// <param name="key">The key of the configuration value</param>
        /// <returns>The configuration value with specified type</returns>
        public T Configure<T>(string key) => PointBlankPlugin.Instance.Configure<T>(key);
        #endregion
    }
}
