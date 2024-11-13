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
        public OpenAIController(AzureOpenAIClient azureopenaiclient)
        {
            _client = azureopenaiclient;
        }

        [HttpGet("generate-query")]
        public async Task<IActionResult> GenerateQuery([FromQuery] string prompt)
        {
            if(string.IsNullOrEmpty(prompt))
            {
                return BadRequest("Prompt is required.");
            }

            try
            {
                _messages = new List<ChatMessage>
                {
                    
                };
                _messages.Add(ChatMessage.CreateSystemMessage(new ChatMessageContent("You are an assistant helping to filter data from the 'Trades' Table." +
                    " The table has the following columns : TradeId (long), TradeDate (datetime) , SettlementDate (datetime), SourceTradeId (long) , SourceTradeSystem (nvarchar) , Trader(nvarchar), Quantity (decimal) , Fund (nvarchar) , Security(nvarchar)" +
                "Your response should only contain SQL statement and no english part")));

                
                   //"Do not give any results if you do not understand the entity other than Trades.Just retrun 'NAN'"


                _messages.Add(prompt);
                var chatCpmpletionOptions = new ChatCompletionOptions
                {

                };

                chatCpmpletionOptions.ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat();
                var completion = await _client.GetChatClient("gpt-35-turbo").CompleteChatAsync(_messages);//, chatCpmpletionOptions);

                //var assistanceResponse = completion.Result.Value.Content;
                return Ok(completion.Value.Content[0].Text);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

