using Microsoft.AspNetCore.Mvc;
using MultiTenantApps.Models;
using Newtonsoft.Json;
using System.Reflection;

namespace MultiTenantApps.Controllers
{
    public class MonthlySalesreportController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public MonthlySalesreportController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            

            
            List<MonthlyReport> lst = null;

            string tenantId = string.Empty;

            try
            {
                lst = new List<MonthlyReport>();

                tenantId = HttpContext.Session.GetString("TenantId");

                lst = await GenerateReport(tenantId);
                
                return View(lst);

            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, "Failed to retrieve orders from the API.");
                return View();
            }

            
        }

        private async Task<List<MonthlyReport>> GenerateReport(string TenantId)
        {

            List<MonthlyReport> lst = null;

            List<Orders> lstOrder = null;

            List<string> lstMonth = null;

            bool found = false;

            try
            {
                lst = new List<MonthlyReport>();

                lstMonth = new List<string>();

                lstOrder = new List<Orders>();

                lstOrder = await GetOrderByTenantId(TenantId);

                foreach (var data in lstOrder)
                {
                    DateTime date = Convert.ToDateTime(data.OrderDate);
                    found = false;
                    foreach (var data1 in lstMonth)
                    {
                        if (date.ToString("MMM- yyyy") ==data1)
                        {
                            found = true;
                            break;
                        }

                    }
                    
                    if (found==false)
                    {
                        lstMonth.Add(date.ToString("MMM- yyyy"));
                    }


                }


                foreach (var data in lstMonth)
                {
                    MonthlyReport model = new MonthlyReport();
                    foreach (var data1 in lstOrder)
                    {
                        DateTime date = Convert.ToDateTime(data1.OrderDate);

                        if (data == date.ToString("MMM- yyyy"))
                        {
                            model.Month = data;
                            foreach (var p in data1.Details)
                            {
                                model.TotalOrders = model.TotalOrders + p.Quantity;
                                model.TotalRevenue = model.TotalRevenue + (p.UnitPrice * p.Quantity);
                            }
                            
                        }
                    }
                    lst.Add(model);
                }
                return lst;
            }
            catch (Exception ex)
            {

                throw ex;
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
