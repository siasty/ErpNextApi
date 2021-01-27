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

        #region Update costumer (put,patch)
        
        public async Task<IActionResult> Patch([FromODataUri] string key,[FromBody] Delta<CustomerRoot> customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await _customer.GetCustomerById(key);
            if (entity == null)
            {
                return NotFound();
            }
            
            customer.Patch(entity.FirstOrDefault());

            var result = new CustomerRoot();
            try
            {
                result = await _customer.UpdateCystomer(entity.FirstOrDefault());    
            }
            catch (Exception ex)
            {
                var exists = await _customer.CustomerExists(key);
                if (!exists)
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex.Message);
                    throw;
                    
                }
            }
            return Updated(result);
        }

        public async Task<IActionResult> Put([FromODataUri] string key,[FromBody] CustomerRoot update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != update.name)
            {
                return BadRequest("Not compatible ID: "+key);
            }

            var result = new CustomerRoot();

            try
            {
                result = await _customer.UpdateCystomer(update);
            }
            catch (Exception ex)
            {
                var exists = await _customer.CustomerExists(key);

                if (!exists)
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex.Message);
                    throw;
                }
            }
            return Updated(result);
        }
        #endregion

        #region delete customer
        public async Task<IActionResult> Delete([FromODataUri] string key)
        {
            var entity = await _customer.GetCustomerById(key);
            if (entity == null)
            {
                return NotFound();
            }

            if (await _customer.RemoveCystomer(entity.FirstOrDefault().name))
            {
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }
        }
        #endregion


    }
}
