## Full list of Commands (grouped by module):

- [Admin Commands](#admin-commands)
- [Announcements](#announcements)
- [Benzen Facts](#benzen-facts)
- [Cat pictures](#cat-pictures)
- [Chuck Norris jokes](#chuck-norris-jokes)
- [Dog pictures](#dog-pictures)
- [Feeds](#feeds)
- [Game Subscriptions](#game-subscriptions)
- [Guild Configuration](#guild-configuration)
- [Help](#help)
- [Info](#info)
- [MineCraftGameServer](#minecraftgameserver)
- [Misc](#misc)
- [Moderator Commands](#moderator-commands)
- [Picture Search](#picture-search)
- [Simple poll](#simple-poll)
- [Role Buttons](#role-buttons)
- [Roles](#roles)
- [SteamGameServer](#steamgameserver)
- [Trivia](#trivia)
- [Xkcd](#xkcd)

### Admin Commands
**Preconditions:** Minimum permission: *Server Admin*  
  
`!addowner _username`  
*Preconditions:* Minimum permission: *Bot Owner*  
*Remarks:* Adds the specified user to the list of bot owners  
  
`!removeowner _username`  
*Preconditions:* Minimum permission: *Bot Owner*  
*Remarks:* Removes the specified user from the list of bot owners  
  
---  
### Announcements
**Preconditions:** Minimum permission: *Server Admin*, Can only be used in a *channel*  
  
`!announcements addrecurring _announcementId _cronExpression _announcement _channelName`  
*Example:* `!announcements addrecurring "weeklyMsg1" "0 19 * * 5" "It is Friday 19:00" "general"`  
*Remarks:* Adds the specified recurring announcement to the specified channel  
  
`!announcements addsingle _announcementId _time _announcement _channelName`  
*Example:* `!announcements addsingle "reminder1" "19:00" "It is 19:00" "general"`  
*Remarks:* Adds the specified single announcement at the given time to the specified channel  
  
`!announcements list `  
*Remarks:* Lists all upcoming announcements  
  
`!announcements remove _id`  
*Example:* `!announcements remove announcement1`  
*Remarks:* Removes the announcement with the specified ID  
  
`!announcements nextrun _id`  
*Example:* `!announcements nextrun announcement1`  
*Remarks:* Gets the next execution time of the announcement with the specified ID.  
  
---  
### Benzen Facts
  
`!benzen `  
*Remarks:* Returns a random fact about Benzen  
  
`!addbenzenfact _fact`  
*Remarks:* Add a fact about Benzen  
  
---  
### Cat pictures
**Preconditions:** Minimum permission: *User*, Can only be used in a *channel*  
  
`!cat `  
*Remarks:* Gets a random cat picture.  
  
`!cat _breed`  
*Remarks:* Gets a random Cat picture for the given breed.  
  
`!catbreeds `  
*Remarks:* Gets a list of available cat breeds.  
  
---  
### Chuck Norris jokes
**Preconditions:** Minimum permission: *User*, Can only be used in a *channel*  
  
`!chuck `  
*Remarks:* Gets a random Chuck Norris fact.  
  
`!chuck _username`  
*Remarks:* Gets a random Chuck Norris fact and replaces Chuck Norris with the given name.  
  
---  
### Dog pictures
**Preconditions:** Minimum permission: *User*, Can only be used in a *channel*  
  
`!dog `  
*Remarks:* Gets a random Dog picture.  
  
`!dog _breed`  
*Remarks:* Gets a random Dog picture for the given breed.  
  
`!dogbreeds `  
*Remarks:* Gets a list of available dog breeds.  
  
---  
### Feeds
**Preconditions:** Minimum permission: *Server Admin*, Can only be used in a *channel*, Bot requires guild permission: *Embed Links*  
  
`!feeds add _name _url _channelName`  
*Example:* `!Feeds add https://blogs.msdn.microsoft.com/dotnet/feed/`  
*Remarks:* Adds an atom or RSS feed to the list of listened feeds.  
  
`!feeds remove _nameOrUrl _channelName`  
*Example:* `!Feeds remove https://blogs.msdn.microsoft.com/dotnet/feed/`  
*Remarks:* Removes the specified feed from the list of feeds.  
  
`!feeds list _channelName`  
*Remarks:* List all current feed urls  
  
`!feeds removeall _channelName`  
*Remarks:* Removes all feed urls  
  
---  
### Game Subscriptions
**Preconditions:** Minimum permission: *User*, Can only be used in a *channel*  
  
`!subscribe _gameName`  
*Example:* `!Subscribe "Battlefield 1"`  
*Remarks:* Subscribes to the specified game. You will get a private message every time someone launches it  
  
`!unsubscribe _gameName`  
*Example:* `!Unsubscribe "Battlefield 1"`  
*Remarks:* Unsubscribes to the specified game  
  
`!subscriptions `  
*Example:* `!Subscriptions`  
*Remarks:* Lists all your game subscriptions  
  
---  
### Guild Configuration
**Preconditions:** Minimum permission: *Server Admin*, Can only be used in a *channel*  
  
`!setwelcomemessage _welcomeMsg`  
*Example:* `!SetWelcomeMessage "Hello %user%, welcome to %server%"`  
*Remarks:* Sets the welcome message for new users. Can make use of %user% and %server%  
  
`!setwelcomechannel _channelName`  
*Example:* `!SetWelcomeChannel general`  
*Remarks:* Sets the channel where the welcome message will be posted  
  
`!setgoodbyemessage _goodbyeMsg`  
*Example:* `!SetGoodbyeMessage "Goodbye %user%, farewell!"`  
*Remarks:* Sets the Goodbye message for new users. Can make use of %user% and %server%  
  
`!setgoodbyechannel _channelName`  
*Example:* `!SetGoodbyeChannel general`  
*Remarks:* Sets the channel where the Goodbye message will be posted  
  
`!addrule _rule`  
*Example:* `!AddRule "You shall not pass!"`  
*Remarks:* Adds a rule to the server.  
  
`!removerules `  
*Remarks:* Removes all rules from a server.  
  
`!enablebattlefieldupdates _channel`  
*Example:* `!EnableBattlefieldUpdates #general`  
*Remarks:* Enables automated posting of Battlefield update news in provided channel  
  
`!disablebattlefieldupdates `  
*Remarks:* Disables automated posting of Battlefield update news  
  
---  
### Help
**Preconditions:** Minimum permission: *User*  
  
`!help `  
*Remarks:* List all usable commands.  
  
`!help _command`  
*Example:* `!help Chuck`  
*Remarks:* Gets help for the specified command  
  
---  
### Info
  
`!rules `  
*Preconditions:* Can only be used in a *channel*  
*Remarks:* The bot replies with the server rules in a private message  
  
`!findmessageid _messageContent`  
*Preconditions:* Can only be used in a *channel*  
*Remarks:* Gets the message id of a message in the current channel with the provided message text  
  
---  
### MineCraftGameServer
**Preconditions:** Minimum permission: *Server Admin*, Can only be used in a *channel*, Bot requires guild permission: *Embed Links*  
  
`!minecraftgameserver add _ip`  
*Example:* `!gameserver add 127.0.0.1:1234`  
*Remarks:* Adds the specified game server and posts it's info info in the current channel  
  
`!minecraftgameserver add _ip _channel`  
*Example:* `!gameserver add "127.0.0.1:1234" "general"`  
*Remarks:* Adds the specified game server and sets the channel where the info will be posted.  
  
`!minecraftgameserver remove _ip`  
*Example:* `!gameserver remove 127.0.0.1:1234`  
*Remarks:* Removes the specified game server  
  
---  
### Misc
  
`!lmgtfy _searchText`  
*Example:* `!lmgtfy Monkey Gamers`  
*Remarks:* Generate a 'let me google that for you' link  
  
---  
### Moderator Commands
**Preconditions:** Minimum permission: *Server Mod*, Can only be used in a *channel*  
  
`!prune _count`  
*Example:* `!Prune 10`  
*Preconditions:* User requires channel permission: *Manage Messages*, Bot requires channel permission: *Manage Messages*  
*Remarks:* Deletes the specified amount of messages  
  
`!prune _userName _count`  
*Example:* `!Prune JohnDoe 10`  
*Preconditions:* User requires channel permission: *Manage Messages*, Bot requires channel permission: *Manage Messages*  
*Remarks:* Deletes the specified amount of messages for the specified user  
  
---  
### Picture search
**Preconditions:** Minimum permission: *User*, Can only be used in a *channel*  
  
`!picture _searchterm`  
*Remarks:* Gets a random picture for the given search term.  
  
---  
### Simple poll
**Preconditions:** Can only be used in a *channel*, Minimum permission: *User*  
  
`!poll _question`  
*Example:* `!poll "Is MonkeyBot awesome?"`  
*Preconditions:* Bot requires channel permissions: *Add Reactions, Manage Messages*  
*Remarks:* Starts a new poll with the specified question and automatically adds reactions  
  
`!poll _question _answers`  
*Example:* `!poll "How cool is MonkeyBot?" "supercool" "over 9000" "bruh..."`  
*Preconditions:* Bot requires channel permissions: *Add Reactions, Manage Messages*  
*Remarks:* Starts a new poll with the specified question and the list answers and automatically adds reactions  
  
---  
### Role Buttons
**Preconditions:** Minimum permission: *Server Admin*, Bot requires guild permissions: *Add Reactions, Manage Messages, Manage Roles*  
  
`!rolebuttons addlink _messageId _roleName _emoteString`  
*Remarks:* Adds a reaction to the specified message with a link to the specified role  
  
`!rolebuttons removelink _messageId _roleName`  
*Remarks:* Removes a reaction from the specified message with a link to the specified role  
  
`!rolebuttons removeall `  
*Remarks:* Removes all Role Button Links  
  
`!rolebuttons list `  
*Remarks:* Lists all Role Button Links  
  
---  
### Roles
**Preconditions:** Minimum permission: *User*, Can only be used in a *channel*, Bot requires guild permission: *Manage Roles*  
  
`!roles add _roleName`  
*Example:* `!roles add bf`  
*Remarks:* Adds the specified role to your own roles.  
  
`!roles remove _roleName`  
*Example:* `!roles remove bf`  
*Remarks:* Removes the specified role from your roles.  
  
`!roles list `  
*Remarks:* Lists all roles that can be mentioned and assigned.  
  
`!roles listmembers `  
*Remarks:* Lists all roles and the users who have these roles  
  
`!roles listmembers _roleName`  
*Example:* `!roles listmembers bf`  
*Remarks:* Lists all the members of the specified role  
  
---  
### SteamGameServer
**Preconditions:** Minimum permission: *Server Admin*, Can only be used in a *channel*, Bot requires guild permission: *Embed Links*  
  
`!steamgameserver add _ip`  
*Example:* `!gameserver add 127.0.0.1:1234`  
*Remarks:* Adds the specified game server and posts it's info info in the current channel  
  
`!steamgameserver add _ip _channel`  
*Example:* `!gameserver add "127.0.0.1:1234" "general"`  
*Remarks:* Adds the specified game server and sets the channel where the info will be posted.  
  
`!steamgameserver remove _ip`  
*Example:* `!gameserver remove 127.0.0.1:1234`  
*Remarks:* Removes the specified game server  
  
---  
### Trivia
**Preconditions:** Minimum permission: *User*, Can only be used in a *channel*  
  
`!trivia start _questionAmount`  
*Example:* `!trivia start 5`  
*Remarks:* Starts a new trivia with the specified amount of questions.  
  
`!trivia stop `  
*Remarks:* Stops a running trivia  
  
`!trivia skip `  
*Remarks:* Skips the current question  
  
`!trivia scores _amount`  
*Example:* `!trivia scores 10`  
*Remarks:* Gets the global scores  
  
---  
### Xkcd
  
`!xkcd _arg`  
*Example:* `!xkcd latest`  
*Preconditions:* Bot requires channel permission: *Embed Links*  
*Remarks:* Gets a random xkcd comic or the latest xkcd comic by appending "latest" to the command  
  
`!xkcd _number`  
*Example:* `!xkcd 101`  
*Preconditions:* Bot requires channel permission: *Embed Links*  
*Remarks:* Gets the xkcd comic with the specified number  
  
---
