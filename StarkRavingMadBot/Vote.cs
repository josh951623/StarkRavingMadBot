
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Discord;

//namespace DiscordBot.Commands
//{
//    partial class StarkRavingMadBot
//    {
//        private class VoteOption
//        {
//            public string Name { get; set; }
//            public int Votes { get; set; }
//        }
//        private class Poll
//        {
//            public List<string> Voters = new List<string>();
//            public List<VoteOption> Votes = new List<VoteOption>();
//        }
//        private Dictionary<string, Poll> OpenPolls = new Dictionary<string, Poll>();
//        private void Vote(object s, MessageEventArgs e)
//        {
//            var r = e.Message.Text.Split().ToList();
//            r.RemoveAt(0);

//            if (r[0] == "cast")
//            {
//                var chanPoll = OpenPolls[e.Message.ChannelId];
//                try
//                {
//                    if (chanPoll.Voters.Contains(e.Message.UserId))
//                    {
//                        Client.SendMessage(e.Channel, $"You've already voted <@{e.Message.UserId}>.").Wait();
//                        return;
//                    }
//                    chanPoll.Votes
//                        .Where(x => x.Name.ToLower() == r[1].ToLower())
//                        .Single().Votes++;
//                    chanPoll.Voters.Add(e.Message.UserId);
//                }
//                catch
//                {
//                    Client.SendMessage(e.Channel, "Invalid voting option").Wait();
//                }
//            }
//            else if (r[0] == "result")
//            {
//                var chanPoll = OpenPolls[e.Message.ChannelId];
//                var str = new StringBuilder();
//                str.AppendLine("Voting results:");
//                foreach (var v in chanPoll.Votes.OrderByDescending(x => x.Votes))
//                {
//                    str.AppendLine($" - {v.Name}: {v.Votes} Vote(s)");
//                }
//                str.AppendLine($"Thank you to all {chanPoll.Voters.Count()} participant(s).");
//                Client.SendMessage(e.Channel, str.ToString());
//                OpenPolls.Remove(e.Message.ChannelId);
//            }
//            else if (r[0] == "start")
//            {
//                if (OpenPolls.ContainsKey(e.Message.ChannelId))
//                {
//                    Client.SendMessage(e.Message.ChannelId, "This channel already has a poll going. You must end it before starting a new one").Wait();
//                }

//                r.RemoveAt(0);
//                var p = new Poll();
//                foreach (var o in r)
//                {
//                    p.Votes.Add(new VoteOption { Name = o.ToString().Trim().ToLower(), Votes = 0 });
//                }
//                OpenPolls.Add(e.Message.ChannelId, p);
//                Client.SendMessage(e.Channel, $"Poll started. Options are: {p.Votes.OrderByDescending(x => x.Name).Select(x => x.Name).Aggregate((n, c) => $"{c}, {n}")}");
//            }
//            else if (r[0] == "kick")
//            {

//            }
//        }
//    }
//}