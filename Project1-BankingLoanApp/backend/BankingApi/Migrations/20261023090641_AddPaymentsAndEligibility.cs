using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentsAndEligibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "Repayments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApplicantDateOfBirth",
                table: "LoanApplications",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyIncome",
                table: "LoanApplications",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionRef = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Method = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Success"),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.CheckConstraint("CK_Payments_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_Payments_Method", "[Method] IN ('Card', 'UPI', 'NetBanking', 'Branch')");
                    table.CheckConstraint("CK_Payments_Status", "[Status] IN ('Success', 'Reversed')");
                    table.CheckConstraint("CK_Payments_Type", "[Type] IN ('Installment', 'EarlyPayoff')");
                    table.ForeignKey(
                        name: "FK_Payments_LoanApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "LoanApplications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Repayments_PaymentId",
                table: "Repayments",
                column: "PaymentId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Applications_Income",
                table: "LoanApplications",
                sql: "[MonthlyIncome] IS NULL OR [MonthlyIncome] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ApplicationId",
                table: "Payments",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionRef",
                table: "Payments",
                column: "TransactionRef",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Repayments_Payments_PaymentId",
                table: "Repayments",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "PaymentId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Repayments_Payments_PaymentId",
                table: "Repayments");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Repayments_PaymentId",
                table: "Repayments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Applications_Income",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Repayments");

            migrationBuilder.DropColumn(
                name: "ApplicantDateOfBirth",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "MonthlyIncome",
                table: "LoanApplications");
        }
    }
}
