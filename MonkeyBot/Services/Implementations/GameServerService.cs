﻿using Discord;
using Discord.WebSocket;
using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;
using MonkeyBot.Services.Common.SteamServerQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MonkeyBot.Services
{
    public class GameServerService : IGameServerService
    {
        private DbService db;
        private DiscordSocketClient client;

        //private List<DiscordGameServerInfo> servers = new List<DiscordGameServerInfo>();

        public GameServerService(IServiceProvider provider)
        {
            db = provider.GetService<DbService>();
            client = provider.GetService<DiscordSocketClient>();
        }

        public void Initialize()
        {
            JobManager.AddJob(async () => await PostAllServerInfoAsync(), (x) => x.ToRunNow().AndEvery(1).Minutes());
        }

        public async Task AddServerAsync(IPEndPoint endpoint, ulong guildID, ulong channelID)
        {
            var server = new DiscordGameServerInfo(endpoint, guildID, channelID);
            //servers.Add(server);
            using (var uow = db.UnitOfWork)
            {
                await uow.GameServers.AddOrUpdateAsync(server);
                await uow.CompleteAsync();
            }
            await PostServerInfoAsync(server);
        }

        private async Task PostAllServerInfoAsync()
        {
            var servers = await GetServersAsync();
            foreach (var server in servers)
            {
                try
                {
                    await PostServerInfoAsync(server);
                }
                catch (Exception)
                {
                }
            }
        }

        private async Task PostServerInfoAsync(DiscordGameServerInfo discordGameServer)
        {
            if (discordGameServer == null)
                return;
            try
            {
                var server = new SteamGameServer(discordGameServer.IP);
                var serverInfo = await server?.GetServerInfoAsync();
                var playerInfo = (await server?.GetPlayersAsync()).Where(x => !string.IsNullOrEmpty(x.Name)).ToList();
                if (serverInfo == null || playerInfo == null)
                    return;
                var guild = client?.GetGuild(discordGameServer.GuildId);
                var channel = guild?.GetTextChannel(discordGameServer.ChannelId);
                if (guild == null || channel == null)
                    return;
                var builder = new EmbedBuilder();
                builder.WithColor(new Color(21, 26, 35));
                builder.WithTitle($"{serverInfo.Description} Server ({discordGameServer.IP.Address}:{serverInfo.Port})");
                builder.WithDescription(serverInfo.Name);
                builder.AddField("Online Players", $"{playerInfo.Count}/{serverInfo.MaxPlayers}");
                builder.AddField("Current Map", serverInfo.Map);
                if (playerInfo != null && playerInfo.Count > 0)
                    builder.AddField("Currently connected players:", string.Join(", ", playerInfo.Select(x => x.Name).Where(x => !string.IsNullOrEmpty(x)).OrderBy(x => x)));
                string connectLink = $"steam://rungameid/{serverInfo.GameId}//%20+connect%20{discordGameServer.IP.Address}:{serverInfo.Port}";
                builder.AddField("Connect", $"[Click to connect]({connectLink})");

                if (string.IsNullOrEmpty(discordGameServer.GameVersion))
                {
                    discordGameServer.GameVersion = serverInfo.GameVersion;
                    using (var uow = db.UnitOfWork)
                    {
                        await uow.GameServers.AddOrUpdateAsync(discordGameServer);
                        await uow.CompleteAsync();
                    }
                }
                else
                {
                    if (serverInfo.GameVersion != discordGameServer.GameVersion)
                    {
                        discordGameServer.GameVersion = serverInfo.GameVersion;
                        discordGameServer.LastVersionUpdate = DateTime.Now;
                        using (var uow = db.UnitOfWork)
                        {
                            await uow.GameServers.AddOrUpdateAsync(discordGameServer);
                            await uow.CompleteAsync();
                        }
                    }
                }
                string lastServerUpdate = "";
                if (discordGameServer.LastVersionUpdate.HasValue)
                    lastServerUpdate = $" (Last update: {discordGameServer.LastVersionUpdate.Value})";
                builder.AddField("Server version", $"{serverInfo.GameVersion}{lastServerUpdate}");

                builder.WithFooter($"Last check: {DateTime.Now}");
                if (discordGameServer.MessageId == null)
                {
                    discordGameServer.MessageId = (await channel?.SendMessageAsync("", false, builder.Build())).Id;
                    using (var uow = db.UnitOfWork)
                    {
                        await uow.GameServers.AddOrUpdateAsync(discordGameServer);
                        await uow.CompleteAsync();
                    }
                }
                else
                {
                    var msg = await channel.GetMessageAsync(discordGameServer.MessageId.Value) as Discord.Rest.RestUserMessage;
                    await msg?.ModifyAsync(x => x.Embed = builder.Build());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} Error getting updates for server {discordGameServer.IP}" + Environment.NewLine + ex.Message);
                throw;
            }
        }

        public async Task RemoveServerAsync(IPEndPoint endPoint, ulong guildID)
        {
            var servers = await GetServersAsync();
            var serverToRemove = servers.Where(x => x.IP.Address.ToString() == endPoint.Address.ToString() && x.IP.Port == endPoint.Port && x.GuildId == guildID).FirstOrDefault();
            if (serverToRemove == null)
                throw new ArgumentException("The specified server does not exist");
            if (serverToRemove.MessageId != null)
            {
                var guild = client.GetGuild(serverToRemove.GuildId);
                var channel = guild?.GetTextChannel(serverToRemove.ChannelId);
                var msg = await channel?.GetMessageAsync(serverToRemove.MessageId.Value) as Discord.Rest.RestUserMessage;
                await msg?.DeleteAsync();
            }

            using (var uow = db.UnitOfWork)
            {
                await uow.GameServers.RemoveAsync(serverToRemove);
                await uow.CompleteAsync();
            }
        }

        private async Task<List<DiscordGameServerInfo>> GetServersAsync()
        {
            using (var uow = db.UnitOfWork)
            {
                return await uow.GameServers.GetAllAsync();
            }
        }
    }
}