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
                        url = @"http://www.eurometeo.ru/russia/tatarstan/kazan/export/xml/data/";
                        break;
                    }
                case "Москва":
                    {
                        url = @"http://www.eurometeo.ru/russia/moskva/export/xml/data/";
                        break;
                    }
                case "Уфа":
                    {
                        url = @"http://www.eurometeo.ru/russia/bashkortostan/ufa/export/xml/data/";
                        break;
                    }
                case "Красноярск":
                    {
                        url = @"http://www.eurometeo.ru/russia/krasnoyarskiy-kray/krasnoyarsk/export/xml/data/";
                        break;
                    }
                case "Набережные челны":
                    {
                        url = @"http://www.eurometeo.ru/russia/tatarstan/naberezhnyie-chelnyi/export/xml/data/";
                        break;
                    }
                case "Татарстан":
                    {
                        url = @"http://www.eurometeo.ru/russia/tatarstan/tukaevskiy-rayon/sovhoza-tatarstan/export/xml/data/";
                        break;
                    }
                default:
                    {
                        return "Выберете из следующих городов:\nКазань;\nМосква;\nУфа;\nКрасноярск;\nНабережные челны;\nТатарстан.\n Если желаете добавить свой город обращайтесь к разработчику телеграмм бота Заманову Ренату";
                    }
            }

            string xmlData = new WebClient().DownloadString(url);

            var xmlColItem = XDocument.Parse(xmlData)
                                      .Descendants("weather")
                                      .Descendants("city")
                                      .Descendants("step").ToArray();

            string text = string.Empty;

            foreach (var item in xmlColItem)
            {
                text +=
                    $"Погода: {item.Element("datetime").Value.Replace("04:00:00", "Утром").Replace("10:00:00", "Днем").Replace("16:00:00", "Вечером").Replace("22:00:00", "Ночью")} \n" +
                    $"Атмосферное давление в мм рт.столба: {item.Element("pressure").Value} \n" +
                    $"Температура воздуха в °C: {item.Element("temperature").Value} \n" +
                    $"Относительная влажность в %: {item.Element("humidity").Value} \n" +
                    $"Облачность в %: {item.Element("cloudcover").Value} \n" +
                    $"Скорость ветра в м/с: {item.Element("windspeed").Value} \n" +
                    $"Скорость порывов ветра в м/с: {item.Element("windgust").Value} \n" +
                    $"Осадки в мм за 3 часа: {item.Element("precipitation").Value} \n\n";
            }
            return (text);

        }
    }
}
