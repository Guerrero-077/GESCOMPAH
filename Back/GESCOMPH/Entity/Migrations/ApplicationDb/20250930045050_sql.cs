using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entity.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class sql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clauses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clauses", x => x.Id);
                });

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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                name: "RefreshToken",
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
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "Establishments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaM2 = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RentValueBase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UvtQty = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PlazaId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Establishments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Establishments_plaza_PlazaId",
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    PublicId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    EstablishmentId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_Establishments_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTimeAssigned = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    EstablishmentId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_Establishments_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    TotalBaseRentAgreed = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalUvtQtyAgreed = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contracts_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "contractClauses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    ClauseId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contractClauses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contractClauses_clauses_ClauseId",
                        column: x => x.ClauseId,
                        principalTable: "clauses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contractClauses_contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObligationMonths",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UvtQtyApplied = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UvtValueApplied = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    VatRateApplied = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    BaseAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    VatAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    DaysLate = table.Column<int>(type: "int", nullable: true),
                    LateAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Locked = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObligationMonths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObligationMonths_contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "premisesLeaseds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    EstablishmentId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_premisesLeaseds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_premisesLeaseds_Establishments_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_premisesLeaseds_contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Amazonas" },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Antioquia" },
                    { 3, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Arauca" },
                    { 4, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Atlántico" },
                    { 5, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Bolívar" },
                    { 6, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Boyacá" },
                    { 7, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Caldas" },
                    { 8, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Caquetá" },
                    { 9, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Casanare" },
                    { 10, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Cauca" },
                    { 11, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Cesar" },
                    { 12, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Chocó" },
                    { 13, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Córdoba" },
                    { 14, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Cundinamarca" },
                    { 15, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Guainía" },
                    { 16, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Guaviare" },
                    { 17, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Huila" },
                    { 18, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "La Guajira" },
                    { 19, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Magdalena" },
                    { 20, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Meta" },
                    { 21, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Nariño" },
                    { 22, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Norte de Santander" },
                    { 23, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Putumayo" },
                    { 24, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Quindío" },
                    { 25, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Risaralda" },
                    { 26, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "San Andrés y Providencia" },
                    { 27, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Santander" },
                    { 28, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Sucre" },
                    { 29, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Tolima" },
                    { 30, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Valle del Cauca" },
                    { 31, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Vaupés" },
                    { 32, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Vichada" }
                });

            migrationBuilder.InsertData(
                table: "Forms",
                columns: new[] { "Id", "Active", "CreatedAt", "Description", "IsDeleted", "Name", "Route" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vista general de la pagina principal", false, "Inicio", "dashboard" },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ver y gestionar establecimientos", false, "Establecimientos", "establishments/main" },
                    { 3, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gestión de arrendatarios", false, "Arrendatarios", "tenants" },
                    { 4, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gestión de contratos", false, "Contratos", "contracts" },
                    { 5, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gestión de Citas", false, "Citas", "appointment" },
                    { 6, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Configuración de modelos de seguridad", false, "Modelos de Seguridad", "security/main" },
                    { 7, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gestión de roles", false, "Roles", "security/roles" },
                    { 8, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gestión de Formularios", false, "Formularios", "security/forms" },
                    { 9, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gestión de módulos", false, "Modulos", "security/modules" },
                    { 10, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gestión de Permisos", false, "Permisos", "security/permissions" },
                    { 11, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ajustes principales del sistema", false, "Configuración Principal", "settings/main" }
                });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "Active", "CreatedAt", "Description", "Icon", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Panel de control principal", "home", false, "Panel Principal" },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gestión de establecimientos", "store", false, "Establecimientos" },
                    { 3, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gestión de arrendatarios", "people", false, "Arrendatarios" },
                    { 4, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gestión de contratos", "description", false, "Contratos" },
                    { 5, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gestión de citas", "event_note", false, "Gestión de Citas" },
                    { 6, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gestión de seguridad y permisos", "security", false, "Seguridad" },
                    { 7, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Configuración general del sistema", "settings", false, "Configuración" }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Active", "CreatedAt", "Description", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permite ver registros", false, "Ver" },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permite crear registros", false, "Crear" },
                    { 3, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permite editar registros", false, "Editar" },
                    { 4, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Permite eliminar registros", false, "Eliminar" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Active", "CreatedAt", "Description", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rol Con permisos Administrativos", false, "Administrador" },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rol con permisos de arrendador", false, "Arrendador" }
                });

            migrationBuilder.InsertData(
                table: "clauses",
                columns: new[] { "Id", "Active", "CreatedAt", "Description", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "En virtud del presente contrato, el ARRENDADOR concede al ARRENDATARIO el uso y goce del inmueble de las siguientes características: a) Locales 5A, 6A Y 7A, ubicados en el costado norte de la Plaza de Mercado del municipio de Palermo (H).", false, "CLÁUSULA PRIMERA. - OBJETO" },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "El término de duración del presente contrato será de SEIS (6) MESES, contados a partir de la fecha de la firma del presente contrato.", false, "CLÁUSULA SEGUNDA: - VIGENCIA" },
                    { 3, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "EL ARRENDATARIO se obliga a pagar mensualmente al ARRENDADOR el Valor/Canon, de 0,8  U.V.T. por cada uno de los Locales dados en arriendo, por un valor/Canon total de 2,4 U.V.T., que al momento de la suscripción del presente contrato (año 2025) es de, CIENTO DIECINUEVE MIL QUINIENTOS DIECISIETE PESOS ($119.517) M/CTE, más I.V.A., según lo estipulado en el artículo 362 del Acuerdo No. 037 (Estatuto Tributario Municipal) del 04 de diciembre del 2012,  pagaderos de manera anticipada dentro de los cinco (5) primeros días de cada mes previa expedición del recibo de pago en las oficinas de la Secretaría de Hacienda-Tesorería del Municipio de Palermo (H). Parágrafo: El valor se ajustará periódicamente en los términos señalados por el Estatuto Tributario Municipal y el Honorable Concejo Municipal.", false, "CLÁUSULA TERCERA.- VALOR DEL CANON DE ARRENDAMIENTO Y FORMA DE PAGO" },
                    { 4, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "EL ARRENDATARIO se compromete a utilizar los Locales Nº 5A, 6A Y 7A ubicados en el costado norte de la Plaza de Mercado del municipio de Palermo (H), para establecer un centro de acopio y gestión de residuos reciclables, operado por el establecimiento de comercio supermercado centro sur Palermo, EL ARRENDATARIO no podrá dar un uso distinto al aquí establecido. El incumplimiento de esta obligación, dará derecho al ARRENDADOR para la terminación unilateral del presente contrato y exigir la entrega del bien inmueble.", false, "CLÁUSULA CUARTA. - DESTINACIÓN" },
                    { 5, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Los daños que se ocasionen a los bienes entregados en arrendamiento durante la vigencia del presente contrato, serán reparados y cubiertos sus costos de reparación en su totalidad por el ARRENDATARIO.", false, "CLÁUSULA QUINTA. – REPARACIONES" },
                    { 6, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "El ARRENDATARIO declara que ha recibido el inmueble en buen uso y estado de funcionamiento para desarrollar la destinación contratada a su vez se obliga a cuidarlo, conservarlo y mantenerlo y, que en el mismo estado lo restituirá a EL ARRENDADOR. Los daños al inmueble derivados del uso, mal trato y/o descuido por parte del ARRENDATARIO durante su tenencia, serán de su cargo y EL ARRENDADOR estará facultado para hacerlos por su cuenta y posteriormente reclamar su valor al ARRENDATARIO.", false, "CLÁUSULA SEXTA. - RECIBO Y ESTADO" },
                    { 7, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "El ARRENDATARIO tendrá a su cargo las reparaciones locativas a que se refiere la Ley y no podrán realizarse otras sin el consentimiento previo expreso del ARRENDADOR.  Parágrafo: Las mejoras realizadas al bien serán del ARRENDADOR y no habrá lugar al reconocimiento del precio, costo o indemnización alguna por parte del Arrendatario por las mejoras hechas. Las mejoras no podrán retirarse salvo que el ARRENDADOR lo exija por escrito, a lo que el Arrendatario accederá inmediatamente a su costa, dejando el Inmueble en el mismo buen estado en que lo recibió del ARRENDADOR, salvo el deterioro natural por el uso legítimo.", false, "CLÁUSULA SEPTIMA. – MEJORAS" },
                    { 8, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "EL ARRENDATARIO pagará oportuna y totalmente los servicios públicos de agua y energía y los demás que se encontraren instalados en los locales comerciales, desde la fecha en que inicie el arrendamiento hasta la terminación del mismo. El incumplimiento del Arrendatario en el pago oportuno de los servicios públicos antes mencionados se tendrá como incumplimiento del Contrato. Parágrafo 1: EL ARRENDATARIO declara que ha recibido en perfecto estado de funcionamiento y de conservación las instalaciones para uso de los servicios públicos del bien, que se abstendrá de modificarlas sin permiso previo y escrito del ARRENDADOR y que responderá por daños y/o violaciones de los reglamentos de las correspondientes empresas de servicios públicos, según contrato de condiciones uniformes. Parágrafo 2: EL ARRENDATARIO reconoce que el ARRENDADOR en ningún caso y bajo ninguna circunstancia es responsable por la interrupción o deficiencia en la prestación de cualquiera de los servicios públicos del bien. En caso de la prestación deficiente o suspensión de cualquiera de los servicios públicos del bien, el Arrendatario reclamará de manera directa a las empresas prestadoras del servicio y no al ARRENDADOR.", false, "CLÁUSULA OCTAVA. – SERVICIOS PÚBLICOS" },
                    { 9, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A) DEL ARRENDADOR: 1. EL ARRENDADOR, Hará entrega material del bien inmueble al ARRENDATARIO, de los locales 5A , 6A Y 7A ubicados en el costado norte de la Plaza de Mercado del municipio de Palermo (H), en buen estado de servicio, seguridad, sanidad y demás de usos conexos convenidos en el presente contrato mediante inventario, el cual hará el ARRENDADOR. B) DEL ARRENDATARIO: 1. Pagar al ARRENDADOR en el lugar convenido en la cláusula segunda del presente contrato, mensualmente el canon de arrendamiento. 2. Efectuar del pago mensual de los Servicios Públicos, si este goza de ellos 3. Gozar del inmueble según los términos del contrato. 4. Velar por la conservación del bien inmueble y las cosas recibidas en arrendamiento. 5. Restituir/Entregar el bien inmueble a la terminación del contrato de arrendamiento, en el estado en que fue entregado, poniéndolo a disposición del ARRENDADOR, éste debe encontrarse paz y salvo por todo concepto entiéndase canon de arrendamiento y servicios públicos. 6. Permitir en cualquier tiempo las visitas del Arrendador o de sus representantes, para constatar el estado y la conservación del inmueble u otras circunstancias que sean de su interés, siempre y cuando dichas visitas no afecten la continuidad regular del servicio a cargo del Arrendatario. 7. Analizar y responder los requerimientos que formule razonablemente el Arrendador. Parágrafo: La responsabilidad del Arrendatario subsistirá aún después de restituido el inmueble, mientras el Municipio no haya entregado el paz y salvo correspondiente por escrito al ARRENDATARIO.", false, "CLÁUSULA NOVENA. – OBLIGACIONES DE LAS PARTES" },
                    { 10, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Son causales de terminación del contrato de arrendamiento las estipuladas en artículo 17 de la Ley 80 de 1993, entre otras: 1. El no pago del canon de arrendamiento y/o servicios públicos por parte del arrendatario. 2. La enajenación, el subarriendo y/o La Cesión del inmueble y/o contrato de arrendamiento y el cambio de destinación del bien sin previa autorización expresa y escrita por parte del ARRENDADOR. 3. Las mejoras, cambios, ampliaciones, modificaciones y demás que se le realicen al bien sin previa autorización expresa y escrita por parte del ARRENDADOR. 4. Comportamientos por parte del arrendatario que afecten la tranquilidad y sana convivencia de los demás arrendatarios y ciudadanos. 5. La suspensión de la prestación de los servicios públicos del bien inmueble por mora en el pago de las facturas y/o acción del ARRENDATARIO. 6. Por la expiración del tiempo pactado. 7. Por incumplimiento de alguna de las obligaciones pactadas en el presente contrato. parágrafo: Las partes de común acuerdo en cualquier tiempo podrán dar por terminado el presente contrato de arrendamiento, aclarando que por cualquiera que sea la causa y/o motivo de la terminación del presente contrato de arrendamiento, éste debe ser entregado encontrándose a paz y salvo por todo concepto entiéndase canon de arrendamiento y servicios públicos y en el mismo estado en que fue recibido por parte del ARRENDATARIO.", false, "CLÁUSULA DÉCIMA. – TERMINACIÓN DEL CONTRATO" },
                    { 11, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vencido el periodo inicial o la última prórroga del Contrato o declarada la terminación del mismo por cualquier causa, EL ARRENDATARIO (i) restituirá el bien al ARRENDADOR en las mismas buenas condiciones en que lo recibió  del ARRENDADOR, salvo el deterioro natural causado por el uso legítimo, (ii) entregará al  ARRENDADOR los ejemplares originales de las facturas de cobro por concepto de servicios públicos del Inmueble correspondientes a los últimos tres (3) meses, debidamente canceladas por el Arrendatario, con una antelación de dos (2) días hábiles a la fecha fijada para la restitución material del bien inmueble al ARRENDADOR. parágrafo: La responsabilidad del Arrendatario subsistirá aún después de restituido el Inmueble, mientras el ARRENDADOR no haya entregado el paz y salvo correspondiente por escrito al Arrendatario.", false, "CLÁUSULA DÉCIMA PRIMERA: RESTITUCIÓN" },
                    { 12, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "EL ARRENDATARIO declara que (i) no ha tenido ni tiene posesión del bien, y (ii) que renuncia en beneficio del ARRENDADOR, a todo requerimiento para constituirlo en mora en el cumplimiento de las obligaciones a su cargo derivadas de este Contrato.", false, "CLÁUSULA DÉCIMA TERCERA. – RENUNCIA" },
                    { 13, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "El incumplimiento del ARRENDATARIO a cualquiera de  sus obligaciones legales o contractuales faculta al ARRENDADOR para ejercer las siguientes acciones, simultáneamente o en el orden que él elija: a) Declarar terminado el presente contrato y ordenar la restitución inmediatamente del bien, judicial y/o extrajudicialmente; b) Exigir y perseguir a través de cualquier medio, judicial o extrajudicialmente, al  ARRENDATARIO el monto y/o valor de los perjuicios resultantes del incumplimiento, así como de la multa por incumplimiento pactada en este Contrato.", false, "CLÁUSULA DÉCIMA TERCERA. – INCUMPLIMIENTO" },
                    { 14, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "EL ARRENDATARIO autoriza de manera expresa e irrevocable al ARRENDADOR para ingresar al bien en aras de recuperar su tenencia y ordenar la restitución del bien, con el solo requisito de la presencia de dos (2) testigos, en procura de evitar el deterioro o desmantelamiento del bien, en el evento que por cualquier causa o circunstancia el bien permanezca abandonado o deshabitado por el término de dos (2) meses o más.", false, "CLÁUSULA DÉCIMA CUARTA. – ABANDONO" },
                    { 15, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cuando el ARRENDATARIO incumpliera en el pago del canon de arrendamiento establecido en la cláusula tercera del presente contrato, EL ARRENDADOR, podrá dar por terminado de manera unilateral el contrato y exigir la restitución del bien inmueble.", false, "CLÁUSULA DÉCIMA QUINTA. – MORA" },
                    { 16, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "La vigilancia, seguimiento, verificación y cumplimiento del presente contrato de arrendamiento, serán ejercidos por el alcalde del Municipio de Palermo y/o el funcionario que designe para la Supervisión, quien podrá impartir al ARRENDATARIO las instrucciones e indicaciones necesarias para la cabal ejecución del objeto contratado y desarrollará las demás actividades previstas en este contrato y en el contrato de supervisión o en el acto de designación, según sea el caso.", false, "CLÁUSULA DÉCIMA SEXTA.- SUPERVISIÓN" },
                    { 17, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tanto EL ARRENDADOR como EL ARRENDATARIO podrán dar por terminado el presente contrato de arrendamiento mediante preaviso por escrito dado a la otra parte con mínimo Un (01) meses de antelación del vencimiento del término pactado en el presente contrato.", false, "CLÁUSULA DÉCIMA SÉPTIMA. - PREAVISO" },
                    { 18, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "El presente contrato no se prorrogará de forma automática, sin embargo, las partes previo aviso por escrito podrán manifestar la voluntad continuar con el presente, con mínimo Un (01) mes de antelación a la terminación del contrato. parágrafo: Para concederse la prórroga del contrato, será requisito encontrarse paz y salvo por todo concepto.", false, "CLÁUSULA DÉCIMA OCTAVA. – PRÓRROGA" },
                    { 19, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "En caso de incumplimiento de las obligaciones derivadas del presente contrato a cargo del ARRENDATARIO establecidas en cualquiera de las cláusulas establecidas, éste deberá pagar al ARRENDADOR a título de cláusula penal el valor equivalente al treinta por ciento (30%) del valor total del contrato, sin menoscabo del pago del canon de arrendamiento y de los perjuicios que pudieren ocasionarse como consecuencia del incumplimiento contractual.", false, "CLÁUSULA DÉCIMA NOVENA. – CLÁUSULA PENAL" },
                    { 20, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "El presente contrato se rige por lo dispuesto en las leyes del derecho civil y comercial respecto al contrato de arrendamiento y lo estipulado en la Ley 80 de 1993 y sus Decretos reglamentarios, la Ley 9 de 1989, Ley 388 de 1997, Decreto 1504 de 1998 y demás normatividad vigente o la que modificase las anteriores.", false, "CLÁUSULA VIGÉSIMA. – RÉGIMEN LEGAL" },
                    { 21, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "El presente Contrato anula todo convenio anterior relativo al arrendamiento del mismo inmueble y solamente podrá ser modificado por acuerdo suscrito por las partes o por EL ARRENDADOR.", false, "CLÁUSULA VIGÉSIMA PRIMERA. - VALIDEZ" },
                    { 22, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "EL ARRENDATARIO acepta, entiende y conoce, de manera voluntaria e inequívoca, que EL ARRENDADOR  en cumplimiento de su obligación legal de prevenir y controlar el lavado de activos y la financiación del terrorismo, por considerarlo una causal objetiva, podrá terminar unilateralmente el presente contrato en cualquier momento y sin previo aviso, cuando el ARRENDATARIO, llegare a ser: (i) vinculado por parte de las autoridades nacionales e internacionales a cualquier tipo de investigación por delitos de narcotráfico, terrorismo, secuestro, lavado de activos, financiación del terrorismo y administración de recursos relacionados con actividades terroristas u otros delitos relacionados con el lavado de activos y financiación del terrorismo; (ii) incluido en listas para el control de lavado de activos y financiación del terrorismo administradas por cualquier autoridad nacional o extranjera, tales como la lista de la Oficina de Control de Activos en el Exterior – OFAC emitida por la Oficina del Tesoro de los Estados Unidos de Norte América, la lista de la Organización de las Naciones Unidas y otras listas públicas relacionadas con el tema del lavado de activos y financiación del terrorismo; (iii) condenado por parte de las autoridades nacionales o internacionales en cualquier tipo de proceso judicial relacionado con la comisión de los anteriores delitos; o iv) llegare a ser señalado públicamente por cualquier como investigados por delitos de narcotráfico, terrorismo, corrupción, secuestro, lavado de activos, financiación del terrorismo y administración de recursos relacionados con actividades terroristas y/o cualquier delito colateral o subyacente a estos. parágrafo. De llegarse a presentar alguna de las situaciones anteriormente mencionadas frente a algún beneficiario, usuario, u otra persona natural o jurídica que tenga inherencia en el flujo de recursos EL ARRENDATARIO deberá asumir la responsabilidad.", false, "CLÁUSULA VIGÉSIMA SEGUNDA. - CONTROL PARA EL LAVADO DE ACTIVOS Y FINANCIACIÓN DEL TERRORISMO" },
                    { 23, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "EL ARRENDADOR prohíbe expresa y terminantemente al ARRENDATARIO dar al inmueble destinación con los fines contemplados en el literal b) del Parágrafo del Artículo 3º del Decreto 180 del 1988 y del Artículo 34 de la Ley 30 del 1986. En consecuencia, el ARRENDATARIO se obliga a no utilizar el inmueble objeto de este contrato, para ocultar o como depósito de armas o explosivos y dineros de grupos terroristas, o para que en él se elabore o almacene, venda o use drogas, estupefacientes o sustancias alucinógenas, tales como marihuana, hachís, cocaína, morfina, heroína, metacualona y afines. EL ARRENDATARIO se obliga a no guardar, ni permitir que se guarden en el inmueble arrendado sustancias inflamables o explosivas que pongan en peligro la seguridad de él, y en caso de que ocurriere dentro del mismo, enfermedad infectocontagiosa serán de cuenta del ARRENDATARIO los gastos de desinfección que ordenen las autoridades sanitarias. Por lo anterior el bien inmueble de propiedad DEL ARRENDADOR, será protegido al suscribir el presente contrato, de lo preceptuado en la Ley 1708 del 20 de enero de 2014.", false, "CLÁUSULA VIGÉSIMA TERCERA. - PROHIBICIONES ESPECIALES" },
                    { 24, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "El ARRENDATARIO no está facultado para ceder el arriendo ni subarrendar, a menos que medie autorización previa y escrita de EL ARRENDADOR.  En caso de contravención, EL ARRENDADOR podrá dar por terminado el contrato de arrendamiento y exigir la entrega del inmueble.", false, "CLÁUSULA VIGÉSIMA CUARTA. - SUBARRIENDO Y CESIÓN" },
                    { 25, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "El ARRENDADOR es una entidad independiente del ARRENDATARIO y, en consecuencia, el ARRENDADOR no es su representante, agente o mandatario. El ARRENDADOR no tiene la facultad de hacer declaraciones, representaciones o compromisos en nombre del ARRENDATARIO, ni de tomar decisiones o iniciar acciones que generen obligaciones a su cargo y viceversa.", false, "CLÁUSULA VIGÉSIMA QUINTA. - INDEPENDENCIA DEL ARRENDADOR" },
                    { 26, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "El ARRENDATARIO se obliga a mantener indemne al ARRENDADOR de cualquier daño o perjuicio originado en reclamaciones de terceros que tenga como causa sus actuaciones hasta por el monto del daño o perjuicio causado. El ARRENDATARIO mantendrá indemne al ARRENDADOR por cualquier obligación de carácter laboral o relacionado que se originen en el incumplimiento de las obligaciones laborales que el ARRENDATARIO asume frente al personal, subordinados o terceros que se vinculen a la ejecución de las obligaciones derivadas del presente contrato.", false, "CLÁUSULA VIGÉSIMA SEXTA. - INDEMNIDAD" },
                    { 27, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "EL ARRENDATARIO Con la suscripción del presente contrato, afirma bajo la gravedad de juramento que no se haya incurso en ninguna de las inhabilidades e incompatibilidades y demás prohibiciones para contratar previstas en la constitución política de Colombia la Ley 80 de 1993 Artículo 8,Artículo 5 Ley 1738 de 2014y demás disposiciones legales sobre la materia. En el evento de llegar a sobrevenir alguna causal actuara conforme a lo previsto a la Ley.", false, "CLÁUSULA VIGÉSIMA SEPTIMA. - INCOMPATIBILIDADES" },
                    { 28, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "EL CONTRATISTA se compromete a guardar estricta confidencialidad y a dar cumplimiento a la normatividad aplicable vigente, respecto de toda la información y datos personales que conozca y se le entregue por cualquier medio durante el plazo de ejecución, y por ende éste no podrá realizar su publicación, divulgación y utilización para fines propios o de terceros no autorizados. Así mismo, respetará los acuerdos de confidencialidad suscritos por EL CONTRATANTE con terceros para la celebración de negocios, preacuerdos o acuerdos por el mismo tiempo por el que EL CONTRATANTE se compromete con los terceros a guardar la debida reserva.", false, "CLÁUSULA VIGÉSIMA OCTAVA. – CONFIDENCIALIDAD" },
                    { 29, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Las partes quedan exoneradas de responsabilidad por el incumplimiento de cualquiera de sus obligaciones o por la demora en la satisfacción de cualquiera de las prestaciones a su cargo derivadas del presente Contrato, cuando el incumplimiento sea resultado o consecuencia de la ocurrencia de un evento de fuerza mayor y caso fortuito debidamente invocadas y constatadas de acuerdo con la ley y la jurisprudencia colombiana.", false, "CLÁUSULA VIGÉSIMA NOVENA. - CASO FORTUITO Y FUERZA MAYOR" },
                    { 30, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "EL ARRENDATARIO declara de manera expresa que reconoce y acepta que este Contrato presta mérito ejecutivo para exigir del ARRENDATARIO y a favor  del ARRENDADOR el pago de PRIMERO:(i) los cánones de arrendamiento causados y no pagados por EL ARRENDATARIO, SEGUNDO:(ii) las multas y sanciones que se causen por el incumplimiento del ARRENDATARIO de cualquiera de las obligaciones a su cargo en virtud de la ley o de este Contrato, TERCERO:(iii) las sumas causadas y no pagadas por EL ARRENDATARIO por concepto de servicios públicos del Inmueble, cuotas de administración y cualquier otra suma de dinero que por cualquier concepto deba ser pagada por EL ARRENDATARIO; para lo cual bastará la sola afirmación de incumplimiento del Arrendatario hecha por EL ARRENDADOR, afirmación que solo podrá ser desvirtuada por el ARRENDATARIO con la presentación de los respectivos recibos de pago. Parágrafo 1: Las Partes acuerdan que cualquier copia autentica de este Contrato tendrá mismo valor que el original para efectos judiciales y extrajudiciales. Parágrafo 2. El cobro de los conceptos antes referidos se adelantará mediante jurisdicción coactiva del municipio", false, "CLÁUSULA TRIGÉSIMA. – MÉRITO EJECUTIVO" },
                    { 31, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Todas las actuaciones que se deriven del presente documento se harán con sujeción a lo dispuesto en la Ley 1712 de 2014.", false, "CLÁUSULA TRIGÉSIMA PRIMERA. - SUJECIÓN A LA LEY DE TRANSPARENCIA Y DEL DERECHO A LA INFORMACIÓN PÚBLICA NACIONAL" },
                    { 32, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "EL ARRENDATARIO, en la condición de titular de la información personal autoriza al ARRENDADOR para almacenar en sus bases de información los datos personales y tener acceso a los mismos en cualquier momento, tanto durante la vigencia de la relación contractual como con posterioridad a la misma, esta autorización abarca la posibilidad de recolectar y almacenar dichos datos en las bases de datos y sistemas o software de EL CONTRATANTE. Entiendo que el tratamiento de los datos personales por parte del ARRENDADOR, tiene una finalidad legitima de acuerdo con la ley y la constitución y obedece al manejo interno de los datos en desarrollo de la relación contractual existente entre las partes y que la información personal será manejada con las medidas técnicas, humanas y administrativas necesarias para garantizar la seguridad y reserva de la información. Parágrafo: EL ARRENDADOR ha enterado a EL ARRENDATARIO, del derecho a conocer el uso a sus datos, acceder a ellos, actualizarlos y rectificarlos en cualquier momento. Igualmente, EL ARRENDADOR ha informado sobre el carácter facultativo de la respuesta a las preguntas que versen sobre datos sensibles.", false, "CLÁUSULA TRIGÉSIMA SEGUNDA. - AUTORIZACIÓN TRATAMIENTO DE DATOS" },
                    { 33, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Los avisos, solicitudes, comunicaciones y notificaciones que las partes deban hacer en desarrollo del presente contrato, deben constar por escrito y se entenderán debidamente efectuadas si se envía a cualquiera de los canales de notificación entregados por el ARRENDATARIO.", false, "CLÁUSULA TRIGÉSIMA TERCERA. - NOTIFICACIONES" },
                    { 34, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Hacen parte integrante de este contrato los siguientes documentos: 1. Los estudios y documentos previos. 2. La solicitud presentada por el ARRENDATARIO. 3. Acta de entrega e inventario de los inmuebles.", false, "CLÁUSULA TRIGÉSIMA CUARTA. – ANEXOS DEL CONTRATO" },
                    { 35, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Para efectos legales, se consagra como Domicilio contractual el Municipio de Palermo (H).", false, "CLÁUSULA TRIGÉSIMA QUINTA. – DOMICILIO" }
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
                table: "systemParameters",
                columns: new[] { "Id", "Active", "CreatedAt", "EffectiveFrom", "EffectiveTo", "IsDeleted", "Key", "Value" },
                values: new object[,]
                {
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "UVT", "51300" },
                    { 3, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "IVA", "0.19" }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Active", "CreatedAt", "DepartmentId", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Acevedo" },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Agrado" },
                    { 3, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Aipe" },
                    { 4, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Algeciras" },
                    { 5, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Altamira" },
                    { 6, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Baraya" },
                    { 7, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Campoalegre" },
                    { 8, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Colombia" },
                    { 9, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Elías" },
                    { 10, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Garzón" },
                    { 11, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Gigante" },
                    { 12, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Guadalupe" },
                    { 13, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Hobo" },
                    { 14, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Iquira" },
                    { 15, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Isnos" },
                    { 16, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "La Argentina" },
                    { 17, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "La Plata" },
                    { 18, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Nátaga" },
                    { 19, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Neiva" },
                    { 20, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Oporapa" },
                    { 21, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Paicol" },
                    { 22, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Palermo" },
                    { 23, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Palestina" },
                    { 24, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Pital" },
                    { 25, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Pitalito" },
                    { 26, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Rivera" },
                    { 27, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Saladoblanco" },
                    { 28, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "San Agustín" },
                    { 29, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Santa María" },
                    { 30, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Suaza" },
                    { 31, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Tarqui" },
                    { 32, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Tello" },
                    { 33, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Teruel" },
                    { 34, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Tesalia" },
                    { 35, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Timaná" },
                    { 36, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Villavieja" },
                    { 37, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, false, "Yaguará" }
                });

            migrationBuilder.InsertData(
                table: "Establishments",
                columns: new[] { "Id", "Active", "Address", "AreaM2", "CreatedAt", "Description", "IsDeleted", "Name", "PlazaId", "RentValueBase", "UvtQty" },
                values: new object[,]
                {
                    { 1, true, "Cra. 15 # 93-60, Bogotá", 120m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Local comercial en centro urbano con alto flujo peatonal.", false, "Centro Comercial Primavera", 1, 4500000m, 30m },
                    { 2, true, "Av. El Dorado # 69-76, Bogotá", 85m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Oficina empresarial con vista y sala de reuniones.", false, "Oficina Torre Norte Piso 7", 2, 3200000m, 22m },
                    { 3, true, "Cl. 57 Sur # 30-15, Bogotá", 750m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bodega con muelle de carga y altura de 8m.", false, "Bodega Industrial Sur", 1, 6800000m, 45m },
                    { 4, true, "Cra. 5 # 69A-19, Bogotá", 95m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Espacio para restaurante con extracción y bodega.", false, "Local Gastronómico Zona G", 1, 5200000m, 35m },
                    { 5, true, "Centro Comercial Primavera, pasillo central", 12m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Isla en pasillo principal, ideal retail liviano.", false, "Isla Comercial Pasillo Central", 1, 1200000m, 8m },
                    { 6, true, "Av. El Dorado # 69-76, Bogotá", 110m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Oficina abierta con 2 parqueaderos y cableado estructurado.", false, "Oficina Torre Norte Piso 12", 2, 3900000m, 26m }
                });

            migrationBuilder.InsertData(
                table: "FormModules",
                columns: new[] { "Id", "Active", "CreatedAt", "FormId", "IsDeleted", "ModuleId" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, 1 },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, false, 2 },
                    { 3, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, false, 3 },
                    { 4, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, false, 4 },
                    { 5, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, false, 5 },
                    { 6, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, false, 6 },
                    { 7, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, false, 6 },
                    { 8, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, false, 6 },
                    { 9, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, false, 6 },
                    { 10, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, false, 6 },
                    { 11, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 11, false, 7 }
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
                    { 40, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, false, 4, 1 },
                    { 41, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 11, false, 1, 1 },
                    { 42, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 11, false, 2, 1 },
                    { 43, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 11, false, 3, 1 },
                    { 44, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 11, false, 4, 1 },
                    { 45, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, 1, 2 },
                    { 46, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, false, 1, 2 },
                    { 47, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, false, 1, 2 },
                    { 48, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 11, false, 1, 2 }
                });

            migrationBuilder.InsertData(
                table: "Images",
                columns: new[] { "Id", "Active", "CreatedAt", "EstablishmentId", "FileName", "FilePath", "IsDeleted", "PublicId" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "primavera_1.jpg", "https://res.cloudinary.com/dmbndpjlh/image/upload/v1755031443/defaul_cj5nqv.png", false, "primavera_1" },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "primavera_2.jpg", "https://res.cloudinary.com/dmbndpjlh/image/upload/v1755031443/defaul_cj5nqv.png", false, "primavera_2" },
                    { 3, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "torre_1.jpg", "https://res.cloudinary.com/dmbndpjlh/image/upload/v1755031443/defaul_cj5nqv.png", false, "torre_1" },
                    { 4, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "torre_2.jpg", "https://res.cloudinary.com/dmbndpjlh/image/upload/v1755031443/defaul_cj5nqv.png", false, "torre_2" },
                    { 5, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "bodega_1.jpg", "https://res.cloudinary.com/dmbndpjlh/image/upload/v1755031443/defaul_cj5nqv.png", false, "bodega_1" },
                    { 6, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "bodega_2.jpg", "https://res.cloudinary.com/dmbndpjlh/image/upload/v1755031443/defaul_cj5nqv.png", false, "bodega_2" }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "Id", "Active", "Address", "CityId", "CreatedAt", "Document", "FirstName", "IsDeleted", "LastName", "Phone" },
                values: new object[,]
                {
                    { 1, true, "Calle Principal 123", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "123456789", "Administrador", false, "General", "3000000000" },
                    { 2, true, "Calle Principal 123", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "1000000000", "Usuario", false, "General", "3000000000" }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "Active", "CreatedAt", "DateTimeAssigned", "Description", "EstablishmentId", "IsDeleted", "Observation", "PersonId", "RequestDate" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 4, 10, 0, 0, 0, DateTimeKind.Utc), "Solicitud para conocer el local", 1, false, null, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 5, 11, 0, 0, 0, DateTimeKind.Utc), "Revisión de contrato anterior", 2, false, null, 2, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, true, new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 6, 9, 0, 0, 0, DateTimeKind.Utc), "Consulta sobre requisitos para arriendo", 3, false, null, 2, new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Active", "CreatedAt", "Email", "IsDeleted", "Password", "PersonId" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@gescomph.com", false, "AQAAAAEAACcQAAAAEK1QvWufDHBzB3acG5GKxdQTabH8BhbyLLyyZHo4WoOEvRYijXcOtRqsb3OeOpoGqw==", 1 },
                    { 2, true, new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "usuario@gescomph.com", false, "AQAAAAIAAYagAAAAEGNtpwDVV/mpIlUqi5xrPjpvzCejMXq142erkCJONaKJSiXb73eZm1tPxzj+2RvBXw==", 2 }
                });

            migrationBuilder.InsertData(
                table: "contracts",
                columns: new[] { "Id", "Active", "CreatedAt", "EndDate", "IsDeleted", "PersonId", "StartDate", "TotalBaseRentAgreed", "TotalUvtQtyAgreed" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), false, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5700000m, 38m },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 30, 23, 59, 59, 0, DateTimeKind.Utc), false, 1, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7100000m, 48m },
                    { 3, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 6, 30, 23, 59, 59, 0, DateTimeKind.Utc), false, 1, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6800000m, 45m }
                });

            migrationBuilder.InsertData(
                table: "RolUsers",
                columns: new[] { "Id", "Active", "CreatedAt", "IsDeleted", "RolId", "UserId" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 1, 1 },
                    { 2, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "premisesLeaseds",
                columns: new[] { "Id", "Active", "ContractId", "CreatedAt", "EstablishmentId", "IsDeleted" },
                values: new object[,]
                {
                    { 1, true, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false },
                    { 2, true, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, false },
                    { 3, true, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, false },
                    { 4, true, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, false },
                    { 5, true, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, false }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_EstablishmentId",
                table: "Appointments",
                column: "EstablishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PersonId",
                table: "Appointments",
                column: "PersonId");

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
                name: "IX_contractClauses_ClauseId",
                table: "contractClauses",
                column: "ClauseId");

            migrationBuilder.CreateIndex(
                name: "IX_contractClauses_ContractId",
                table: "contractClauses",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_contracts_PersonId",
                table: "contracts",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Establishments_PlazaId",
                table: "Establishments",
                column: "PlazaId");

            migrationBuilder.CreateIndex(
                name: "IX_FormModules_CreatedAt_Id",
                table: "FormModules",
                columns: new[] { "CreatedAt", "Id" },
                descending: new bool[0]);

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
                name: "IX_Forms_CreatedAt_Id",
                table: "Forms",
                columns: new[] { "CreatedAt", "Id" },
                descending: new bool[0]);

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
                name: "IX_Modules_CreatedAt_Id",
                table: "Modules",
                columns: new[] { "CreatedAt", "Id" },
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Modules_Name",
                table: "Modules",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObligationMonths_ContractId_Year_Month",
                table: "ObligationMonths",
                columns: new[] { "ContractId", "Year", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_CreatedAt_Id",
                table: "Permissions",
                columns: new[] { "CreatedAt", "Id" },
                descending: new bool[0]);

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
                name: "IX_Persons_CreatedAt_Id",
                table: "Persons",
                columns: new[] { "CreatedAt", "Id" },
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_Document",
                table: "Persons",
                column: "Document",
                unique: true,
                filter: "[Document] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_premisesLeaseds_ContractId_EstablishmentId",
                table: "premisesLeaseds",
                columns: new[] { "ContractId", "EstablishmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_premisesLeaseds_EstablishmentId",
                table: "premisesLeaseds",
                column: "EstablishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_TokenHash",
                table: "RefreshToken",
                column: "TokenHash");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_CreatedAt_Id",
                table: "Roles",
                columns: new[] { "CreatedAt", "Id" },
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolFormPermissions_CreatedAt_Id",
                table: "RolFormPermissions",
                columns: new[] { "CreatedAt", "Id" },
                descending: new bool[0]);

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
                name: "IX_RolUsers_CreatedAt_Id",
                table: "RolUsers",
                columns: new[] { "CreatedAt", "Id" },
                descending: new bool[0]);

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
                name: "IX_systemParameters_Key",
                table: "systemParameters",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedAt_Id",
                table: "Users",
                columns: new[] { "CreatedAt", "Id" },
                descending: new bool[0]);

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
                name: "contractClauses");

            migrationBuilder.DropTable(
                name: "FormModules");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "ObligationMonths");

            migrationBuilder.DropTable(
                name: "PasswordResetCodes");

            migrationBuilder.DropTable(
                name: "premisesLeaseds");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "RolFormPermissions");

            migrationBuilder.DropTable(
                name: "RolUsers");

            migrationBuilder.DropTable(
                name: "systemParameters");

            migrationBuilder.DropTable(
                name: "clauses");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Establishments");

            migrationBuilder.DropTable(
                name: "contracts");

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
