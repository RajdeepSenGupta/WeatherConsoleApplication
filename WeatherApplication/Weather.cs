using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApplication
{
    public class Weather
    {
        public string Location { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Humidity { get; set; }
        public string Wind { get; set; }
        public string Temperature { get; set; }
        public string Status { get; set; }
    }
}
