﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Havit.GoranG3.Contracts.System;
using Havit.Services.Caching;

namespace Havit.GoranG3.Facades.System
{
	public class MaintenanceFacade : IMaintenanceFacade
	{
		private readonly ICacheService cacheService;

		public MaintenanceFacade(ICacheService cacheService)
		{
			this.cacheService = cacheService;
		}

		public Task ClearCache(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			cacheService.Clear();

			return Task.CompletedTask;
		}
	}
}