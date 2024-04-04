using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Adnc.Demo.Maint.Repository.Migrations
{
    public partial class InitDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sys_config",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: ""),
                    isdeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: ""),
                    name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false, comment: "参数名"),
                    value = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "参数值"),
                    description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, comment: "备注"),
                    createby = table.Column<long>(type: "bigint", nullable: false, comment: "创建人"),
                    createtime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "创建时间/注册时间"),
                    modifyby = table.Column<long>(type: "bigint", nullable: true, comment: "最后更新人"),
                    modifytime = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "最后更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sys_config", x => x.id);
                },
                comment: "系统参数");

            migrationBuilder.CreateTable(
                name: "sys_dictionary",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: ""),
                    isdeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: ""),
                    name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false, comment: ""),
                    ordinal = table.Column<int>(type: "int", nullable: false, comment: ""),
                    pid = table.Column<long>(type: "bigint", nullable: false, comment: ""),
                    value = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false, comment: ""),
                    createby = table.Column<long>(type: "bigint", nullable: false, comment: "创建人"),
                    createtime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "创建时间/注册时间"),
                    modifyby = table.Column<long>(type: "bigint", nullable: true, comment: "最后更新人"),
                    modifytime = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "最后更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sys_dictionary", x => x.id);
                },
                comment: "字典");

            migrationBuilder.CreateTable(
                name: "sys_eventtracker",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    eventid = table.Column<long>(type: "bigint", nullable: false, comment: ""),
                    trackername = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: ""),
                    createby = table.Column<long>(type: "bigint", nullable: false, comment: "创建人"),
                    createtime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "创建时间/注册时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sys_eventtracker", x => x.id);
                },
                comment: "事件跟踪/处理信息");

            migrationBuilder.CreateTable(
                name: "sys_notice",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: ""),
                    content = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: ""),
                    title = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false, comment: ""),
                    type = table.Column<int>(type: "int", nullable: false, comment: ""),
                    createby = table.Column<long>(type: "bigint", nullable: false, comment: "创建人"),
                    createtime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "创建时间/注册时间"),
                    modifyby = table.Column<long>(type: "bigint", nullable: true, comment: "最后更新人"),
                    modifytime = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "最后更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sys_notice", x => x.id);
                },
                comment: "通知");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sys_config");

            migrationBuilder.DropTable(
                name: "sys_dictionary");

            migrationBuilder.DropTable(
                name: "sys_eventtracker");

            migrationBuilder.DropTable(
                name: "sys_notice");
        }
    }
}
