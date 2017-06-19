﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PointBlank.API.Commands;
using PointBlank.API.Unturned.Player;
using PointBlank.API.Unturned.Chat;
using Steamworks;

namespace PointBlank.Commands
{
    [PointBlankCommand("Spy", 1)]
    internal class CommandSpy : PointBlankCommand
    {
        #region Properties
        public override string[] DefaultCommands => new string[]
        {
            "Spy"
        };

        public override string Help => "Sends you a screenshot of the player";

        public override string Usage => Commands[0] + " <player>";

        public override string DefaultPermission => "unturned.commands.admin.spy";
        #endregion

        public override void Execute(UnturnedPlayer executor, string[] args)
        {
            if(!UnturnedPlayer.TryGetPlayer(args[0], out UnturnedPlayer ply))
            {
                UnturnedChat.SendMessage(executor, "Could not find player!", ConsoleColor.Red);
                return;
            }

            ply.Player.sendScreenshot((executor == null ? CSteamID.Nil : executor.SteamID), null);
            UnturnedChat.SendMessage(executor, "Screenshot successfully sent!", ConsoleColor.Red);
        }
    }
}