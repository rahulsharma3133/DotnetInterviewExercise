using System.Collections.Generic;

namespace DotnetInterviewExercise.Models
{
    public class StationListResponse
    {
        public List<StationFeature> Features { get; set; }
    }

    public class StationFeature
    {
        public StationProperties Properties { get; set; }
    }

    public class StationProperties
    {
        public string StationIdentifier { get; set; }
        public string Name { get; set; }
    }
}