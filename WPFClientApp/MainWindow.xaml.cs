using CommonData;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting.Hosting;
using Microsoft.Win32;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WPFClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ServerThreadInterface connectServer;
        private string urlStr;
        ScriptEngine pythonEngine;
        RestClient APIClient;

        public MainWindow()
        {

            InitializeComponent();
            APIClient = new RestClient("http://localhost:65228/");
            pythonEngine = Python.CreateEngine();
            Task.Run(() => startNetwork());
            //Console.SetOut(new BoxWriter(SearchBox));
            Jobs.listOfJobs = new List<Job>();
            Jobs.results = new List<string>();
        }

        public async Task InitializeServerThreadAsync()
        {
            try
            {
                ServiceHost host;
                NetTcpBinding tcp = new NetTcpBinding();
                //Task.Run(() => testing());
                host = new ServiceHost(typeof(ServerThreadInterfaceImpl));
                host.AddServiceEndpoint(typeof(ServerThreadInterface), tcp, "net.tcp://" + urlStr + "/ServerThread");
                host.Open();
                urlBox.IsReadOnly = true;
                Host.IsEnabled = false;
            
            string[] urlPart = urlStr.Split(':');
            Client nClient = new Client
            {
                Ip = urlPart[0],
                Port = Int32.Parse(urlPart[1])
            };

            RestRequest request = new RestRequest("ptp/RegisterClient", RestSharp.Method.Post);
            request.AddJsonBody(nClient);
            RestResponse response = APIClient.Execute(request);
            Client rClient = JsonConvert.DeserializeObject<Client>(response.Content);

            Instance.Id = rClient.Id;
            counter.Text = "ID:" + Instance.Id;

            Console.WriteLine("Server Thread Online");
            serverStatus.Text = "Online";
            Task.Run(() => checkResult());
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("");
            }
        }

        private void PostButton_Click(object sender, RoutedEventArgs e)
        {
            if (PythonBox.Text.ToString() != "")
            {
                SHA256 sha256 = SHA256.Create();
                Job job = new Job
                {
                    Code = B64Encode(PythonBox.Text.ToString())
                    
                };
                job.hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(job.Code));
                Jobs.listOfJobs.Add(job);

            }
            else
            {
                PythonBox.Text = "Enter Value";
            }
        }

        private void GetFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Python (*.py)|*.py";
                if (openFileDialog.ShowDialog() == true)
                {
                    PythonBox.Text = File.ReadAllText(openFileDialog.FileName);
                    FileLoc.Text = openFileDialog.FileName;
                }
            }
            catch (IOException e1)
            {
                Console.WriteLine(e1.Message);
            }
        }


        private async void Host_Click(object sender, RoutedEventArgs e)
        {

            if (urlBox.Text.ToString() != null)
            {
                urlStr = urlBox.Text.ToString();
                try
                {
                    InitializeServerThreadAsync();
                }
                catch (Exception er)
                {
                    Console.WriteLine(er.Message);
                }
            }
            else
            {
                urlBox.Text = "Enter Url";
            }
        }

        private async void startNetwork()
        {
            Random rd = new Random();
            while (true)
            {
                Thread.Sleep(5000);
                List<Client> loClient = null;
                clientList.Dispatcher.Invoke(new Action(() => { clientList.Items.Clear(); }));
                try
                {
                    RestRequest request = new RestRequest("api/Clients");
                    RestResponse listOfClients = APIClient.Get(request);
                    loClient = JsonConvert.DeserializeObject<List<Client>>(listOfClients.Content);
                }
                catch(HttpRequestException HRE)
                {
                    Console.WriteLine(HRE.Message);
                }
                    Client client = null;
                if (loClient != null)
                {
                    if (loClient.Count > 0)
                    {
                        try
                        { 
                            client = loClient[rd.Next(0, (loClient.Count))];
                            string url = client.Ip + ":" + client.Port
                                .ToString();
                            if (!url.Equals(urlStr)) //if the client isnt own client
                            {
                                ChannelFactory<ServerThreadInterface> factory;
                                NetTcpBinding tcp = new NetTcpBinding();
                                string nurl = "net.tcp://" + url + "/ServerThread";
                                factory = new ChannelFactory<ServerThreadInterface>(tcp, nurl);
                                connectServer = factory.CreateChannel();
                               
                                for (int ii = 0; ii < connectServer.GetJobCount(); ii++)
                                {
                                    Job currJob = connectServer.GetJob(ii);
                                    if (!currJob.Busy)
                                    {
                                        if (compareHash(currJob))
                                        {
                                            currJob.Busy = true;//set to busy so that job is being worked on and no other client can work on it
                                            connectServer.updateJob(ii, currJob);
                                            await Task.Run(() => runPython(ii, currJob));
                                        }
                                    }
                                }
                            }

                            foreach (Client client2 in loClient)
                            {

                                string url2 = client2.Ip + ":" + client2.Port
                               .ToString();

                                if (!url2.Equals(urlStr)) //if the client isnt own client
                                {
                                    ChannelFactory<ServerThreadInterface> factory;
                                    NetTcpBinding tcp = new NetTcpBinding();
                                    string nurl = "net.tcp://" + url2 + "/ServerThread";
                                    factory = new ChannelFactory<ServerThreadInterface>(tcp, nurl);
                                    connectServer = factory.CreateChannel();
                                    ClientList nClient = new ClientList
                                    {
                                        ClientName = url2,
                                        JobCount = connectServer.GetJobCount()
                                    };
                                    clientList.Dispatcher.Invoke(new Action(() => { clientList.Items.Add(nClient); }));
                                }
                            }
                        }
                        catch(EndpointNotFoundException err)
                        {
                            Console.WriteLine(err);
                            RestClient APIClient = new RestClient("http://localhost:65228/");
                            RestRequest delrequest = new RestRequest("ptp/RemoveClient/{id}", RestSharp.Method.Delete);
                            delrequest.AddParameter("id", client.Id);
                            APIClient.Execute(delrequest);
                            Console.WriteLine("Removing Client ID:" + client.Id);

                        }
                    }
                }
            }
        }

        private async void runPython(int ii, Job inJob)
        {
            try
            {
                taskExeStatus.Dispatcher.Invoke(new Action(() => { taskExeStatus.Text = ""; }));
                taskProgBar.Dispatcher.Invoke(new Action(() => { taskProgBar.IsIndeterminate = true; }));
                string results = "";
                List<string> Functions = new List<string>();
                ScriptSource pythonScript = pythonEngine.CreateScriptSourceFromString(B64Decode(inJob.Code));
                ScriptScope scope = pythonEngine.CreateScope();
                results += pythonScript.Execute(scope);
                Console.Out.WriteLine("List of variables in the scope:");
                foreach (KeyValuePair<string, dynamic> keyValuePair in scope.GetItems())
                {
                    Console.WriteLine(keyValuePair.Key);
                    if (!keyValuePair.Key.StartsWith("_"))
                    {
                        Functions.Add(keyValuePair.Key);
                    }
                }

                if (Functions.Count > 0)
                {
                    string functionName = Functions[0];


                    Console.WriteLine("executing " + functionName);
                    dynamic function = scope.GetVariable(functionName);
                    try
                    {
                        var result = function();
                        results += result + " ";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }


                }
                RestClient client = new RestClient("http://localhost:65228/");
                RestRequest request = new RestRequest("api/clients/" + (Instance.Id));
                RestResponse response = client.Get(request);
                Client cClient = JsonConvert.DeserializeObject<Client>(response.Content);

                cClient.JobsDone++;

                request = new RestRequest("api/clients/" + Instance.Id);
                request.AddJsonBody(cClient);

                client.Put(request);

                connectServer.uploadResult(ii, results);
                taskProgBar.Dispatcher.Invoke(new Action(() => { taskProgBar.IsIndeterminate = false; }));

            }
            catch (SyntaxErrorException SEE)
            {
                Console.WriteLine(SEE.Message);
                taskProgBar.Dispatcher.Invoke(new Action(() => { taskProgBar.IsIndeterminate = false; }));
                taskExeStatus.Dispatcher.Invoke(new Action(() => { taskExeStatus.Text = "Error: Check Console"; }));
                connectServer.uploadResult(ii, "Error");
            }
            catch (UnboundNameException UNE)
            {
                Console.WriteLine(UNE.Message);
                taskProgBar.Dispatcher.Invoke(new Action(() => { taskProgBar.IsIndeterminate = false; }));
                taskExeStatus.Dispatcher.Invoke(new Action(() => { taskExeStatus.Text = "Error: Check Console"; }));
                connectServer.uploadResult(ii, "Error");

            }
            catch (Exception EX)
            {
                Console.WriteLine(EX.Message);
                taskProgBar.Dispatcher.Invoke(new Action(() => { taskProgBar.IsIndeterminate = false; }));
                taskExeStatus.Dispatcher.Invoke(new Action(() => { taskExeStatus.Text = "Error: Check Console"; }));
                connectServer.uploadResult(ii, "Error");

            }
            finally
            {
                Console.WriteLine("");
            }
        }

        private async void checkResult()
        {
            while (true)
            {
                Thread.Sleep(10000);
                ResultList.Dispatcher.Invoke(new Action(() => { ResultList.Items.Clear(); }));
                if (Jobs.results.Count > 0)
                {
                    foreach (string result in Jobs.results)
                    {
                        ResultList.Dispatcher.Invoke(new Action(() =>
                        {
                            ResultList.Items.Add(result);
                        }));
                    }
                }
            }

        }

        private string B64Encode(string str)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(str);
            return System.Convert.ToBase64String(bytes);
        }

        private string B64Decode(string b64str)
        {
            var b64Bytes = System.Convert.FromBase64String(b64str);
            return System.Text.Encoding.UTF8.GetString(b64Bytes); 
        }

        private bool compareHash(Job inJob)
        {
            SHA256 sha256 = SHA256.Create();
            return inJob.hash.SequenceEqual(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(inJob.Code)));

        }
    }
}
