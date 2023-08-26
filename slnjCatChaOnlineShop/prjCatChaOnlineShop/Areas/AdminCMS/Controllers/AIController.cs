using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using prjCatChaOnlineShop.Areas.AdminCMS.Models;

namespace prjCatChaOnlineShop.Areas.AdminCMS.Controllers
{
    public class AIController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GenerateArticle([FromForm] Dictionary<string, string> keywords)
        {
            string keyword1 = keywords.GetValueOrDefault("keyword1", "Keyword1");
            string keyword2 = keywords.GetValueOrDefault("keyword2", "Keyword2");
            string keyword3 = keywords.GetValueOrDefault("keyword3", "Keyword3");

            // 創建GPT-3請求內容
            var prompt = $"Write a 150-word article that includes the terms {keyword1}, {keyword2}, and {keyword3}.";
            var payload = new { prompt, max_tokens = 150 };

            // 發送到GPT-3 API
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "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");

            var response = await httpClient.PostAsync(
                "https://api.openai.com/v1/engines/davinci-codex/completions",
                new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
            );

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Gpt3Response>(responseBody);

                return Ok(new { article = result.choices[0].text.Trim() });
            }

            return BadRequest("Failed to generate article.");
        }
    }
}
