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

		[SlashCommand("repo", "Link to the LC solutions github repo.")]
		public async Task GetRepoLink(InteractionContext ctx) {
			var linkMessage = new DiscordInteractionResponseBuilder()
								.AddEmbed(new DiscordEmbedBuilder()
								.WithColor(DiscordColor.Goldenrod)
								.WithTitle("LeetCode solutions repository")
								.WithAuthor("thuanle123")
								.WithDescription("Solutions written in Python, C#, and Java!\n https://github.com/thuanle123/Leetcode")
								);	
								
			await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, linkMessage);
		}
	}
}
