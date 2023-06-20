using DSharpPlus;
using Newtonsoft.Json;
using Octokit;
using System.Text;

namespace discordBot.ExternalClasses {
    internal class CreateConfig {
        public ConfigJson ConfigJson { get; internal set; }
        public DiscordConfiguration Configuration { get; internal set; }
        public Credentials GitHubToken { get; internal set; }
        public CreateConfig() {
            var json = string.Empty;
            using(var fs = File.OpenRead("config.json"))
            using(var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            this.ConfigJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            this.GitHubToken = new Credentials(this.ConfigJson.GithubToken);

            this.Configuration = new DiscordConfiguration() {
                Intents = DiscordIntents.All,
                Token = this.ConfigJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };

        }
    }
}
