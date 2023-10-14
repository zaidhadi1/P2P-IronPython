using System;
using CommonData;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WPFClientApp
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]
    public class ServerThreadInterfaceImpl : ServerThreadInterface
    {
        public static List<Job> jobs;

        public ServerThreadInterfaceImpl() { jobs = new List<Job>(); }

        //public void GetJob(int id, out Job job)
        //{
        //    throw new NotImplementedException();
        //}

        public List<Job> GetJobs()
        {
            return Jobs.listOfJobs;
        }

        public int GetJobCount()
        {
            if (Jobs.listOfJobs != null)
            {
                return Jobs.listOfJobs.Count;
            }
            else
            {
                return 0;
            }
        }

        // Client parameter here is the one that completed the job, used for incrementing their jobs done counter
        public void uploadResult(int id, string result)
        {
            Jobs.results.Add(result);
            Jobs.listOfJobs.RemoveAt(id);
        }


        public Job GetJob(int i)
        {
            return Jobs.listOfJobs[i];
        }

        public void updateJob(int id, Job job)
        {
            Jobs.listOfJobs[id] = job;
        }

        public void deleteJob(int id)
        {
            Jobs.listOfJobs.RemoveAt(id);
        }
    }
}
