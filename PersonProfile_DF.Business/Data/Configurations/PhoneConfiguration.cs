using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersonProfile_DF.Business.Data.Models;

namespace PersonProfile_DF.Business.Data.Configurations
{
	internal static class PhoneConfiguration
	{
		public static ModelBuilder AddPhoneConfiguration(this ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Phone>(entity =>
			{
				entity.ToTable("Phone");

				// PRIMARY KEY
				entity.HasKey(e => e.PhoneId)
					.HasName("PK_Phone");

				// COLUMNS
				entity.Property(e => e.PhoneId)
					.HasColumnName(@"PhoneId")
					.HasColumnType("int")
					.IsRequired()
					.ValueGeneratedOnAdd();

				entity.Property(e => e.PersonId)
					.HasColumnName(@"PersonId")
					.HasColumnType("int")
					.IsRequired();

				entity.Property(e => e.PhoneNumber)
					.HasColumnName(@"PhoneNumber")
					.HasColumnType("nvarchar")
					.IsRequired()
					.IsUnicode(false)
					.HasMaxLength(50);


				// FOREIGN KEYS
				entity.HasOne(a => a.Person)
					.WithMany(b => b.Phones)
					.HasForeignKey(c => c.PersonId)
					.OnDelete(DeleteBehavior.Restrict)
					.HasConstraintName("FK_Phone_Person");

			});

			return modelBuilder;
		}
	}

}

