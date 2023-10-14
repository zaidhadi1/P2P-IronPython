using System;
using CommonData;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClientRemoting
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class ServerThreadInterfaceImpl : ServerThreadInterface
    {
        public static List<Job> jobs;
        
        public ServerThreadInterfaceImpl() { jobs = new List<Job>();}

        public void GetJob(int id, out Job job)
        {
            throw new NotImplementedException();
        }

        public List<Job> GetJobs()
        {
            return jobs;
        }

        public int GetJobCount()
        {
            return jobs.Count;
        }

        // Client parameter here is the one that completed the job, used for incrementing their jobs done counter
        public void uploadResult(int id, Job job, Client client)
        {
            jobs[id] = job;
        }

        public void updateJobList(Job job)
        {
            jobs.Add(job);
        }
    }
}
