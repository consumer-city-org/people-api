using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsumerCity.People.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PeopleController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<PeopleController> _logger;

        public PeopleController(ILogger<PeopleController> logger)
        {
            _logger = logger;
        }

        [HttpGet("forecast")]
        public IEnumerable<WeatherForecast> GetWeatherForecast()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("v")]
        public string GetVersion()
        {
            return "6";
        }

        [HttpGet("e")]
        public ActionResult GetEnv()
        {

            var env = new {
                AwsId = Environment.GetEnvironmentVariable("DynamoUsername"),
                AwsPassword = Environment.GetEnvironmentVariable("DynamoPassword")
            };

            return Ok(env);
        }

        [HttpGet("p")]
        public async Task<ActionResult> GetPeople()
        {
            var credentials = new BasicAWSCredentials(Environment.GetEnvironmentVariable("AwsId"), Environment.GetEnvironmentVariable("AwsPassword"));
            var config = new AmazonDynamoDBConfig() {
                RegionEndpoint = RegionEndpoint.SAEast1
            };
            var client = new AmazonDynamoDBClient(credentials, config);

            var context = new DynamoDBContext(client);

            //var result = await context.LoadAsync<Person>("1");

            //await context.SaveAsync(new Person() { Id = "1", Name = "Pedro"});

            var result = await context.ScanAsync<Person>(new List<ScanCondition>()).GetRemainingAsync();

            return Ok(result);

        }

        public class Person
        {
            [DynamoDBHashKey("id")]
            public string Id { get; set; }

            [DynamoDBProperty("name")]
            public string Name { get; set; }
        }
    }
}
