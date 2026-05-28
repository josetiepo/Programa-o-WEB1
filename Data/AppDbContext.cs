using Academico.Models;
using Microsoft.EntityFrameworkCore;

namespace Academico.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Aluno> Alunos => Set<Aluno>();
    public DbSet<Professor> Professores => Set<Professor>();
    public DbSet<Disciplina> Disciplinas => Set<Disciplina>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Pessoa>(entity =>
        {
            entity.ToTable("Pessoas");
            entity.UseTptMappingStrategy();
            entity.Property(x => x.Nome).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Cpf).HasMaxLength(14).IsRequired();
        });

        modelBuilder.Entity<Aluno>(entity =>
        {
            entity.ToTable("Alunos");
            entity.Property(x => x.Matricula).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Curso).HasMaxLength(80).IsRequired();
        });

        modelBuilder.Entity<Professor>(entity =>
        {
            entity.ToTable("Professores");
            entity.Property(x => x.Siape).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Area).HasMaxLength(80).IsRequired();
        });

        modelBuilder.Entity<Disciplina>(entity =>
        {
            entity.ToTable("Disciplinas");
            entity.Property(x => x.Nome).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Periodo).HasMaxLength(30).IsRequired();
            entity.HasOne(x => x.Professor)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Nome).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(160).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Perfil).HasMaxLength(30).IsRequired();
        });
    }
}
