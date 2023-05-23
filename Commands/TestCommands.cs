using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

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

		// making a poll with interactivity module
		[Command("poll")]
		// string[] question must be the last parameter
		public async Task PollCommand(CommandContext ctx, int timeLimit, string option1, string option2, string option3, string option4, params string[] question) {
			var interactivity = ctx.Client.GetInteractivity();  // import from client
			int count1 = 0;
			int count2 = 0;
			int count3 = 0;
			int count4 = 0;
			TimeSpan timer = TimeSpan.FromSeconds(timeLimit);	// converts timeLimit to an acceptable type

			string title = string.Join(" ", question);
			DiscordEmoji[] optionEmojis = { DiscordEmoji.FromName(ctx.Client, ":one:", false),
											DiscordEmoji.FromName(ctx.Client, ":two:", false),
											DiscordEmoji.FromName(ctx.Client, ":three:", false),
											DiscordEmoji.FromName(ctx.Client, ":four:", false) };

			string optionsStrings = (	$"{optionEmojis[0]} | {option1}\n" +
										$"{optionEmojis[1]} | {option2}\n" +
										$"{optionEmojis[2]} | {option3}\n" +
										$"{optionEmojis[3]} | {option4}"	);

			var pollMessage = new DiscordMessageBuilder()
				.AddEmbed(new DiscordEmbedBuilder()
				.WithTitle(title)
				.WithDescription(optionsStrings)
				.WithColor(DiscordColor.Sienna)
				);

			var message = await ctx.RespondAsync(pollMessage);

			foreach(var emoji in optionEmojis) {
				await message.CreateReactionAsync (emoji);
			}

			var result = await interactivity.CollectReactionsAsync(message, timer);

			// count the results
			foreach (var emoji in result) {
				if (emoji.Emoji == optionEmojis[0]) {
					count1++;
				}
				if (emoji.Emoji == optionEmojis[1]) {
					count2++;
				}
				if (emoji.Emoji == optionEmojis[2]) {
					count3++;
				}
				if (emoji.Emoji == optionEmojis[3]) {
					count4++;
				}
			}


			int totalVotes = count1 + count2 + count3 + count4;
			string resultStrings = ($"{optionEmojis[0]} | {count1} votes\n" +
										$"{optionEmojis[1]} | {count2} votes\n" +
										$"{optionEmojis[2]} | {count3} votes\n" +
										$"{optionEmojis[3]} | {count4} votes\n\n" +
										$"Total votes: {totalVotes}");

			var resultMessage = new DiscordMessageBuilder()
				.AddEmbed(new DiscordEmbedBuilder()
				.WithColor(DiscordColor.Green)
				.WithTitle("Results of poll")
				.WithDescription(resultStrings)
				);

			await ctx.Channel.SendMessageAsync(resultMessage);
		}
	}
}

