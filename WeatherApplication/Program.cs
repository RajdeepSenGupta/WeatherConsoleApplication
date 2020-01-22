using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace WeatherApplication
{
    static class Program
    {
        public static void Main(string[] args)
        {
            string appId = "0ff0028b695d833b6817c6fdef55146c";
            Weather weather = new Weather();

            do
            {
                string api = "http://api.openweathermap.org/data/2.5/weather?q={0}&APPID={1}";
                HttpClient client = new HttpClient();

                // Take Input
                Console.Write("Location Name/Coordinate/ZipCode: ");
                string location = Console.ReadLine();

                if (location.ToLower(CultureInfo.InvariantCulture).Equals("clear"))
                {
                    Console.Clear();
                }
                else if(location.ToLower(CultureInfo.InvariantCulture).Equals("exit"))
                {
                    return;
                }
                else
                {
                    api = String.Format(api, location, appId);

                    // Connect to http client
                    client.BaseAddress = new Uri(api);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Get weather details
                    HttpResponseMessage response = client.GetAsync(api).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var weatherObj = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                        // Get Coordinates
                        Coordinates coord = GetCoOrdinates(weatherObj);
                        weather.Latitude = coord.Latitude;
                        weather.Longitude = coord.Longitude;

                        // Get Humidity (%)
                        weather.Humidity = GetHumidity(weatherObj);

                        // Get Location Name
                        weather.Location = weatherObj.SelectToken("name").ToString() + ", "
                            + weatherObj.SelectToken("sys").SelectToken("country").ToString();

                        // Get Wind Speed (km/h)
                        weather.Wind = GetWindSpeed(weatherObj);

                        // Get Temperature (°C)
                        weather.Temperature = GetTemparature(weatherObj);

                        weather.Status = GetStatus(weatherObj);
                    }

                    PrintWeather(weather);
                }
            } while (true);
        }
        
        // Get Coordinates
        public static Coordinates GetCoOrdinates(JToken weatherObj)
        {
            var coord = weatherObj.SelectToken("coord");
            string latitude = JObject.Parse(coord.ToString()).SelectToken("lat").ToString();
            string longitude = JObject.Parse(coord.ToString()).SelectToken("lon").ToString();

            return new Coordinates()
            {
                Latitude = latitude,
                Longitude = longitude
            };
        }

        // Get Humidity
        public static string GetHumidity(JToken weatherObj)
        {
            var main = weatherObj.SelectToken("main");
            return JObject.Parse(main.ToString()).SelectToken("humidity").ToString() + "%";
        }

        // Get Wind Speed
        public static string GetWindSpeed(JToken weatherObj)
        {
            var wind = weatherObj.SelectToken("wind");
            double windSpeed = Convert.ToDouble(JObject.Parse(wind.ToString()).SelectToken("speed"));
            return (windSpeed * 3.6).ToString() + " km/h";
        }

        // Get Temperature
        public static string GetTemparature(JToken weatherObj)
        {
            var main = weatherObj.SelectToken("main");
            double temp = Convert.ToDouble(JObject.Parse(main.ToString()).SelectToken("temp")) - 273.15;
            return temp.ToString() + " °C";
        }

        // Get Status
        public static string GetStatus(JToken weatherObj)
        {
            var weather = weatherObj.SelectToken("weather");
            
            return JObject.Parse(weather[0].ToString()).SelectToken("main").ToString() + '/'
                + JObject.Parse(weather[0].ToString()).SelectToken("description").ToString();
        }

        // Print Weather
        public static void PrintWeather(Weather weather)
        {
            Console.WriteLine();
            Console.WriteLine("Location: " + weather.Location);
            Console.WriteLine("Coordinate: \n\t Latitude - " + weather.Latitude + "  \n\t Longitude - " + weather.Longitude);
            Console.WriteLine("Status: " + weather.Status);
            Console.WriteLine("Temperature: " + weather.Temperature);
            Console.WriteLine("Humidity: " + weather.Humidity);
            Console.WriteLine("Wind: " + weather.Wind);
            Console.WriteLine("\nUse the command 'clear' for clearing the console...");
            Console.WriteLine("Use the command 'exit' for exiting...\n\n");
        }
    }
}
