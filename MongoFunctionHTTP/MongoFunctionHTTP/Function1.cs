using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MongoFunctionHTTP
{
    public static class FunctionMongo
    {
        [FunctionName("FunctionMongo")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            const string connectionString = "mongodb://jlrmongodb:uo8lffJvu6bYAWF4kSIAeZnfsjXSGL0fqnqaoAtsPewFmH1r9VnkJje1YMShe2F9BLYSI2xikTsQzleykpkDGw==@jlrmongodb.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";

            // Create a MongoClient object by using the connection string
            var client = new MongoClient(connectionString);

            //Use the MongoClient to access the server
            var database = client.GetDatabase("mongodb");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var messages = JsonConvert.DeserializeObject<List<object>>(requestBody);

            foreach (var message in messages)
            {
                Console.WriteLine("message: " + message);
                BsonDocument document;
                var collection = database.GetCollection<BsonDocument>("data");
                document = BsonSerializer.Deserialize<BsonDocument>(message.ToString());
                collection.InsertOne(document);
            }

            return messages != null
                ? (ActionResult)new OkObjectResult($"Hello, {messages}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");

        }
    }
}
