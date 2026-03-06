using System.Collections.Generic;

namespace DotnetInterviewExercise.Models
{
    public class ObservationResponse
    {
        public List<ObservationFeature> Features { get; set; }
    }

    public class ObservationFeature
    {
        public ObservationProperties Properties { get; set; }
    }

    public class ObservationProperties
    {
        public string Timestamp { get; set; }
        public TemperatureValue Temperature { get; set; }
        public string TextDescription { get; set; }
        public TemperatureValue Dewpoint { get; set; }
        public TemperatureValue WindSpeed { get; set; }
        public TemperatureValue WindDirection { get; set; }
        public TemperatureValue BarometricPressure { get; set; }
        public TemperatureValue RelativeHumidity { get; set; }
    }

    public class TemperatureValue
    {
        public double? Value { get; set; }
        public string UnitCode { get; set; }
    }
}