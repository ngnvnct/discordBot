using DSharpPlus;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace discordBot {
	class Program {
		static void Main(string[] args) {
			var bot = new Bot();
			bot.RunAsync().GetAwaiter().GetResult();
		}
	}
}