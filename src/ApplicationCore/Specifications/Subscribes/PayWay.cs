using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Specifications
{
    public class PayWayFilterSpecification : BaseSpecification<PayWay>
    {
        public PayWayFilterSpecification() : base(item => !item.Removed)
        {
           
        }

        public PayWayFilterSpecification(int id) : base(item => !item.Removed && item.Id == id)
        {

        }

        public PayWayFilterSpecification(string code) : base(item => !item.Removed && item.Code.ToLower() == code.ToLower())
        {

        }

    }
}
