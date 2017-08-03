﻿using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using MonkeyBot.Common;
using MonkeyBot.Preconditions;
using MonkeyBot.Services;
using MonkeyBot.Services.Common.Trivia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonkeyBot.Modules
{
    /// <summary>Module that provides support for trivia game</summary>
    [Group("Trivia")]
    [Name("Trivia")]
    [MinPermissions(AccessLevel.User)]
    [RequireContext(ContextType.Guild)]
    public class TriviaModule : ModuleBase
    {
        private ITriviaService triviaService; // The TriviaService will get injected in CommandHandler
        private CommandManager commandManager;
        private DbService db;

        public TriviaModule(IServiceProvider provider) // Create a constructor for the TriviaService dependency
        {
            triviaService = provider.GetService<ITriviaService>();
            commandManager = provider.GetService<CommandManager>();
            db = provider.GetService<DbService>();
        }

        [Command("Start")]
        [Remarks("Starts a new trivia with the specified amount of questions.")]
        public async Task StartTriviaAsync([Summary("The number of questions to play.")] int questionAmount = 10)
        {
            if (!await triviaService?.StartTriviaAsync(questionAmount, Context.Guild.Id, Context.Channel.Id))
                await ReplyAsync("Trivia could not be started :(");
        }

        [Command("Stop")]
        [Remarks("Stops a running trivia")]
        public async Task StopTriviaAsync()
        {
            if (!(await triviaService?.StopTriviaAsync(Context.Guild.Id, Context.Channel.Id)))
                await ReplyAsync($"No trivia is running! Use {commandManager.GetPrefixAsync(Context.Guild)}trivia start to create a new one.");
        }

        [Command("Skip")]
        [Remarks("Skips the current question")]
        public async Task SkipQuestionAsync()
        {
            if (!(await triviaService?.SkipQuestionAsync(Context.Guild.Id, Context.Channel.Id)))
                await ReplyAsync($"No trivia is running! Use {commandManager.GetPrefixAsync(Context.Guild)}trivia start to create a new one.");
        }

        [Command("Scores")]
        [Remarks("Gets the global scores")]
        public async Task GetScoresAsync([Summary("The amount of scores to get.")] int amount = 5)
        {
            List<TriviaScore> userScoresAllTime;
            using (var uow = db.UnitOfWork)
            {
                userScoresAllTime = (await uow.TriviaScores.GetGuildScoresAsync(Context.Guild.Id));
            }
            int correctedCount = Math.Min(amount, userScoresAllTime.Count());
            if (userScoresAllTime == null || correctedCount < 1)
            {
                await ReplyAsync("No scores found!");
                return;
            }
            var sortedScores = userScoresAllTime.OrderByDescending(x => x.Score).Take(correctedCount).ToList();
            List<string> scoresList = new List<string>();
            for (int i = 0; i < sortedScores.Count; i++)
            {
                var score = sortedScores[i];
                var userName = (await Context.Client.GetUserAsync(score.UserID))?.Username;
                scoresList.Add($"**#{i+1}: {userName}** - {score.Score} point{(score.Score == 1 ? "" : "s")}");
            }            
            var builder = new EmbedBuilder();
            builder.Color = new Color(46, 191, 84);
            builder.AddField($"**Top {correctedCount} of all time**:", string.Join(Environment.NewLine, scoresList));
            await ReplyAsync("", false, builder.Build());
        }        
    }
}