using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.API;
using System.Threading;

namespace StarkRavingMadBot
{
    partial class StarkRavingMadBot
	{
		private void Report(object s, MessageEventArgs e) 
		{
			if(e.Channel.Name.ToLower() == "staff")
			{
				var msg = Command.GetParameters (e.Message.Text);
				SendBotMessage (e.Server, $"At {DateTime.UtcNow} UTC, {e.User.Name} reported: \n```{msg}```");
			}
		}

		private async void Shout(object s, MessageEventArgs e)
		{
			if (e.User.GetServerPermissions ().ManageMessages) {

				var timeout = 15;
				if (!int.TryParse (Command.GetParameters (e.Message.Text).Split () [0], out timeout)) {
					timeout = 15;
				}
				timeout = Math.Min (Math.Max (timeout, 10), 300);//10s < x < 30s

				var p = new DualChannelPermissions ();
				p.SendMessages = false;
				p.Speak = false;
				p.SendTTSMessages = false;
				p.AttachFiles = false;
				p.EmbedLinks = false;
				await Client.SetChannelPermissions (e.Channel, e.Server.EveryoneRole, p);

				p.SendMessages = true;
				p.Speak = true;
				p.SendTTSMessages = true;
				p.AttachFiles = true;
				p.EmbedLinks = true;


				await Task.Delay(1000*timeout);

				await Client.RemoveChannelPermissions (e.Channel, e.User);
				await Client.SetChannelPermissions (e.Channel, e.Server.EveryoneRole, p);

			}
		}

		private async void Slash(object s, MessageEventArgs e)
		{
			if (e.Message.MentionedUsers.Count() == 1 && e.User.GetServerPermissions().KickMembers ) {
				var timeout = 15;
				if (!int.TryParse (Command.GetParameters (e.Message.Text).Split () [0], out timeout)) {
					timeout = 15;
				}
				timeout = Math.Min (Math.Max (timeout, 10), 60);//10s < x < 60s
				var user = e.Message.MentionedUsers.Single ();

				var p = new DualChannelPermissions ();
				p.AttachFiles 		= false;
				p.EmbedLinks 		= false;
				p.SendMessages 		= false;
				p.SendTTSMessages 	= false;
				p.Speak 			= false;

				Parallel.ForEach(e.Server.Channels.Where(x=>x.Members.Contains(user)), c => {
					try{Client.SetChannelPermissions (c, user, p);}catch{}//shhhh, we don't have to tell a soul.....
				});


				await Client.SendMessage (e.Channel, $"Slashing '{user.Name}' for {timeout}s");

				await Task.Delay(1000*timeout);


				Parallel.ForEach(e.Server.Channels.Where(x=>x.Members.Contains(user)), c => {
					try{Client.RemoveChannelPermissions(c,user);}catch{}
				});
			}
		}

        private async void Invite(object s, MessageEventArgs e)
        {
			await Client.AcceptInvite (await Client.GetInvite (Command.GetParameters (e.Message.Text).Split () [0]));
        }

        private void Flair(object s, MessageEventArgs e)
        {
            var type = "";
            try
            {
                type = e.Message.Text.Split()[0].Remove(0, PREDICATE.Length).ToLower();
            }
            catch(IndexOutOfRangeException)
            {
                //do nothing
            }
            var reg = new Regex("[ie][ns][tf][jp]");
            if (reg.IsMatch(type))
            {

                var r = e.User.Roles.Where(x => !reg.IsMatch(x.Name.ToLower()) && type != "xxxx").ToList();
                r.Add(e.Server.Roles.Where(x => x.Name.ToLower() == type).Single());

                Client.EditUser(e.User, null, null, r);
                Client.SendMessage(e.Channel, $"Changed '{e.User.Name}'s type to {type.ToUpper()}");
            }
        }
    }
}
