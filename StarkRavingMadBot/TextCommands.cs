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

namespace DiscordBot
{
    partial class StarkRavingMadBot
    {
        //This code is a PoS, yeah. Ain't it cool.
        private List<EventHandler<MessageEventArgs>> GetCommands()
        {
            return new List<EventHandler<MessageEventArgs>>()
            {
                new EventHandler<MessageEventArgs>(Say),
                new EventHandler<MessageEventArgs>(Help),
                new EventHandler<MessageEventArgs>(Git),
                new EventHandler<MessageEventArgs>(Swole),
                new EventHandler<MessageEventArgs>(Sleep),
                new EventHandler<MessageEventArgs>(UD),
                new EventHandler<MessageEventArgs>(Blame),
                new EventHandler<MessageEventArgs>(Noot),
                new EventHandler<MessageEventArgs>(Roll),
                new EventHandler<MessageEventArgs>(Truth),
                new EventHandler<MessageEventArgs>(Flip),
                new EventHandler<MessageEventArgs>(In),
                new EventHandler<MessageEventArgs>(Out),
                new EventHandler<MessageEventArgs>(NewGame),
                new EventHandler<MessageEventArgs>(Ubreddit),
                new EventHandler<MessageEventArgs>(Avatar),
                new EventHandler<MessageEventArgs>(WhoIs),
                new EventHandler<MessageEventArgs>(Rip),
                new EventHandler<MessageEventArgs>(Choose),
#if DEBUG
                //Beta Features
                //new EventHandler<MessageEventArgs>(Wiki),//No, not even beta
                new EventHandler<MessageEventArgs>(Reddit),//Out until XML configs are up and running
#else
                //Release-Only Features
                new EventHandler<MessageEventArgs>(Invite),
#endif
            };
        }

        #region Simple Text Commands

        private async void Choose(object s, MessageEventArgs e)
        {
            var c = GetAfterCommand(e.Message.Text).Split(';');
            await Client.SendMessage(e.Channel, c[Rand.Next(c.Length)]); 
        }

        private async void Flip(object s, MessageEventArgs e)
        {
            await Client.SendMessage(e.Channel, Rand.Next(2) == 0 ? "Heads" : "Tails");
        }

        private async void Truth(object s, MessageEventArgs e)
        {
            var name = e.Message.MentionedUsers.Any() ? e.Message.MentionedUsers.First().Name + " is " : @"They are ";
            await Client.SendMessage(e.Channel, name + (Rand.Next(2) == 0 ? "telling the truth." : "lying."));
        }

        private async void HesStarkRavingMad(object s, MessageEventArgs e)
        {
            await Client.SendMessage(e.Channel, "Welcome to fucking boatmurdered!");
        }

        private async void Blame(object s, MessageEventArgs e)
        {
            if(e.Message.MentionedUsers.Any())
            {
                await Client.SendMessage(e.Channel, $"Fuck you {e.Message.MentionedUsers.First().Name}");
            }
            else
            {
                await Client.SendMessage(e.Channel, $"Fuck you {GetAfterCommand(e.Message.Text).Trim()}");
            }
        }

        private async void Noot(object s, MessageEventArgs e)
        {
            var m = await Client.SendMessage(e.Channel, "Penguins will rule the earth!");
            await Task.Delay(1000);
            await Client.EditMessage(m, "noot noot");
        }

        private async void Ubreddit(object s, MessageEventArgs e)
        {
            await Client.SendMessage(e.Channel, "Subreddit can be found at https://www.reddit.com/r/JCFDiscord/.");
        }
        
        private async void Swole(object s, MessageEventArgs e)
        {
            if (e.Message.User.Name.ToLower() == "swolebro")
            {
                await Client.SendMessage(e.Channel, $"Dude, you so swole <@{e.User.Id}>");
            }
            else if (e.Message.Channel.Name.Contains("fitness") || e.Message.Channel.Name.Contains("swole"))
            {
                await Client.SendMessage(e.Channel, $"<#{e.Channel.Id}> is the best place to get swole with swolebro.");
            }
            else
            {
                await Client.SendMessage(e.Channel, $"Too bad you're not as swole as swolebro <@{e.User.Id}>.");
            }
        }

