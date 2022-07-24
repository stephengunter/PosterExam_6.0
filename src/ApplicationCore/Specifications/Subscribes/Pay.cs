using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationCore.Specifications
{
    public class PayFilterSpecification : BaseSpecification<Pay>
    {
        public PayFilterSpecification() : base(item => !item.Removed)
        {
           
        }

        public PayFilterSpecification(string code) : base(item => item.Code == code)
        {
            AddInclude(item => item.Bill);
        }

        public PayFilterSpecification(int id) : base(item => !item.Removed && item.Id == id)
        {
           
        }

        

        public PayFilterSpecification(PayWay payWay) : base(item => !item.Removed && item.PayWay == payWay.Code)
        {
           
        }

    }

    public class PayBillFilterSpecification : BaseSpecification<Pay>
    {
        public PayBillFilterSpecification(IEnumerable<int> billIds) : base(item => !item.Removed && billIds.Contains(item.BillId))
        {
            
        }
    }

   
}
