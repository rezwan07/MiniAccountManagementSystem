using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MiniAccountManagementSystem.Controllers
{
    public class UserRoleController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        [HttpGet]
        public ActionResult AssignRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AssignRole(string userEmail, string roleName)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new SqlCommand("sp_AssignUserRole", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserEmail", userEmail);
                    cmd.Parameters.AddWithValue("@RoleName", roleName);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            ViewBag.Message = "Role assigned successfully.";
            return View();
        }
    }
}