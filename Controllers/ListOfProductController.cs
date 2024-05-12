using Microsoft.AspNetCore.Mvc;
using MultiTenantApps.Models;
using Newtonsoft.Json;
using System.Reflection;

namespace MultiTenantApps.Controllers
{
    public class ListOfProductController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public ListOfProductController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            List<int> lstOrder = null;

            
            List<Products> lstProduct = null;

            List<ListOfProduct> lst = null;

            List<OrderDetail> lstDetail = null;

            string tenantId = string.Empty;

            try
            {
                lstOrder = new List<int>();


                lstDetail = new List<OrderDetail>();

                lst = new List<ListOfProduct>();

                tenantId = HttpContext.Session.GetString("TenantId");

                lstOrder = await GetOrderByTenantId(tenantId);

                lstDetail = await  GetOrderDetailByTenantId(tenantId);

                foreach (var orderData in lstOrder)
                {

                    lstProduct = new List<Products>();

                    lstProduct = await GetProductById(orderData);
                                        

                    ListOfProduct model = new ListOfProduct();

                    foreach (var dataProd in lstDetail)
                    {
                        if (orderData == dataProd.ProductId)
                        {
                            model.ProductName = lstProduct[0].Name;
                            model.NoSold = model.NoSold + dataProd.Quantity;
                            model.NoOfOrder = model.NoOfOrder + 1;
                        }
                    }

                    lst.Add(model);



                    //}
                }

                    

                

                return View(lst);

            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, "Failed to retrieve orders from the API.");
                return View();
            }

            
        }

        private async Task<List<int>> GetOrderByTenantId(string TenantId)
        {
            List<Orders> lst = null;

            List<int> lstProductId = null;

            int id = 0;

            bool found = false;

            try
            {
                lst = new List<Orders>();

                lstProductId = new List<int>();

                var client = _clientFactory.CreateClient("BaseApiClient");

                var response = await client.GetAsync("api/orders");

                

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();

                    var dataItems = JsonConvert.DeserializeObject<List<Orders>>(responseData);

                    
                    lst = dataItems.Where(orders => orders.ShipAddress.Country == TenantId).ToList<Orders>();

                    foreach (var data in lst)
                    {
                        foreach (var detail in data.Details)
                        {
                            found = false;

                            foreach(var l in lstProductId)
                            {
                                if (detail.ProductId==l)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found==false)
                            {
                                lstProductId.Add(detail.ProductId);
                            }
                        }
                    }

                    


                    return lstProductId;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve orders from the API.");
                    
                }

                return lstProductId;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task<List<OrderDetail>> GetOrderDetailByTenantId(string TenantId)
        {
            List<Orders> lst = null;

            List<OrderDetail> lstDetail = null;

            try
            {
                lst = new List<Orders>();

                lstDetail = new List<OrderDetail>();


                var client = _clientFactory.CreateClient("BaseApiClient");

                var response = await client.GetAsync("api/orders");

                

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();

                    var dataItems = JsonConvert.DeserializeObject<List<Orders>>(responseData);


                    lst = dataItems.Where(orders => orders.ShipAddress.Country == TenantId).ToList<Orders>();

                    foreach (var data in lst)
                    {
                        foreach (var data1 in data.Details)
                        {
                            OrderDetail model = new OrderDetail();

                            model = data1;
                            lstDetail.Add(model);
                        }

                        
                    }

                    


                    return lstDetail;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve orders from the API.");

                }

                return lstDetail;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        private async Task<List<Products>> GetProductById(int ProductId)
        {
            List<Products> lst = null;
            try
            {
                lst = new List<Products>();

                var client = _clientFactory.CreateClient("BaseApiClient");

                var response = await client.GetAsync("api/products");



                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();


                    lst = JsonConvert.DeserializeObject<List<Products>>(responseData).Where(product =>
                            product.Id == ProductId).ToList();


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
