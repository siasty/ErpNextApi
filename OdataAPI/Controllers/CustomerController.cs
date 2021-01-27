using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Formatter.Value;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OdataAPI.Data;
using OdataAPI.ERPNext.Customer;
using System.Net.Http;

namespace OdataAPI.Controllers
{
    
 
    public class CustomerController : ODataController
    {
        private readonly IHttpClientFactory _clientFactory;
        private Customer _customer { get; set; }
        private readonly ILogger _logger;

        public CustomerController(IHttpClientFactory clientFactory, ILogger<CustomerController> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;

            _customer = new Customer(_clientFactory,_logger);
        }

        #region Get Customer

        [EnableQuery]
        public Task<IQueryable<CustomerRoot>> Get()
        {
            return _customer.CustomerGet();
        }

        [EnableQuery]
        public SingleResult<CustomerRoot> Get([FromODataUri] string key)
        {
            IQueryable<CustomerRoot> result = _customer.GetCustomerById(key).Result;
            return SingleResult.Create(result);
        }
        
        #endregion

        #region Create Customer

        public async Task<IActionResult> Post([FromBody] CustomerRoot customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _customer.CreateCystomer(customer);

            if (result != null)
            {
                _logger.LogInformation($"Customer: {result.name} created.");

            }
            return Created(result); 

        }

        #endregion
    }
}
