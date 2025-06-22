using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniAccountManagementSystem.Models
{
    public class Voucher
    {
        public int Id { get; set; }
        public string VoucherType { get; set; }
        public DateTime VoucherDate { get; set; }
        public string ReferenceNo { get; set; }

        public List<VoucherDetail> Details { get; set; }
    }
}