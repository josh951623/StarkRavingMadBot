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
        private const long USER_JOSH_ID = 93398876408524800;
        private const long USER_SPREADLINK_ID = 118798518138830853;
        /****************BOT SETTINGS***************/
        private const string PREDICATE = "$";
        private const string VERSION = "0.0.2";
        /******************************************/

        private List<long> ListeningServers = new List<long>();
        public Random Rand = new Random();
        private DiscordClient Client = new DiscordClient();

        private ManualResetEvent Handler = new ManualResetEvent(false);
        public event EventHandler<MessageEventArgs> MessageOnSever;

        public StarkRavingMadBot(string email, string pass)
        {
            //Display all log messages in the console
            Client.LogMessage += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

            Client.Connect(email, pass).Wait();
            Client.EditProfile(pass, "StarkRavingMadBot", email);
            
            //Add Main handler (just filters messages to ones sent on the server
            Client.MessageReceived += new EventHandler<MessageEventArgs>(CheckIfOnServer);

            //Add Handlers
            this.MessageOnSever += new EventHandler<MessageEventArgs>(Mentioned);
            this.MessageOnSever += new EventHandler<MessageEventArgs>(ServerCommand);


        }

        public void Start()
        {
            Handler.WaitOne();
        }

        public void AddServer(long serverId)
        {
            ListeningServers.Add(serverId);
        }

        /********************Handlers*************************/
        private void CheckIfOnServer(object s, MessageEventArgs e)
        {
            if (ListeningServers.Contains(e.Server.Id))
            {
                //Troll responses
                //JangoIsTalkingAboutHitlerAgain(s,e);
                //DincfusVeryOwnTimeVoid(s, e).Wait();
                OnlyOnes(s, e);

                FitePost(s, e);

                MessageOnSever(s, e);
            }
        }

        private void ServerCommand(object sender, MessageEventArgs e)
        {
            if (e.User.Id == Client.CurrentUserId) return;//Ignores self

            var rt = e.Message.RawText;
            if (rt.Contains('$'))
            {
                rt = rt.Substring(rt.IndexOf('$'));
                foreach (var c in GetCommands())
                {
                    var q = e.Message.Text.Split()[0];
                    if (c.Method.Name.ToLower() == rt.Split()[0].ToLower().Substring(1))
                    {
                        c.Invoke(sender, e);
                        return;
                    }
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
                if (e.Message.RawText.ToLower().Contains("good bot"))
                {
                    Client.SendMessage(e.Channel, "☜(⌒▽⌒)☞ Senpai master noticed me.").Wait();
                }
                if (e.Message.RawText.ToLower().Contains("bad bot"))
                {
                    Client.SendMessage(e.Channel, "ಥ_ಥ But Senpai, why?").Wait();
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
    }
}
