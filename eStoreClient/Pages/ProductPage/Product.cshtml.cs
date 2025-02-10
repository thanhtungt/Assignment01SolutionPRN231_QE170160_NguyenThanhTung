using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using BusinessObjects.Models;

namespace eStoreClient.Pages.ProductPage
{
    public class ProductModel : PageModel
    {
        private readonly HttpClient client;
        private readonly IConfiguration _configuration;
        private string ProductApiUrl = "";
        public List<Product> ListProduct { get; set; }

        [BindProperty]
        public string? Keyword { get; set; }
        [BindProperty]
        public string? Price { get; set; }

        public ProductModel(IConfiguration configuration)
        {
            _configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = _configuration.GetValue<string>("DomainURL") + "Product/GetAllProduct";

            // Khởi tạo ListProduct
            ListProduct = new List<Product>();
        }

        public async Task<IActionResult> OnGetAsync(string? keyword, string? price)
        {
            Keyword = keyword;
            Price = price;

            string url = ProductApiUrl + "?keyword=" + keyword + "&unitP=" + price;
            if (keyword == null && price == null)
            {
                url = ProductApiUrl;
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
                List<Product> listProducts = JsonSerializer.Deserialize<List<Product>>(strData, options);

                // Nếu không có dữ liệu trả về, đảm bảo ListProduct không null
                ListProduct = listProducts ?? new List<Product>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Error: {ex.Message}");
                // Xử lý lỗi hoặc đưa ra thông báo cho người dùng
                ListProduct = new List<Product>(); // Khởi tạo ListProduct nếu có lỗi
            }

            return Page();
        }
    }
}
