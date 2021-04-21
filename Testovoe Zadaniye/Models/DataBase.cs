using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Testovoe_Zadaniye
{
    public partial class DataBase : DbContext
    {
        public DataBase()
            : base("name=DataBase")
        {
        }

        public virtual DbSet<MoneyIncome> MoneyIncome { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<Payments> Payments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MoneyIncome>()
                .HasMany(e => e.Payments)
                .WithOptional(e => e.MoneyIncome)
                .HasForeignKey(e => e.IdOfMoneyIncome);

            modelBuilder.Entity<Orders>()
                .HasMany(e => e.Payments)
                .WithRequired(e => e.Orders)
                .HasForeignKey(e => e.IdOfOrder)
                .WillCascadeOnDelete(false);
        }
    }
}
