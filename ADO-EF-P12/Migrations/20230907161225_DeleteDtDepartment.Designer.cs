﻿// <auto-generated />
using System;
using ADO_EF_P12.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ADO_EF_P12.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20230907161225_DeleteDtDepartment")]
    partial class DeleteDtDepartment
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ADO_EF_P12.Data.Entity.Department", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeleteDt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("ADO_EF_P12.Data.Entity.Manager", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Avatar")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeleteDt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("IdChief")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IdMainDep")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("IdSecDep")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PassDk")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PassSalt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Secname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IdMainDep");

                    b.HasIndex("IdSecDep");

                    b.ToTable("Managers");
                });

            modelBuilder.Entity("ADO_EF_P12.Data.Entity.Manager", b =>
                {
                    b.HasOne("ADO_EF_P12.Data.Entity.Department", "MainDep")
                        .WithMany()
                        .HasForeignKey("IdMainDep")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ADO_EF_P12.Data.Entity.Department", "SecDep")
                        .WithMany()
                        .HasForeignKey("IdSecDep");

                    b.Navigation("MainDep");

                    b.Navigation("SecDep");
                });
#pragma warning restore 612, 618
        }
    }
}
