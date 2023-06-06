using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discordBot {
	public struct ConfigJson {
		[JsonProperty("token")]
		public string Token { get; private set; }
		[JsonProperty("prefix")]
		public string Prefix { get; private set; }
		[JsonProperty("guildID")]
		public ulong GuildID { get; private set; }
		[JsonProperty("githubToken")]
		public string GithubToken { get; private set; }
	}
}
