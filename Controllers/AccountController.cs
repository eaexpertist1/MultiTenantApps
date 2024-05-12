using Microsoft.AspNetCore.Mvc;
using System.Text;

using System;
using System.Net.Http;
using System.Net.Http.Headers;

using System.Threading.Tasks;

using Newtonsoft.Json;
using MultiTenantApps.Models;
using System.Diagnostics.Metrics;
using System.Data;

namespace MultiTenantApps.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        
        public AccountController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {

            try
            {
                var client = _clientFactory.CreateClient("BaseApiClient");

                
                var response = await client.GetAsync("api/Customers");

                if (response.IsSuccessStatusCode)
                {

                    var responseData = await response.Content.ReadAsStringAsync();


                    var dataItems = JsonConvert.DeserializeObject<List<CustomersModel>>(responseData);

                    
                    var filteredData = dataItems.Where(customer =>
                        customer.Address.Country==model.TenantId &&
                        customer.Id == model.Username && customer.Address.PostalCode==model.Password);

                    if (filteredData.Any())
                    {
                        HttpContext.Session.SetString("TenantId", model.TenantId);
                        return RedirectToAction("Menu", "Home");
                    }

                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");

                    return View(model);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            

            
        }


    }
}
