using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebServer.Models;

namespace WebServer.Controllers
{
    //Peer to Peer controller
    [RoutePrefix("ptp")]
    public class PTPController : ApiController
    {

        [Route("RegisterClient")]
        [HttpPost]
        [ResponseType(typeof(Client))]
        public IHttpActionResult regClient([FromBody] Client inClient)
        {
            RestClient client = new RestClient("http://localhost:65228/");
            int maxId = 0 ;
            if (inClient.Ip != null)
            {
                if (!inClient.Ip.Equals(""))
                {
                    RestRequest request = new RestRequest("api/clients", Method.Get);
                    RestResponse response = client.Execute(request);
                    if (response != null)
                    {
                        List<Client> clientList = JsonConvert.DeserializeObject<List<Client>>(response.Content);
                        foreach(Client client1 in clientList)
                        {
                            if(maxId < client1.Id)
                            {
                                maxId = client1.Id;
                            }
                        }
                    }

                    inClient.Id = maxId + 1;

                    request = new RestRequest("api/clients");
                    request.AddJsonBody(inClient);
                    RestResponse nresponse = client.Post(request);
                    Client returnClient = JsonConvert.DeserializeObject<Client>(nresponse.Content);
                    return Ok(returnClient);
                }
                else
                { return BadRequest("enter valid value"); }
            }
            else
            {
                return BadRequest("enter valid value");
            }
        }

        [Route("RemoveClient/{id}")]
        [HttpDelete]
        public IHttpActionResult delClient(int id)
        {
            RestClient client = new RestClient("http://localhost:65228/" );
            RestRequest request = new RestRequest("api/clients/{id}", Method.Delete);
            request.AddParameter("id", id);
            client.Execute(request);
            return Ok();
        }

        [Route("CompleteJob/{id}")]
        [HttpPut]
        public IHttpActionResult complete(int id, [FromBody]Client inClient)
        {
            RestClient client = new RestClient("http://localhost:65228/");
            RestRequest request = new RestRequest("api/clients/{id}", Method.Get);
            request.AddParameter("id", id);
            RestResponse response = client.Execute(request);
            Client cClient = JsonConvert.DeserializeObject<Client>(response.Content);

            cClient.JobsDone++;

            request = new RestRequest("api/clients/{id}");
            request.AddParameter("id", id);
            request.AddJsonBody(cClient);

            client.Put(request);

            return Ok();
        }
    }
}
