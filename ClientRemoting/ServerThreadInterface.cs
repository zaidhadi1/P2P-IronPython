using System;
using CommonData;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClientRemoting
{
    [ServiceContract]
    public interface ServerThreadInterface
    {
        [OperationContract]
        void updateJobList(Job job);
        [OperationContract]
        int GetJobCount();
        [OperationContract]
        List<Job> GetJobs();
        [OperationContract] void GetJob(int i, out Job job);
        [OperationContract] void uploadResult(int id, Job job, Client client);

    }
}

