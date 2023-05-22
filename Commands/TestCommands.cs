using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace discordBot.Commands {
	public class TestCommands : BaseCommandModule {

		// declare command and name and then establish a method to start the command
		[Command("test")]
		// all commands must have type CommandContext as the first parameter or else it wont execute
		public async Task TestCommand(CommandContext ctx) {
			await ctx.Channel.SendMessageAsync("hello");
		}

		[Command("add")]
		public async Task Addition(CommandContext ctx, int x, int y) {
			await ctx.Channel.SendMessageAsync((x+y).ToString());
		}

		// a little easier way to make embedded messages but not as customizable
		[Command("embed2")]
		public async Task Embed2(CommandContext ctx) {
			var embedMessage = new DiscordEmbedBuilder() {
				Title = "asdf title",
				Description = "qwert description",
				Color = DiscordColor.CornflowerBlue
			};
			await ctx.Channel.SendMessageAsync(embed: embedMessage);	// in this method, we must specify that the passed in message is an embed message w the "embed:"
		}
	}
}
