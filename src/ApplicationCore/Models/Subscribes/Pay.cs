using ApplicationCore.Helpers;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Models
{
	public class Pay : BaseRecord
	{
		public static Pay Create(Bill bill, PayWay payWay, ThirdPartyPayment thirdPartyPayment)
		{
			return new Pay
			{
				BillId = bill.Id,
				PayWay = payWay.Code,
				Provider = thirdPartyPayment.ToString()
			};

		}


		public int BillId { get; set; }
		
		public string Code { get; set; } = TickId.Create(20);

		public string? PayWay { get; set; } = string.Empty;

		public string? TradeNo { get; set; } = string.Empty;

		public string? BankCode { get; set; } = string.Empty;

		public string? BankAccount { get; set; } = string.Empty;

		public string? Provider { get; set; } = string.Empty;

		public decimal Money { get; set; }

		public DateTime? PayedDate { get; set; }

		public string? TradeData { get; set; } = string.Empty; //json string


		public virtual Bill Bill { get; set; }


		[NotMapped]
		public bool HasMoney => Removed ? false : Money > 0;
	}

	public class PayWay : BaseRecord
	{
		public string? Code { get; set; } = string.Empty;

		public string? Title { get; set; } = string.Empty;

	}
}