        private async void Sleep(object s, MessageEventArgs e)
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
                await Client.SendMessage(e.Channel, $"{msg} <@{e.Message.MentionedUsers.First().Id}>.");
            }
            else
            {
                await Client.SendMessage(e.Channel, $"{msg}.");
            }
        }

        private async void Rip(object s, MessageEventArgs e)
        {
            await Client.SendMessage(e.Channel, $"Funeral service will be held on {DateTime.Today.AddDays(Rand.Next(14)).ToLongDateString()}");
        }

        #endregion

        #region Useful Commands

        private async void Avatar(object s, MessageEventArgs e)
        {
            var serverBaseURL = @"https://discordapp.com/api/";
            if (e.Message.MentionedUsers.Any())
            {
                await Client.SendMessage(e.Channel, serverBaseURL + e.Message.MentionedUsers.First().AvatarUrl);
            }
            else
            {
                await Client.SendMessage(e.Channel, serverBaseURL + e.User.AvatarUrl);
            }
        }

        private async void WhoIs(object s, MessageEventArgs e)
        {
            var msg = "";
            var role = e.Server.Roles.Where(x => x.Name.ToLower() == GetAfterCommand(e.Message.Text).ToLower().Trim()).FirstOrDefault();
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
                    sb.AppendLine($"       - {r.Name.Replace("@","")}");
                }
                sb.AppendLine($"```");


                msg = sb.ToString();
            }
            await Client.SendMessage(e.Channel, msg);
        }

        private async void Wiki(object s, MessageEventArgs e)
        {
            Console.WriteLine("\na\n");
            using (var wc = new WebClient())
            {
                var query = GetAfterCommand(e.Message.Text);
                Console.WriteLine("\nb\n");
                var postUrl = @"/w/api.php?action=query&format=json&prop=info&inprop=url&redirects";

                Console.WriteLine("\nc\n");
                var url = $"https://simple.wikipedia.org{postUrl}&titles={Uri.EscapeUriString(query)}";
                var json = wc.DownloadString(url);
                url = Regex.Match(json, "\"fullurl\":\"(.*?)\"").Groups[1].Value;
                var title = Regex.Match(json, "\"title\":\"(.*?)\"").Groups[1].Value;
                Console.WriteLine("\nd\n");


                var markdown = "";
                Console.WriteLine("\ne\n");
                try
                {
                    markdown = wc.DownloadString($"{url}?action=raw");
                    Console.WriteLine("\nf\n");
                }
                //catch//Try Wikipedia on fail
                //{
                //    url = $"https://en.wikipedia.org{postUrl}&titles={Uri.EscapeUriString(query)}";
                //    json = wc.DownloadString(url);

                //    url = Regex.Match(json, "\"fullurl\":\"(.*?)\"").Groups[1].Value;
                //    title = Regex.Match(json, "\"title\":\"(.*?)\"").Groups[1].Value;
                    
                //    try
                //    {
                //        markdown = wc.DownloadString($"{url}?action=raw");
                //    }
                catch
                {
                    await Client.SendMessage(e.Channel, "_No Results. Try Different Capitalization._");
                    Console.WriteLine("\ng\n");
                }
                //}


                Console.WriteLine("\nh\n");

                markdown = markdown.Remove(markdown.IndexOf("=="));//First Paragraph



                markdown = Regex.Replace(markdown, @"\[\[File:.*?\]\]", "");
                markdown = Regex.Replace(markdown, @"\[\[", "");
                markdown = Regex.Replace(markdown, @"\]\]", "");
                markdown = markdown.Replace('\'', '`');
                markdown = markdown.Replace("```", "`");

                Console.WriteLine("\ni\n");

                await Client.SendMessage(e.Channel, $"**{title}**\n---\n{markdown}".Remove(1999));
                Console.WriteLine("\ni\n");

            }
        }

        private async void UD(object s, MessageEventArgs e)//Urban Dictionary
        {
            await Client.SendMessage(e.Channel, $"http://www.urbandictionary.com/define.php?term={GetAfterCommand(e.Message.RawText).Replace(' ','+')}");
        }

        private async void Say(object s, MessageEventArgs e)
        {
            if (e.User.Id == USER_JOSH_ID || Rand.Next() % 4 < 3)
            {
                var msg = GetAfterCommand(e.Message.RawText);
                if(msg.StartsWith("!"))
                {
                    msg = "." + msg;
                }
                msg.Replace("@everyone", "@ everyone");
                await Client.SendMessage(e.Channel, msg);
            }
            else
            {
                await Client.SendMessage(e.Channel, "I have a free mind, and won't be told what to do.");
            }
        }

        private async void Roll(object s, MessageEventArgs e)
        {
            var reg = new Regex("([0-9]*)[dD]([0-9]*)");
            var str = GetAfterCommand(e.Message.RawText).Replace("+"," + ").Replace("-", " - ").Split();
            var val = 0;
            var add = true;
            foreach(var item in str)
            {
                if(item.Trim() == "")
                {
                    continue;
                }
                else if(item.Trim() == "+")
                {
                    add = true;
                }
                else if(item == "-")
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
                    await Client.SendMessage(e.Channel, "Why don't you try entering that again. ಠ_ಠ");
                    return;
                }
            }

            await Client.SendMessage(e.Channel, $"Result is: {val}");
        }

        private async void Reddit(object s, MessageEventArgs e)
        {
            const int NUM_POSTS = 25;

            var sub = (new Reddit()).GetSubreddit(GetAfterCommand(e.Message.Text).Split()[0]);

            if (sub == null)
            {
                await Client.SendMessage(e.Channel, "Subreddit does not exist.");
                return;
            }

            var posts = sub.Hot.Take(NUM_POSTS).ToList();//Red ones go fasta, and so do smaller data sets and things that don't require http requests.

            var sw = System.Diagnostics.Stopwatch.StartNew();
            if (!posts.Any())
            {
                await Client.SendMessage(e.Channel, "Subreddit has no posts.");
                return;
            }
            Console.WriteLine($"sub-any took {sw.ElapsedMilliseconds}ms");
            sw.Restart();

            if (posts.All(x=>x.NSFW))//Safe to assume NSFW if 25 newest are NSFW. Not sure if reddit automatically marks posts NSFW in NSFW subs, so this could cause issues later....
            {
                await Client.SendMessage(e.Channel, "NSFW is not currently allowed");
                return;
            }


            Console.WriteLine($"sub-nsfw took {sw.ElapsedMilliseconds}ms");
            sw.Restart();


            var post = posts.Where(x=>!x.NSFW).ElementAt(Rand.Next(NUM_POSTS));
            var msg = $"**{post.Title}**\n--\n{(post.IsSelfPost ? post.SelfText : post.Url.ToString())}";
            if (msg.Length > 2000) msg.Remove(1999);
            await Client.SendMessage(e.Channel, msg);

            Console.WriteLine($"sub-msg took {sw.ElapsedMilliseconds}ms");
            sw.Restart();

            //Random hello to random person on github/b7de
        }

        #endregion

        #region Meta Commands

        private async void Help(object s, MessageEventArgs e)
        {
            var str = new StringBuilder();
            str.AppendLine("Available commands:");
            foreach (var c in GetCommands().Select(x => x.Method.Name.ToLower()).OrderBy(x => x))
            {
                if (c == "noot") continue;
                str.AppendLine($" - `{PREDICATE}{c}`");
            }
            str.AppendLine($"_Better documentation can be found on github (use {PREDICATE}git)._");
            await Client.SendMessage(e.Channel, str.ToString());
        }

        private async void Test(object s, MessageEventArgs e)
        {
            await Client.SendMessage(e.Channel, $"I'm working <@{e.User.Id}>");
        }

        private async void Git(object s, MessageEventArgs e)
        {
            await Client.SendMessage(e.Channel, "You can find my [terribly out of date] source at https://github.com/josh951623/StarkRavingMadBot/tree/master. If you'd like to suggest a feature, go ahead and create an issue.");
        }

        #endregion

        #region Truth Commands

        private string lastOrder = "";
        private List<string> truthChannels = new List<string>() { "truth", "t-r-u-t-h" };
                
        private async void In(object s, MessageEventArgs e)
        {
            try
            {
                var person = e.Message.MentionedUsers.FirstOrDefault()?.Id ?? e.User.Id;
                if (truthChannels.Contains(e.Channel.Name.ToLower()) && !e.Channel.Topic.Contains($"<@{person}>"))
                {
                    if (UserInRole(e.User, e.Server, "staff") || person == e.User.Id)
                    {
                        lastOrder = e.Channel.Topic;
                        if(string.IsNullOrWhiteSpace(e.Channel.Topic))
                        {
                            await Client.EditChannel(e.Channel, e.Channel.Name, $"<@{person}>", e.Channel.Position);
                        }
                        else
                        {
                            await Client.EditChannel(e.Channel, e.Channel.Name, $"{e.Channel.Topic} --> <@{person}>", e.Channel.Position);
                        }
                        await Client.SendMessage(e.Channel, $"Added '{Client.GetUser(e.Server, person).Name}' to game.");
                    }
                }
            }
            catch { }
        }

        private async void Out(object s, MessageEventArgs e)
        {
            try
            {
                var person = e.Message.MentionedUsers.FirstOrDefault()?.Id ?? e.User.Id;
                if (truthChannels.Contains(e.Channel.Name.ToLower()))
                {
                    if (UserInRole(e.User, e.Server, "staff") || person == e.User.Id)
                    {
                        lastOrder = e.Channel.Topic;
                        await Client.EditChannel(e.Channel, e.Channel.Name, e.Channel.Topic.Replace($" --> <@{person}>", "").Replace($"<@{person}> --> ", "").Replace($"<@{person}>", ""), e.Channel.Position);
                        await Client.SendMessage(e.Channel, $"Removed '{Client.GetUser(e.Server, person).Name}' from game.");
                    }
                }
            }
            catch { }
        }

        private async void RestoreOrder(object s, MessageEventArgs e)
        {
            try
            {
                if (truthChannels.Contains(e.Channel.Name.ToLower()) && UserInRole(e.User, e.Server, "staff"))
                {
                    await Client.EditChannel(e.Channel, e.Channel.Name, lastOrder, e.Channel.Position);
                }
            }
            catch { }
        }

        private async void NewGame(object s, MessageEventArgs e)
        {
            try
            {
                if (truthChannels.Contains(e.Channel.Name.ToLower()) && UserInRole(e.User, e.Server, "staff"))
                {
                    await Client.EditChannel(e.Channel, e.Channel.Name, "", e.Channel.Position);
                }
            }
            catch { }
        }

        #endregion
    }
}
