using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;

namespace DiscordBot
{
    partial class StarkRavingMadBot
    {
        private List<EventHandler<MessageEventArgs>> GetCommands()
        {
            return new List<EventHandler<MessageEventArgs>>()
            {
                //new EventHandler<MessageEventArgs>(Ay),
                //new EventHandler<MessageEventArgs>(Help),
                //new EventHandler<MessageEventArgs>(Git),
                //new EventHandler<MessageEventArgs>(Wole),
                //new EventHandler<MessageEventArgs>(Leep),
                //new EventHandler<MessageEventArgs>(UD),
                //new EventHandler<MessageEventArgs>(Test),
                //new EventHandler<MessageEventArgs>(Vote),
                //new EventHandler<MessageEventArgs>(Ubreddit),
                //new EventHandler<MessageEventArgs>(Flair),
            };
        }

        private string GetAfterCommand(string str)
        {
            var msg = str.Substring(str.IndexOf('$'));//cuts out stuff before command
            try {
                msg = msg.Substring(msg.IndexOf(' ')).Trim();//cuts out command
            }
            catch(ArgumentOutOfRangeException)//nothing after
            {
                msg = "";
            }
            return msg;
        }

        private void UD(object s, MessageEventArgs e)//Urban Dictionary
        {
            Client.SendMessage(e.Channel, $"http://www.urbandictionary.com/define.php?term={GetAfterCommand(e.Message.RawText).Replace(' ','+')}").Wait();
        }

        private void Test(object s, MessageEventArgs e)
        {
            Client.SendMessage(e.Channel, $"I'm working <@{e.User.Id}>");
        }

        private void Ay(object s, MessageEventArgs e)
        {
            if (e.User.Id == USER_JOSH_ID || Rand.Next() % 4 < 3)
            {
                var msg = GetAfterCommand(e.Message.RawText);
                Client.DeleteMessage(e.Message);
                if(msg.StartsWith("!"))
                {
                    msg = "." + msg;
                }

                Client.SendMessage(e.Channel, msg).Wait();
            }
            else
            {
                Client.SendMessage(e.Channel, "I have a free mind, and won't be told what to do.").Wait();
            }
        }

        private void Help(object s, MessageEventArgs e)
        {
            var str = new StringBuilder();
            str.AppendLine("Available commands:");
            foreach (var c in GetCommands().Select(x => x.Method.Name.ToLower()).OrderBy(x => x))
            {
                str.AppendLine($" - `${c}`");
            }
            str.AppendLine("_Better documentation can be found on github (use $git)._");
            Client.SendMessage(e.Channel, str.ToString()).Wait();
        }

        private void Git(object s, MessageEventArgs e)
        {
            Client.SendMessage(e.Channel, "You can find my source at https://github.com/josh951623/StarkRavingMadBot/tree/master. If you'd like to suggest a feature, go ahead and create an issue.");
        }

        private void Ubreddit(object s, MessageEventArgs e)
        {
            Client.SendMessage(e.Channel, "Subreddit can be found at https://www.reddit.com/r/JCFDiscord/.");
        }
        
        private void Wole(object s, MessageEventArgs e)
        {
            if (e.Message.User.Name.ToLower() == "swolebro")
            {
                Client.SendMessage(e.Channel, $"Dude, you so swole <@{e.User.Id}>").Wait();
            }
            else if (e.Message.Channel.Name.Contains("fitness") || e.Message.Channel.Name.Contains("swole"))
            {
                Client.SendMessage(e.Channel, $"<#{e.Channel.Id}> is the best place to get swole with swolebro.").Wait();
            }
            else
            {
                Client.SendMessage(e.Channel, $"Too bad you're not as swole as swolebro <@{e.User.Id}>.").Wait();
            }
        }
        
        private void Leep(object s, MessageEventArgs e)
        {
            var slmsg = new List<string>()
            {
                "Go to sleep",
                "Git to bed",
                "The addiction is more satisfying while conscious",
                "(ﾉಠ_ಠ)ﾉ*:・ﾟ✧\ngit to sleep"
            };

            var msg = slmsg.ElementAt(Rand.Next(slmsg.Count));

            if(e.Message.MentionedUsers.Any())
            {
                Client.SendMessage(e.Channel, $"{msg} <@{e.Message.MentionedUsers.First().Id}>.");
            }
            else
            {
                Client.SendMessage(e.Channel, $"{msg}.");
            }
        }
    }
}
