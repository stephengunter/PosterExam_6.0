using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations.Data
{
    public partial class addTermNotesQIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QIds",
                table: "TermNotes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RQIds",
                table: "TermNotes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QIds",
                table: "TermNotes");

            migrationBuilder.DropColumn(
                name: "RQIds",
                table: "TermNotes");
        }
    }
}
