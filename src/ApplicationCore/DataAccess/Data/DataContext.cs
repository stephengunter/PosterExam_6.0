using ApplicationCore.Models;
using ApplicationCore.Models.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace ApplicationCore.DataAccess
{
	public class DataContext : DbContext
	{ 
		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{
		}

		public DataContext(string connectionString) : base(new DbContextOptionsBuilder<DefaultContext>().UseSqlServer(connectionString).Options)
		{

		}

		public DbSet<ExamSettings> ExamSettings { get; set; }
		public DbSet<NoteParams> NoteParams { get; set; }
		public DbSet<SubjectQuestions> SubjectQuestions { get; set; }
		public DbSet<YearRecruit> YearRecruits { get; set; }
		public DbSet<NoteCategories> NoteCategories { get; set; }
		public DbSet<TermNotes> TermNotes { get; set; }

	}
}
