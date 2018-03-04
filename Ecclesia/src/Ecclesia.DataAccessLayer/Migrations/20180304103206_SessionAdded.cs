using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Ecclesia.DataAccessLayer.Migrations
{
    public partial class SessionAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    LastPolling = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    MnemonicsTable = table.Column<string>(type: "jsonb", nullable: true),
                    OperationsStatus = table.Column<string>(type: "jsonb", nullable: true),
                    OriginalGraph = table.Column<string>(type: "jsonb", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sessions");
        }
    }
}
