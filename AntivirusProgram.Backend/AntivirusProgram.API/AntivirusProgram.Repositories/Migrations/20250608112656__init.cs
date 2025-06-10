using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AntivirusProgram.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileScanResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    IsVirus = table.Column<bool>(type: "bit", nullable: false),
                    IsTestFile = table.Column<bool>(type: "bit", nullable: false),
                    ScanDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdditionalInfo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileScanResults", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileScanResults_FileHash",
                table: "FileScanResults",
                column: "FileHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileScanResults");
        }
    }
}
