using Microsoft.EntityFrameworkCore.Migrations;

namespace LexiconLMS.Migrations
{
    public partial class AddedDocuments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Document_Activities_ActivityId",
                table: "Document");

            migrationBuilder.DropForeignKey(
                name: "FK_Document_AspNetUsers_AppUserId",
                table: "Document");

            migrationBuilder.DropForeignKey(
                name: "FK_Document_Courses_CourseId",
                table: "Document");

            migrationBuilder.DropForeignKey(
                name: "FK_Document_Modules_ModuleId",
                table: "Document");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Document",
                table: "Document");

            migrationBuilder.RenameTable(
                name: "Document",
                newName: "Documents");

            migrationBuilder.RenameIndex(
                name: "IX_Document_ModuleId",
                table: "Documents",
                newName: "IX_Documents_ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_Document_CourseId",
                table: "Documents",
                newName: "IX_Documents_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Document_AppUserId",
                table: "Documents",
                newName: "IX_Documents_AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Document_ActivityId",
                table: "Documents",
                newName: "IX_Documents_ActivityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Documents",
                table: "Documents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Activities_ActivityId",
                table: "Documents",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_AspNetUsers_AppUserId",
                table: "Documents",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Courses_CourseId",
                table: "Documents",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Modules_ModuleId",
                table: "Documents",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Activities_ActivityId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_AspNetUsers_AppUserId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Courses_CourseId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Modules_ModuleId",
                table: "Documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Documents",
                table: "Documents");

            migrationBuilder.RenameTable(
                name: "Documents",
                newName: "Document");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_ModuleId",
                table: "Document",
                newName: "IX_Document_ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_CourseId",
                table: "Document",
                newName: "IX_Document_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_AppUserId",
                table: "Document",
                newName: "IX_Document_AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_ActivityId",
                table: "Document",
                newName: "IX_Document_ActivityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Document",
                table: "Document",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Document_Activities_ActivityId",
                table: "Document",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Document_AspNetUsers_AppUserId",
                table: "Document",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Document_Courses_CourseId",
                table: "Document",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Document_Modules_ModuleId",
                table: "Document",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
