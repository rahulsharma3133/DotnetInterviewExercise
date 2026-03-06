namespace DotnetInterviewExercise.Models
{
    public class WeatherResponse
    {
        public string StationName { get; set; }
        public string StationId { get; set; }
        public string Timestamp { get; set; }
        public double? TemperatureCelsius { get; set; }
        public string Description { get; set; }
        public double? DewpointCelsius { get; set; }
        public double? WindSpeedKmh { get; set; }
        public double? WindDirectionDegrees { get; set; }
        public double? BarometricPressurePa { get; set; }
        public double? RelativeHumidityPercent { get; set; }
    }
}