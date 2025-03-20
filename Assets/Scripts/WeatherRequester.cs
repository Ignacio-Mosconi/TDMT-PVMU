using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

namespace Class1
{
    public class WeatherRequester : MonoBehaviour
    {    
        [SerializeField] private TMP_InputField searchField;
        [SerializeField] private Button searchButton;
        [SerializeField] private TMP_Text cityNameText;
        [SerializeField] private TMP_Text temperatureText;
        [SerializeField] private TMP_Text humidityText;
        [SerializeField] private TMP_Text pressureText;
        [SerializeField] private string apiKey;


        void Start ()
        {
            searchButton.onClick.AddListener(RequestWeatherForecast);
        }

        void OnDestroy ()
        {
            searchButton.onClick.RemoveListener(RequestWeatherForecast);
        }


        private IEnumerator SendGetRequest<T> (string requestUrl, Action<T> onSuccess, Action<string> onError)
        {
            T response;

            UnityWebRequest webRequest = UnityWebRequest.Get(requestUrl);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                response = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onError?.Invoke(webRequest.error);
            }

            webRequest.Dispose();
        }

        private float KelvinToCelcius (float k) => k - 273.15f;

        private void RequestWeatherForecast ()
        {
            string cityName = searchField.text;
            string requestUrl = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={apiKey}";

            StartCoroutine(SendGetRequest<WeatherData>(requestUrl,
            onSuccess: (data) =>
            {
                cityNameText.text = cityName.ToUpper();
                temperatureText.text = KelvinToCelcius(data.main.temp).ToString() + "°C";
                humidityText.text = data.main.humidity + "%";
                pressureText.text = data.main.pressure + "HPa";
            },
            onError: (error) =>
            {
                Debug.LogError(error);
            }));
        }
    }
}