using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityProject.Migrations
{
    /// <inheritdoc />
    public partial class newPropForUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegisterGeneratedCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegisterGeneratedCode",
                table: "Users");
        }
    }
}
