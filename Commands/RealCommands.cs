using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discordBot.Commands {
	public class RealCommands : BaseCommandModule {
		[Command("link")]
		[Cooldown(1, 10, CooldownBucketType.User)]
		public async Task GetLink(CommandContext ctx) {
			var embedMessage = new DiscordMessageBuilder()
				.AddEmbed(new DiscordEmbedBuilder()
				.WithTitle("Leetcode solutions repository")
				.WithAuthor("TyVip")
				.WithDescription("Solutions written in Python, C#, and Java! \n https://github.com/thuanle123/Leetcode")
				.WithColor(DiscordColor.Lilac)
				);
			await ctx.Channel.SendMessageAsync(embedMessage);
		}
	}
}
