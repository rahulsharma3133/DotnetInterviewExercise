using System.Threading.Tasks;

namespace DotnetInterviewExercise.Services
{
    public interface IWeatherService
    {
        Task<string> GetActiveAlertsStatusAsync();
    }
}