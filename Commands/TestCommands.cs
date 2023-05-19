using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
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
		[Command("link")]
		public async Task getLink(CommandContext ctx) {
			await ctx.Channel.SendMessageAsync("Here is a link to all of TyVip's LeetCode solutions! Written in Python, C#, and Java.\n https://github.com/thuanle123/Leetcode");
		}
	}
}
