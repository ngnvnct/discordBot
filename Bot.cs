using discordBot.Commands;
using DSharpPlus;
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
		public CommandsNextExtension ?Commands { get; private set; }
		public SlashCommandsExtension ?SlashCommands { get; private set; }

		public async Task RunAsync() {
			// sets up our api in a way that prevents hardcoding the api-key by using a file reading system
			var json = string.Empty;
			using(var fs = File.OpenRead("config.json"))
			using(var sr = new StreamReader(fs, new UTF8Encoding(false)))
				json = await sr.ReadToEndAsync();

			var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
			
			// creating conditions of our new bot client instance
			var config = new DiscordConfiguration() {
				Intents = DiscordIntents.All,
				Token = configJson.Token,
				TokenType = TokenType.Bot,
				AutoReconnect = true,
			};

			Client = new DiscordClient(config);
			Client.UseInteractivity(new InteractivityConfiguration() {
				Timeout = TimeSpan.FromMinutes(2)
			});

			var commandsConfig = new CommandsNextConfiguration() {
				StringPrefixes = new string[] { configJson.Prefix },
				EnableMentionPrefix = true,
				EnableDms = true,
				EnableDefaultHelp = true,
				UseDefaultCommandHandler = false
			};


			// register commands
			Commands = Client.UseCommandsNext(commandsConfig);
			Commands.RegisterCommands<RealCommands>();
			Commands.RegisterCommands<TestCommands>();

			SlashCommands = Client.UseSlashCommands();
			SlashCommands.RegisterCommands<SlashCommands>(configJson.GuildID);

			// error handling
			Commands.CommandErrored += OnCommandError;
            SlashCommands.SlashCommandErrored += OnSlashCommandError;

			Client.Ready += OnClientReady;

			await Client.ConnectAsync();
			await Task.Delay(-1);

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
											.AddEmbed(new DiscordEmbedBuilder()
											.WithColor(DiscordColor.Red)
											.WithTitle("Max amount of uses for this command, please wait for the cooldown to end.")
											.WithDescription(cooldownTimer)
											);
				await e.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, cooldownMessage);
			}
        }


        private async Task OnCommandError(CommandsNextExtension sender, CommandErrorEventArgs e) {
            if (e.Exception is ChecksFailedException) {
				var castedException = (ChecksFailedException)e.Exception;
				string cooldownTimer = string.Empty;

				foreach (var check in castedException.FailedChecks) {
					var cooldown = (CooldownAttribute)check;
					TimeSpan timeLeft = cooldown.GetRemainingCooldown(e.Context);
					cooldownTimer = timeLeft.ToString(@"hh\:mm\:ss");
				}
				var cooldownMessage = new DiscordEmbedBuilder() {
					Title = "Wait for the cooldown to end",
					Description = cooldownTimer,
					Color = DiscordColor.Red
				};
				await e.Context.Channel.SendMessageAsync(cooldownMessage);
			}
        }

        private Task OnClientReady(DiscordClient s, ReadyEventArgs e) {
			return Task.CompletedTask;
		}
	}
}
