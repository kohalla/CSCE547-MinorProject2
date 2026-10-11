using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
/*
 * This is generated file is the database context for the ScheduleOptimizerDB database. It contains DbSet properties for each table in the database, and the OnModelCreating method configures the relationships and properties of the entities.
 */

namespace DataLayer
{
	public partial class DataLayerContext : DbContext //changed class from Goals to DataLayerContext to avoid naming conflict with Goals table
	{
		public DataLayerContext()
			: base("name=ScheduleOptimizerDB")
		{
		}

		public virtual DbSet<Appointment> Appointments { get; set; }
		public virtual DbSet<AuditLog> AuditLogs { get; set; }
		public virtual DbSet<Category> Categories { get; set; }
		public virtual DbSet<Goal> Goals { get; set; }
		public virtual DbSet<Role> Roles { get; set; }
		public virtual DbSet<TaskCompletion> TaskCompletions { get; set; }
		public virtual DbSet<Task> Tasks { get; set; }
		public virtual DbSet<User> Users { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Appointment>()
				.Property(e => e.StartDateTime)
				.HasPrecision(0);

			modelBuilder.Entity<Appointment>()
				.Property(e => e.EndDateTime)
				.HasPrecision(0);

			modelBuilder.Entity<AuditLog>()
				.Property(e => e.LoggedAt)
				.HasPrecision(0);

			modelBuilder.Entity<Category>()
				.Property(e => e.ColorHex)
				.IsFixedLength()
				.IsUnicode(false);

			modelBuilder.Entity<Category>()
				.HasMany(e => e.Appointments)
				.WithRequired(e => e.Category)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Category>()
				.HasMany(e => e.Goals)
				.WithRequired(e => e.Category)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Category>()
				.HasMany(e => e.Tasks)
				.WithRequired(e => e.Category)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Goal>()
				.Property(e => e.TargetValue)
				.HasPrecision(10, 2);

			modelBuilder.Entity<Goal>()
				.Property(e => e.CurrentValue)
				.HasPrecision(10, 2);

			modelBuilder.Entity<Goal>()
				.Property(e => e.ProgressPercent)
				.HasPrecision(5, 2);

			modelBuilder.Entity<Role>()
				.HasMany(e => e.Users)
				.WithRequired(e => e.Role)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<TaskCompletion>()
				.Property(e => e.CompletedAt)
				.HasPrecision(0);

			modelBuilder.Entity<Task>()
				.Property(e => e.PreferredTime)
				.HasPrecision(0);

			modelBuilder.Entity<User>()
				.Property(e => e.CreatedAt)
				.HasPrecision(0);

			modelBuilder.Entity<User>()
				.Property(e => e.LastLoginAt)
				.HasPrecision(0);

			modelBuilder.Entity<User>()
				.HasMany(e => e.Appointments)
				.WithRequired(e => e.User)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<User>()
				.HasMany(e => e.Goals)
				.WithRequired(e => e.User)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<User>()
				.HasMany(e => e.Tasks)
				.WithRequired(e => e.User)
				.WillCascadeOnDelete(false);
		}
	}
}
