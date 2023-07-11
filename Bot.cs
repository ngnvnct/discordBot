using discordBot.Commands;
using discordBot.ExternalClasses;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Octokit;

namespace discordBot {
    public class Bot {
        public DiscordClient? Client { get; private set; }
        public InteractivityExtension? Interactivity { get; private set; }
        public SlashCommandsExtension? SlashCommands { get; private set; }
        public long LEETCODEREPOID = 455058602;

        private GitHubClient CreateClient() {
            var config = new CreateConfig();
            var ghClient = new GitHubClient(new ProductHeaderValue("loogibot")) {
                Credentials = config.GitHubToken
            };
            return ghClient;
        }

        public async Task RunAsync() {
            // sets up our api in a way that prevents hardcoding the api-key by using a file reading system
            // coulda also used user secrets but whateva
            var Config = new CreateConfig();

            Client = new DiscordClient(Config.Configuration);
            
            // set up interactivity
            Client.UseInteractivity(new InteractivityConfiguration() {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(30)
            });

            // register commands
            SlashCommands = Client.UseSlashCommands();
            SlashCommands.RegisterCommands<GithubSCommands>(Config.ConfigJson.GuildID);
            SlashCommands.RegisterCommands<ModerationSCommands>(Config.ConfigJson.GuildID);
            SlashCommands.RegisterCommands<CommunitySCommands>(Config.ConfigJson.GuildID);

            Client.Ready += OnClientReady;

            // event handler subscriptions
            Client.ComponentInteractionCreated += OnButtonPress;
            Client.ComponentInteractionCreated += DropDownEvent;
            Client.ModalSubmitted += ModalSubmissionEventHandler;

            // error handling
            SlashCommands.SlashCommandErrored += OnSlashCommandError;

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private async Task ModalSubmissionEventHandler(DiscordClient sender, ModalSubmitEventArgs e) {
            if (e.Interaction.Type == InteractionType.ModalSubmit) {

                var values = e.Values;  // represents all components in the modal
                var timeStamp = e.Interaction.CreationTimestamp;

                var confessMessage = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithColor(DiscordColor.Brown)
                        .WithTitle($"**{values.Values.First()}**")
                        .WithFooter(timeStamp
                            .ToLocalTime()
                            .DateTime
                            .ToString())
                        );

                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Confession received, amen {DiscordEmoji.FromName(sender, ":pray:")}").AsEphemeral(true));

                await e.Interaction.Channel.SendMessageAsync(confessMessage);
            }
        }

        private async Task DropDownEvent(DiscordClient sender, ComponentInteractionCreateEventArgs e) {
            
            var ghClient = CreateClient();
            var interactivity = sender.GetInteractivity();

            switch (e.Id) {
                // TODO: fix code duplication for getting github repo content with path, making the list, and outputting the message
                case "directoryDropDown":
                    await e.Interaction.DeferAsync();

                    var language = e.Values[0];
                    string path = $"{language}/";

                    if (language == "JavaMaven")
                        path += "src/test/java/";
                    else if (language == "csharp")
                        path += "solution_and_tests/";

                    var info = await ghClient.Repository.Content.GetAllContents(LEETCODEREPOID, path);
                    var message = new DiscordMessageBuilder();
                    List<DiscordSelectComponentOption> optionsList = new List<DiscordSelectComponentOption>();
                
                    if (language == "sql") {
                        foreach (var item in info)
                            optionsList.Add(new DiscordSelectComponentOption(item.Name, $"{path}{item.Name}/"));

                        var SQLoptions = optionsList.AsEnumerable();

                        var SQLdropDown = new DiscordSelectComponent("problemsDropDown", "Select problem...", SQLoptions);
                        message.AddComponents(SQLdropDown);

                        await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder(message));

                        break;
                    }

                    foreach (var item in info) {
                        if (item.Type == ContentType.Dir)
                            optionsList.Add(new DiscordSelectComponentOption(item.Name, $"{path}{item.Name}"));
                    }

                    var options = optionsList.AsEnumerable();

                    var dropDown = new DiscordSelectComponent("algoDropDown", "Select algorithm...", options);

                    message.AddComponents(dropDown);

                    await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder(message));
                    break;

                case "algoDropDown":
                    await e.Interaction.DeferAsync();
                    
                    var selectedAlgoPath = e.Values[0];

