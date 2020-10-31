﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions;
using Havit.Data.Patterns.DataSeeds;
using Havit.GoranG3.DataLayer.Seeds.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.GoranG3.Web.Server.Tools
{
	public static class DatabaseMigration
	{
		public static void UpgradeDatabaseSchemaAndData(this IApplicationBuilder app)
		{
			using (IServiceScope serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				Debug.Assert(serviceScope.ServiceProvider.GetService<ManyToManyEntityKeyDiscoveryConvention>() != null);

				var context = serviceScope.ServiceProvider.GetService<IDbContext>();
				context.Database.Migrate();

				var dataSeedRunner = serviceScope.ServiceProvider.GetService<IDataSeedRunner>();
				dataSeedRunner.SeedData<CoreProfile>();
			}
		}
	}
}
