﻿using System.Text;
using Gazillion;
using MHServerEmu.Auth;
using MHServerEmu.Common.Commands;
using MHServerEmu.Frontend;

namespace MHServerEmu.PlayerManagement.Accounts
{
    [CommandGroup("account", "Allows you to manage accounts.", AccountUserLevel.User)]
    public class AccountCommands : CommandGroup
    {
        [Command("create", "Creates a new account.\nUsage: account create [email] [playerName] [password]", AccountUserLevel.User)]
        public string Create(string[] @params, FrontendClient client)
        {
            if (@params == null) return Fallback();
            if (@params.Length < 3) return "Invalid arguments. Type 'help account create' to get help.";

            AccountManager.CreateAccount(@params[0].ToLower(), @params[1], @params[2], out string message);
            return message;
        }

        [Command("playername", "Changes player name for the specified account.\nUsage: account playername [email] [playername]", AccountUserLevel.User)]
        public string PlayerName(string[] @params, FrontendClient client)
        {
            if (@params == null) return Fallback();
            if (@params.Length < 2) return "Invalid arguments. Type 'help account playername' to get help.";

            string email = @params[0].ToLower();

            if (client != null && client.Session.Account.UserLevel < AccountUserLevel.Moderator && email != client.Session.Account.Email)
                return "You are allowed to change player name only for your own account.";

            AccountManager.ChangeAccountPlayerName(email, @params[1], out string message);
            return message;
        }

        [Command("password", "Changes password for the specified account.\nUsage: account password [email] [password]", AccountUserLevel.User)]
        public string Password(string[] @params, FrontendClient client)
        {
            if (@params == null) return Fallback();
            if (@params.Length < 2) return "Invalid arguments. Type 'help account password' to get help.";

            string email = @params[0].ToLower();

            if (client != null && client.Session.Account.UserLevel < AccountUserLevel.Moderator && email != client.Session.Account.Email)
                return "You are allowed to change password only for your own account.";

            AccountManager.ChangeAccountPassword(email, @params[1], out string message);
            return message;
        }

        [Command("userlevel", "Changes user level for the specified account.\nUsage: account userlevel [email] [0|1|2]", AccountUserLevel.Admin)]
        public string UserLevel(string[] @params, FrontendClient client)
        {
            if (@params == null) return Fallback();
            if (@params.Length < 2) return "Invalid arguments. Type 'help account userlevel' to get help.";

            if (byte.TryParse(@params[1], out byte userLevel))
            {
                if (userLevel > 2) return "Invalid arguments. Type 'help account userlevel' to get help.";
                AccountManager.SetAccountUserLevel(@params[0].ToLower(), (AccountUserLevel)userLevel, out string message);
                return message;
            }
            else
            {
                return "Failed to parse user level";
            }
        }

        [Command("verify", "Checks if an email/password combination is valid.\nUsage: account verify [email] [password]", AccountUserLevel.Admin)]
        public string Verify(string[] @params, FrontendClient client)
        {
            if (@params == null) return Fallback();
            if (@params.Length < 2) return "Invalid arguments. Type 'help account verify' to get help.";

            var loginDataPB = LoginDataPB.CreateBuilder().SetEmailAddress(@params[0].ToLower()).SetPassword(@params[1]).Build();
            AuthStatusCode statusCode = AccountManager.TryGetAccountByLoginDataPB(loginDataPB, out _);

            if (statusCode == AuthStatusCode.Success)
                return "Account credentials are valid.";
            else
                return $"Account credentials are NOT valid: {statusCode}!";
        }

        [Command("ban", "Bans the specified account.\nUsage: account ban [email]", AccountUserLevel.Moderator)]
        public string Ban(string[] @params, FrontendClient client)
        {
            if (@params == null) return Fallback();
            if (@params.Length == 0) return "Invalid arguments. Type 'help account ban' to get help.";

            AccountManager.BanAccount(@params[0].ToLower(), out string message);
            return message;
        }

        [Command("unban", "Unbans the specified account.\nUsage: account unban [email]", AccountUserLevel.Moderator)]
        public string Unban(string[] @params, FrontendClient client)
        {
            if (@params == null) return Fallback();
            if (@params.Length == 0) return "Invalid arguments. Type 'help account unban' to get help.";

            AccountManager.UnbanAccount(@params[0].ToLower(), out string message);
            return message;
        }

        [Command("info", "Shows information for the logged in account.\nUsage: account info", AccountUserLevel.User)]
        public string Info(string[] @params, FrontendClient client)
        {
            if (client == null) return "You can only invoke this command from the game.";

            StringBuilder sb = new();
            sb.Append($"Account Info:\n");
            sb.Append($"Email: {client.Session.Account.Email}\n");
            sb.Append($"PlayerName: {client.Session.Account.PlayerName}\n");
            sb.Append($"UserLevel: {client.Session.Account.UserLevel}\n");
            sb.Append($"IsBanned: {client.Session.Account.IsArchived}\n");
            sb.Append($"IsArchived: {client.Session.Account.IsArchived}\n");
            sb.Append($"IsPasswordExpired: {client.Session.Account.IsPasswordExpired}\n");
            return sb.ToString();
        }
    }
}
