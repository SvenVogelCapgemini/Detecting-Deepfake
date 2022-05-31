using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main_Node.Migrations
{
    public partial class renameEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Methode",
                table: "Task",
                newName: "Method");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Method",
                table: "Task",
                newName: "Methode");
        }
    }
}
