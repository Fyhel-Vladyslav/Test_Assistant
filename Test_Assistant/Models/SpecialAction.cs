using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Assistant.Models
{
    public class SpecialAction
    {
        public int id { get; set; }
        public string actionName { get; set; }

        public int testCaseId { get; set; }
        //public int afterTestCaseActionId { get; set; }

        public int xAreaStart { get; set; }
        public int yAreaStart { get; set; }
        public int xAreaEnd { get; set; }
        public int yAreaEnd { get; set; }
        public string path { get; set; }
        public string comparedTo { get; set; }
    }
}