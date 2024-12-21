﻿// <auto-generated />
using System;
using DemoAPIBot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DemoAPIBot.Migrations
{
    [DbContext(typeof(DemoContext))]
    [Migration("20230629140542_AddMessageRelation")]
    partial class AddMessageRelation
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("DemoAPIBot.Models.InProgressSub", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("mId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("timeSubRequest")
                        .HasColumnType("TEXT");

                    b.Property<string>("token")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("InProgressSubs");
                });

            modelBuilder.Entity("DemoAPIBot.Models.Macchina", b =>
                {
                    b.Property<string>("mId")
                        .HasColumnType("TEXT");

                    b.Property<string>("model")
                        .HasColumnType("TEXT");

                    b.HasKey("mId");

                    b.ToTable("Machines");
                });

            modelBuilder.Entity("DemoAPIBot.Models.RefreshToken", b =>
                {
                    b.Property<string>("UserID")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("UserID");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("DemoAPIBot.Models.Role", b =>
                {
                    b.Property<string>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("RoleId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("DemoAPIBot.Models.Sub", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("Subs");
                });

            modelBuilder.Entity("DemoAPIBot.Models.SubMachine", b =>
                {
                    b.Property<int>("SubId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MacchinaId")
                        .HasColumnType("TEXT");

                    b.Property<string>("serviceDispatcher")
                        .HasColumnType("TEXT");

                    b.Property<int>("levelPriority")
                        .HasColumnType("INTEGER");

                    b.HasKey("SubId", "MacchinaId", "serviceDispatcher");

                    b.HasIndex("MacchinaId");

                    b.ToTable("SubMachines");
                });

            modelBuilder.Entity("DemoAPIBot.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TelegramClientServer.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Mex")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("mId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("model")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("DemoAPIBot.Models.SubMachine", b =>
                {
                    b.HasOne("DemoAPIBot.Models.Macchina", "MacchinaRef")
                        .WithMany("subMachines")
                        .HasForeignKey("MacchinaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DemoAPIBot.Models.Sub", "SubRef")
                        .WithMany("subMachines")
                        .HasForeignKey("SubId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MacchinaRef");

                    b.Navigation("SubRef");
                });

            modelBuilder.Entity("DemoAPIBot.Models.User", b =>
                {
                    b.HasOne("DemoAPIBot.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("DemoAPIBot.Models.Macchina", b =>
                {
                    b.Navigation("subMachines");
                });

            modelBuilder.Entity("DemoAPIBot.Models.Sub", b =>
                {
                    b.Navigation("subMachines");
                });
#pragma warning restore 612, 618
        }
    }
}