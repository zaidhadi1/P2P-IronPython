using System;
using CommonData;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WPFClientApp
{
    [ServiceContract]
    public interface ServerThreadInterface
    {
        [OperationContract]
        int GetJobCount();
        [OperationContract]
        List<Job> GetJobs();
        [OperationContract] 
        Job GetJob(int i);
        [OperationContract] 
        void uploadResult(int id, string result);
        [OperationContract]
        void updateJob(int id, Job job);
        [OperationContract]
        void deleteJob(int id);




    }
}

