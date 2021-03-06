﻿using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MonkeyBot.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildID = table.Column<ulong>(nullable: false),
                    ChannelID = table.Column<ulong>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Message = table.Column<string>(nullable: false),
                    ExecutionTime = table.Column<DateTime>(nullable: true),
                    CronExpression = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BenzenFacts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Fact = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenzenFacts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Feeds",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildID = table.Column<ulong>(nullable: false),
                    ChannelID = table.Column<ulong>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    URL = table.Column<string>(nullable: false),
                    LastUpdate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feeds", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GameServers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildID = table.Column<ulong>(nullable: false),
                    ChannelID = table.Column<ulong>(nullable: false),
                    MessageID = table.Column<ulong>(nullable: true),
                    GameServerType = table.Column<string>(nullable: false),
                    ServerIP = table.Column<string>(nullable: false),
                    GameVersion = table.Column<string>(nullable: true),
                    LastVersionUpdate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameServers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GameSubscriptions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildID = table.Column<ulong>(nullable: false),
                    UserID = table.Column<ulong>(nullable: false),
                    GameName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSubscriptions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GuildConfigs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildID = table.Column<ulong>(nullable: false),
                    CommandPrefix = table.Column<string>(nullable: false),
                    WelcomeMessageText = table.Column<string>(nullable: true),
                    WelcomeMessageChannelId = table.Column<ulong>(nullable: false),
                    GoodbyeMessageText = table.Column<string>(nullable: true),
                    GoodbyeMessageChannelId = table.Column<ulong>(nullable: false),
                    Rules = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildConfigs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RoleButtonLinks",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildID = table.Column<ulong>(nullable: false),
                    MessageID = table.Column<ulong>(nullable: false),
                    RoleID = table.Column<ulong>(nullable: false),
                    EmoteString = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleButtonLinks", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TriviaScores",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildID = table.Column<ulong>(nullable: false),
                    UserID = table.Column<ulong>(nullable: false),
                    Score = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriviaScores", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "BenzenFacts");

            migrationBuilder.DropTable(
                name: "Feeds");

            migrationBuilder.DropTable(
                name: "GameServers");

            migrationBuilder.DropTable(
                name: "GameSubscriptions");

            migrationBuilder.DropTable(
                name: "GuildConfigs");

            migrationBuilder.DropTable(
                name: "RoleButtonLinks");

            migrationBuilder.DropTable(
                name: "TriviaScores");
        }
    }
}
