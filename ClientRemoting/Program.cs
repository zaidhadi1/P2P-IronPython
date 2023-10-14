using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClientRemoting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();

            host = new ServiceHost(typeof(ServerThreadInterfaceImpl));
            host.AddServiceEndpoint(typeof(ServerThreadInterface), tcp, "net.tcp://0.0.0.0:8100/ServerThread");
            host.AddServiceEndpoint(typeof(ServerThreadInterface), tcp, "net.tcp://0.0.0.0:8200/ServerThread");
            host.Open();
            Console.WriteLine("Server Thread Online");
            Console.ReadLine();
            host.Close();
        }
    }
}
