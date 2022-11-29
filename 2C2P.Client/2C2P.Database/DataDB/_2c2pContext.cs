using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace _2C2P.Database.DataDB
{
    public partial class _2c2pContext : DbContext
    {
        public _2c2pContext()
        {
        }

        public _2c2pContext(DbContextOptions<_2c2pContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ImportExcel> ImportExcels { get; set; } = null!;
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=localhost;Database=2c2p;Trusted_Connection=True;user=sa;password=Password123@;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImportExcel>(entity =>
            {
                entity.ToTable("import_excel");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Body).HasColumnName("body");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("createdAt");

                entity.Property(e => e.Format)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("format");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("transactions");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("amount");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("createdAt");

                entity.Property(e => e.CurrencyCode)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("currencyCode");

                entity.Property(e => e.ExcelId).HasColumnName("excelId");

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.TransactionDate)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("transactionDate");

                entity.Property(e => e.TransactionId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("transactionId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
