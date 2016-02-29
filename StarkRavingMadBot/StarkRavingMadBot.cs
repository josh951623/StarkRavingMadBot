using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using ChatterBotAPI;

namespace StarkRavingMadBot
{
    partial class StarkRavingMadBot
	{

		private const long USER_JOSH_ID = 93398876408524800;//Me!

		#if DEBUG
		public const string PREDICATE = "b$";
		#else  
		public const string PREDICATE = "$";
#endif

        private ChatterBot Cleverbot { get; set; }
        private Dictionary<ulong,ChatterBotSession> CleverbotSessions { get; set; }

        /****************BOT SETTINGS***************/
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

			AttemptConnect ();
#if DEBUG
            //Add Handlers
            //Client.LogMessage += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");
#endif
            Client.MessageReceived += new EventHandler<MessageEventArgs> (Mentioned);
            Client.MessageReceived += new EventHandler<MessageEventArgs>(ServerCommand);

#if !DEBUG
            Client.MessageReceived += new EventHandler<MessageEventArgs>(JangoIsTalkingAboutHitlerAgain);
#endif
			Client.ServerUnavailable += (s, e) => {
				Console.WriteLine ("Server unavail");
			};
#if !DEBUG
            Client.UserJoined += UserJoinedEvent;
#endif

            Cleverbot = (new ChatterBotFactory()).Create(ChatterBotType.CLEVERBOT);
            CleverbotSessions = new Dictionary<ulong, ChatterBotSession>();
		}

        private async void UserJoinedEvent(object sender, UserEventArgs e)
        {
            SendBotMessage(e.Server, $"'{e.User.Name}' (UID: {e.User.Id}) joined the server");
            await Task.Delay(5000);
            await e.User.SendMessage("Welcome to the new and improved MBTI v3. Let our animatronics direct your attention to #code-of-conduct (they're out of date right now we're working on a revision....), and remember to have fun!");

        }

        private void AttemptConnect(object sender = null, DisconnectedEventArgs e = null)
        {
            Console.Write("Connecting...");
            Client.Connect(this.Email, this.Password);
            while (Client.State != ConnectionState.Connected)
            {
                Thread.Sleep(500);
                Console.Write(".");
            }
            Console.WriteLine("\nConnected");
        }

        public void SendBotMessage(Server s, string msg)
        {
            var c = s.TextChannels.Where(x => x.Name.ToLower() == "bot-messages").SingleOrDefault();
            if (c != null) c.SendMessage(msg);
        }

        public void Start()
        {
            Handler.WaitOne();
        }
        
        private bool IsBot(ulong id)
        {
            var list = new List<ulong>()
            {
                133695051099406336,//Typey
                121281613660160000,//Stark
                134799811147726848,//Beta Stark
            };

            return list.Contains(id);
        }

        private bool IsBot(User u)
        {
            return IsBot(u.Id);
        }

        /********************Handlers*************************/
        private void ServerCommand(object sender, MessageEventArgs e)
        {
            if (IsBot(e.User)) return;//Ignores self
            if (!e.Message.Text.StartsWith(PREDICATE)) return;//should start with predicate

            Flair(sender, e);
			var c = GetCommands ();
			c.Where(x => x.IsCommand(e.Message.Text)).FirstOrDefault().Method.Invoke(sender, e);
        }

        private void Mentioned(object sender, MessageEventArgs e)
        {
#if DEBUG
            if (IsBot(e.User)) return;//Ignores self
            if (e.Message.Text.StartsWith(PREDICATE)) return;//shouldn't start with predicate

            if (e.Message.IsMentioningMe())
            {
                e.Channel.SendIsTyping();
                if(!CleverbotSessions.ContainsKey(e.User.Id))
                {
                    CleverbotSessions.Add(e.User.Id, Cleverbot.CreateSession());
                }
                e.Channel.SendMessage( $"<@{e.User.Id}> {CleverbotSessions[e.User.Id].Think(e.Message.Text)}");
            }
#endif
        }

        private void Senpai(object sender, MessageEventArgs e)
        {
            if (e.User.Id == USER_JOSH_ID)
            {
                if (e.Message.RawText.ToLower().Contains("good"))
                {
                    e.Channel.SendMessage("☜(⌒▽⌒)☞");
                }
                if (e.Message.RawText.ToLower().Contains("bad"))
                {
                    e.Channel.SendMessage("ಥ_ಥ");
                }
            }
        }

        private void FitePost(object s, MessageEventArgs e)
        {
            if (e.Channel.Id == 125011643515011072 && Regex.Replace(e.Message.Text, @"[^a-zA-Z0-9]", "").ToLower() == "posted")//fite
            {
                Client.GetChannel(125836510971822080).SendMessage($"**<@{e.User.Id}> Posted.**");
            }
        }

        private bool UserInRole(User u, Server s, string r)
        {
            return u.HasRole(s.Roles.First(x=>x.Name.ToLower() == r.ToLower()));
        }
    }
}
