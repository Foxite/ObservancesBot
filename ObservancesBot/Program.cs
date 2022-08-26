using System.Net.Http.Headers;
using Foxite.Text;
using ObservancesBot;

string source = Util.GetEnv("SOURCE", "wikipedia");

var http = new HttpClient() {
	DefaultRequestHeaders = {
		UserAgent = {
			new ProductInfoHeaderValue("ObservancesBot", "0.2"),
			new ProductInfoHeaderValue("(https://github.com/Foxite/ObservancesBot)")
		}
	}
};

Target target = new DiscordWebhookTarget();
ObservanceService observanceService = source switch {
	"wikipedia" => new WikipediaObservanceService(http),
	"daysoftheyear.com" => new DaysOfTheYearDotComObservanceService(http)
};


DateTime date = DateTime.Today;
IReadOnlyCollection<IText>? observances = await observanceService.GetObservances(date);

if (observances == null) {
	Console.WriteLine("null");
} else {
	await target.Send(new Observances(observances, date, observanceService.GetSourceUri(date), observanceService.Name));
}
