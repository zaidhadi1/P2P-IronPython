using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ClientAppRemote
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();

            host = new ServiceHost(typeof(DataServerInterfaceImpl));
            host.AddServiceEndpoint(typeof(DataServerInterface), tcp, "net.tcp://0.0.0.0:8100/DataService");

            host.Open();
            Console.WriteLine("Server Thread Online");
            Console.ReadLine();
            host.Close();
        }
    }
}
