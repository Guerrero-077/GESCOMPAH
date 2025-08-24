using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Route = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "plaza",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plaza", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    ReplacedByTokenHash = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "systemParameters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_systemParameters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormModules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormId = table.Column<int>(type: "int", nullable: false),
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormModules_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormModules_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Establishment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaM2 = table.Column<double>(type: "float", nullable: false),
                    RentValueBase = table.Column<double>(type: "float", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlazaId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Establishment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Establishment_plaza_PlazaId",
                        column: x => x.PlazaId,
                        principalTable: "plaza",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolFormPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    FormId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolFormPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolFormPermissions_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolFormPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolFormPermissions_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Document = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persons_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTimeAssigned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstablishmentId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_Establishment_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublicId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstablishmentId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_Establishment_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolUsers_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Active", "CreatedAt", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Amazonas" },
                    { 2, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Antioquia" },
                    { 3, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Arauca" },
                    { 4, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Atlántico" },
                    { 5, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Bolívar" },
                    { 6, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Boyacá" },
                    { 7, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Caldas" },
                    { 8, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Caquetá" },
                    { 9, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Casanare" },
                    { 10, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Cauca" },
                    { 11, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Cesar" },
                    { 12, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Chocó" },
                    { 13, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Córdoba" },
                    { 14, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Cundinamarca" },
                    { 15, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Guainía" },
                    { 16, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Guaviare" },
                    { 17, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Huila" },
                    { 18, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "La Guajira" },
                    { 19, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Magdalena" },
                    { 20, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Meta" },
                    { 21, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Nariño" },
                    { 22, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Norte de Santander" },
                    { 23, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Putumayo" },
                    { 24, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Quindío" },
                    { 25, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Risaralda" },
                    { 26, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "San Andrés y Providencia" },
                    { 27, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Santander" },
                    { 28, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Sucre" },
                    { 29, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Tolima" },
                    { 30, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Valle del Cauca" },
                    { 31, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Vaupés" },
                    { 32, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Vichada" }
                });

            migrationBuilder.InsertData(
                table: "Forms",
                columns: new[] { "Id", "Active", "CreatedAt", "Description", "IsDeleted", "Name", "Route" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vista general del dashboard", false, "Inicio", "dashboard" },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ver y gestionar establecimientos", false, "Establecimientos", "establishments/main" },
                    { 3, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gestión de arrendatarios", false, "Arrendatarios", "tenants" },
                    { 4, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gestión de Citas", false, "Citas", "quotes" },
                    { 5, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Configuración de modelos de seguridad", false, "Modelos de Seguridad", "security/main" },
                    { 6, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gestión de roles", false, "Roles", "security/roles" },
                    { 7, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gestión de Formularios", false, "Formularios", "security/forms" },
                    { 8, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gestión de módulos", false, "Modulos", "security/modules" },
                    { 9, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gestión de Permisos", false, "Permisos", "security/permissions" },
                    { 10, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ajustes principales del sistema", false, "Configuración Principal", "settings/main" }
                });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "Active", "CreatedAt", "Description", "Icon", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Panel de control principal", "home", false, "Panel Principal" },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gestión de establecimientos", "store", false, "Establecimientos" },
                    { 3, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gestión de arrendatarios", "people", false, "Arrendatarios" },
                    { 4, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gestión de citas", "event_note", false, "Gestion de citas" },
                    { 5, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gestión de seguridad y permisos", "security", false, "Seguridad" },
                    { 6, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Configuración general del sistema", "settings", false, "Configuración" }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Active", "CreatedAt", "Description", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Permite ver registros", false, "Ver" },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Permite crear registros", false, "Crear" },
                    { 3, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Permite editar registros", false, "Editar" },
                    { 4, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Permite eliminar registros", false, "Eliminar" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Active", "CreatedAt", "Description", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rol Con permisos Administrativos", false, "Administrador" },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rol con permisos de arrendador", false, "Arrendador" }
                });

            migrationBuilder.InsertData(
                table: "plaza",
                columns: new[] { "Id", "Active", "CreatedAt", "Description", "IsDeleted", "Location", "Name" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Espacio principal para eventos masivos", false, "Centro Ciudad", "Plaza Central" },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Zona adecuada para ferias temporales", false, "Zona Norte", "Plaza Norte" }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Active", "CreatedAt", "DepartmentId", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Acevedo" },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Agrado" },
                    { 3, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Aipe" },
                    { 4, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Algeciras" },
                    { 5, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Altamira" },
                    { 6, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Baraya" },
                    { 7, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Campoalegre" },
                    { 8, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Colombia" },
                    { 9, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Elías" },
                    { 10, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Garzón" },
                    { 11, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Gigante" },
                    { 12, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Guadalupe" },
                    { 13, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Hobo" },
                    { 14, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Iquira" },
                    { 15, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Isnos" },
                    { 16, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "La Argentina" },
                    { 17, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "La Plata" },
                    { 18, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Nátaga" },
                    { 19, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Neiva" },
                    { 20, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Oporapa" },
                    { 21, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Paicol" },
                    { 22, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Palermo" },
                    { 23, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Palestina" },
                    { 24, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Pital" },
                    { 25, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Pitalito" },
                    { 26, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Rivera" },
                    { 27, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Saladoblanco" },
                    { 28, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "San Agustín" },
                    { 29, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Santa María" },
                    { 30, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Suaza" },
                    { 31, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Tarqui" },
                    { 32, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Tello" },
                    { 33, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Teruel" },
                    { 34, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Tesalia" },
                    { 35, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Timaná" },
                    { 36, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Villavieja" },
                    { 37, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 17, false, "Yaguará" }
                });

            migrationBuilder.InsertData(
                table: "Establishment",
                columns: new[] { "Id", "Active", "Address", "AreaM2", "CreatedAt", "Description", "IsDeleted", "Name", "PlazaId", "RentValueBase" },
                values: new object[,]
                {
                    { 1, true, "Cr 1 ", 500.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Establecimiento amplio con excelente ubicación.", false, "Centro Comercial Primavera", 1, 2500.0 },
                    { 2, true, "Cr 1 ", 120.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Oficina moderna en zona empresarial.", false, "Oficina Torre Norte", 2, 1500.0 },
                    { 3, true, "Cr 1 ", 1000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Espacio para almacenamiento de gran capacidad.", false, "Bodega Industrial Sur", 1, 3000.0 }
                });

            migrationBuilder.InsertData(
                table: "FormModules",
                columns: new[] { "Id", "Active", "CreatedAt", "FormId", "IsDeleted", "ModuleId" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false, 1 },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, false, 2 },
                    { 3, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, false, 3 },
                    { 4, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, false, 4 },
                    { 5, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, false, 5 },
                    { 6, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, false, 5 },
                    { 7, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, false, 5 },
                    { 8, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, false, 5 },
                    { 9, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, false, 5 },
                    { 10, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, false, 6 }
                });

            migrationBuilder.InsertData(
                table: "RolFormPermissions",
                columns: new[] { "Id", "Active", "CreatedAt", "FormId", "IsDeleted", "PermissionId", "RolId" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, 1, 1 },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, 2, 1 },
                    { 3, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, 3, 1 },
                    { 4, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, 4, 1 },
                    { 5, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, false, 1, 1 },
                    { 6, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, false, 2, 1 },
                    { 7, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, false, 3, 1 },
                    { 8, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, false, 4, 1 },
                    { 9, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, false, 1, 1 },
                    { 10, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, false, 2, 1 },
                    { 11, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, false, 3, 1 },
                    { 12, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, false, 4, 1 },
                    { 13, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, false, 1, 1 },
                    { 14, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, false, 2, 1 },
                    { 15, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, false, 3, 1 },
                    { 16, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, false, 4, 1 },
                    { 17, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, false, 1, 1 },
                    { 18, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, false, 2, 1 },
                    { 19, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, false, 3, 1 },
                    { 20, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, false, 4, 1 },
                    { 21, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, false, 1, 1 },
                    { 22, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, false, 2, 1 },
                    { 23, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, false, 3, 1 },
                    { 24, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, false, 4, 1 },
                    { 25, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, false, 1, 1 },
                    { 26, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, false, 2, 1 },
                    { 27, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, false, 3, 1 },
                    { 28, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, false, 4, 1 },
                    { 29, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, false, 1, 1 },
                    { 30, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, false, 2, 1 },
                    { 31, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, false, 3, 1 },
                    { 32, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, false, 4, 1 },
                    { 33, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, false, 1, 1 },
                    { 34, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, false, 2, 1 },
                    { 35, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, false, 3, 1 },
                    { 36, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, false, 4, 1 },
                    { 37, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, false, 1, 1 },
                    { 38, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, false, 2, 1 },
                    { 39, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, false, 3, 1 },
                    { 40, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, false, 4, 1 }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "Active", "CreatedAt", "DateTimeAssigned", "Description", "Email", "EstablishmentId", "FullName", "IsDeleted", "Phone", "RequestDate" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 4, 10, 0, 0, 0, DateTimeKind.Unspecified), "Solicitud para conocer el local", "juan.perez@example.com", 1, "Juan Pérez", false, "3001234567", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 5, 11, 0, 0, 0, DateTimeKind.Unspecified), "Revisión de contrato anterior", "maria.gomez@example.com", 2, "María Gómez", false, "3019876543", new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, true, new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 6, 9, 0, 0, 0, DateTimeKind.Unspecified), "Consulta sobre requisitos para arriendo", "carlos.ramirez@example.com", 3, "Carlos Ramírez", false, "3021122334", new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Images",
                columns: new[] { "Id", "Active", "CreatedAt", "EstablishmentId", "FileName", "FilePath", "IsDeleted", "PublicId" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "primavera_1.jpg", "https://res.cloudinary.com/dmbndpjlh/image/upload/v1755031443/defaul_cj5nqv.png", false, "primavera_1" },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "primavera_2.jpg", "https://res.cloudinary.com/dmbndpjlh/image/upload/v1755031443/defaul_cj5nqv.png", false, "primavera_2" },
                    { 3, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "torre_1.jpg", "https://res.cloudinary.com/dmbndpjlh/image/upload/v1755031443/defaul_cj5nqv.png", false, "torre_1" },
                    { 4, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "torre_2.jpg", "https://res.cloudinary.com/dmbndpjlh/image/upload/v1755031443/defaul_cj5nqv.png", false, "torre_2" },
                    { 5, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "bodega_1.jpg", "https://res.cloudinary.com/dmbndpjlh/image/upload/v1755031443/defaul_cj5nqv.png", false, "bodega_1" },
                    { 6, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "bodega_2.jpg", "https://res.cloudinary.com/dmbndpjlh/image/upload/v1755031443/defaul_cj5nqv.png", false, "bodega_2" }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "Id", "Active", "Address", "CityId", "CreatedAt", "Document", "FirstName", "IsDeleted", "LastName", "Phone" },
                values: new object[] { 1, true, "Calle Principal 123", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "123456789", "Administrador", false, "General", "3000000000" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Active", "CreatedAt", "Email", "IsDeleted", "Password", "PersonId" },
                values: new object[] { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@gescomph.com", false, "AQAAAAEAACcQAAAAEK1QvWufDHBzB3acG5GKxdQTabH8BhbyLLyyZHo4WoOEvRYijXcOtRqsb3OeOpoGqw==", 1 });

            migrationBuilder.InsertData(
                table: "RolUsers",
                columns: new[] { "Id", "Active", "CreatedAt", "IsDeleted", "RolId", "UserId" },
                values: new object[] { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_EstablishmentId",
                table: "Appointments",
                column: "EstablishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_DepartmentId",
                table: "Cities",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name_DepartmentId",
                table: "Cities",
                columns: new[] { "Name", "DepartmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Establishment_PlazaId",
                table: "Establishment",
                column: "PlazaId");

            migrationBuilder.CreateIndex(
                name: "IX_FormModules_FormId_ModuleId",
                table: "FormModules",
                columns: new[] { "FormId", "ModuleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormModules_ModuleId",
                table: "FormModules",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_Name",
                table: "Forms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_EstablishmentId",
                table: "Images",
                column: "EstablishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Modules_Name",
                table: "Modules",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_CityId",
                table: "Persons",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_Document",
                table: "Persons",
                column: "Document",
                unique: true,
                filter: "[Document] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolFormPermissions_FormId",
                table: "RolFormPermissions",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_RolFormPermissions_PermissionId",
                table: "RolFormPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolFormPermissions_RolId_FormId_PermissionId",
                table: "RolFormPermissions",
                columns: new[] { "RolId", "FormId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolUsers_RolId",
                table: "RolUsers",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_RolUsers_UserId_RolId",
                table: "RolUsers",
                columns: new[] { "UserId", "RolId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PersonId",
                table: "Users",
                column: "PersonId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "FormModules");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "PasswordResetCodes");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RolFormPermissions");

            migrationBuilder.DropTable(
                name: "RolUsers");

            migrationBuilder.DropTable(
                name: "systemParameters");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Establishment");

            migrationBuilder.DropTable(
                name: "Forms");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "plaza");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
