using discordBot.ExternalClasses;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Octokit;

namespace discordBot.Commands {

    public class GithubSCommands : ApplicationCommandModule {
        private GitHubClient CreateClient() {
            var config = new CreateConfig();
            var ghClient = new GitHubClient(new ProductHeaderValue("loogibot")) {
                Credentials = config.GitHubToken
            };
            return ghClient;
        }

        // LEETCODE REPO RELATED COMMANDS
        // GET /repos/{owner}/{repo}/contents/{path}
        // owner and repo will be hard coded since these commands only concern ty's LC repo

        [SlashCommand("repo", "Link to the LC solutions github repo.")]
        [SlashCooldown(1, 10, SlashCooldownBucketType.User)]
        public async Task GetRepoLink(InteractionContext ctx) {
            var linkMessage = new DiscordInteractionResponseBuilder()
                                .AddEmbed(new DiscordEmbedBuilder()

                                .WithColor(DiscordColor.Goldenrod)
                                .WithTitle("LeetCode solutions repository")
                                .WithUrl("https://github.com/thuanle123/Leetcode")
                                .WithAuthor("thuanle123")
                                .WithDescription("Solutions written in Python, C#, and Java!\n https://github.com/thuanle123/Leetcode")
                                );

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, linkMessage);
        }

        [SlashCommand("root", "root directory info. test command for now to play around w github api")]
        public async Task GetRoot(InteractionContext ctx) {

            await ctx.DeferAsync();

            var ghClient = CreateClient();

            var message = new DiscordMessageBuilder()
                .AddEmbed (new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.SpringGreen)
                    .WithTitle("***Root directory***")
                    .WithUrl("https://github.com/thuanle123/Leetcode"));

            var info = await ghClient.Repository.Content.GetAllContents("thuanle123", "Leetcode");
            List<RepositoryContent> directories = new List<RepositoryContent>();
            foreach (var item in info) {
                if (item.Type == ContentType.Dir) {
                    directories.Add(item);
                }
            }

            List<DiscordLinkButtonComponent> buttonList = new List<DiscordLinkButtonComponent>();
            for ( int i = 0; i <= directories.Count; i++ ) {
                // components can only hold 5 buttons, so we have to do this funny shit
                if (i != 0 && i % 5 == 0) {
                    DiscordLinkButtonComponent[] buttonArray = buttonList.ToArray();
                    message.AddComponents(buttonArray);
                    buttonList.Clear();
                }

                if (i == directories.Count) {
                    DiscordLinkButtonComponent[] buttonArray = buttonList.ToArray();
                    message.AddComponents(buttonArray);
                    break;
                }

                buttonList.Add(new DiscordLinkButtonComponent(directories[i].HtmlUrl, directories[i].Name));
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
        }

        [SlashCommand("solution", "select different options to find the solution you want")]
        public async Task GetSolution(InteractionContext ctx) {
            await ctx.DeferAsync();

            var ghClient = CreateClient();

            var info = await ghClient.Repository.Content.GetAllContents("thuanle123", "Leetcode");
            List<DiscordSelectComponentOption> languages = new List<DiscordSelectComponentOption>();    // must be a list of componentoptions
            foreach ( var item in info) {
                if (item.Type == ContentType.Dir && item.Name[0] != '.')
                    languages.Add(new DiscordSelectComponentOption(item.Name, item.Name));
            }
            var langOptions = languages.AsEnumerable(); // convert list to enumerable in order to pass it into a component

            var languageDropDown = new DiscordSelectComponent("directoryDropDown", "Select language...", langOptions);

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.HotPink)
                    .WithTitle("**Leetcode solutions explorer**")
                    .WithDescription("Choose your language"))
                .AddComponents(languageDropDown);

            await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
        }
    }
}
