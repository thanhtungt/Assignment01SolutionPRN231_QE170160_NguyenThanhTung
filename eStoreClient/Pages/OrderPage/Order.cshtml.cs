using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace eStoreClient.Pages.OrderPage
{
    public class OrderModel : PageModel
    {
        private readonly HttpClient client;
        private readonly IConfiguration _configuration;
        private string OrderApiUrl = "";
        public List<Order> ListOrder { get; set; }

        [BindProperty]
        public string? Keyword { get; set; }

        public OrderModel(IConfiguration configuration)
        {
            _configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            OrderApiUrl = _configuration.GetValue<string>("DomainURL") + "Order/GetAllOrder";

            // Khởi tạo ListOrder
            ListOrder = new List<Order>();
        }

        public async Task<IActionResult> OnGetAsync(string? keyword)
        {
            Keyword = keyword;
            string url = OrderApiUrl + "?keyword=" + keyword;
            if (keyword == null)
            {
                url = OrderApiUrl;
            }

            HttpResponseMessage resp = await client.GetAsync(url);

            // Kiểm tra mã trạng thái của phản hồi
            if (!resp.IsSuccessStatusCode)
            {
                // Ghi lại mã trạng thái và lý do
                Console.WriteLine($"Error: {resp.StatusCode} - {resp.ReasonPhrase}");
                return Page(); // Hoặc xử lý theo cách bạn muốn
            }

            var strData = await resp.Content.ReadAsStringAsync();
            Console.WriteLine(strData); // Ghi lại giá trị của strData

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            try
            {
                // Phân tích cú pháp JSON
                List<Order> listOrders = JsonSerializer.Deserialize<List<Order>>(strData, options);

                // Nếu không có dữ liệu trả về, đảm bảo ListOrder không null
                ListOrder = listOrders ?? new List<Order>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Error: {ex.Message}");
                // Xử lý lỗi hoặc đưa ra thông báo cho người dùng
                ListOrder = new List<Order>(); // Khởi tạo ListOrder nếu có lỗi
            }

            return Page();
        }
    }
}
