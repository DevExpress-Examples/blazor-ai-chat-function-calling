using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace DXBlazorChatFunctionCalling.Semantic.Services {
    public class WeatherPlugin {
        [KernelFunction]
        [Description("Gets the current weather in the city")]
        public static string GetWeather([Description("The name of the city")] string city) {
            switch(city) {
                case "Los Angeles":
                case "LA":
                    return GetTemperatureValue(20);
                case "London":
                    return GetTemperatureValue(15);
                default:
                    return $"The information about the weather in {city} is not available.";
            }
        }
        static string GetTemperatureValue(int value)
        {
            var valueInFahrenheits = value * 9 / 5 + 32;
            return $"{valueInFahrenheits}\u00b0F ({value}\u00b0C)";
        }
    }
}
