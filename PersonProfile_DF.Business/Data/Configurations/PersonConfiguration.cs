using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PersonProfile_DF.Business.Data.Models;

namespace PersonProfile_DF.Business.Data.Configurations
{
	internal static class PersonConfiguration
	{
		public static ModelBuilder AddPersonConfiguration(this ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Person>(entity =>
			{
				entity.ToTable("Person");

				// PRIMARY KEY
				entity.HasKey(e => e.PersonId)
					.HasName("PK_Customer");

				// COLUMNS
				entity.Property(e => e.PersonId)
					.HasColumnName(@"PersonId")
					.HasColumnType("int")
					.IsRequired()
					.ValueGeneratedOnAdd();

				entity.Property(e => e.Name)
					.HasColumnName(@"Name")
					.HasColumnType("nvarchar")
					.IsRequired()
					.IsUnicode(false)
					.HasMaxLength(70);

				entity.Property(e => e.EmailAddress)
					.HasColumnName(@"EmailAddress")
					.HasColumnType("nvarchar")
					.IsRequired()
					.IsUnicode(false)
					.HasMaxLength(80);

				entity.Property(e => e.MailingAddress)
					.HasColumnName(@"MailingAddress")
					.HasColumnType("nvarchar")
					.IsRequired()
					.IsUnicode(false)
					.HasMaxLength(200);

				entity.Property(e => e.PhoneNumber)
					.HasColumnName(@"PhoneNumber")
					.HasColumnType("nvarchar")
					.IsRequired()
					.IsUnicode(false)
					.HasMaxLength(50);

				entity.Property(e => e.LastUpdateDateTimeStamp)
					.HasColumnName(@"LastUpdateDateTimeStamp")
					.HasColumnType("datetime2")
					.IsRequired();


			});

			return modelBuilder;
		}
	}

}

