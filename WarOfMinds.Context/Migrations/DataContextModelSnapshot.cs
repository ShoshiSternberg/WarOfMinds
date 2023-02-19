﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WarOfMinds.Context;

#nullable disable

namespace WarOfMinds.Context.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WarOfMinds.Repositories.Entities.Game", b =>
                {
                    b.Property<int>("GameID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GameID"));

                    b.Property<DateTime>("GameDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("GameLength")
                        .HasColumnType("int");

                    b.Property<int>("GameManagerPlayerID")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<int>("SubjectID")
                        .HasColumnType("int");

                    b.HasKey("GameID");

                    b.HasIndex("GameManagerPlayerID");

                    b.HasIndex("SubjectID");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("WarOfMinds.Repositories.Entities.Player", b =>
                {
                    b.Property<int>("PlayerID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PlayerID"));

                    b.Property<DateTime>("DateOfRegistration")
                        .HasColumnType("datetime2");

                    b.Property<int>("ELORating")
                        .HasColumnType("int");

                    b.Property<int?>("GameID")
                        .HasColumnType("int");

                    b.Property<string>("PlayerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerPassword")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PlayerID");

                    b.HasIndex("GameID");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("WarOfMinds.Repositories.Entities.PlayerRatingBySubject", b =>
                {
                    b.Property<int>("PlayerRatingBySubjectID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PlayerRatingBySubjectID"));

                    b.Property<int>("ELORating")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerID")
                        .HasColumnType("int");

                    b.Property<int>("SubjectID")
                        .HasColumnType("int");

                    b.HasKey("PlayerRatingBySubjectID");

                    b.HasIndex("PlayerID");

                    b.HasIndex("SubjectID");

                    b.ToTable("PlayerRatingsBySubject");
                });

            modelBuilder.Entity("WarOfMinds.Repositories.Entities.Subject", b =>
                {
                    b.Property<int>("SubjectID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SubjectID"));

                    b.Property<int>("DifficultyLevel")
                        .HasColumnType("int");

                    b.Property<string>("Subjectname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SubjectID");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("WarOfMinds.Repositories.Entities.Game", b =>
                {
                    b.HasOne("WarOfMinds.Repositories.Entities.Player", "GameManager")
                        .WithMany()
                        .HasForeignKey("GameManagerPlayerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarOfMinds.Repositories.Entities.Subject", "Subject")
                        .WithMany()
                        .HasForeignKey("SubjectID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GameManager");

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("WarOfMinds.Repositories.Entities.Player", b =>
                {
                    b.HasOne("WarOfMinds.Repositories.Entities.Game", null)
                        .WithMany("Players")
                        .HasForeignKey("GameID");
                });

            modelBuilder.Entity("WarOfMinds.Repositories.Entities.PlayerRatingBySubject", b =>
                {
                    b.HasOne("WarOfMinds.Repositories.Entities.Player", null)
                        .WithMany("RatingsBySubjects")
                        .HasForeignKey("PlayerID");

                    b.HasOne("WarOfMinds.Repositories.Entities.Subject", "Subject")
                        .WithMany()
                        .HasForeignKey("SubjectID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("WarOfMinds.Repositories.Entities.Game", b =>
                {
                    b.Navigation("Players");
                });

            modelBuilder.Entity("WarOfMinds.Repositories.Entities.Player", b =>
                {
                    b.Navigation("RatingsBySubjects");
                });
#pragma warning restore 612, 618
        }
    }
}
