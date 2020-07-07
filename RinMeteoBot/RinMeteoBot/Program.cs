using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Xml.Linq;
using System.Net;
using Newtonsoft.Json.Linq;

namespace RinMeteoBot
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Telebot

            int update_id = 0;
            string messagFronId = "";
            string messageText = "";
            string firstName = "";
            string token = "448565330:AAFMb8-t98c2GWNh2wM392x3EjhNtYLnlTg";

            WebClient webClient = new WebClient();

            string startUrl = $"https://api.telegram.org/bot{token}";

            while (true)
            {
                string url = $"{startUrl}/getUpdates?offset={update_id + 1}";
                string response = webClient.DownloadString(url);

                var Messages = JObject.Parse(response)["result"].ToArray();

                foreach (var currentMessage in Messages)
                {
                    update_id = Convert.ToInt32(currentMessage["update_id"]);
                    try
                    {

                        firstName = currentMessage["message"]["from"]["first_name"].ToString();
                        messagFronId = currentMessage["message"]["from"]["id"].ToString();
                        messageText = currentMessage["message"]["text"].ToString();

                        Console.WriteLine($"{firstName} {messagFronId} {messageText}");

                        //messageText = "Вы прислали " + messageText;
                        messageText = GetWeather(messageText);

                        //messageText += "\n\n http://t.me/teamgeek";

                        url = $"{startUrl}/sendMessage?chat_id={messagFronId}&text={messageText}";
                        webClient.DownloadString(url);
                    }

                    catch { }
                }
                Thread.Sleep(100); //Задрежка 0,1 сек.
            }

            #endregion

            //string text = GetWeather("Казань");
            //Console.WriteLine(text);
            //Console.ReadKey();

        }

        private static string GetWeather(string CityName)
        {
            string url = string.Empty;

            switch (CityName)
            {
                case "Казань":
                    {
                        url = @"https://xml.meteoservice.ru/export/gismeteo/point/486.xml";
                        break;
                    }
                case "Москва":
                    {
                        url = @"https://xml.meteoservice.ru/export/gismeteo/point/37.xml";
                        break;
                    }
                case "Уфа":
                    {
                        url = @"https://xml.meteoservice.ru/export/gismeteo/point/140.xml";
                        break;
                    }
                case "Красноярск":
                    {
                        url = @"https://xml.meteoservice.ru/export/gismeteo/point/146.xml";
                        break;
                    }
                case "Набережные челны":
                    {
                        url = @"https://xml.meteoservice.ru/export/gismeteo/point/12.xml";
                        break;
                    }
                case "Татарстан":
                    {
                        url = @"https://xml.meteoservice.ru/export/gismeteo/point/31608.xml";
                        break;
                    }
                default:
                    {
                        return "Выберете из следующих городов:\nКазань;\nМосква;\nУфа;\nКрасноярск;\nНабережные челны;\nТатарстан.\n Если желаете добавить свой город обращайтесь к разработчику телеграмм бота Заманову Ренату";
                    }
            }

            string xmlData = new WebClient().DownloadString(url);

            var xmlColItem = XDocument.Parse(xmlData)
                          .Descendants("MMWEATHER")
                          .Descendants("REPORT")
                          .Descendants("TOWN")
                          .Descendants("FORECAST").ToArray();

            string text = string.Empty;

            foreach (var item in xmlColItem)
            {
                text +=
                    $"Погода {item.Attribute("weekday").Value.Replace("2", "в понедельник").Replace("3", "во вторник").Replace("4", "в среду").Replace("5", "в четверг").Replace("6", "в пятницу").Replace("7", "в субботу").Replace("1", "в воскресенье")}" +
                    $" {item.Attribute("tod").Value.Replace("0", "ночью").Replace("1", "утром").Replace("2", "днем").Replace("3", "вечером")}" +
                    $" {item.Element("PHENOMENA").Attribute("precipitation").Value.Replace("4", "дождь").Replace("5", "ливень").Replace("6", "снег").Replace("7", "снег").Replace("8", "гроза").Replace("10", "без осадков")}." +
                    $" Атмосферное давление: от {item.Element("PRESSURE").Attribute("min").Value} до {item.Element("PRESSURE").Attribute("max").Value} мм.рт.ст." +
                    $" Температура воздуха от {item.Element("TEMPERATURE").Attribute("min").Value} до {item.Element("TEMPERATURE").Attribute("max").Value} °C." +
                    $" Ветер {item.Element("WIND").Attribute("direction").Value.Replace("0", "северный").Replace("0", "северный").Replace("1", "северо-восточный").Replace("2", "восточный").Replace("3", "юго-восточный").Replace("4", "южный").Replace("5", "юго-западный").Replace("6", "западный").Replace("7", "северо-западный")}" +
                    $" со скоростью от {item.Element("WIND").Attribute("min").Value} до {item.Element("WIND").Attribute("max").Value} м/с." +
                    $" Относительная влажность воздуха от {item.Element("RELWET").Attribute("min").Value} до {item.Element("RELWET").Attribute("max").Value} %" +
                    $"\n \n";
            }
            return (text);

        }
    }
}
