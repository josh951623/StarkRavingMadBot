using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MarkdownSharp;
using RedditSharp;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;

namespace StarkRavingMadBot
{
    partial class StarkRavingMadBot
    {
		//This code is a PoS, yeah. Ain't it cool.new Command (
		private List<Command> GetCommands()
		{
			return new List<Command> () {
				new Command (new EventHandler<MessageEventArgs> (Say)),
				new Command (new EventHandler<MessageEventArgs>(Help)),
				new Command (new EventHandler<MessageEventArgs>(Git)),
				new Command (new EventHandler<MessageEventArgs>(Swole)),
				new Command (new EventHandler<MessageEventArgs>(Sleep)),
				new Command (new EventHandler<MessageEventArgs>(UD)),
				new Command (new EventHandler<MessageEventArgs>(Blame)),
				new Command (new EventHandler<MessageEventArgs>(Noot),null,null,true),
				new Command (new EventHandler<MessageEventArgs>(Report),null,null,true),
				new Command (new EventHandler<MessageEventArgs>(Roll)),
				new Command (new EventHandler<MessageEventArgs>(Truth)),
				new Command (new EventHandler<MessageEventArgs>(Flip)),
				new Command (new EventHandler<MessageEventArgs>(In),null,null,true),
				new Command (new EventHandler<MessageEventArgs>(Out),null,null,true),
				new Command (new EventHandler<MessageEventArgs>(NewGame),null,null,true),
				new Command (new EventHandler<MessageEventArgs>(Subreddit)),
				new Command (new EventHandler<MessageEventArgs>(Avatar)),
				new Command (new EventHandler<MessageEventArgs>(WhoIs)),
				new Command (new EventHandler<MessageEventArgs>(Rip)),
				new Command (new EventHandler<MessageEventArgs>(Choose)),
                new Command (new EventHandler<MessageEventArgs>(ServerStats)),
                new Command (new EventHandler<MessageEventArgs>(Spooderman)),
                new Command (new EventHandler<MessageEventArgs>(HesStarkRavingMad),null,null,true,"StarkRavingMad"),
                new Command (new EventHandler<MessageEventArgs>(Pengu)),
                new Command (new EventHandler<MessageEventArgs>(Intensify)),
                new Command (new EventHandler<MessageEventArgs>(Color)),
                new Command (new EventHandler<MessageEventArgs>(ChannelSearch),null,null,true,"c"),
#if DEBUG
				//Beta Features
				//new EventHandler<MessageEventArgs>(Wiki),//No, not even beta
				new Command (new EventHandler<MessageEventArgs>(Reddit)),//Out until XML configs are up and running
				new Command (new EventHandler<MessageEventArgs>(Slash),null,null,true),
				new Command (new EventHandler<MessageEventArgs>(Shout),null,null,true),
#else
				//Release-Only Features
				new Command (new EventHandler<MessageEventArgs>(Invite)),
#endif
			};
		}

        #region Imgur Images
        private void Spooderman(object s, MessageEventArgs e)
        {
            GetRandFromImgur(e.Channel, "tmWOW", Command.GetParameters(e.Message.Text));
        }

        private void Pengu(object s, MessageEventArgs e)
        {
            GetRandFromImgur(e.Channel, "pgDSZ", Command.GetParameters(e.Message.Text));
        }

        private void Intensify(object s, MessageEventArgs e)
        {
            GetRandFromImgur(e.Channel, "FVmJU", Command.GetParameters(e.Message.Text));
        }

        private async void GetRandFromImgur(Channel channel, string album, string para)
        {
            //Setup imgur
            var imgCli = new ImgurClient("3479c594880bdd5", "1bab71841002bc7ebe1eae4c7d68378ef5606240");
            var ep = new AlbumEndpoint(imgCli);
            var alb = await ep.GetAlbumImagesAsync(album);
            IImage img;

            //Get image
            var searchTerms = para.Split().Where(xs => !string.IsNullOrWhiteSpace(xs)).ToList();
            if (searchTerms.Any())
            {
                var max = alb.Max(x => string.IsNullOrWhiteSpace(x.Description) ? 0 : x.Description.Split().Where(xs => !string.IsNullOrWhiteSpace(xs)).Count(xs => searchTerms.Contains(xs)));
                var imgs = alb.Where(x => (string.IsNullOrWhiteSpace(x.Description) ? 0 : x.Description.Split().Where(xs => !string.IsNullOrWhiteSpace(xs)).Count(xs => searchTerms.Contains(xs))) == max);
                img = imgs.ElementAt(Rand.Next(imgs.Count()));
            }
            else
            {
                img = alb.ElementAt(Rand.Next(alb.Count()));
            }


            //Upload Image
            using (var wc = new WebClient())
            {
                var name = $"temp.{img.Link.Split('.').Last()}";
                wc.DownloadFile(img.Link, name);
                await Client.SendFile(channel, name);
            }
        }
        #endregion

