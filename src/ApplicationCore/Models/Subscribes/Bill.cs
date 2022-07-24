using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using ApplicationCore.Helpers;
using Infrastructure.Entities;

namespace ApplicationCore.Models
{
	public class Bill : BaseRecord
	{
		public string UserId { get; set; }

		public int PlanId { get; set; }

		public decimal Amount { get; set; }

		public bool HasDiscount { get; set; }

		public DateTime? DeadLine { get; set; }

		public int PayWayId { get; set; }

		public virtual PayWay PayWay { get; set; }

		public virtual Plan Plan { get; set; }

		public virtual User User { get; set; }

		public virtual ICollection<Subscribe> Subscribes { get; set; } = new List<Subscribe>();

		public virtual ICollection<Pay> Pays { get; set; } = new List<Pay>();

		[NotMapped]
		public bool Payed => TotalPayed >= Amount;

		[NotMapped]
		public decimal NeedPayMoney => Amount - TotalPayed;


		public decimal TotalPayed => Pays.IsNullOrEmpty() ? 0 : Pays.Where(p => p.HasMoney).Sum(p => p.Money);

		[NotMapped]
		public DateTime? PayedDate
		{
			get
			{
				if (Payed) return Pays.Where(p => p.HasMoney).OrderByDescending(p => p.PayedDate).FirstOrDefault().PayedDate;
				return null;
			}
		}

		public bool Expired
		{
			get
			{
				if (Payed) return false;
				if (DeadLine.HasValue) return DateTime.Now > DeadLine;
				return false;
			}
		}
	}
}
