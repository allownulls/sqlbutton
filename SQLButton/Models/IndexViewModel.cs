using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SQLButton.Models
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            SPParameters = new List<string>();
        }

        public List<String> SPParameters { get; set; }        

        public string ErrorMessage { get; set; }

        public string Button1 { get; set; }
        public string Button2 { get; set; }
        public string Button3 { get; set; }


    }
}
