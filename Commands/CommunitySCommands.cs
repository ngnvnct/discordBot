using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discordBot.Commands {
    public class CommunitySCommands : ApplicationCommandModule {

        // COMMUNITY RELATED COMMANDS

        [SlashCommand("help", "general help command for all commands offered")]
        public async Task HelpCommand(InteractionContext ctx) {

            await ctx.DeferAsync();

            var leetcodeButton = new DiscordButtonComponent(ButtonStyle.Primary, "leetcodeButton", "LeetCode");
            var communityButton = new DiscordButtonComponent(ButtonStyle.Primary, "communityButton", "Community");

            var helpMessage = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Lilac)
                .WithTitle("Help menu")
                .WithDescription("Click on a button for more information about the command!");

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(helpMessage).AddComponents(leetcodeButton, communityButton));
        }

        [SlashCommand("createpoll", "create a poll and let users vote")]
        [SlashCooldown(1, 60, SlashCooldownBucketType.Channel)]
        public async Task PollCommand(InteractionContext ctx, [Option("question", "poll subject")] string question,
                                                                [Choice("60s", 60)][Choice("30s", 30)][Choice("15s", 15)]
                                                                    [Option("timelimit", "the amount of time that the poll is active")] long timelimit,
                                                                [Option("option1", "first option")] string option1,
                                                                [Option("option2", "second option")] string option2,
                                                                [Option("option3", "third option")] string option3,
                                                                [Option("option4", "fourth option")] string option4) {

            await ctx.DeferAsync();

            var interactivity = ctx.Client.GetInteractivity();
            int count1 = 0; int count2 = 0; int count3 = 0; int count4 = 0;
            TimeSpan timer = TimeSpan.FromSeconds(timelimit);
            string title = String.Join("", question);

            DiscordEmoji[] optionsEmojis = {    DiscordEmoji.FromName(ctx.Client, ":one:", false),
                                                DiscordEmoji.FromName(ctx.Client, ":two:", false),
                                                DiscordEmoji.FromName(ctx.Client, ":three:", false),
                                                DiscordEmoji.FromName(ctx.Client, ":four:", false) };

            string pollOptionsStrings = ($"{optionsEmojis[0]} | {option1}\n" +
                                            $"{optionsEmojis[1]} | {option2}\n" +
                                            $"{optionsEmojis[2]} | {option3}\n" +
                                            $"{optionsEmojis[3]} | {option4}");

            var pollMessage = new DiscordEmbedBuilder() {
                Color = DiscordColor.Sienna,
                Title = title,
                Description = pollOptionsStrings
            };

            var message = await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(pollMessage));

            foreach (var emoji in optionsEmojis) {
                await message.CreateReactionAsync(emoji);
            }

            var result = await interactivity.CollectReactionsAsync(message, timer);

            foreach (var emoji in result) {
                if (emoji.Emoji == optionsEmojis[0])
                    count1++;
                if (emoji.Emoji == optionsEmojis[1])
                    count2++;
                if (emoji.Emoji == optionsEmojis[2])
                    count3++;
                if (emoji.Emoji == optionsEmojis[3])
                    count4++;
            }

            int totalVotes = count1 + count2 + count3 + count4;

            string resultsStrings = ($"{optionsEmojis[0]} | {count1} votes\n" +
                                        $"{optionsEmojis[1]} | {count2} votes\n" +
                                        $"{optionsEmojis[2]} | {count3} votes\n" +
                                        $"{optionsEmojis[3]} | {count4} votes\n\n" +
                                        $"Total votes: {totalVotes}");

            var resultMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder() {
                    Color = DiscordColor.Green,
                    Title = "Result of poll",
                    Description = resultsStrings
                });

            await ctx.Channel.SendMessageAsync(resultMessage);

        }

        [SlashCommand("caption", "attach an image and then write a caption")]
        public async Task CaptionCommand(InteractionContext ctx, [Option("image", "attach an image to caption")] DiscordAttachment image,
                                                                    [Option("caption", "bottom text")] string caption) {

            await ctx.DeferAsync();

            var captionMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()

                .WithImageUrl(image.Url)
                .WithFooter(caption)
                );

            await ctx.DeleteResponseAsync();
            await ctx.Channel.SendMessageAsync(captionMessage);
        }


        [SlashCommand("test", "test slash command using whatever the fk")]
        public async Task TestCommand(InteractionContext ctx, [Choice("variable", "value")]
                                                                [Option("displayName", "description of what to input")] string text) {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                                                                .WithContent("test"));

            var embedMessage = new DiscordEmbedBuilder() {
                Color = DiscordColor.Chartreuse,
                Title = "test",
                Description = text
            };

            await ctx.Channel.SendMessageAsync(embed: embedMessage);
        }
    }
}