                    var algoContent = await ghClient.Repository.Content.GetAllContents(LEETCODEREPOID, selectedAlgoPath);
                    List<DiscordSelectComponentOption> problems = new List<DiscordSelectComponentOption>();

                    foreach (var item in algoContent) {
                        problems.Add(new DiscordSelectComponentOption(item.Name, $"{selectedAlgoPath}{item.Name}"));
                    }
                    var problemsOptions = problems.AsEnumerable();
                    var problemsDropDown = new DiscordSelectComponent("problemsDropDown", "Select problem...", problemsOptions);

                    var problemsMessage = new DiscordMessageBuilder()
                        .AddComponents(problemsDropDown);

                    await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder(problemsMessage));
                    break;

                case "problemsDropDown":
                    await e.Interaction.DeferAsync();

                    var selectedSolution = e.Values.FirstOrDefault();
                    var solutionInfo = await ghClient.Repository.Content.GetAllContents(LEETCODEREPOID, selectedSolution);

                    var solutionContent = await ghClient.Repository.Content.GetRawContent("thuanle123", "Leetcode", selectedSolution);
                    var solutionContentAsString = System.Text.Encoding.UTF8.GetString(solutionContent);
                    var messagePages = interactivity.GeneratePagesInContent($"{solutionContentAsString}");

                    await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"{e.User.Mention} -- Solution for: {solutionInfo[0].Name}"));
                    await e.Channel.SendPaginatedMessageAsync(e.User, messagePages);

                    break;
            }
        }

        private async Task OnButtonPress(DiscordClient sender, ComponentInteractionCreateEventArgs e) {
            switch (e.Interaction.Data.CustomId) {
                case "leetcodeButton":
                    var leetcodeCommandsList = new DiscordEmbedBuilder() {
                        Color = DiscordColor.Gold,
                        Title = "LeetCode Commands",
                        Description = "***repo*** --> returns a link to the github repo with leetcode solutions\n" +
                                      "***root*** --> returns the root directory contents\n" +
                                      "***solution*** --> initiates the repo browser via dropdown menus\n\n" +
                                      "more commands for getting and returning repo info will be added with the implementation of the github api"
                    };

                    await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(leetcodeCommandsList));

                    break;
                case "communityButton":
                    var communityCommandsList = new DiscordEmbedBuilder() {
                        Color = DiscordColor.Lilac,
                        Title = "Community Commands",
                        Description = "***createpoll*** --> create a poll and vote with reactions set by the command\n" +
                                      "***caption*** --> attach any saved image and give it a caption\n" +
                                      "***confess*** --> send an anonymous message to this channel"
                    };

                    await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(communityCommandsList));

                    break;
            }
        }

        private async Task OnSlashCommandError(SlashCommandsExtension sender, DSharpPlus.SlashCommands.EventArgs.SlashCommandErrorEventArgs e) {
            if (e.Exception is SlashExecutionChecksFailedException slex) {
                var castedException = (SlashExecutionChecksFailedException)e.Exception;

                foreach (var check in castedException.FailedChecks) {
                    if (check is SlashCooldownAttribute cdAtt) {
                        string cooldownTimer = string.Empty;
                        var cooldown = (SlashCooldownAttribute)check;
                        TimeSpan timeLeft = cooldown.GetRemainingCooldown(e.Context);
                        cooldownTimer = timeLeft.ToString(@"hh\:mm\:ss");

                        var cooldownMessage = new DiscordInteractionResponseBuilder()
                                                    .AddEmbed(new DiscordEmbedBuilder() {
                                                        Color = DiscordColor.Red,
                                                        Title = "Max amount of uses for this command, please wait for the cooldown to end.",
                                                        Description = $"Time Remaining: {cooldownTimer}"
                                                    });

                        await e.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, cooldownMessage);
                    }

                    else if (check is SlashRequirePermissionsAttribute slAtt) {
                        var permissionsMessage = new DiscordInteractionResponseBuilder()
                            .AddEmbed (new DiscordEmbedBuilder() {
                                Color = DiscordColor.Red,
                                Title = "Access denied",
                                Description = $"You don't have {slAtt.Permissions} permissions to execute this command."
                            });

                        await e.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, permissionsMessage);
                    }
                }
            }
        }

        private Task OnClientReady(DiscordClient s, ReadyEventArgs e) {
            return Task.CompletedTask;
        }
    }
}
