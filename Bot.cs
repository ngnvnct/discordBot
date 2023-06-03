using discordBot.Commands;
using discordBot.ExternalClasses;
using DSharpPlus;
using DSharpPlus.AsyncEvents;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discordBot {
    public class Bot {
        public DiscordClient ?Client { get; private set; }
        public InteractivityExtension ?Interactivity { get; private set; }
        public SlashCommandsExtension ?SlashCommands { get; private set; }

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

        private Task OnButtonPress(DiscordClient sender, ComponentInteractionCreateEventArgs e) {
            switch (e.Interaction.Data.CustomId) {
                case "ex":
                    // functionality here
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
