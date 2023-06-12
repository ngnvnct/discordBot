using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands.Attributes;

namespace discordBot.Commands {
    [SlashCommandGroup("mod", "moderation commands")]
    public class ModerationSCommands : ApplicationCommandModule {
        
        // MODERATION RELATED COMMANDS

        [SlashCommand("help", "list of moderation commands")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public async Task ModHelp(InteractionContext ctx) {

            await ctx.DeferAsync(true);


            var message = new DiscordWebhookBuilder()
                .AddEmbed(new DiscordEmbedBuilder {
                    Color = DiscordColor.PhthaloGreen,
                    Title = "Moderation related commands",
                    Description = "***ban*** --> bans a user\n" +
                                  "***kick*** --> kicks a user\n" +
                                  "***timeout*** --> mutes a user for a specified amount of time"
                });

            await ctx.EditResponseAsync(message);

        }
        
        [SlashCommand("ban", "ban a user")]
        [SlashRequirePermissions(Permissions.BanMembers)]
        public async Task Ban(InteractionContext ctx, [Option("user", "user to ban")] DiscordUser user,
                                                      [Option("reason", "reason for ban")] string reason = null) {
            await ctx.DeferAsync(true);

            if (ctx.Member.Permissions.HasPermission(Permissions.BanMembers)) {

                var member = (DiscordMember)user;
                
                await ctx.Guild.BanMemberAsync(member, 0, reason);

                var banMessage = new DiscordEmbedBuilder() {
                    Color = DiscordColor.DarkRed,
                    Title = $"Banned {member}",
                    Description = reason != null? reason:"no reason provided",
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(banMessage));
            }
            else {
                var nonAdminMessage = new DiscordEmbedBuilder() {
                    Color = DiscordColor.DarkRed,
                    Title = "You need to have ban permissions to execute this command"
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(nonAdminMessage));
            }
        }

        [SlashCommand("kick", "kick a user")]
        [SlashRequirePermissions(Permissions.KickMembers)]
        public async Task Kick(InteractionContext ctx, [Option("user", "user to kick")] DiscordUser user) {
            await ctx.DeferAsync(true);

            var member = (DiscordMember)user;
            await member.RemoveAsync();

            var kickMessage = new DiscordEmbedBuilder() {
                Color = DiscordColor.Black,
                Title = $"Kicked {member.Username}",
                Description = $"Kicked by {ctx.User.Username}"
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(kickMessage));
        }

        [SlashCommand("timeout", "mutes a user for a set amount of time")]
        [SlashRequirePermissions(Permissions.MuteMembers)]
        public async Task Timeout(InteractionContext ctx, [Option("user", "user to timeout")] DiscordUser user,
                                                          [Choice("60s", 60)][Choice("5m", 300)][Choice("10m", 600)]
                                                          [Option("duration", "duration of timeout")] long duration) {
            await ctx.DeferAsync();

            var muteUntil = DateTime.Now + TimeSpan.FromSeconds(duration);
            var member = (DiscordMember)user;

            await member.TimeoutAsync(muteUntil);

            var timeoutMessage = new DiscordEmbedBuilder() {
                Color = DiscordColor.None,
                Title = $"{member.Username} timed out",
                Description = $"Timed out until: {muteUntil}\nDuration: {TimeSpan.FromSeconds(duration)}"
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(timeoutMessage));
        }
    }
}
