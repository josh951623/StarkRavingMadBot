using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Discord;

namespace DiscordBot
{
    

    partial class StarkRavingMadBot
    {
        private const long USER_JOSH_ID         = 93398876408524800;
        private const long USER_SPREADLINK_ID   = 118798518138830853;
        private const long USER_STARK_ID        = 121281613660160000;
        private const long USER_BETASTARK_ID    = 134799811147726848;

        private const long MBTI_SERVER_ID       = 133691613334470656;
        /****************BOT SETTINGS***************/
#if DEBUG
        private const string PREDICATE = "deb$";
#else  
        private const string PREDICATE = "$";
#endif
        private const string VERSION = "0.0.2";
        /******************************************/

        public Random Rand = new Random();
        private DiscordClient Client = new DiscordClient();
        private ManualResetEvent Handler = new ManualResetEvent(false);

        private string Email { get; set; }
        private string Password { get; set; }

        public StarkRavingMadBot(string email, string pass)
        {
            this.Email = email;
            this.Password = pass;

            AttemptConnect();

            //Add Handlers
            //Client.LogMessage += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");
            
            Client.MessageReceived += new EventHandler<MessageEventArgs>(Mentioned);
            Client.MessageReceived += new EventHandler<MessageEventArgs>(ServerCommand);

            Client.Disconnected += (s, e) =>
            {
                Console.WriteLine("Bot was disconnected.");
                AttemptConnect();
            };

            Client.UserJoined += (s, e) => SendBotMessage(e.Server, $"'{e.User.Name}' joined the server");
            Client.UserLeft   += (s, e) => SendBotMessage(e.Server, $"'{e.User.Name}' left the server");

        }

        private async void AttemptConnect(object sender = null, DisconnectedEventArgs e = null)
        {
            Console.Write("Connecting...");
            while (Client.State != DiscordClientState.Connected)
            {
                if(Client.State == DiscordClientState.Connecting)
                {
                    Thread.Sleep(1000);
                    Console.Write(".");
                    continue;
                }

                Thread.Sleep(1000);

                await Client.Connect(this.Email, this.Password);
            }
            Console.WriteLine("\nConnected");
        }

        public async void SendBotMessage(Server s, string msg)
        {
            var c = s.Channels.Where(x => x.Name.ToLower() == "bot-messages").SingleOrDefault();
            if (c != null) await Client.SendMessage(c, msg);
        }

        public void Start()
        {
            Handler.WaitOne();
        }
        
        private bool IsBot(long id)
        {
            var list = new List<long>()
            {
                Client.CurrentUserId,
                USER_BETASTARK_ID,
                USER_STARK_ID
            };

            return list.Contains(id);
        }

        private bool IsBot(User u)
        {
            return IsBot(u.Id);
        }

        /********************Handlers*************************/
        private string GetAfterCommand(string str)
        {
            return str.Remove(0, str.Split()[0].Length).Trim();
        }

        private void ServerCommand(object sender, MessageEventArgs e)

        {
            if (IsBot(e.User)) return;//Ignores self
            if (!e.Message.Text.StartsWith(PREDICATE)) return;

            Flair(sender, e);

            var rt = e.Message.RawText;
            if (rt.StartsWith(PREDICATE))
            {
                rt = rt.Remove(0, PREDICATE.Length);
                rt = rt.Split()[0].ToLower();
                GetCommands().Where(x => x.Method.Name.ToLower() == rt).FirstOrDefault().Invoke(sender, e);

                if(rt.StartsWith("starkravingmad") || rt.StartsWith("tarkravingmad"))
                {
                    HesStarkRavingMad(sender, e);
                }
            }
        }

        private void Mentioned(object sender, MessageEventArgs e)
        {
            if (e.Message.IsMentioningMe)
            {
                Senpai(sender, e);
            }
        }

        private void Senpai(object sender, MessageEventArgs e)
        {
            if (e.User.Id == USER_JOSH_ID)
            {
                if (e.Message.RawText.ToLower().Contains("good"))
                {
                    Client.SendMessage(e.Channel, "☜(⌒▽⌒)☞ Senpai master noticed me.");
                }
                if (e.Message.RawText.ToLower().Contains("bad"))
                {
                    Client.SendMessage(e.Channel, "ಥ_ಥ But Senpai, why?");
                }
            }
        }

        private void FitePost(object s, MessageEventArgs e)
        {
            if (e.Channel.Id == 125011643515011072 && Regex.Replace(e.Message.Text, @"[^a-zA-Z0-9]", "").ToLower() == "posted")//fite
            {
                Client.SendMessage(Client.GetChannel(125836510971822080), $"**<@{e.User.Id}> Posted.**");
            }
        }

        private bool UserInRole(User u, Server s, string r)
        {
            return u.HasRole(s.Roles.First(x=>x.Name.ToLower() == r.ToLower()));
        }
    }
}
