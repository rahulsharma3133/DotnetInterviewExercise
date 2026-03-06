using System.Threading.Tasks;
using DotnetInterviewExercise.Models;

namespace DotnetInterviewExercise.Services
{
    public interface IWeatherService
    {
        Task<string> GetActiveAlertsStatusAsync();
        Task<WeatherResponse> GetWeatherByStationNameAsync(string stationName);
    }
}