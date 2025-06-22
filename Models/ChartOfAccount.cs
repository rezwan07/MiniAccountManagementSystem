using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniAccountManagementSystem.Models
{
    public class ChartOfAccount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public string AccountType { get; set; }
        public bool IsActive { get; set; }
    }
}