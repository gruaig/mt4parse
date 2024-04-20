using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mt4LogParser.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Metas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MetaId = table.Column<long>(type: "bigint", nullable: true),
                    Account = table.Column<int>(type: "integer", nullable: false),
                    Cid = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    FirstLoginTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsInvestor = table.Column<bool>(type: "boolean", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Device = table.Column<int>(type: "integer", nullable: false),
                    NumberOfLogins = table.Column<int>(type: "integer", nullable: false),
                    NumberOfOrders = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_Metas_MetaId",
                        column: x => x.MetaId,
                        principalTable: "Metas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Errors",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MetaId = table.Column<long>(type: "bigint", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Errors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Errors_Metas_MetaId",
                        column: x => x.MetaId,
                        principalTable: "Metas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Monitoring",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MetaId = table.Column<long>(type: "bigint", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Connections = table.Column<int>(type: "integer", nullable: false),
                    FreeMemory = table.Column<int>(type: "integer", nullable: false),
                    Cpu = table.Column<int>(type: "integer", nullable: false),
                    Net = table.Column<int>(type: "integer", nullable: false),
                    Sockets = table.Column<int>(type: "integer", nullable: false),
                    Threads = table.Column<int>(type: "integer", nullable: false),
                    Handles = table.Column<int>(type: "integer", nullable: false),
                    MaxMemoryBlock = table.Column<int>(type: "integer", nullable: false),
                    ProcessCpu = table.Column<int>(type: "integer", nullable: false),
                    NetIn = table.Column<int>(type: "integer", nullable: false),
                    NetOut = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monitoring", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Monitoring_Metas_MetaId",
                        column: x => x.MetaId,
                        principalTable: "Metas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MetaId = table.Column<long>(type: "bigint", nullable: true),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Account = table.Column<int>(type: "integer", nullable: false),
                    Symbol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Operation = table.Column<int>(type: "integer", nullable: false),
                    Entry = table.Column<int>(type: "integer", nullable: false),
                    Volume = table.Column<decimal>(type: "numeric", nullable: false),
                    Bid = table.Column<decimal>(type: "numeric", nullable: false),
                    Ask = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Metas_MetaId",
                        column: x => x.MetaId,
                        principalTable: "Metas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MetaId = table.Column<long>(type: "bigint", nullable: true),
                    Path = table.Column<string>(type: "text", nullable: false),
                    Position = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                    table.ForeignKey(
                        name: "FK_States_Metas_MetaId",
                        column: x => x.MetaId,
                        principalTable: "Metas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_Account",
                table: "Activities",
                column: "Account");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_Cid",
                table: "Activities",
                column: "Cid");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_Device",
                table: "Activities",
                column: "Device");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_IpAddress",
                table: "Activities",
                column: "IpAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_IsInvestor",
                table: "Activities",
                column: "IsInvestor");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_LastLoginTime",
                table: "Activities",
                column: "LastLoginTime");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_MetaId",
                table: "Activities",
                column: "MetaId");

            migrationBuilder.CreateIndex(
                name: "IX_Errors_MetaId",
                table: "Errors",
                column: "MetaId");

            migrationBuilder.CreateIndex(
                name: "IX_Errors_Timestamp",
                table: "Errors",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Monitoring_MetaId",
                table: "Monitoring",
                column: "MetaId");

            migrationBuilder.CreateIndex(
                name: "IX_Monitoring_Timestamp",
                table: "Monitoring",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_MetaId",
                table: "Orders",
                column: "MetaId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Time",
                table: "Orders",
                column: "Time");

            migrationBuilder.CreateIndex(
                name: "IX_States_MetaId",
                table: "States",
                column: "MetaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "Errors");

            migrationBuilder.DropTable(
                name: "Monitoring");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "Metas");
        }
    }
}
