using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace eStoreClient.Pages.MemberPage
{
    public class MemberModel : PageModel
    {
        private readonly HttpClient client;
        private readonly IConfiguration _configuration;
        private string MemberApiUrl = "";
        public List<Member> ListMember { get; set; }

        [BindProperty]
        public string? Keyword { get; set; }

        public MemberModel(IConfiguration configuration)
        {
            _configuration = configuration;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            MemberApiUrl = _configuration.GetValue<string>("DomainURL") + "Member/GetAllMember";

            // Khởi tạo ListMember
            ListMember = new List<Member>();
        }

        public async Task<IActionResult> OnGetAsync(string? keyword)
        {
            Keyword = keyword;
            string url = MemberApiUrl + "?keyword=" + keyword;
            if (keyword == null)
            {
                url = MemberApiUrl;
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
                List<Member> listMembers = JsonSerializer.Deserialize<List<Member>>(strData, options);

                // Nếu không có dữ liệu trả về, đảm bảo ListMember không null
                ListMember = listMembers ?? new List<Member>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Error: {ex.Message}");
                // Xử lý lỗi hoặc đưa ra thông báo cho người dùng
                ListMember = new List<Member>(); // Khởi tạo ListMember nếu có lỗi
            }

            return Page();
        }
    }
}
