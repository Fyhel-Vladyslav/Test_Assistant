using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Assistant.Models
{
    public class TestcaseData
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<TestCaseAction> actions { get; set; }
    }
}
