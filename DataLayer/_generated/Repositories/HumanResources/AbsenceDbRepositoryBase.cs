﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.DataEntries;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.Infrastructure;

namespace Havit.GoranG3.DataLayer.Repositories.HumanResources
{
	[System.CodeDom.Compiler.GeneratedCode("Havit.Data.EntityFrameworkCore.CodeGenerator", "1.0")]
	public abstract class AbsenceDbRepositoryBase : DbRepository<Havit.GoranG3.Model.HumanResources.Absence>
	{
		protected AbsenceDbRepositoryBase(IDbContext dbContext, Havit.GoranG3.DataLayer.DataSources.HumanResources.IAbsenceDataSource dataSource, IEntityKeyAccessor<Havit.GoranG3.Model.HumanResources.Absence, int> entityKeyAccessor, IDataLoader dataLoader, ISoftDeleteManager softDeleteManager, IEntityCacheManager entityCacheManager)
			: base(dbContext, dataSource, entityKeyAccessor, dataLoader, softDeleteManager, entityCacheManager)
		{
		}

	}
}