﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Patterns.Repositories;
using Havit.GoranG3.Model.Projects;

namespace Havit.GoranG3.DataLayer.Repositories.Projects
{
	public partial interface IProjectRepository
	{
		List<Project> GetAllIncludingDeleted();
	}
}