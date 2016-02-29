using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;

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

        //private async void Shout(object s, MessageEventArgs e)
        //{
        //    if (e.User.GetServerPermissions ().ManageMessages) {

        //        var timeout = 15;
        //        if (!int.TryParse (Command.GetParameters (e.Message.Text).Split () [0], out timeout)) {
        //            timeout = 15;
        //        }
        //        timeout = Math.Min (Math.Max (timeout, 10), 300);//10s < x < 30s

        //        var p = new DualChannelPermissions ();
        //        p.SendMessages = false;
        //        p.Speak = false;
        //        p.SendTTSMessages = false;
        //        p.AttachFiles = false;
        //        p.EmbedLinks = false;
        //        await Client.SetChannelPermissions (e.Channel, e.Server.EveryoneRole, p);

        //        p.SendMessages = true;
        //        p.Speak = true;
        //        p.SendTTSMessages = true;
        //        p.AttachFiles = true;
        //        p.EmbedLinks = true;


        //        await Task.Delay(1000*timeout);

        //        await Client.RemoveChannelPermissions (e.Channel, e.User);
        //        await Client.SetChannelPermissions (e.Channel, e.Server.EveryoneRole, p);

        //    }
        //}

        //private async void Slash(object s, MessageEventArgs e)
        //{
        //    if (e.Message.MentionedUsers.Count() == 1 && e.User.GetServerPermissions().KickMembers ) {
        //        var timeout = 15;
        //        if (!int.TryParse (Command.GetParameters (e.Message.Text).Split () [0], out timeout)) {
        //            timeout = 15;
        //        }
        //        timeout = Math.Min (Math.Max (timeout, 10), 60);//10s < x < 60s
        //        var user = e.Message.MentionedUsers.Single ();

        //        var p = new DualChannelPermissions ();
        //        p.AttachFiles         = false;
        //        p.EmbedLinks         = false;
        //        p.SendMessages         = false;
        //        p.SendTTSMessages     = false;
        //        p.Speak             = false;

        //        Parallel.ForEach(e.Server.Channels.Where(x=>x.Members.Contains(user)), c => {
        //            try{Client.SetChannelPermissions (c, user, p);}catch{}//shhhh, we don't have to tell a soul.....
        //        });


        //        await Client.SendMessage (e.Channel, $"Slashing '{user.Name}' for {timeout}s");

        //        await Task.Delay(1000*timeout);


        //        Parallel.ForEach(e.Server.Channels.Where(x=>x.Members.Contains(user)), c => {
        //            try{Client.RemoveChannelPermissions(c,user);}catch{}
        //        });
        //    }
        //}

        //private async void Invite(object s, MessageEventArgs e)
        //{
        //    await Client.AcceptInvite (await Client.GetInvite (Command.GetParameters (e.Message.Text).Split () [0]));
        //}

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
            if (reg.IsMatch(type) /*|| type == "xxxx"*/)
            {

                var r = e.User.Roles.Where(x => !reg.IsMatch(x.Name.ToLower()) /*&& type != "xxxx"*/).ToList();
                r.Add(e.Server.Roles.Where(x => x.Name.ToLower() == type).Single());

                e.User.Edit(null, null, null, r);
                e.Channel.SendMessage($"Changed '{e.User.Name}'s type to {type.ToUpper()}");
            }
        }

        private async void Color(object s, MessageEventArgs e)
        {
            var reg = new Regex("[0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F][0-9a-fA-F]");
            var cl = Command.GetParameters(e.Message.Text).ToLower().Replace("#","").Trim();

            if (cl == "clear")
            {
                var rx = e.User.Roles.Where(x => !reg.IsMatch(x.Name)).ToList();
                await e.User.Edit(null, null, null, rx);
                await e.Channel.SendMessage( $"Cleared '{e.User.Name}'s color");
            }

            if (!reg.IsMatch(cl)) return;

            var c = e.Server.Roles.ToList().Where(x => x.Name.Equals(cl)).ToList().SingleOrDefault();

            if (c == null)
            {
                await e.Server.CreateRole(cl);
                c = e.Server.Roles.Where(x => x.Name == cl).Single();
                await c.Edit(null, ServerPermissions.None, new Color(Convert.ToUInt32(cl, 16)), null, e.Server.Roles.Count() - 1);
            }

            var r = e.User.Roles.Where(x => !reg.IsMatch(x.Name)).ToList();

            r.Add(c);
            await e.User.Edit(null, null, null, r);

            await e.Channel.SendMessage($"Changed '{e.User.Name}'s color to #{cl}");

            Client.GetServer(e.Server.Id).Roles.Where(x => !x.Members.Any() && reg.IsMatch(x.Name)).ToList().ForEach(x => { x.Delete(); });
        }
    }
}