        #region Simple Text Commands
        private void Choose(object s, MessageEventArgs e)
        {
			var c = Command.GetParameters(e.Message.Text).Split(';');
            Client.SendMessage(e.Channel, c[Rand.Next(c.Length)]);
        }

        private void Flip(object s, MessageEventArgs e)
        {
            Client.SendMessage(e.Channel, Rand.Next(2) == 0 ? "Heads" : "Tails");
        }

        private void Truth(object s, MessageEventArgs e)
        {
            var name = e.Message.MentionedUsers.Any() ? e.Message.MentionedUsers.First().Name + " is " : @"They are ";
            Client.SendMessage(e.Channel, name + (Rand.Next(2) == 0 ? "telling the truth." : "lying."));
		}

		private void HesStarkRavingMad(object s, MessageEventArgs e)
		{
			Client.SendMessage(e.Channel, "Welcome to fucking boatmurdered!");
		}

        private void Blame(object s, MessageEventArgs e)
        {
            if (e.Message.MentionedUsers.Any())
            {
                Client.SendMessage(e.Channel, $"Fuck you {e.Message.MentionedUsers.First().Name}");
            }
            else
            {
				Client.SendMessage(e.Channel, $"Fuck you {Command.GetParameters(e.Message.Text).Trim()}");
            }
        }

        private async void Noot(object s, MessageEventArgs e)
        {
            var m = await Client.SendMessage(e.Channel, "Penguins will rule the earth!");
            await Task.Delay(1000);
            await Client.EditMessage(m, "noot noot");
        }

        private void Subreddit(object s, MessageEventArgs e)
        {
            Client.SendMessage(e.Channel, "Subreddit can be found at https://www.reddit.com/r/JCFDiscord/.");
        }

        private void Swole(object s, MessageEventArgs e)
        {
            if (e.User.Id == 100335016025788416)
            {
                Client.SendMessage(e.Channel, $"Dude, you so swole <@{e.User.Id}>");
            }
            else if (e.Message.Channel.Name.Contains("fitness") || e.Message.Channel.Name.Contains("swole"))
            {
                Client.SendMessage(e.Channel, $"<#{e.Channel.Id}> is the best place to get swole with swolebro.");
            }
            else
            {
                Client.SendMessage(e.Channel, $"Too bad you're not as swole as swolebro <@{e.User.Id}>.");
            }
        }

        private void Sleep(object s, MessageEventArgs e)
        {
            var slmsg = new List<string>()
            {
                "Go to sleep",
                "Git to bed",
                "The addiction is more satisfying while conscious",
                "(ﾉಠ_ಠ)ﾉ*:・ﾟ✧\ngit to sleep"
            };

            var msg = slmsg.ElementAt(Rand.Next(slmsg.Count));

            if (e.Message.MentionedUsers.Any())
            {
                Client.SendMessage(e.Channel, $"{msg} <@{e.Message.MentionedUsers.First().Id}>.");
            }
            else
            {
                Client.SendMessage(e.Channel, $"{msg}.");
            }
        }

        private void Rip(object s, MessageEventArgs e)
        {
            Client.SendMessage(e.Channel, $"Funeral service will be held on {DateTime.Today.AddDays(Rand.Next(12)+3).ToLongDateString()}");
        }

        #endregion

        #region Useful Commands

        private void ServerStats(object s, MessageEventArgs e)
        {
            var str = new StringBuilder();
            str.AppendLine($"```");
            str.AppendLine($"Server Name: {e.Server.Name}");
            str.AppendLine($"ID: {e.Server.Id}");
            str.AppendLine($"Owner: {e.Server.Owner.Name}");
            str.AppendLine($"Creation Date: {e.Server.Owner.JoinedAt.ToShortDateString()}");
            str.AppendLine($"Text Channels: {e.Server.TextChannels.Count()}");
            str.AppendLine($"Voice Channels: {e.Server.VoiceChannels.Count()}");
            str.AppendLine($"Memebers: {e.Server.Members.Count()}");
            str.AppendLine($"```");

            Client.SendMessage(e.Channel, str.ToString());
        }

