using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Specifications
{
    public class BillFilterSpecification : BaseSpecification<Bill>
    {
        public BillFilterSpecification() : base(item => !item.Removed)
        {
            AddInclude(item => item.User);
            AddInclude(item => item.Pays);
            AddInclude(item => item.Plan);
            AddInclude(item => item.Subscribes);
        }

        public BillFilterSpecification(int id) : base(item => !item.Removed && item.Id == id)
        {
            AddInclude(item => item.User);
            AddInclude(item => item.Pays);
            AddInclude(item => item.Plan);
            AddInclude(item => item.Subscribes);
        }

        public BillFilterSpecification(User user) : base(item => !item.Removed && item.UserId == user.Id)
        {
            AddInclude(item => item.User);
            AddInclude(item => item.Pays);
            AddInclude(item => item.Plan);
            AddInclude(item => item.Subscribes);
        }

        public BillFilterSpecification(User user, Plan plan) : base(item => !item.Removed && item.UserId == user.Id && item.PlanId == plan.Id)
        {
            AddInclude(item => item.User);
            AddInclude(item => item.Pays);
            AddInclude(item => item.Plan);
            AddInclude(item => item.Subscribes);
        }

    }

    public class BillSimpleFilterSpecification : BaseSpecification<Bill>
    {
        public BillSimpleFilterSpecification(Plan plan) : base(item => !item.Removed && item.PlanId == plan.Id)
        {
            
        }
    }

}
