using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoAPIBot.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InProgressSubs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    mId = table.Column<string>(type: "TEXT", nullable: false),
                    timeSubRequest = table.Column<DateTime>(type: "TEXT", nullable: false),
                    token = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InProgressSubs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    mId = table.Column<string>(type: "TEXT", nullable: false),
                    model = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.mId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Subs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubMachines",
                columns: table => new
                {
                    SubId = table.Column<int>(type: "INTEGER", nullable: false),
                    MacchinaId = table.Column<string>(type: "TEXT", nullable: false),
                    serviceDispatcher = table.Column<string>(type: "TEXT", nullable: false),
                    levelPriority = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubMachines", x => new { x.SubId, x.MacchinaId, x.serviceDispatcher });
                    table.ForeignKey(
                        name: "FK_SubMachines_Machines_MacchinaId",
                        column: x => x.MacchinaId,
                        principalTable: "Machines",
                        principalColumn: "mId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubMachines_Subs_SubId",
                        column: x => x.SubId,
                        principalTable: "Subs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InProgressSubs_Id",
                table: "InProgressSubs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SubMachines_MacchinaId",
                table: "SubMachines",
                column: "MacchinaId");

            migrationBuilder.CreateIndex(
                name: "IX_Subs_Id",
                table: "Subs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InProgressSubs");

            migrationBuilder.DropTable(
                name: "SubMachines");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Machines");

            migrationBuilder.DropTable(
                name: "Subs");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
