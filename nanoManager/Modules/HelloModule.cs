using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace nanoManager.Modules
{
    public class HelloModule : ModuleBase
    {
        [Command("hey")]
        public async Task HelloCommand()
        {
            var sb = new StringBuilder();

            //var user = Context.User;
            sb.AppendLine("Hey!");

            await ReplyAsync(sb.ToString());
        }
    }
}
