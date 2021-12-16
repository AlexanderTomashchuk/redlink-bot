using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class InitialCreate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastMessageWorkflowType",
                table: "AppUser",
                newName: "InProgressChainWorkflowName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InProgressChainWorkflowName",
                table: "AppUser",
                newName: "LastMessageWorkflowType");
        }
    }
}
