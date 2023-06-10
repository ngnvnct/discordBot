using discordBot.Commands;
using discordBot.ExternalClasses;
using DSharpPlus;
using DSharpPlus.AsyncEvents;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discordBot {
    public class Bot {
        public DiscordClient? Client { get; private set; }
        public InteractivityExtension? Interactivity { get; private set; }
        public SlashCommandsExtension? SlashCommands { get; private set; }
        

        public async Task RunAsync() {
            // sets up our api in a way that prevents hardcoding the api-key by using a file reading system
            // coulda also used user secrets but whateva
            var Config = new CreateConfig();

            Client = new DiscordClient(Config.Configuration);
            Client.UseInteractivity(new InteractivityConfiguration() {
                Timeout = TimeSpan.FromMinutes(2)
            });

            // register commands
            SlashCommands = Client.UseSlashCommands();
            SlashCommands.RegisterCommands<SlashCommands>(Config.ConfigJson.GuildID);

            // error handling
            SlashCommands.SlashCommandErrored += OnSlashCommandError;

            Client.Ready += OnClientReady;
            Client.ComponentInteractionCreated += OnButtonPress;

            await Client.ConnectAsync();
            await Task.Delay(-1);

        }

        private async Task OnButtonPress(DiscordClient sender, ComponentInteractionCreateEventArgs e) {
            switch (e.Interaction.Data.CustomId) {
                case "leetcodeButton":
                    var leetcodeCommandsList = new DiscordEmbedBuilder() {
                        Color = DiscordColor.Gold,
                        Title = "LeetCode Commands",
                        Description = "***repo*** --> returns a link to the github repo with leetcode solutions\n\n" +
                                      "more commands for getting and returning repo info will be added with the implementation of the github api"
                    };

                    await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(leetcodeCommandsList));

                    break;
                case "communityButton":
                    var communityCommandsList = new DiscordEmbedBuilder() {
                        Color = DiscordColor.Lilac,
                        Title = "Community Commands",
                        Description = "***createpoll*** --> create a poll and vote with reactions set by the command\n\n" +
                                      "***caption*** --> attach any saved image and give it a caption"
                    };

                    await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(communityCommandsList));

                    break;
            }
        }

        private async Task OnSlashCommandError(SlashCommandsExtension sender, DSharpPlus.SlashCommands.EventArgs.SlashCommandErrorEventArgs e) {
            if (e.Exception is SlashExecutionChecksFailedException) {
                var castedException = (SlashExecutionChecksFailedException)e.Exception;
                string cooldownTimer = string.Empty;

                foreach (var check in castedException.FailedChecks) {
                    var cooldown = (SlashCooldownAttribute)check;
                    TimeSpan timeLeft = cooldown.GetRemainingCooldown(e.Context);
                    cooldownTimer = timeLeft.ToString(@"hh\:mm\:ss");
                }

                var cooldownMessage = new DiscordInteractionResponseBuilder()
                                            .AddEmbed(new DiscordEmbedBuilder() {
                                                Color = DiscordColor.Red,
                                                Title = "Max amount of uses for this command, please wait for the cooldown to end.",
                                                Description = cooldownTimer
                                            });
                await e.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, cooldownMessage);
            }
        }

        private Task OnClientReady(DiscordClient s, ReadyEventArgs e) {
            return Task.CompletedTask;
        }
    }
}
