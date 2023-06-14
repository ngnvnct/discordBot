﻿using discordBot.ExternalClasses;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace discordBot.Commands {
    public class GithubSCommands : ApplicationCommandModule {

        // LEETCODE REPO RELATED COMMANDS

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
            // GET /repos/{owner}/{repo}/contents/{path}
            // owner and repo will be hard coded since these commands only concern ty's LC repo

            await ctx.DeferAsync();

            var config = new CreateConfig();
            var ghClient = new GitHubClient(new ProductHeaderValue("loogibot")) {
                Credentials = config.GitHubToken
            };


            var info = await ghClient.Repository.Content.GetAllContents("thuanle123", "Leetcode");

            List<string> directories = new List<string>();
            List<string> directoryLinks = new List<string>();

            foreach (var item in info) {
                if (item.Type == ContentType.Dir) {
                    directories.Add(item.Name);
                    directoryLinks.Add(item.HtmlUrl);
                }
            }

            var message = new DiscordMessageBuilder()
                .AddEmbed (new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.SpringGreen)
                    .WithTitle("***Root directory***")
                    .WithUrl("https://github.com/thuanle123/Leetcode"));


            for (int i = 0; i < directories.Count; i++) {
                message.AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Gold)
                    .WithTitle(directories[i])
                    .WithUrl(directoryLinks[i])
                    );
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
        }
    }
}
