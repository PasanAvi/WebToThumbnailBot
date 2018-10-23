using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using WebToThumbnailBot.Controllers;

namespace WebToThumbnailBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
               ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                await ProcessResponse(connector, activity);
               
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
        public bool checkUri(string uri, out string exMsg)
        {
            exMsg = string.Empty;
            try
            {
                using(var client = new WebClient())
                {
                    using(var stream = client.OpenRead(uri))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                exMsg = ex.Message;
                return false;  
            }
        }
        public bool IsvalidUri(string uriName, out string exMsg)
        {
            uriName = uriName.ToLower().Replace(Thumbnails.Chttps, string.Empty);
            uriName = (uriName.ToLower().Contains(Thumbnails.Chttp)) ? uriName : Thumbnails.Chttp + uriName;
            return checkUri(uriName, out exMsg);
        }
        private async Task ProcessResponse(ConnectorClient connector, Activity input)
        {
            Activity reply = null;
            string exMsg = string.Empty;
            if (IsvalidUri(input.Text,out exMsg))
            {
                await Thumbnails.ProcessScreenshots(connector, input);
            }
            else
            {
                reply = input.CreateReply("Hii, What is the URL you want a thumbnail for..?");

                await connector.Conversations.ReplyToActivityAsync(reply);
            }
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}