        private void Avatar(object s, MessageEventArgs e)
        {
            var serverBaseURL = @"https://discordapp.com/api/";
            if (e.Message.MentionedUsers.Any())
            {
                Client.SendMessage(e.Channel, serverBaseURL + e.Message.MentionedUsers.First().AvatarUrl);
            }
            else
            {
                Client.SendMessage(e.Channel, serverBaseURL + e.User.AvatarUrl);
            }
        }

        private void WhoIs(object s, MessageEventArgs e)
        {
            var msg = "";
			var role = e.Server.Roles.Where(x => x.Name.ToLower() == Command.GetParameters(e.Message.Text).ToLower().Trim()).FirstOrDefault();
            if (!e.Message.MentionedUsers.Any() && role != null)
            {
                msg = $"```Members in role '{role.Name}'";
                foreach (var m in role.Members) msg += $"\n - {m.Name}";
                msg += "```";
            }
            else
            {
                var person = e.Message.MentionedUsers.FirstOrDefault() ?? e.User;
                var sb = new StringBuilder();
                sb.AppendLine("```");
                sb.AppendLine($"    User: {person.Name}");
                sb.AppendLine($"    ID: {person.Id}");
                sb.AppendLine($"    Joined On: {person.JoinedAt.ToShortDateString()}");
                var la = person.LastActivityAt?.ToString();
                la = la == null ? "none" : $"{la} GMT";
                sb.AppendLine($"    Last Activity: {la}");
                sb.AppendLine($"    Roles:");
                foreach (var r in person.Roles)
                {
                    sb.AppendLine($"       - {r.Name.Replace("@", "")}");
                }
                sb.AppendLine($"```");


                msg = sb.ToString();
            }
            Client.SendMessage(e.Channel, msg);
        }

        private void UD(object s, MessageEventArgs e)//Urban Dictionary
        {
			Client.SendMessage(e.Channel, $"http://www.urbandictionary.com/define.php?term={Command.GetParameters(e.Message.RawText).Replace(' ', '+')}");
        }

        private void Say(object s, MessageEventArgs e)
        {
            if (e.User.Id == USER_JOSH_ID || Rand.Next() % 4 < 3)
            {
				var msg = Command.GetParameters(e.Message.RawText);
                if (msg.StartsWith("!"))
                {
                    msg = "." + msg;
                }
                msg.Replace("@everyone", "@ everyone");
                Client.SendMessage(e.Channel, msg);
            }
            else
            {
                Client.SendMessage(e.Channel, "I have a free mind, and won't be told what to do.");
            }
        }

        private void Roll(object s, MessageEventArgs e)
        {
            var reg = new Regex("([0-9]*)[dD]([0-9]*)");
			var str = Command.GetParameters(e.Message.RawText).Replace("+", " + ").Replace("-", " - ").Split();
            var val = 0;
            var add = true;
            foreach (var item in str)
            {
                if (item.Trim() == "")
                {
                    continue;
                }
                else if (item.Trim() == "+")
                {
                    add = true;
                }
                else if (item == "-")
                {
                    add = false;
                }
                else if (reg.IsMatch(item.Trim()))
                {
                    for (int i = int.Parse(reg.Match(item.Trim()).Groups[1].ToString()); i > 0; i--)
                    {
                        val += (add ? 1 : -1) * (Rand.Next(int.Parse(reg.Match(item.Trim()).Groups[2].ToString())) + 1);
                    }
                }
                else if (Regex.IsMatch(item.Trim(), "[0-9]*"))
                {
                    val += (add ? 1 : -1) * int.Parse(item.Trim());
                }
                else
                {
                    Client.SendMessage(e.Channel, "Why don't you try entering that again. ಠ_ಠ");
                    return;
                }
            }

            Client.SendMessage(e.Channel, $"Result is: {val}");
        }

        private void Reddit(object s, MessageEventArgs e)
        {
            const int NUM_POSTS = 25;

			var sub = (new Reddit()).GetSubreddit(Command.GetParameters(e.Message.Text).Split()[0]);

            if (sub == null)
            {
                Client.SendMessage(e.Channel, "Subreddit does not exist.");
                return;
            }

            var posts = sub.Hot.Take(NUM_POSTS).ToList();//Red ones go fasta, and so do smaller data sets and things that don't require http requests.

            var sw = System.Diagnostics.Stopwatch.StartNew();
            if (!posts.Any())
            {
                Client.SendMessage(e.Channel, "Subreddit has no posts.");
                return;
            }
            Console.WriteLine($"sub-any took {sw.ElapsedMilliseconds}ms");
            sw.Restart();

            if (posts.All(x => x.NSFW))//Safe to assume NSFW if 25 newest are NSFW. Not sure if reddit automatically marks posts NSFW in NSFW subs, so this could cause issues later....
            {
                Client.SendMessage(e.Channel, "NSFW is not currently allowed");
                return;
            }


            Console.WriteLine($"sub-nsfw took {sw.ElapsedMilliseconds}ms");
            sw.Restart();


            var post = posts.Where(x => !x.NSFW).ElementAt(Rand.Next(NUM_POSTS));
            var msg = $"**{post.Title}**\n--\n{(post.IsSelfPost ? post.SelfText : post.Url.ToString())}";
            if (msg.Length > 2000) msg.Remove(1999);
            Client.SendMessage(e.Channel, msg);

            Console.WriteLine($"sub-msg took {sw.ElapsedMilliseconds}ms");
            sw.Restart();

            //Random hello to random person on github/b7de
        }

