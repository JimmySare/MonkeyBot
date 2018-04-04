﻿using CodeHollow.FeedReader;
using Discord;
using Discord.Commands;
using dokas.FluentStrings;
using MonkeyBot.Common;
using MonkeyBot.Preconditions;
using MonkeyBot.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MonkeyBot.Modules
{
    [Group("Feeds")]
    [Name("Feeds")]
    [MinPermissions(AccessLevel.ServerAdmin)]
    [RequireContext(ContextType.Guild)]
    public class FeedModule : ModuleBase
    {
        private readonly IFeedService feedService;

        public FeedModule(IFeedService feedService)
        {
            this.feedService = feedService;
        }

        [Command("Add")]
        [Remarks("Adds an atom or RSS feed to the list of listened feeds.")]
        public async Task AddFeedUrlAsync([Summary("The url to the feed (Atom/RSS)")] string url, [Summary("Optional: The name of the channel where the Feed updates should be posted. Defaults to current channel")] string channelName = "")
        {
            if (url.IsEmpty())
            {
                await ReplyAsync("Please enter a feed url!");
                return;
            }
            ITextChannel channel = await GetChannelAsync(channelName);
            if (channel == null)
            {
                await ReplyAsync("The specified channel was not found");
                return;
            }
            var urls = await FeedReader.GetFeedUrlsFromUrlAsync(url);
            string feedUrl;
            if (urls.Count() < 1) // no url - probably the url is already the right feed url
                feedUrl = url;
            else if (urls.Count() == 1)
                feedUrl = urls.First().Url;
            else
            {
                await ReplyAsync($"Multiple feeds were found at this url. Please be more specific:{Environment.NewLine}{string.Join(Environment.NewLine, urls)}");
                return;
            }
            var currentFeedUrls = await feedService.GetFeedUrlsForGuildAsync(Context.Guild.Id, channel.Id);
            if (currentFeedUrls.Contains(feedUrl))
            {
                await ReplyAsync("The specified feed is already in the list!");
                return;
            }
            await feedService.AddFeedAsync(feedUrl, Context.Guild.Id, channel.Id);
        }

        [Command("Remove")]
        [Remarks("Removes the specified feed from the list of feeds.")]
        public async Task RemoveFeedUrlAsync([Summary("The url of the feed")] string url, [Summary("Optional: The name of the channel where the Feed url should be removed. Defaults to current channel")] string channelName = "")
        {
            if (url.IsEmpty())
            {
                await ReplyAsync("Please enter a feed url");
                return;
            }
            ITextChannel channel = await GetChannelAsync(channelName);
            if (channel == null)
            {
                await ReplyAsync("The specified channel was not found");
                return;
            }
            var currentFeedUrls = await feedService.GetFeedUrlsForGuildAsync(Context.Guild.Id, channel?.Id);
            if (!currentFeedUrls.Contains(url))
            {
                await ReplyAsync("The specified feed is not in the list!");
                return;
            }

            await feedService.RemoveFeedAsync(url, Context.Guild.Id, channel.Id);
        }

        [Command("List")]
        [Remarks("List all current feed urls")]
        public async Task ListFeedUrlsAsync([Summary("Optional: The name of the channel where the Feed urls should be listed for. Defaults to all channels")] string channelName = "")
        {
            ITextChannel channel = await GetChannelAsync(channelName);
            var feedUrls = await feedService.GetFeedUrlsForGuildAsync(Context.Guild.Id, channel?.Id);
            if (feedUrls == null || feedUrls.Count < 1)
            {
                await ReplyAsync("No feeds have been added yet.");
            }
            else
            {
                string where = channel == null ? "in all channels" : "in this channel";
                await ReplyAsync($"The following feeds are listed {where}:{Environment.NewLine}{string.Join(Environment.NewLine, feedUrls)})");
            }
        }

        [Command("RemoveAll")]
        [Remarks("Removes all feed urls")]
        public async Task RemoveFeedUrlsAsync([Summary("Optional: The name of the channel where the Feed urls should be removed. Defaults to all channels")] string channelName = "")
        {
            ITextChannel channel = await GetChannelAsync(channelName);
            await feedService.RemoveAllFeedsAsync(Context.Guild.Id, channel?.Id);
        }

        private async Task<ITextChannel> GetChannelAsync(string channelName)
        {
            var allChannels = await Context.Guild.GetTextChannelsAsync();
            ITextChannel channel = null;
            if (!channelName.IsEmpty())
            {
                channel = allChannels.FirstOrDefault(x => x.Name.ToLower() == channelName.ToLower());
            }
            else
            {
                channel = Context.Channel as ITextChannel;
            }

            return channel;
        }
    }
}