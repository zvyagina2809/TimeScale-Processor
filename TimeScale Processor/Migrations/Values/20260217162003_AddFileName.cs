using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeScale_Processor.Migrations.Values
{
    /// <inheritdoc />
    public partial class AddFileName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Values",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Values");
        }
    }
}
