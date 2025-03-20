using System;

namespace Class1
{
    [Serializable]
    public class WeatherData
    {
        public MainWeatherData main;
    }

    [Serializable]
    public class MainWeatherData
    {
        public float temp;
        public float pressure;
        public float humidity;
    }
}