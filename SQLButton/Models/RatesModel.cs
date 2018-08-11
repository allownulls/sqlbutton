using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SQLButton.Models
{
    public class RatesModel
    {
        public int Id { get; set; }
        public string MarcNumber { get; set; }
        public string CommodityType { get; set; }

        public decimal Rate100 { get; set; }

        public decimal Rate600 { get; set; }

        public decimal Rate1200 { get; set; }

        public decimal Rate1700 { get; set; }

        public decimal Rate5000 { get; set; }

    }
}
