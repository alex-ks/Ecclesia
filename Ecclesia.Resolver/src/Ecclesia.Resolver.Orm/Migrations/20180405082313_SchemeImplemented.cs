using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Ecclesia.Resolver.Orm.Migrations
{
    public partial class SchemeImplemented : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Atoms",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Kind = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Version = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atoms", x => x.Id);
                    table.UniqueConstraint("AK_Atoms_Kind_Name_Version", x => new { x.Kind, x.Name, x.Version });
                });

            migrationBuilder.CreateTable(
                name: "AtomContents",
                columns: table => new
                {
                    AtomId = table.Column<long>(nullable: false),
                    Content = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtomContents", x => x.AtomId);
                    table.ForeignKey(
                        name: "FK_AtomContents_Atoms_AtomId",
                        column: x => x.AtomId,
                        principalTable: "Atoms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AtomDependencies",
                columns: table => new
                {
                    DependentId = table.Column<long>(nullable: false),
                    DependencyId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtomDependencies", x => new { x.DependentId, x.DependencyId });
                    table.ForeignKey(
                        name: "FK_AtomDependencies_Atoms_DependencyId",
                        column: x => x.DependencyId,
                        principalTable: "Atoms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AtomDependencies_Atoms_DependentId",
                        column: x => x.DependentId,
                        principalTable: "Atoms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AtomDependencies_DependencyId",
                table: "AtomDependencies",
                column: "DependencyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AtomContents");

            migrationBuilder.DropTable(
                name: "AtomDependencies");

            migrationBuilder.DropTable(
                name: "Atoms");
        }
    }
}
