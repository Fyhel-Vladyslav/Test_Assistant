using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Test_Assistant.Models
{
    [StructLayout(LayoutKind.Sequential)]
    struct Input
    {
        public uint Type;
        public MouseInput Data;
    }
}
