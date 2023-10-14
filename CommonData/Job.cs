using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonData
{
    /*** WARNING DOES NOT MATCH WITH THE JOB CLASS IN DATABASE MODEL ***/
    public partial class Job
    {
        public string Code { get; set; }
        public string Result { get; set; }
        public bool Busy { get; set; }
        public byte[] hash { get; set; }
    }
}
