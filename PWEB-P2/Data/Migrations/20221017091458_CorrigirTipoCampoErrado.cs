using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWEB_P2.Data.Migrations
{
    public partial class CorrigirTipoCampoErrado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdadeMinima",
                table: "Cursos",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IdadeMinima",
                table: "Cursos",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
