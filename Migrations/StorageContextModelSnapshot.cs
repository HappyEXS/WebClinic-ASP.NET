﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using przychodnia.Models;

#nullable disable

namespace przychodnia.Migrations
{
    [DbContext(typeof(StorageContext))]
    partial class StorageContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("przychodnia.Models.DoctorModel", b =>
                {
                    b.Property<int>("DoctorID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DoctorID"), 1L, 1);

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsDirector")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<int>("SpecialityID")
                        .HasColumnType("int");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("DoctorID");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("SpecialityID");

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("przychodnia.Models.PatientModel", b =>
                {
                    b.Property<int>("PatientID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PatientID"), 1L, 1);

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("PatientID");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("przychodnia.Models.ScheduleModel", b =>
                {
                    b.Property<int>("ScheduleID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ScheduleID"), 1L, 1);

                    b.Property<int>("DoctorID")
                        .HasColumnType("int");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.HasKey("ScheduleID");

                    b.HasIndex("DoctorID");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("przychodnia.Models.SpecialityModel", b =>
                {
                    b.Property<int>("SpecialityID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SpecialityID"), 1L, 1);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SpecialityID");

                    b.ToTable("Specialities");
                });

            modelBuilder.Entity("przychodnia.Models.VisitModel", b =>
                {
                    b.Property<int>("VisitID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("VisitID"), 1L, 1);

                    b.Property<bool>("Available")
                        .HasColumnType("bit");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int?>("PatientID")
                        .HasColumnType("int");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<int>("ScheduleID")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.HasKey("VisitID");

                    b.HasIndex("PatientID");

                    b.HasIndex("ScheduleID", "StartTime")
                        .IsUnique();

                    b.ToTable("Visits");
                });

            modelBuilder.Entity("przychodnia.Models.DoctorModel", b =>
                {
                    b.HasOne("przychodnia.Models.SpecialityModel", "Speciality")
                        .WithMany("Doctors")
                        .HasForeignKey("SpecialityID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Speciality");
                });

            modelBuilder.Entity("przychodnia.Models.ScheduleModel", b =>
                {
                    b.HasOne("przychodnia.Models.DoctorModel", "Doctor")
                        .WithMany("Schedules")
                        .HasForeignKey("DoctorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Doctor");
                });

            modelBuilder.Entity("przychodnia.Models.VisitModel", b =>
                {
                    b.HasOne("przychodnia.Models.PatientModel", "Patient")
                        .WithMany()
                        .HasForeignKey("PatientID");

                    b.HasOne("przychodnia.Models.ScheduleModel", "Schedule")
                        .WithMany()
                        .HasForeignKey("ScheduleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Patient");

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("przychodnia.Models.DoctorModel", b =>
                {
                    b.Navigation("Schedules");
                });

            modelBuilder.Entity("przychodnia.Models.SpecialityModel", b =>
                {
                    b.Navigation("Doctors");
                });
#pragma warning restore 612, 618
        }
    }
}
