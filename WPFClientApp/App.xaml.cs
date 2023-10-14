using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WPFClientApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void AppClose(object sender, EventArgs e)
        {
            RestClient APIClient = new RestClient("http://localhost:65228/");
            RestRequest request = new RestRequest("ptp/RemoveClient/{id}", Method.Delete);
            request.AddParameter("id", Instance.Id);
            APIClient.Execute(request);
            Console.WriteLine("Closing Client");
        }
    }
}
