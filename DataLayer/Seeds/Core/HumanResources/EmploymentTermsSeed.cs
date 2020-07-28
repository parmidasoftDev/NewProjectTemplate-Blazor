﻿using Havit.Data.Patterns.DataSeeds;
using Havit.GoranG3.DataLayer.DataSources.HumanResources;
using Havit.GoranG3.DataLayer.Repositories.HumanResources;
using Havit.GoranG3.Model.HumanResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.GoranG3.DataLayer.Seeds.Core.HumanResources
{
	public class EmploymentTermsSeed : DataSeed<CoreProfile>
	{
		private readonly IEmploymentTermsDataSource employmentTermsDataSource;

		public EmploymentTermsSeed(IEmploymentTermsDataSource employmentTermsDataSource)
		{
			this.employmentTermsDataSource = employmentTermsDataSource;
		}

		public override void SeedData()
		{
			if (employmentTermsDataSource.DataWithDeleted.Any())
			{
				return; // one-off seed of defaults
			}

			var employmentTerms = new[]
			{
				new EmploymentTerms()
				{
					MigrationId = -1,
					Name = "hodinové odměňování",
					RateType = EmployeeRateType.HourRate,
					HoursPerDay = 8
				},
				new EmploymentTerms()
				{
					MigrationId = -2,
					Name = "plný úvazek",
					RateType = EmployeeRateType.MonthRate,
					HoursPerDay = 8
				}
			};

			Seed(For(employmentTerms).PairBy(at => at.Name).WithoutUpdate());
		}
	}
}