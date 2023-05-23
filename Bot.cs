using discordBot.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
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

			

			Commands = Client.UseCommandsNext(commandsConfig);
			SlashCommands = Client.UseSlashCommands();
			// register commands
			Commands.RegisterCommands<RealCommands>();
			Commands.RegisterCommands<TestCommands>();
			Commands.RegisterCommands<GameCommands>();
			SlashCommands.RegisterCommands<SlashCommands>(configJson.GuildID);



			Client.Ready += OnClientReady;

			await Client.ConnectAsync();
			await Task.Delay(-1);

		}

		private Task OnClientReady(DiscordClient s, ReadyEventArgs e) {
			return Task.CompletedTask;
		}
	}
}
