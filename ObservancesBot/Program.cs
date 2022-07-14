using System.Net.Http.Headers;
using ObservancesBot;

var observanceService = new WikipediaObservanceService(new HttpClient() {
	DefaultRequestHeaders = {
		UserAgent = {
			new ProductInfoHeaderValue("ObservancesBot", "0.1"),
			new ProductInfoHeaderValue("(https://github.com/Foxite/ObservancesBot)"),
		}
	}
});

string[]? observances = await observanceService.GetObservances(DateTime.Today);
if (observances == null) {
	Console.WriteLine("null");
} else {
	foreach (string observance in observances) {
		Console.WriteLine($"- {observance}");
	}
}
