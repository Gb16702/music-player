using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MusicPlayer.Infrastructure.Persistence;

#nullable disable

namespace MusicPlayer.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(MusicPlayerDbContext))]
    [Migration("20260913150000_AddUserProfileOnboarding")]
    public class AddUserProfileOnboarding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "user_profiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "user_profiles",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OnboardingCompleted",
                table: "user_profiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(
                """
                UPDATE user_profiles
                SET "OnboardingCompleted" = TRUE
                WHERE "DisplayName" IS NOT NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE user_profiles
                SET "DisplayName" = ''
                WHERE "DisplayName" IS NULL;
                """);

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "OnboardingCompleted",
                table: "user_profiles");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "user_profiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
