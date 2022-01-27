# Alaska Airlines .NET Interview Exercise

Welcome to the Alaska Airlines .NET coding exercise!

## Getting Started

### Prerequisites

* Install [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet)
* Install [Visual Studio Code](https://code.visualstudio.com/)
* Install the [C# Dev Kit extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) for VS Code

### Initial Setup

1. Create a [GitHub account](https://github.com/) if you do not have one already
2. Fork this repository
3. Open the project folder in Visual Studio Code
4. Press **F5** to run the application in debug mode
5. A swagger page should automatically open in your browser with two health endpoints. Both should work if things are configured correctly.

### VS Code Tips

* **F5**: Start debugging the application
* **Ctrl+Shift+P**: Open command palette (useful for running .NET commands)
* **Ctrl+`**: Open/close the integrated terminal
* Use the **Run and Debug** panel (Ctrl+Shift+D) for debugging controls
* The **Problems** panel will show build errors and warnings

## Prompt

Create an endpoint that takes a station id as an input and retrieves the latest weather information for that station.

### Acceptance Criteria

* Use the [National Weather Service API](https://www.weather.gov/documentation/services-web-api#/default/station_observation_list) to gather weather information.
* The Station List can be retrieved from: https://api.weather.gov/stations?limit=100
    * Station ID can be found at: `features[x].properties.stationIdentifier`
* Current weather information can be retrieved from: https://api.weather.gov/stations/{StationID}/observations?limit=1

### Keys to Success

Here are a few things to keep in mind as you work through this exercise:

* The code provided is not perfect! We value a "refactor mentality" so don't feel like you should leave the existing skeleton as is.
* Try to exercise quality software engineering practices such as separation of concerns and test automation.
* Please check in early and check in often. We'd like to use your git history to see your process.

