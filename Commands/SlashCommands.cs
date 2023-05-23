using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discordBot.Commands {
	public class SlashCommands : ApplicationCommandModule {
		[SlashCommand("test", "slash command for testing purposes wow")]
		public async Task TestCommand(InteractionContext ctx) {
			await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("hello bro"));
		}
	}
}
