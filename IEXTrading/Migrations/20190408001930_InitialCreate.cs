using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace IEXTrading.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    symbol = table.Column<string>(nullable: false),
                    date = table.Column<string>(nullable: true),
                    iexId = table.Column<string>(nullable: true),
                    isEnabled = table.Column<bool>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.symbol);
                });

            migrationBuilder.CreateTable(
                name: "FinancialData",
                columns: table => new
                {
                    sno = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    cashChange = table.Column<double>(nullable: false),
                    cashFlow = table.Column<double>(nullable: false),
                    costOfRevenue = table.Column<double>(nullable: false),
                    currentAssets = table.Column<double>(nullable: false),
                    currentCash = table.Column<double>(nullable: false),
                    currentDebt = table.Column<double>(nullable: false),
                    grossProfit = table.Column<double>(nullable: false),
                    netIncome = table.Column<double>(nullable: false),
                    operatingExpense = table.Column<double>(nullable: false),
                    operatingGainsLosses = table.Column<string>(nullable: true),
                    operatingIncome = table.Column<double>(nullable: false),
                    operatingRevenue = table.Column<double>(nullable: false),
                    reportDate = table.Column<string>(nullable: true),
                    researchAndDevelopment = table.Column<double>(nullable: false),
                    shareholderEquity = table.Column<double>(nullable: false),
                    symbol = table.Column<string>(nullable: true),
                    totalAssets = table.Column<double>(nullable: false),
                    totalCash = table.Column<double>(nullable: false),
                    totalLiabilities = table.Column<double>(nullable: false),
                    totalRevenue = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialData", x => x.sno);
                });

            migrationBuilder.CreateTable(
                name: "Gainers",
                columns: table => new
                {
                    symbol = table.Column<string>(nullable: false),
                    companyName = table.Column<string>(nullable: true),
                    primaryExchange = table.Column<string>(nullable: true),
                    sector = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gainers", x => x.symbol);
                });

            migrationBuilder.CreateTable(
                name: "Losers",
                columns: table => new
                {
                    symbol = table.Column<string>(nullable: false),
                    companyName = table.Column<string>(nullable: true),
                    primaryExchange = table.Column<string>(nullable: true),
                    sector = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Losers", x => x.symbol);
                });

            migrationBuilder.CreateTable(
                name: "Equities",
                columns: table => new
                {
                    EquityId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    change = table.Column<float>(nullable: false),
                    changeOverTime = table.Column<float>(nullable: false),
                    changePercent = table.Column<float>(nullable: false),
                    close = table.Column<float>(nullable: false),
                    date = table.Column<string>(nullable: true),
                    high = table.Column<float>(nullable: false),
                    label = table.Column<string>(nullable: true),
                    low = table.Column<float>(nullable: false),
                    open = table.Column<float>(nullable: false),
                    symbol = table.Column<string>(nullable: true),
                    unadjustedVolume = table.Column<int>(nullable: false),
                    volume = table.Column<int>(nullable: false),
                    vwap = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equities", x => x.EquityId);
                    table.ForeignKey(
                        name: "FK_Equities_Companies_symbol",
                        column: x => x.symbol,
                        principalTable: "Companies",
                        principalColumn: "symbol",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equities_symbol",
                table: "Equities",
                column: "symbol");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Equities");

            migrationBuilder.DropTable(
                name: "FinancialData");

            migrationBuilder.DropTable(
                name: "Gainers");

            migrationBuilder.DropTable(
                name: "Losers");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
