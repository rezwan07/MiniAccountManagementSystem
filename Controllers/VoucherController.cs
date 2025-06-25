using MiniAccountManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

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

        public ActionResult ExportToExcel()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Voucher No");
            dt.Columns.Add("Date");
            dt.Columns.Add("Type");
            dt.Columns.Add("Account");
            dt.Columns.Add("Debit");
            dt.Columns.Add("Credit");
            dt.Columns.Add("Remarks");

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                string query = @"
            SELECT 
                V.ReferenceNo,
                V.VoucherDate,
                V.VoucherType,
                A.Name AS AccountName,
                D.DebitAmount,
                D.CreditAmount,
                D.Remarks
            FROM Vouchers V
            INNER JOIN VoucherDetails D ON V.Id = D.VoucherId
            INNER JOIN ChartOfAccounts A ON D.AccountId = A.Id
            ORDER BY V.VoucherDate DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dt.Rows.Add(
                        reader["ReferenceNo"],
                        Convert.ToDateTime(reader["VoucherDate"]).ToString("yyyy-MM-dd"),
                        reader["VoucherType"],
                        reader["AccountName"],
                        reader["DebitAmount"],
                        reader["CreditAmount"],
                        reader["Remarks"]
                    );
                }
            }

            string filename = "Vouchers_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

            GridView gv = new GridView();
            gv.DataSource = dt;
            gv.DataBind();

            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gv.RenderControl(hw);

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", $"attachment; filename={filename}");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return null;
        }
    }
}