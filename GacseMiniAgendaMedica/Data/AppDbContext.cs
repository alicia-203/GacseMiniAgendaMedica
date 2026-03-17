using GacseMiniAgendaMedica.DTOs;
using GacseMiniAgendaMedica.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GacseMiniAgendaMedica.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Medico> Medicos { get; set; }

    public DbSet<Paciente> Pacientes { get; set; }

    public DbSet<Cita> Citas { get; set; }

    public DbSet<HorarioMedico> HorariosMedicos { get; set; }

    public DbSet<CitaAgendaDTO> CitasAgenda { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
        });

        modelBuilder.Entity<Medico>()
       .HasKey(m => m.Id);

        modelBuilder.Entity<Medico>()
            .Property(m => m.Id)
            .HasColumnName("MedicoId");

        modelBuilder.Entity<HorarioMedico>()
            .HasKey(h => h.Id);

        modelBuilder.Entity<HorarioMedico>()
            .Property(h => h.Id)
            .HasColumnName("HorarioId");

        modelBuilder.Entity<HorarioMedico>()
            .HasOne<Medico>()
            .WithMany(m => m.HorariosMedico)
            .HasForeignKey(h => h.MedicoId); 

        modelBuilder.Entity<HorarioMedico>()
       .Property(h => h.DiaSemana)
       .HasConversion<byte>();

        modelBuilder.Entity<Paciente>()
        .HasKey(m => m.Id);

        modelBuilder.Entity<Paciente>()
           .Property(m => m.Id)
           .HasColumnName("PacienteId");

        modelBuilder.Entity<CitaAgendaDTO>().HasNoKey();
    }
}
