﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MoMoBot.Infrastructure.Database;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MoMoBot.Infrastructure.Migrations
{
    [DbContext(typeof(MoMoDbContext))]
    [Migration("20190315022016_RemoveChatRecord")]
    partial class RemoveChatRecord
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("MoMoBot.Infrastructure.Models.AnswerQueries", b =>
                {
                    b.Property<Guid>("AnswerId");

                    b.Property<int>("ParameterId");

                    b.Property<string>("Alias")
                        .HasMaxLength(50);

                    b.HasKey("AnswerId", "ParameterId");

                    b.HasIndex("ParameterId");

                    b.ToTable("AnswerQueries");
                });

            modelBuilder.Entity("MoMoBot.Infrastructure.Models.Chat", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("DisplayInList");

                    b.Property<string>("GroupName")
                        .HasMaxLength(200);

                    b.Property<bool>("Online");

                    b.Property<string>("Other")
                        .IsRequired();

                    b.Property<string>("Owner")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<DateTime>("UpdateTime");

                    b.HasKey("Id");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("MoMoBot.Infrastructure.Models.Department", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationTime");

                    b.Property<string>("DepartId")
                        .HasMaxLength(100);

                    b.Property<string>("DepartName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<long?>("ParentId");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("MoMoBot.Infrastructure.Models.FeedbackInfo", b =>
                {
                    b.Property<Guid>("FBId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AnswerTextContent");

                    b.Property<int>("FeedbackResults");

                    b.Property<string>("Intent");

                    b.Property<bool>("IsDelete");

                    b.Property<string>("PutQuestionsId");

                    b.Property<string>("QuestionContent");

                    b.Property<double>("Score");

                    b.Property<DateTime>("TimeOfOccurrence");

                    b.HasKey("FBId");

                    b.ToTable("FeedbackInfos");
                });

            modelBuilder.Entity("MoMoBot.Infrastructure.Models.HotIntent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Count");

                    b.Property<string>("Intent")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<DateTime>("UpdateTime");

                    b.HasKey("Id");

                    b.ToTable("HotIntents");
                });

            modelBuilder.Entity("MoMoBot.Infrastructure.Models.Modular", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsPublic");

                    b.Property<string>("ModularId")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("ModularName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Remarks")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.ToTable("Modulars");
                });

            modelBuilder.Entity("MoMoBot.Infrastructure.Models.ModularPermission", b =>
                {
                    b.Property<long>("DepartmentId");

                    b.Property<long>("ModularId");

                    b.HasKey("DepartmentId", "ModularId");

                    b.HasIndex("ModularId");

                    b.ToTable("ModularPermissions");
                });

            modelBuilder.Entity("MoMoBot.Infrastructure.Models.QAPermission", b =>
                {
                    b.Property<long>("DepartmentId");

                    b.Property<Guid>("QAId");

                    b.HasKey("DepartmentId", "QAId");

                    b.HasIndex("QAId");

                    b.ToTable("QAPermissions");
                });

            modelBuilder.Entity("MoMoBot.Infrastructure.Models.QandA", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasMaxLength(2147483647);

                    b.Property<string>("Intent")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<bool>("IsPublic");

                    b.Property<int>("Method");

                    b.Property<string>("RequestUrl")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Answers");
                });

            modelBuilder.Entity("MoMoBot.Infrastructure.Models.QueryParameter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Enable");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Remarks")
                        .HasMaxLength(2147483647);

                    b.HasKey("Id");

                    b.ToTable("QueryParameters");
                });

            modelBuilder.Entity("MoMoBot.Infrastructure.Models.ServiceRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("EndOfServiceTime");

                    b.Property<bool>("IsDone");

                    b.Property<string>("RecordFilePath");

                    b.Property<string>("Remarks");

                    b.Property<DateTime>("RevordCompletionTime");

                    b.Property<int>("Score");

                    b.Property<string>("Title");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("ServiceRecords");
                });

            modelBuilder.Entity("MoMoBot.Infrastructure.Models.Unknown", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<bool>("IsResolved");

                    b.Property<string>("Remarks");

                    b.Property<DateTime>("TimeOfOccurrence");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("Unknowns");
                });

            modelBuilder.Entity("MoMoBot.Infrastructure.Models.Voice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AudioType");

                    b.Property<DateTime>("CreationTime");

                    b.Property<double>("Duration");

                    b.Property<string>("SavePath")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.ToTable("Voices");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MoMoBot.Infrastructure.Models.AnswerQueries", b =>
                {
                    b.HasOne("MoMoBot.Infrastructure.Models.QandA", "Answer")
                        .WithMany("AnswerQueries")
                        .HasForeignKey("AnswerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MoMoBot.Infrastructure.Models.QueryParameter", "Parameter")
                        .WithMany("AnswerParameters")
                        .HasForeignKey("ParameterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MoMoBot.Infrastructure.Models.Department", b =>
                {
                    b.HasOne("MoMoBot.Infrastructure.Models.Department", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MoMoBot.Infrastructure.Models.ModularPermission", b =>
                {
                    b.HasOne("MoMoBot.Infrastructure.Models.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MoMoBot.Infrastructure.Models.Modular", "Modular")
                        .WithMany()
                        .HasForeignKey("ModularId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MoMoBot.Infrastructure.Models.QAPermission", b =>
                {
                    b.HasOne("MoMoBot.Infrastructure.Models.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MoMoBot.Infrastructure.Models.QandA", "QA")
                        .WithMany()
                        .HasForeignKey("QAId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
