using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;

namespace DiscordBot
{
    partial class StarkRavingMadBot
    {
        private async void Invite(object s, MessageEventArgs e)
        {
            await Client.AcceptInvite(await Client.GetInvite(GetAfterCommand(e.Message.Text).Split()[0]));
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
                var r = new List<Role>();
                e.User.Roles.ToList().ForEach(x => {
                    if (!reg.IsMatch(x.Name.ToLower())) r.Add(x);
                });

                r.Add(e.Server.Roles.Where(x => x.Name.ToLower() == type).Single());

                Client.EditUser(e.User, null, null, r);
                Client.SendMessage(e.Channel, $"Changed '{e.User.Name}'s type to {type.ToUpper()}");
            }
        }
    }
}
