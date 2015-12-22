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
        public Message mVoid = null;
        public async Task DincfusVeryOwnTimeVoid(object sender, MessageEventArgs e)
        {
            //Time void for dincfu
            if (e.User.Name.ToLower() == "dincfu;")
            {
                var msg = $"<@{e.Message.User.Id}'s Time Void:\n{e.Message.Text.Remove(100).Replace('\n', ' ')}";
                if (mVoid == null)
                {
                    mVoid = await Client.SendMessage(e.Channel, msg);
                }
                else
                {
                    await Client.EditMessage(mVoid, msg);
                }
                await Client.DeleteMessage(e.Message);
            }
        }

        public int n = 0;
        public void JangoIsTalkingAboutHitlerAgain(object sender, MessageEventArgs e)
        {
            var hitlerWords = new List<string>()
            {
                "hitler",
                "anne frank",
                "nazi",
                "reich",
                "himmler",
                "heil",
                "1939",
                "1940",
                "1941",
                "1942",
                "1943",
                "1944",
                "1945"

            };

            //No hitler talk for jango
            if (e.User.Name.ToLower().Contains("jango") && hitlerWords.Any(x => e.Message.Text.ToLower().Contains(x) || hitlerWords.Contains((new Regex(@"[^\d]")).Replace(e.Message.Text, "")) ))
            {
                if (--n < 0)
                {
                    n = 2;
                    Client.SendMessage(e.Channel, "Godwin says you lack creativity. Try again.").Wait();
                }
            }
        }

        public long lastSpeaker = 0;
        public void OnlyOnes(object s, MessageEventArgs e)
        {
            if(e.Channel.Id == 126034662035423232)
            {
                if (e.Message.Text != "1" || lastSpeaker == e.User.Id)
                {
                    Client.DeleteMessage(e.Message).Wait();
                }
                else
                {
                    lastSpeaker = e.User.Id;
                }
            }
        }
    }
}
