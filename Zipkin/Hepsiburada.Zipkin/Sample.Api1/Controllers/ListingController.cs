using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Hepsiburada.Zipkin.HttpHandlers;
using Hepsiburada.Zipkin.Interfaces;
using Sample.Api.Core.Attributes;
using Sample.Api.Core.Models;
using Sample.Api1.ActionFilters;

namespace Sample.Api1.Controllers
{
    [RoutePrefix("api/listing")]
    public class ListingController : ApiController
    {
        private readonly IZipkinClient _client;

        public ListingController(IZipkinClient client)
        {
            _client = client;
        }
        [HttpGet]
        [VersionedRoute("", 1)]
        [ZipkinTrace]
        public async Task<IHttpActionResult> Get()
        {
            return Ok(await Task.Factory.StartNew(() => ProductList.Items));
        }

        [HttpGet]
        [VersionedRoute("{id}", 1)]
        [ZipkinTrace]
        public async Task<IHttpActionResult> Get(int id)
        {
            using (var httpClient = new HttpClient(new ZipkinMessageHandler(_client)))
            {
                var response = await httpClient.GetAsync($"http://localhost:4411/api/products/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return Ok(await response.Content.ReadAsStringAsync());

                }
            }
            return BadRequest();
        }
    } 
}
