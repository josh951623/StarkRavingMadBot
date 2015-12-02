using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;

namespace DiscordBot
{
    partial class StarkRavingMadBot
    {
        private const string USER_JOSH_ID = "93398876408524800";
        /****************BOT SETTINGS***************/
        private const string PREDICATE = "$";
        private const string VERSION = "0.0.2";
        /******************************************/

        private List<string> ListeningServers = new List<string>();
        private DiscordClient Client = new DiscordClient();
        private ManualResetEvent Handler = new ManualResetEvent(false);
        public event EventHandler<MessageEventArgs> MessageOnSever;


        public StarkRavingMadBot(string email, string pass)
        {
            //Display all log messages in the console
            Client.LogMessage += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

            Client.Connect(email, pass).Wait();

            //Add Main handler (just filters messages to ones sent on the server
            Client.MessageCreated += new EventHandler<MessageEventArgs>(CheckIfOnServer);

            //Add Handlers
            this.MessageOnSever += new EventHandler<MessageEventArgs>(Mentioned);
            this.MessageOnSever += new EventHandler<MessageEventArgs>(ServerCommand);
            
            Client.ChangeUsername("StarkRavingMadBot",email,pass);
        }

        public void Start()
        {
            Handler.WaitOne();
        }

        public void AddServer(string serverId)
        {
            ListeningServers.Add(serverId);
        }

        /********************Handlers*************************/
        private void CheckIfOnServer(object sender, MessageEventArgs e)
        {
            if (ListeningServers.Contains(e.Message.ServerId))
            {
                MessageOnSever(sender, e);
            }
        }

        private void ServerCommand(object sender, MessageEventArgs e)
        {
            if (e.Message.Text.StartsWith(PREDICATE))
            {
                foreach (var c in GetCommands())
                {
                    var q = e.Message.Text.Split()[0];
                    if (c.Method.Name.ToLower() == e.Message.Text.Split()[0].ToLower().Substring(1))
                    {
                        c.Invoke(sender, e);
                        return;
                    }
                }
                Client.SendMessage(e.Channel, "Command not found.");
            }
        }

        private void Mentioned(object sender, MessageEventArgs e)
        {
            if (e.Message.IsMentioningMe)
            {
                Client.SendMessage(e.Message.Channel, String.Format("I'm just a bot. Leave me alone <@{0}>.", e.Message.User.Id));
            }
        }
    }
}
