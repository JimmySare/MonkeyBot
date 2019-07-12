﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using dokas.FluentStrings;
using Microsoft.Extensions.DependencyInjection;
using MonkeyBot.Common;
using MonkeyBot.Documentation;
using MonkeyBot.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MonkeyBot
{
    /// <summary> Detect whether a message is a command, then execute it. </summary>
    public class CommandManager
    {
        private readonly IServiceProvider serviceProvider;
        private readonly DiscordSocketClient discordClient;
        private readonly DbService dbService;
        private readonly CommandService commandService;

        /// <summary>
        /// Create a new CommandManager instance with DI. Use <see cref="StartAsync"/> afterwards to actually start the CommandManager/>
        /// </summary>
        public CommandManager(IServiceProvider provider)
        {
            serviceProvider = provider;
            discordClient = provider.GetService<DiscordSocketClient>();
            dbService = provider.GetService<DbService>();
            commandService = provider.GetService<CommandService>();
        }

        public async Task StartAsync()
        {
            await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider).ConfigureAwait(false);

            discordClient.MessageReceived += HandleCommandAsync;
        }

        public Task<string> GetPrefixAsync(IGuild guild) => GetPrefixAsync(guild?.Id);

        public async Task<string> GetPrefixAsync(ulong? guildId)
        {
            if (guildId == null)
                return DiscordClientConfiguration.DefaultPrefix;
            using (var uow = dbService.UnitOfWork)
            {
                var prefix = (await uow.GuildConfigs.GetAsync(guildId.Value).ConfigureAwait(false))?.CommandPrefix;
                if (prefix != null)
                    return prefix;
                else
                    return DiscordClientConfiguration.DefaultPrefix;
            }
        }

        private async Task HandleCommandAsync(SocketMessage socketMsg)
        {
            if (!(socketMsg is SocketUserMessage msg))
                return;
            var context = new SocketCommandContext(discordClient, msg);
            var guild = (msg.Channel as SocketTextChannel)?.Guild;
            var prefix = await GetPrefixAsync(guild?.Id).ConfigureAwait(false);
            int argPos = 0;

            if (msg.HasStringPrefix(prefix, ref argPos))
            {
                string commandText = msg.Content.Substring(argPos).ToLowerInvariant().Trim();
                if (!commandText.IsEmpty())
                {
                    var result = await commandService.ExecuteAsync(context, argPos, serviceProvider).ConfigureAwait(false);

                    if (!result.IsSuccess)
                    {
                        if (result.Error.HasValue)
                        {
                            var error = result.Error.Value;
                            var errorMessage = GetCommandErrorMessage(error, prefix, commandText);
                            await context.Channel.SendMessageAsync(errorMessage).ConfigureAwait(false);
                            if (error == CommandError.Exception || error == CommandError.ParseFailed || error == CommandError.Unsuccessful)
                            {
                                if (discordClient is MonkeyClient monkeyClient)
                                {
                                    await monkeyClient.NotifyAdminAsync(errorMessage).ConfigureAwait(false);
                                }
                            }
                        }
                        else
                        {
                            await context.Channel.SendMessageAsync(result.ToString()).ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        private string GetCommandErrorMessage(CommandError error, string prefix, string commandText)
        {
            switch (error)
            {
                case CommandError.UnknownCommand:
                    {
                        var possibleCommands =
                        commandService
                            .Modules
                            .SelectMany(module => module.Commands)
                            .SelectMany(command => command.Aliases.Select(a => $"{prefix}{a}"))
                            .Distinct()
                            .Where(alias => alias.Contains(commandText, StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        string message = "";
                        if (possibleCommands == null || possibleCommands.Count < 1)
                        {
                            message = $"Command *{commandText}* was not found. Type {prefix}help to get a list of commands";
                        }
                        else if (possibleCommands.Count == 1)
                        {
                            message = $"Did you mean *{possibleCommands.First()}* ? Type {prefix}help to get a list of commands";
                        }
                        else if (possibleCommands.Count > 1 && possibleCommands.Count < 5)
                        {
                            message = $"Did you mean one of the following commands:{Environment.NewLine}{string.Join(Environment.NewLine, possibleCommands)}{Environment.NewLine}Type {prefix}help to get a list of commands";
                        }
                        else
                        {
                            message = $"{possibleCommands.Count} possible commands have been found matching your input. Please be more specific.";
                        }
                        return message;
                    }
                case CommandError.ParseFailed:
                    return "Command could not be parsed, I'm sorry :(";

                case CommandError.BadArgCount:
                    return $"Command did not have the right amount of parameters. Type {prefix}help {commandText} for more info";

                case CommandError.ObjectNotFound:
                    return "Object was not found";

                case CommandError.MultipleMatches:
                    return $"Multiple commands were found like {commandText}. Please be more specific";

                case CommandError.UnmetPrecondition:
                    return $"A precondition for the command was not met. Type {prefix}help {commandText} for more info";

                case CommandError.Exception:
                    return "An exception has occured during the command execution. My developer was notified of this";

                case CommandError.Unsuccessful:
                    return "The command excecution was unsuccessfull, I'm sorry :(";

                default:
                    break;
            }
            return "Can't execute the command!";
        }

        public async Task BuildDocumentationAsync()
        {
            string docuHTML = DocumentationBuilder.BuildDocumentation(commandService, DocumentationOutputTypes.HTML);
            string fileHTML = Path.Combine(AppContext.BaseDirectory, "documentation.html");
            await MonkeyHelpers.WriteTextAsync(fileHTML, docuHTML).ConfigureAwait(false);

            string docuMD = DocumentationBuilder.BuildDocumentation(commandService, DocumentationOutputTypes.MarkDown);
            string fileMD = Path.Combine(AppContext.BaseDirectory, "documentation.md");
            await MonkeyHelpers.WriteTextAsync(fileMD, docuMD).ConfigureAwait(false);
        }
    }
}