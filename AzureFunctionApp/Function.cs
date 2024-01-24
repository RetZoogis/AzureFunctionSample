using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;

namespace AzureFunctionApp
{
    public  class Function
    {

        //Notes:
        //Function App with Blob Input Binding Queue Trigger, Keyvault Access, Startup, Dependency Injection, Custom HealthCheck

        private readonly IConfiguration _configuration;
        public Function(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        //{queueTrigger} name section is mandatory if we want to use blob input binding 
        //Blob Input Binding
        public void Run([QueueTrigger("queuename", Connection = "AzureWebJobsStorage")] string queuemessage,
            [Blob("blobcontainername1/{queueTrigger}", FileAccess.Read, Connection = "AzureWebJobsStorage")] string blobcontent)
        {
            try
            {
                //Deserialize blobcontent since it is json
                //var x = JsonConvert.DeserializeObject<class1>(blobcontent);
                string key = "x.key";

                var ConnectionString = _configuration[key]; //look for the key from main _configuraiton builder

                //Get blob client and delete once transaction has been completed
                var storageconnectionstring = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                //in this case queuemessage is also the blobname.. it makes sense
                BlobClient client = new BlobClient(storageconnectionstring, "blobcontainername1", queuemessage); 
                client.Delete();


            }
            catch (Exception ex)
            {
                throw;
            }
        }



        [FunctionName("Healthcheck")]
        public async Task<ActionResult> Healthcheck([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            // IActionResult is more specific and is commonly used for actions that return standard HTTP status codes
            //ActionResult is a more generic type that allows you to return any type of ActionResult. It could be OkResult, NotFoundResult, JsonResult
            Console.WriteLine(req.Body.ToString);
            return new OkObjectResult("success");
        }

        #region Http Trigger Sample
        //public async Task<IActionResult> Run(
        //    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        //    ILogger log)
        //{
        //    log.LogInformation("C# HTTP trigger function processed a request.");

        //    string name = req.Query["name"];

        //    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //    dynamic data = JsonConvert.DeserializeObject(requestBody);
        //    name = name ?? data?.name;

        //    string responseMessage = string.IsNullOrEmpty(name)
        //        ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
        //        : $"Hello, {name}. This HTTP triggered function executed successfully.";

        //    return new OkObjectResult(responseMessage);
        //}
        #endregion
    }
}
