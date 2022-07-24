using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Specifications
{
	public class ManualFilterSpecification : BaseSpecification<Manual>
	{
		public ManualFilterSpecification() : base(item => !item.Removed) 
		{
			AddInclude(item => item.Features);
		}

		public ManualFilterSpecification(int id) : base(item => !item.Removed && item.Id == id)
		{
			AddInclude(item => item.Features);
		}
	}

	public class ManualParentFilterSpecification : BaseSpecification<Manual>
	{
		public ManualParentFilterSpecification(int parentId, bool features = false) : base(item => !item.Removed && item.ParentId == parentId)
		{
			if(features) AddInclude(item => item.Features);
		}
	}


	public class FeatureFilterSpecification : BaseSpecification<Feature>
	{
		public FeatureFilterSpecification() : base(item => !item.Removed)
		{
			
		}

		public FeatureFilterSpecification(int id) : base(item => !item.Removed && item.Id == id)
		{
			
		}
	}
}
