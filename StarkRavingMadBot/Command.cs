using System;
using System.Collections.Generic;
using Discord;

namespace StarkRavingMadBot
{
	public class Command
	{
		public EventHandler<MessageEventArgs> Method { get; set; }
		public string Name { get; set; }
		public bool Hidden { get; set; }
		public string HelpText { get; set; }
		public ChannelPermissions RequiredPermissions { get; set; }
		public List<Command> Commands {get;set;}

		public Command(EventHandler<MessageEventArgs> method, ChannelPermissions perms = null, string help = null, bool hidden = false, string nameOverride = null)
		{
			this.Method 	= method;
			this.RequiredPermissions = perms ?? ChannelPermissions.None;
			this.HelpText 	= string.IsNullOrWhiteSpace(help) ? "No help text has been set for this command" : help;
			this.Hidden 	= hidden;
			this.Name = string.IsNullOrWhiteSpace(nameOverride) ? Method.Method.Name.ToLower() : nameOverride.ToLower();
		}

		public bool IsCommand(string msg)
		{
			var s = msg.Remove (0, StarkRavingMadBot.PREDICATE.Length).Split () [0].ToLower ();
			return s == Name.ToLower ();
		}


		public static string GetParameters(string str)
		{
			return str.Remove(0, str.Split()[0].Length).Trim();
		}
	}
}

