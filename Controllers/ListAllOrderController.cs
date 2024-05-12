using Microsoft.AspNetCore.Mvc;
using MultiTenantApps.Models;
using Newtonsoft.Json;
using System.Reflection;

namespace MultiTenantApps.Controllers
{
    public class ListAllOrderController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public ListAllOrderController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            List<Orders> lstOrder = null;

            List<CustomersModel> lstCustomer = null;

            List<ListAllOrderModel> lst = null;

            string tenantId = string.Empty;

            try
            {
                lstOrder = new List<Orders>();

                lstCustomer = new List<CustomersModel>();

                lst = new List<ListAllOrderModel>();

                tenantId = HttpContext.Session.GetString("TenantId");

                lstOrder = await GetOrderByTenantId(tenantId);

                lstCustomer = await GetCustomerByTenantId(tenantId);

                var distinctedCustomer = lstCustomer.Select(customer => customer.Id).Distinct().ToList();

                var distinctIdAndName = lstCustomer
    .Where(order => distinctedCustomer.Contains(order.Id))
    .Select(order => new
    {
        Id = order.Id,
        ContactName = order.ContactName
    })
    .ToList();

                foreach (var custData in distinctIdAndName)
                {
                    ListAllOrderModel model = new ListAllOrderModel();

                    foreach (var orderData in lstOrder)
                    {
                        if (custData.Id == orderData.CustomerId)
                        {
                            model.OrderId = orderData.Id.ToString();
                            model.OrderDate = orderData.OrderDate.ToString();
                            model.CustomerName = custData.ContactName;

                            foreach (OrderDetail p in orderData.Details)
                            {
                                model.Total = model.Total + ((p.UnitPrice * p.Quantity) - p.Discount);
                            }

                            

                        }
                    }

                    lst.Add(model);
                
                }

                return View(lst);

            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, "Failed to retrieve orders from the API.");
                return View();
            }

            
        }

        private async Task<List<Orders>> GetOrderByTenantId(string TenantId)
        {
            List<Orders> lst = null;
            try
            {
                lst = new List<Orders>();

                var client = _clientFactory.CreateClient("BaseApiClient");

                var response = await client.GetAsync("api/orders");

                

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();

                    var dataItems = JsonConvert.DeserializeObject<List<Orders>>(responseData);

                    //lst = JsonConvert.DeserializeObject<List<Orders>>(responseData).Where(orders =>
                    //        orders.ShipAddress.Country == TenantId).ToList();


                    lst = dataItems.Where(orders => orders.ShipAddress.Country == TenantId).ToList<Orders>();

                    return lst;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve orders from the API.");
                    
                }

                return lst;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task<List<CustomersModel>> GetCustomerByTenantId(string TenantId)
        {
            List<CustomersModel> lst = null;
            try
            {
                lst = new List<CustomersModel>();

                var client = _clientFactory.CreateClient("BaseApiClient");

                var response = await client.GetAsync("api/customers");

                

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();


                    lst = JsonConvert.DeserializeObject<List<CustomersModel>>(responseData).Where(customer =>
                            customer.Address.Country == TenantId).ToList();


                    return lst;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve orders from the API.");

                }

                return lst;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
