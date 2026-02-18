using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeScale_Processor.Migrations.Values
{
    /// <inheritdoc />
    public partial class FixValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Metric",
                table: "Values",
                newName: "Value");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Values",
                newName: "Metric");
        }
    }
}
