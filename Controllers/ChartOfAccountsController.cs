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
    public class ChartOfAccountsController : Controller
    {
        private string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {
            var list = new List<ChartOfAccount>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM ChartOfAccounts", con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new ChartOfAccount
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        ParentId = reader["ParentId"] as int?,
                        AccountType = reader["AccountType"].ToString(),
                        IsActive = (bool)reader["IsActive"]
                    });
                }
            }

            return View(list);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ChartOfAccount acc)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("sp_ManageChartOfAccounts", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "Insert");
                cmd.Parameters.AddWithValue("@Name", acc.Name);
                cmd.Parameters.AddWithValue("@ParentId", (object)acc.ParentId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AccountType", acc.AccountType);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ChartOfAccount acc = new ChartOfAccount();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM ChartOfAccounts WHERE Id = @Id", con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    acc.Id = (int)reader["Id"];
                    acc.Name = reader["Name"].ToString();
                    acc.ParentId = reader["ParentId"] as int?;
                    acc.AccountType = reader["AccountType"].ToString();
                }
            }

            return View(acc);
        }

        [HttpPost]
        public ActionResult Edit(ChartOfAccount acc)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("sp_ManageChartOfAccounts", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "Update");
                cmd.Parameters.AddWithValue("@Id", acc.Id);
                cmd.Parameters.AddWithValue("@Name", acc.Name);
                cmd.Parameters.AddWithValue("@ParentId", (object)acc.ParentId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AccountType", acc.AccountType);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("sp_ManageChartOfAccounts", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "Delete");
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Name", DBNull.Value);
                cmd.Parameters.AddWithValue("@ParentId", DBNull.Value);
                cmd.Parameters.AddWithValue("@AccountType", DBNull.Value);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public FileResult ExportToExcel()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("ParentId");
            dt.Columns.Add("Type");
            dt.Columns.Add("IsActive");

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT Name, ParentId, AccountType, IsActive FROM ChartOfAccounts", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dt.Rows.Add(reader["Name"], reader["ParentId"], reader["AccountType"], reader["IsActive"]);
                }
            }

            string filename = "ChartOfAccounts_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    GridView gv = new GridView();
                    gv.DataSource = dt;
                    gv.DataBind();
                    gv.RenderControl(hw);

                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", $"attachment; filename={filename}");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();

                    return null; // required but won’t be executed
                }
            }
        }

    }
}