using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discordBot.ExternalClasses {
    internal class CreateConfig {
        public ConfigJson ConfigJson { get; internal set; }
        public DiscordConfiguration Configuration { get; internal set; }

        public CreateConfig() {
            var json = string.Empty;
            using(var fs = File.OpenRead("config.json"))
            using(var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            this.ConfigJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            this.Configuration = new DiscordConfiguration() {
                Intents = DiscordIntents.All,
                Token = this.ConfigJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };

        }
    }
}
