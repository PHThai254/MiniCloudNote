using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniCloudNote.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class LinkNoteToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Notes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Notes_OwnerId",
                table: "Notes",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_AspNetUsers_OwnerId",
                table: "Notes",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_AspNetUsers_OwnerId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_OwnerId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Notes");
        }
    }
}
