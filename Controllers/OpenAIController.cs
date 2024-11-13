using System;
using Azure.AI.OpenAI;
using Azure.AI.OpenAI.Chat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;

namespace Barclays.GenAIHackathon.OpenAIWrapper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OpenAIController : ControllerBase
    {
        private readonly AzureOpenAIClient _client;
        private List<ChatMessage> _messages;
        private readonly HttpClient _httpClient;
        string schemaApiUrl = "http://localhost:5252/api/DatabaseSchema/TM_Trade";
        public OpenAIController(AzureOpenAIClient azureopenaiclient, HttpClient httpClient)
        {
            _client = azureopenaiclient;
            _httpClient = httpClient;
        }

        [HttpGet("generate-query")]
        public async Task<IActionResult> Get([FromQuery] string prompt)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                return BadRequest("Prompt is required.");
            }

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(schemaApiUrl);
                string responseData = string.Empty;
                // Check if the response is successful (2xx status code)
                if (response.IsSuccessStatusCode)
                {
                    responseData = await response.Content.ReadAsStringAsync();
                }
                _messages = new List<ChatMessage>
                {

                };
                _messages.Add(ChatMessage.CreateSystemMessage(new ChatMessageContent(responseData)));


                //"Do not give any results if you do not understand the entity other than Trades.Just retrun 'NAN'"


                _messages.Add(prompt);
                var chatCpmpletionOptions = new ChatCompletionOptions
                {

                };

                chatCpmpletionOptions.ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat();
                var completion = await _client.GetChatClient("gpt-4o").CompleteChatAsync(_messages);//, chatCpmpletionOptions);

                DatabaseReadController db = new DatabaseReadController();
                var dataresult = await db.Get(completion.Value.Content[0].Text.Trim('`').Replace("sql", string.Empty).Replace("\n", " ").Trim());
                //var assistanceResponse = completion.Result.Value.Content;
                return Ok(dataresult);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

