﻿using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using MonkeyBot.Database;
using MonkeyBot.Models;
using MonkeyBot.Services.Implementations.GameServers.SteamServerQuery;
using SteamQueryNet;
using SteamQueryNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonkeyBot.Services
{
    public class SteamGameServerService : BaseGameServerService
    {
        private readonly MonkeyDBContext dbContext;
        private readonly DiscordSocketClient discordClient;
        private readonly ILogger<SteamGameServerService> logger;

        public SteamGameServerService(MonkeyDBContext dbContext, DiscordSocketClient discordClient, ILogger<SteamGameServerService> logger)
            : base(GameServerType.Steam, dbContext, discordClient, logger)
        {
            this.dbContext = dbContext;
            this.discordClient = discordClient;
            this.logger = logger;
        }

        protected override async Task<bool> PostServerInfoAsync(GameServer discordGameServer)
        {
            if (discordGameServer == null)
            {
                return false;
            }

            ServerQuery serverQuery = null;

            try
            {
                using var udpClient = new UdpWrapper();
                serverQuery = new ServerQuery(udpClient, null);
                serverQuery.Connect(discordGameServer.ServerIP.ToString());
                ServerInfo serverInfo = await serverQuery.GetServerInfoAsync().ConfigureAwait(false);
                List<Player> players = (await serverQuery.GetPlayersAsync().ConfigureAwait(false)).Where(p => !p.Name.IsEmptyOrWhiteSpace()).ToList();
                if (serverInfo == null || players == null)
                {
                    return false;
                }
                SocketGuild guild = discordClient?.GetGuild(discordGameServer.GuildID);
                ITextChannel channel = guild?.GetTextChannel(discordGameServer.ChannelID);
                if (guild == null || channel == null)
                {
                    return false;
                }

                string onlinePlayers = players.Count > serverInfo.MaxPlayers
                    ? $"{serverInfo.MaxPlayers}(+{players.Count - serverInfo.MaxPlayers})/{serverInfo.MaxPlayers}"
                    : $"{players.Count}/{serverInfo.MaxPlayers}";

                var builder = new EmbedBuilder()
                    .WithColor(new Color(21, 26, 35))
                    .WithTitle($"{serverInfo.Game} Server ({discordGameServer.ServerIP.Address}:{serverInfo.Port})")
                    .WithDescription(serverInfo.Name)
                    .AddField("Online Players", onlinePlayers)
                    .AddField("Current Map", serverInfo.Map);

                if (players != null && players.Count > 0)
                {
                    _ = builder.AddField("Currently connected players:", string.Join(", ", players.Select(x => x.Name).Where(name => !name.IsEmpty()).OrderBy(x => x)).TruncateTo(1023));
                }

                //Discord removed support for protocols other than http or https so this currently makes no sense. Leaving it here, in case they re-enable it
                //string connectLink = $"steam://connect/{discordGameServer.ServerIP.Address}:{serverInfo.Port}";
                //_ = builder.AddField("Connect using this link", connectLink);

                if (discordGameServer.GameVersion.IsEmpty())
                {
                    discordGameServer.GameVersion = serverInfo.Version;
                    _ = dbContext.GameServers.Update(discordGameServer);
                    _ = await dbContext.SaveChangesAsync().ConfigureAwait(false);
                }
                else
                {
                    if (serverInfo.Version != discordGameServer.GameVersion)
                    {
                        discordGameServer.GameVersion = serverInfo.Version;
                        discordGameServer.LastVersionUpdate = DateTime.Now;
                        _ = dbContext.GameServers.Update(discordGameServer);
                        _ = await dbContext.SaveChangesAsync().ConfigureAwait(false);
                    }
                }


                string lastServerUpdate = "";
                if (discordGameServer.LastVersionUpdate.HasValue)
                {
                    lastServerUpdate = $" (Last update: {discordGameServer.LastVersionUpdate.Value})";
                }

                _ = builder.AddField("Server version", $"{serverInfo.Version}{lastServerUpdate}");
                _ = builder.WithFooter($"Last check: {DateTime.Now}");

                string chart = await GenerateHistoryChartAsync(discordGameServer, serverInfo.Players, serverInfo.MaxPlayers).ConfigureAwait(false);
                if (!chart.IsEmptyOrWhiteSpace())
                {
                    _ = builder.AddField("Player Count History", chart);
                }

                if (discordGameServer.MessageID.HasValue)
                {
                    if (await channel.GetMessageAsync(discordGameServer.MessageID.Value).ConfigureAwait(false) is IUserMessage existingMessage && existingMessage != null)
                    {
                        await existingMessage.ModifyAsync(x => x.Embed = builder.Build()).ConfigureAwait(false);
                    }
                    else
                    {
                        logger.LogWarning($"Error getting updates for server {discordGameServer.ServerIP}. Original message was removed.");
                        await RemoveServerAsync(discordGameServer.ServerIP, discordGameServer.GuildID).ConfigureAwait(false);
                        _ = await channel.SendMessageAsync($"Error getting updates for server {discordGameServer.ServerIP}. Original message was removed. Please use the proper remove command to remove the gameserver").ConfigureAwait(false);
                        return false;
                    }
                }
                else
                {
                    discordGameServer.MessageID = (await (channel?.SendMessageAsync("", false, builder.Build())).ConfigureAwait(false)).Id;
                    _ = dbContext.GameServers.Update(discordGameServer);
                    _ = await dbContext.SaveChangesAsync().ConfigureAwait(false);
                }

            }
            catch (TimeoutException tex)
            {
                logger.LogInformation(tex, $"Timed out trying to get steam server info for {discordGameServer.GameServerType} server {discordGameServer.ServerIP}");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"Error getting updates for {discordGameServer.GameServerType} server {discordGameServer.ServerIP}");
                throw;
            }
            finally
            {
                if (serverQuery != null)
                {
                    serverQuery.Dispose();
                }
            }
            return true;
        }


    }
}