using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Assistant.Models
{
    public class OrderList
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<int> caseIds { get; set; }

    }
}
