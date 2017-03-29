using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace AcademyBot
{
    public class LSAClass
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

            private void GetMatrix()
            {
                string full_text = GetAugurBook();
            }

            private string GetAugurBook()
            {
                string ListUrl =  "http://augurbook.azurewebsites.net/books/v3";
                string AugurUrl = "http://augurbook.azurewebsites.net/books/v3/getaugur/{0}&{1}&{2}&{3}";

                //Отримуємо список книг
                string BookList = GetRequestBody(ListUrl);
                //Отримуємо десеріалізований об'єкт з списком книг
                JavaScriptSerializer js = new JavaScriptSerializer();
                AugurBook[] books = js.Deserialize<AugurBook[]>(BookList);
            
                //Гереруємо випадковий номер книгим, сторінки в книзі і рядка на сторінці
                Random rnd = new Random();
                int rand = 1; //rnd.Next(1, books.Length);
               // int random_page = rnd.Next(1, books[rand].langs[0].pagecount);
                //int random_row = rnd.Next(1, 45);

                //Формуємо Url для отримання пророцтва
                //string GetAugurUrl = String.Format(AugurUrl, books[rand]._id, random_page, random_row, books[rand].langs[0].locale);
                string AugurResult = GetRequestBody(books[rand].langs[0].url);

                //Отримуємо десеріалізований об'єкт з текстом пророцтва
                //JavaScriptSerializer jsresult = new JavaScriptSerializer();
                //Result augur_result = jsresult.Deserialize<Result>(AugurResult);

                //string res_text = "";
                //if (augur_result.status == 1)
                //{
                //    res_text = "авгур каже: " + Environment.NewLine + augur_result.text + Environment.NewLine + " " + Environment.NewLine + books[rand].langs[0].author + " - \"" + books[rand].langs[0].name + "\"";
                //}
                //else
                //{
                //    res_text = "Sorry something went wrong. Please try again later";
                //}
                return AugurResult;
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

            private string[] ParseWrong(string full_text)
            {
                string[] mass = full_text.Split(' ');
                return mass;
            }
    }
}