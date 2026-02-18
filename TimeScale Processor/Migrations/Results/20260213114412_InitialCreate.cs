using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TimeScale_Processor.Migrations.Results
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    DeltaDate = table.Column<int>(type: "integer", nullable: false),
                    FirstData = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AverageExecutionTime = table.Column<double>(type: "double precision", nullable: false),
                    AverageMetric = table.Column<double>(type: "double precision", nullable: false),
                    MedianMetric = table.Column<double>(type: "double precision", nullable: false),
                    MinMetric = table.Column<double>(type: "double precision", nullable: false),
                    MaxMetric = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Results");
        }
    }
}
