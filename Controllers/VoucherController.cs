using MiniAccountManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniAccountManagementSystem.Controllers
{
    public class VoucherController : Controller
    {
        private string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {
            List<Voucher> vouchers = new List<Voucher>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = "SELECT Id, VoucherType, VoucherDate, ReferenceNo FROM Vouchers ORDER BY VoucherDate DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    vouchers.Add(new Voucher
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        VoucherType = reader["VoucherType"].ToString(),
                        VoucherDate = Convert.ToDateTime(reader["VoucherDate"]),
                        ReferenceNo = reader["ReferenceNo"].ToString()
                    });
                }
            }

            return View(vouchers);
        }

        public ActionResult Create()
        {
            ViewBag.Accounts = GetAccounts();
            return View(new Voucher { Details = new List<VoucherDetail> { new VoucherDetail() } });
        }

        [HttpPost]
        public ActionResult Create(Voucher model)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("sp_SaveVoucher", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@VoucherType", model.VoucherType);
                cmd.Parameters.AddWithValue("@VoucherDate", model.VoucherDate);
                cmd.Parameters.AddWithValue("@ReferenceNo", model.ReferenceNo);

                // Prepare TVP
                DataTable dt = new DataTable();
                dt.Columns.Add("AccountId", typeof(int));
                dt.Columns.Add("DebitAmount", typeof(decimal));
                dt.Columns.Add("CreditAmount", typeof(decimal));
                dt.Columns.Add("Remarks", typeof(string));

                foreach (var d in model.Details)
                {
                    dt.Rows.Add(d.AccountId, d.DebitAmount, d.CreditAmount, d.Remarks);
                }

                SqlParameter tvpParam = cmd.Parameters.AddWithValue("@Details", dt);
                tvpParam.SqlDbType = SqlDbType.Structured;
                tvpParam.TypeName = "VoucherDetailType";

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Create");
        }

        private List<SelectListItem> GetAccounts()
        {
            var accounts = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT Id, Name FROM ChartOfAccounts", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    accounts.Add(new SelectListItem
                    {
                        Value = reader["Id"].ToString(),
                        Text = reader["Name"].ToString()
                    });
                }
            }
            return accounts;
        }
    }
}