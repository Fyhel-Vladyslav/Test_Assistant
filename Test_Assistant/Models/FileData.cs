using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Assistant.Models
{
    public class FileData
    {
        public List<OrderList> OrderLists  { get; set; }
        public List<TestcaseData> Testcases { get; set; }
        public List<SpecialAction> SpecialActions { get; set; }
    }
}
