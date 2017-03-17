﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.IO;

namespace AcademyBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public class Result
        {
            public int status { get; set; }
            public string text { get; set; }
        }
        public class languages
        {
            public string name { get; set; }
            public string locale { get; set; }
            public string author { get; set; }
            public string url { get; set; }
            public int pagecount { get; set; }
        }
        public class AugurBook
        {
            public string _id { get; set; }
            public int id { get; set; }
            public languages[] langs { get; set; }
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;
                string answer = GetAugur();
                // return our reply to the user
                //Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
                Activity reply = activity.CreateReply(answer);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
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

        private string GetAugur()
        {
            string ListUrl = "http://augurbook.azurewebsites.net/books/v3";
            string AugurUrl = "http://augurbook.azurewebsites.net/books/v3/getaugur/{0}&{1}&{2}&{3}";

            //Отримуємо список книг
            string BookList = GetRequestBody(ListUrl);
            //Отримуємо десеріалізований об'єкт з списком книг
            JavaScriptSerializer js = new JavaScriptSerializer();
            AugurBook[] books = js.Deserialize<AugurBook[]>(BookList);

            //Гереруємо випадковий номер книгим, сторінки в книзі і рядка на сторінці
            Random rnd = new Random();
            int rand = rnd.Next(1, books.Length);
            int random_page = rnd.Next(1, books[rand].langs[0].pagecount);
            int random_row = rnd.Next(1, 45);

            //Формуємо Url для отримання пророцтва
            string GetAugurUrl = String.Format(AugurUrl, books[rand]._id, random_page, random_row, books[rand].langs[0].locale);
            string AugurResult = GetRequestBody(GetAugurUrl);

            //Отримуємо десеріалізований об'єкт з текстом пророцтва
            JavaScriptSerializer jsresult = new JavaScriptSerializer();
            Result augur_result = jsresult.Deserialize<Result>(AugurResult);

            string res_text = "";
            if (augur_result.status == 1)
            {
                res_text = "Авгур каже: " + Environment.NewLine + augur_result.text + Environment.NewLine + " " + Environment.NewLine + books[rand].langs[0].author + " - \"" + books[rand].langs[0].name + "\"";
            }
            else
            {
                res_text = "Sorry something went wrong. Please try again later";
            }
            return res_text;
        }

        private string GetRequestBody(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream resStream = response.GetResponseStream();

                StreamReader objReader = new StreamReader(resStream);

                string body = "";
                string sLine = "";
                int i = 0;

                while (sLine != null)
                {
                    i++;
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                        body = body + sLine;
                }
                return body;
            }
            catch (Exception err)
            {
                return null;
            }
        }
    }
}