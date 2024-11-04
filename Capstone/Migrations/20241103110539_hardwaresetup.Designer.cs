﻿// <auto-generated />
using System;
using Capstone.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Capstone.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241103110539_hardwaresetup")]
    partial class hardwaresetup
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Capstone.Data.Customers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("datetime2");

                    b.Property<string>("Customer_email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Customer_name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Discount")
                        .HasColumnType("int");

                    b.Property<int>("Num_jumpers")
                        .HasColumnType("int");

                    b.Property<int>("Promo")
                        .HasColumnType("int");

                    b.Property<string>("SignatureData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Total_amount")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("Capstone.Data.DashboardDisplay", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal?>("BatteryLevel")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan?>("EndTime")
                        .HasColumnType("time");

                    b.Property<string>("RemainingTime")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan?>("StartTime")
                        .HasColumnType("time");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TransactionNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WiibandID")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Dashboards");
                });

            modelBuilder.Entity("Capstone.Data.Events", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Addons")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("datetime2");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<int>("ElecFoodCart")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Jumpers")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PartyDecorations")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PartyEquipCD")
                        .HasColumnType("int");

                    b.Property<int>("PartyEquipUtils")
                        .HasColumnType("int");

                    b.Property<int>("PartyGuest")
                        .HasColumnType("int");

                    b.Property<int>("PartyHours")
                        .HasColumnType("int");

                    b.Property<int>("Phone")
                        .HasColumnType("int");

                    b.Property<int>("Socks")
                        .HasColumnType("int");

                    b.Property<TimeOnly>("Time")
                        .HasColumnType("time");

                    b.Property<int>("TrampolineGames")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("Capstone.Data.Users", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("datetime2");

                    b.Property<string>("Display_name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email_address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Email_notify")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Sms_notify")
                        .HasColumnType("bit");

                    b.Property<string>("Theme")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Third_party")
                        .HasColumnType("bit");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Updated_at")
                        .HasColumnType("datetime2");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Visibility")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
