using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        public IActionResult Rates()
        {
            List<RatesModel> model = SQLData.GetRates();

            return View(model);
        }
        [HttpPost, ActionName("SaveRates")]
        public IActionResult RatesPost()
        {
            IEnumerable<RatesModel> model = ParseResultsForm(Request.Form);

            SQLData.SetRates(model);

            return RedirectToAction("Rates");
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static IEnumerable<RatesModel> ParseResultsForm(Microsoft.AspNetCore.Http.IFormCollection form)
        {
            var ret = new List<RatesModel>();

            IEnumerable<String> RatesToSave = form["item.Id"].ToString().Split(',');

            foreach (string r in RatesToSave)
            {
                int id;

                if (int.TryParse(r, out id))
                {
                    RatesModel rm = new RatesModel() { Id = id };

                    if (!String.IsNullOrEmpty(form["rate100_" + id.ToString()]))
                    {
                        decimal val;

                        if (Decimal.TryParse(form["rate100_" + id.ToString()], out val))
                        {
                            rm.Rate100 = val;
                        }
                    }

                    if (!String.IsNullOrEmpty(form["rate600_" + id.ToString()]))
                    {
                        decimal val;

                        if (Decimal.TryParse(form["rate600_" + id.ToString()], out val))
                        {
                            rm.Rate600 = val;
                        }
                    }

                    if (!String.IsNullOrEmpty(form["rate1200_" + id.ToString()]))
                    {
                        decimal val;

                        if (Decimal.TryParse(form["rate1200_" + id.ToString()], out val))
                        {
                            rm.Rate1200 = val;
                        }
                    }

                    if (!String.IsNullOrEmpty(form["rate1700_" + id.ToString()]))
                    {
                        decimal val;

                        if (Decimal.TryParse(form["rate1700_" + id.ToString()], out val))
                        {
                            rm.Rate1700 = val;
                        }
                    }

                    if (!String.IsNullOrEmpty(form["rate5000_" + id.ToString()]))
                    {
                        decimal val;

                        if (Decimal.TryParse(form["rate5000_" + id.ToString()], out val))
                        {
                            rm.Rate5000 = val;
                        }
                    }

                    ret.Add(rm);
                }
            }

            return ret;
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
                cmd.Parameters.Add("@p1", System.Data.SqlDbType.VarChar).Value = p1;
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

        public static List<RatesModel> GetRates()
        {
            List<RatesModel> ret = new List<RatesModel>();            

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {

                cmd.CommandText = "select [Код],[marC number], [Marketing category],[0-100],[100-600],[600-1200],[1200-1700],[1700-5000] from AppEquipmentCommodityRates";
                cmd.CommandType = System.Data.CommandType.Text;                                
                conn.Open();                               

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret.Add(new RatesModel()
                        {
                            Id = reader.GetInt32(0),
                            CommodityType = reader.GetString(1),
                            MarcNumber = reader.GetString(2),
                            Rate100 = Convert.ToDecimal(reader.GetDouble(3)),
                            Rate600 = Convert.ToDecimal(reader.GetDouble(4)),
                            Rate1200 = Convert.ToDecimal(reader.GetDouble(5)),
                            Rate1700 = Convert.ToDecimal(reader.GetDouble(6)),
                            Rate5000 = Convert.ToDecimal(reader.GetDouble(7))
                        });                        
                    }
                }
            }
            return ret;
        }

        public static void SetRates(IEnumerable<RatesModel> rates)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                foreach (RatesModel r in rates)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "update AppEquipmentCommodityRates set [0-100]=@r1,[100-600]=@r2,[600-1200]=@r3,[1200-1700]=@r4,[1700-5000]=@r5 where [Код]=@id";
                        cmd.Parameters.AddWithValue("@r1", r.Rate100);
                        cmd.Parameters.AddWithValue("@r2", r.Rate600);
                        cmd.Parameters.AddWithValue("@r3", r.Rate1200);
                        cmd.Parameters.AddWithValue("@r4", r.Rate1700);
                        cmd.Parameters.AddWithValue("@r5", r.Rate5000);
                        cmd.Parameters.AddWithValue("@id", r.Id);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
