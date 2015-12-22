//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using Discord;

//namespace DiscordBot
//{
//    partial class StarkRavingMadBot
//    {
//        private void Flair(object s, MessageEventArgs e)
//        {
//            if (e.User.Id != USER_JOSH_ID) return;

//            //Client.AddRoleMember("118879902710759425", "118730890363928576", USER_JOSH_ID);

//            var type = e.Message.RawText.ToLower();
//            type = type.Substring(type.IndexOf("$"));
//            type = type.Substring(type.IndexOf(' ')).Trim().Split()[0].Trim();
//            var reg = new Regex("[ie][ns][tf][jp]");
//            if (reg.IsMatch(type))
//            {
//                var r = e.Server.Roles.Where(x => x.Name == type && x.Server == e.Server).Single();
//                e.User.Roles.Where(x => reg.IsMatch(x.Name)).ToList();
//                Client.
//            }
//        }
//    }
//}
