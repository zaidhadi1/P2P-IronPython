using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonData
{
    public partial class Client
    {

        public int Id { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public int JobsDone { get; set; }
    }
}
