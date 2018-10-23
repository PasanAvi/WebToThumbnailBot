using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Connector;
using RestSharp;

namespace WebToThumbnailBot.Controllers
{
    public class Thumbnails
    {
        public const string Chttp = "http://";
        public const string Chttps = "https://";
        public const string CthumbApi = "http://api.webthumbnail.org/?";
        public const string CapiParams = "width=1024&height=1024&screen=1024&url=";

        public static async Task ProcessScreenshots(ConnectorClient connector,Activity msg)
        {
            Activity reply = msg.CreateReply("Processing : "+ msg.Text);
            await connector.Conversations.ReplyToActivityAsync(reply);

            await Task.Run(async () =>
            {
                string imgUrl = GetThumbnail(msg.Text);
                reply = CreateResponceCard(msg, imgUrl);

                await connector.Conversations.ReplyToActivityAsync(reply);
            });
        }
        public static string GetThumbnail(string url)
        {
            string r = CapiParams + url;
            RestClient restClient = new RestClient(CthumbApi);
            RestRequest restRequest = new RestRequest(r, Method.GET);

            IRestResponse restResponse = restClient.Execute(restRequest);

            return CthumbApi+r;
        }
        public static Activity CreateResponceCard(Activity msg,string imageurl)
        {
            Activity reply = msg.CreateReply(imageurl);
            reply.Recipient = msg.From;
            reply.Type = "message";
            reply.Attachments = new List<Attachment>();

            List<CardImage> cardImage = new List<CardImage>();
            cardImage.Add(new CardImage(imageurl));

            ThumbnailCard thumbnailCard = new ThumbnailCard()
            {
                Subtitle = msg.Text,
                Images = cardImage
            };
            Attachment attachment = thumbnailCard.ToAttachment();
            reply.Attachments.Add(attachment);
            return reply;
        } 
    }
}