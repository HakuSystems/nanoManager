using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nanoManager.Api;
using System.Net.Http;
using Newtonsoft.Json;
using Discord;
using System.Net;

namespace nanoManager.Modules
{
    public class CreateRedeemModule : ModuleBase
    {
        
        [Command("Create")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task CreateRedeemCommand()
        {
            await Task.Run(async () =>
            {
                await NanoApiManager.CreateRedeem(1, 1);
                await ReplyAsync("", false, NanoApiManager.CREATEEMBED);
            });
        }
    }
}
