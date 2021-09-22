using System;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using PersonProfile_DF.Business.Data.Models;
using PersonProfile_DF.Business.Data.Configurations;

namespace PersonProfile_DF.Business.Data.Context
{
	internal partial class PersonProfile_DFDbContext : DbContext
	{
		private StreamWriter _logStream = null;

		public PersonProfile_DFDbContext()
		 : base()
		{
		}

		public PersonProfile_DFDbContext(DbContextOptions<PersonProfile_DFDbContext> options)
		 : base(options)
		{
		}

		public virtual DbSet<Person> People { get; set; }
		public virtual DbSet<Phone> Phones { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(App.Configuration.ConnectionString);

			if(App.Configuration.IsDbQueryTraceEnabled && App.Configuration.DbQueryTraceDestination.ToLower() == "textfile")
			{
				if(!Directory.Exists(App.Configuration.TextLogDirectory))
				{
					Directory.CreateDirectory(App.Configuration.TextLogDirectory);
				}

				_logStream = new StreamWriter(App.Configuration.TextLogDirectory + Path.DirectorySeparatorChar + "DbQueries.txt", append: true);
				optionsBuilder.LogTo(_logStream.WriteLine, new[] { DbLoggerCategory.Query.Name });
				optionsBuilder.EnableSensitiveDataLogging();
				optionsBuilder.EnableDetailedErrors();
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder
				.AddPersonConfiguration()
				.AddPhoneConfiguration()
			;
		}
		public override void Dispose()
		{
			base.Dispose();
			if(_logStream != null)
			{
				_logStream.Dispose();
			}
		}

		public override async ValueTask DisposeAsync()
		{
			await base.DisposeAsync();
			if(_logStream != null)
			{
			await _logStream.DisposeAsync();
			}
		}
	}
}