        #endregion

        #region Meta Commands

        private void Help(object s, MessageEventArgs e)
        {
            var str = new StringBuilder();
            str.AppendLine("Available commands:");
			foreach (var c in GetCommands().Where(x => !x.Hidden && x.RequiredPermissions == ChannelPermissions.None).OrderBy(x => x.Name))
            {
                str.AppendLine($" - `{PREDICATE}{c.Name}`");
            }
            Client.SendMessage(e.Channel, str.ToString());
        }

        private void Info(object s, MessageEventArgs e)
        {
            Help(s, e);
        }

        private void Test(object s, MessageEventArgs e)
        {
            Client.SendMessage(e.Channel, $"I'm working <@{e.User.Id}>");
        }

        private void Git(object s, MessageEventArgs e)
        {
            Client.SendMessage(e.Channel, "You can find my source at https://github.com/josh951623/StarkRavingMadBot/tree/master. If you'd like to suggest a feature, go ahead and join me in my dev server: https://discord.gg/0ktzcmJwmeWuQtiM.");
        }

        #endregion

        #region Truth Commands

        private string lastOrder = "";
        private List<string> truthChannels = new List<string>() { "truth", "t-r-u-t-h" };

        private void In(object s, MessageEventArgs e)
        {
            try
            {
                var person = e.Message.MentionedUsers.FirstOrDefault()?.Id ?? e.User.Id;
                if (truthChannels.Contains(e.Channel.Name.ToLower()) && !e.Channel.Topic.Contains($"<@{person}>"))
                {
                    if (UserInRole(e.User, e.Server, "staff") || person == e.User.Id)
                    {
                        lastOrder = e.Channel.Topic;
                        if (string.IsNullOrWhiteSpace(e.Channel.Topic))
                        {
                            Client.EditChannel(e.Channel, e.Channel.Name, $"<@{person}>", e.Channel.Position);
                        }
                        else
                        {
                            Client.EditChannel(e.Channel, e.Channel.Name, $"{e.Channel.Topic} --> <@{person}>", e.Channel.Position);
                        }
                        Client.SendMessage(e.Channel, $"Added '{Client.GetUser(e.Server, person).Name}' to game.");
                    }
                }
            }
            catch { }
        }

        private void Out(object s, MessageEventArgs e)
        {
            try
            {
                var person = e.Message.MentionedUsers.FirstOrDefault()?.Id ?? e.User.Id;
                if (truthChannels.Contains(e.Channel.Name.ToLower()))
                {
                    if (UserInRole(e.User, e.Server, "staff") || person == e.User.Id)
                    {
                        lastOrder = e.Channel.Topic;
                        Client.EditChannel(e.Channel, e.Channel.Name, e.Channel.Topic.Replace($" --> <@{person}>", "").Replace($"<@{person}> --> ", "").Replace($"<@{person}>", ""), e.Channel.Position);
                        Client.SendMessage(e.Channel, $"Removed '{Client.GetUser(e.Server, person).Name}' from game.");
                    }
                }
            }
            catch { }
        }

        private void RestoreOrder(object s, MessageEventArgs e)
        {
            try
            {
                if (truthChannels.Contains(e.Channel.Name.ToLower()) && UserInRole(e.User, e.Server, "staff"))
                {
                    Client.EditChannel(e.Channel, e.Channel.Name, lastOrder, e.Channel.Position);
                }
            }
            catch { }
        }

        private void NewGame(object s, MessageEventArgs e)
        {
            try
            {
                if (truthChannels.Contains(e.Channel.Name.ToLower()) && UserInRole(e.User, e.Server, "staff"))
                {
                    Client.EditChannel(e.Channel, e.Channel.Name, "", e.Channel.Position);
                }
            }
            catch { }
        }

        #endregion
    }
}
