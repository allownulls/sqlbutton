using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQLButton.Models;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SQLButton.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IConfiguration configuration;        

        public HomeController(IConfiguration config)
        {
            configuration = config;
            SQLData.connectionString = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString(configuration, "DefaultConnection");
        }

        public IActionResult Index()
        {
            IndexViewModel model = new IndexViewModel();

            ViewBag.Info1 = SQLData.GetData(1);
            ViewBag.Info2 = SQLData.GetData(2);
            ViewBag.Info3 = SQLData.GetData(3);

            ViewBag.Color1 = ((int)ViewBag.Info1 % 2 == 0 ? "AA0000" : "00AA00");
            ViewBag.Color2 = ((int)ViewBag.Info2 % 2 == 0 ? "AA0000" : "00AA00");
            ViewBag.Color3 = ((int)ViewBag.Info3 % 2 == 0 ? "AA0000" : "00AA00");

            model.SPParameters.Add(String.Empty);
            model.SPParameters.Add(String.Empty);
            model.SPParameters.Add(String.Empty);

            return View(model);
        }
        [HttpPost, ActionName("Index")]
        public IActionResult IndexPost(IndexViewModel model)
        {           

            if (model.Button1 != null)
            {                                
                SQLData.RunProcedure(model.SPParameters[0], 1);
            }

            if (model.Button2 != null)
            {
                SQLData.RunProcedure(model.SPParameters[1], 2);
            }

            if (model.Button3 != null)
            {
                SQLData.RunProcedure(model.SPParameters[2], 3);
            }

            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public static class SQLData
    {
        public static string connectionString { get; set; }

        public static void RunProcedure(string p1, int p2)
        {            
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Test1", con))
                {

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p1", p1);
                    cmd.Parameters.AddWithValue("@p2", p2);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static int GetData(int p1)
        {
            int ret = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {

                cmd.CommandText = "Test2";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@p1", System.Data.SqlDbType.VarChar).Value=p1;
                var returnParameter = cmd.Parameters.Add("@p2", System.Data.SqlDbType.Int);
                returnParameter.Value = 0;
                //var returnParameter = cmd.Parameters.Add("@r1", System.Data.SqlDbType.Int);

                returnParameter.Direction = System.Data.ParameterDirection.ReturnValue;

                conn.Open();
                cmd.ExecuteNonQuery();
                
                //using (SqlDataReader reader = cmd.ExecuteReader())
                //{
                //    while (reader.Read())
                //    {
                ret = (int)returnParameter.Value;
                //    }
                //}
            }

            return ret;
        }
    }
}
