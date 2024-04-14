using AISummariseApplication.Models;
using AISummariseApplication.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using YoutubeTranscriptApi;
namespace AISummariseApplication.Controllers
{
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> _logger;
        private readonly MongoDBService _mongoDBService;

        public BlogController(ILogger<BlogController> logger, MongoDBService mongoDBService)
        {
            _logger = logger;
            _mongoDBService = mongoDBService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var GetSummaries = await GetAllSummaries();

                return View(GetSummaries);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the blog posts.");

            }

        }

        private async Task<List<BlogModel>> GetAllSummaries()
        {
            var Database = _mongoDBService.GetDatabase("AIBlogPosts");
            var Collections = Database.GetCollection<BlogModel>("Posts");
            var Posts = await Collections.Find(FilterDefinition<BlogModel>.Empty).ToListAsync();

            return Posts;
        }
        [HttpPost]
        public async Task<IActionResult> TurnVideoToSummary(string videoId)
        {
            var youTubeTranscriptApi = new YouTubeTranscriptApi();
            var transcriptItems = youTubeTranscriptApi.GetTranscript(videoId);

            // Create a string to hold the transcript
            StringBuilder transcriptBuilder = new StringBuilder();

            foreach (var item in transcriptItems)
            {
                string Text = item.Text.Replace("\r\n", "");
                transcriptBuilder.AppendLine($"{Text}");
            }
            var SummariseText = await SummaryFromGPT(transcriptBuilder.ToString());
            ViewData["VideoID"] = videoId;
            ViewData["Transcript"] = SummariseText;
            // Return the transcript as a JSON response
            return View();
        }
        [HttpPost]
        private async Task<string> SummaryFromGPT(string Text)
        {
            string Result = "";
            string APIKey = "";
            string endpoint = "https://api.openai.com/v1/chat/completions";
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
            new { role = "system", content = "You are going to be given text. Summarise is it to the best ability." },
            new { role = "user", content = Text }
        }
            };

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", APIKey);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                   

                    var chatResponse = JsonSerializer.Deserialize<ChatResponse>(jsonResponse);
                    Console.WriteLine(chatResponse.Choices[0].Message.Content);
                    // Check if Choices is null
                    if (chatResponse.Choices != null && chatResponse.Choices.Length > 0)
                    {
                        Result = chatResponse.Choices[0].Message.Content;
                    }
                    else
                    {
                        Console.WriteLine("Choices array is null or empty.");
                        Result = "Error: Choices array is null or empty.";
                    }
                }
            }

            return Result;
        }

        private class ChatResponse
        {
            [JsonPropertyName("choices")]
            public ChatChoice[] Choices { get; set; }
        }
        private class ChatChoice
        {
            [JsonPropertyName("message")]
            public ChatMessage Message { get; set; }
        }
        private class ChatMessage
        {
            [JsonPropertyName("content")]
            public string Content { get; set; }
        }

        public async Task<IActionResult> SavedSummary(string Transcript, string VideoName)
        {
            var database = _mongoDBService.GetDatabase("AIBlogPosts");
            var collection = database.GetCollection<BlogModel>("Posts");

            BlogModel documentBlog = new BlogModel
            {
                Title = new BsonString("Hello"),
                Id = ObjectId.GenerateNewId(),
                Summary = new BsonString(Transcript), // Use BsonString constructor
                URL = new BsonString(VideoName),
                Date = BsonDateTime.Create(DateTime.Now) // Use BsonDateTime.Create()
            };

            await collection.InsertOneAsync(documentBlog); // Use async version of InsertOne()

            return View();
        }

    }

}
