using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniAccountManagementSystem.Models
{
    public class VoucherDetail
    {
        public int AccountId { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string Remarks { get; set; }
    }
}