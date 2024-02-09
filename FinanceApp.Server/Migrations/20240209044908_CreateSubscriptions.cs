using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class CreateSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SubscriptionId",
                table: "Transactions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Subscription",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    FrequencyCounter = table.Column<int>(type: "INTEGER", nullable: false),
                    FrequencyMeasure = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AccountId = table.Column<long>(type: "INTEGER", nullable: false),
                    CounterpartyId = table.Column<long>(type: "INTEGER", nullable: false),
                    Value = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscription_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscription_Counterparties_CounterpartyId",
                        column: x => x.CounterpartyId,
                        principalTable: "Counterparties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SubscriptionId",
                table: "Transactions",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_AccountId",
                table: "Subscription",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_CounterpartyId",
                table: "Subscription",
                column: "CounterpartyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Subscription_SubscriptionId",
                table: "Transactions",
                column: "SubscriptionId",
                principalTable: "Subscription",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Subscription_SubscriptionId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "Subscription");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_SubscriptionId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "Transactions");
        }
    }
}
