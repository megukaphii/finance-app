using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceApp.Server.Migrations
{
	/// <inheritdoc />
	public partial class InitialCreate : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Counterparties",
				columns: table => new
				{
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					Name = table.Column<string>(type: "TEXT", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Counterparties", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Transactions",
				columns: table => new
				{
					Id = table.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					CounterpartyId = table.Column<int>(type: "INTEGER", nullable: false),
					Value = table.Column<int>(type: "INTEGER", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Transactions", x => x.Id);
					table.ForeignKey(
						name: "FK_Transactions_Counterparties_CounterpartyId",
						column: x => x.CounterpartyId,
						principalTable: "Counterparties",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Transactions_CounterpartyId",
				table: "Transactions",
				column: "CounterpartyId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Transactions");

			migrationBuilder.DropTable(
				name: "Counterparties");
		}
	}
}