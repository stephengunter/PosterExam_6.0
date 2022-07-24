using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Specifications
{
    public class SubscribeFilterSpecification : BaseSpecification<Subscribe>
    {
        public SubscribeFilterSpecification() : base(item => !item.Removed)
        {
            AddInclude(subscribe => subscribe.Bill.Pays);
            AddInclude(subscribe => subscribe.Bill.Plan);
            AddInclude(subscribe => subscribe.User);
        }
        public SubscribeFilterSpecification(int id) : base(item => !item.Removed && item.Id == id)
        {
            AddInclude(subscribe => subscribe.Bill.Pays);
            AddInclude(subscribe => subscribe.Bill.Plan);
            AddInclude(subscribe => subscribe.User);
        }

        public SubscribeFilterSpecification(Bill bill) : base(item => !item.Removed && item.BillId == bill.Id)
        {
            AddInclude(subscribe => subscribe.Bill.Pays);
            AddInclude(subscribe => subscribe.Bill.Plan);
            AddInclude(subscribe => subscribe.User);
        }
    }

    public class SubscribeUserFilterSpecification : BaseSpecification<Subscribe>
    {
        public SubscribeUserFilterSpecification(string userId) : base(item => !item.Removed && item.UserId == userId)
        {
            AddInclude(subscribe => subscribe.Bill.Pays);
            AddInclude(subscribe => subscribe.Bill.Plan);
            AddInclude(subscribe => subscribe.User);
        }
    }

    public class SubscribePlanFilterSpecification : BaseSpecification<Subscribe>
    {
        public SubscribePlanFilterSpecification(int planId) : base(item => !item.Removed && item.PlanId == planId)
        {
            AddInclude(subscribe => subscribe.Bill.Pays);
            AddInclude(subscribe => subscribe.Bill.Plan);
            AddInclude(subscribe => subscribe.User);
        }
    }
}
