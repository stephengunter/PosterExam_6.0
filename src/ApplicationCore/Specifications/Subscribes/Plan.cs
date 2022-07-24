using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Specifications
{
    public class PlanFilterSpecification : BaseSpecification<Plan>
    {
        public PlanFilterSpecification() : base(item => !item.Removed)
        {
            
        }

    }
}
