﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore;
using Havit.Data.Patterns.DataSeeds;
using Havit.Extensions.DependencyInjection.Abstractions;
using Havit.GoranG3.DataLayer.Seeds.Core;
using Havit.GoranG3.G2Migrator.Services.Projects;
using Havit.GoranG3.G2Migrator.Services.Timesheets;
using Havit.GoranG3.G2Migrator.Services.Users;
using Microsoft.EntityFrameworkCore;

namespace Havit.GoranG3.G2Migrator.Services
{
	[Service]
	public class G2MigrationRunner : IG2MigrationRunner
	{
		private readonly IG2UserMigrator userMigrator;
		private readonly IG2ProjectMigrator projectMigrator;
		private readonly IG2TimesheetItemCategoryMigrator timesheetItemCategoryMigrator;
		private readonly IDbContext dbContext;
		private readonly IDataSeedRunner dataSeedRunner;

		public G2MigrationRunner(
			IG2UserMigrator userMigrator,
			IG2ProjectMigrator projectMigrator,
			IG2TimesheetItemCategoryMigrator timesheetItemCategoryMigrator,
			IDbContext dbContext,
			IDataSeedRunner dataSeedRunner)
		{
			this.userMigrator = userMigrator;
			this.projectMigrator = projectMigrator;
			this.timesheetItemCategoryMigrator = timesheetItemCategoryMigrator;
			this.dbContext = dbContext;
			this.dataSeedRunner = dataSeedRunner;
		}

		public void Run()
		{
			dbContext.Database.Migrate();
			dataSeedRunner.SeedData<CoreProfile>();

			userMigrator.MigrateUsers();
			timesheetItemCategoryMigrator.MigrateCategories();
			projectMigrator.MigrateProjects();
		}
	}
}