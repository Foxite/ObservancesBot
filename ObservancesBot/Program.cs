using System.Net.Http.Headers;
using Foxite.Text;
using ObservancesBot;

string sourceName = Util.GetEnv("SOURCE", "wikipedia");
string targetName = Util.GetEnv("TARGET");

var http = new HttpClient() {
	DefaultRequestHeaders = {
		UserAgent = {
			new ProductInfoHeaderValue("ObservancesBot", "0.2"),
			new ProductInfoHeaderValue("(https://github.com/Foxite/ObservancesBot)")
		}
	}
};

Target target = targetName switch {
	"discord" => new DiscordWebhookTarget(),
	"csv" => new SaveToCsvTarget(Util.GetEnv("CSV_PATH")),
};

ObservanceService observanceService = sourceName switch {
	"wikipedia" => new WikipediaObservanceService(http),
	"daysoftheyear.com" => new DaysOfTheYearDotComObservanceService(http),
	"csv" => new CsvObservanceService(Util.GetEnv("CSV_PATH"), Util.GetEnv("SOURCE_NAME"), Util.GetEnv("SOURCE_URI_FORMAT")),
};

async Task SendAllObservances() {
	for (var date = new DateTime(2020, 01, 01); date.Year == 2020; date = date.AddDays(1)) {
		Console.WriteLine(date.ToString("yyyy-MM-dd"));
		IReadOnlyCollection<IText>? observances = await observanceService.GetObservances(date);

		if (observances == null) {
			Console.WriteLine("null");
		} else {
			await target.Send(new Observances(observances, date, observanceService.GetSourceUri(date), observanceService.Name));
		}
	}
}

async Task SendTodaysObservances() {
	DateTime date = DateTime.Today;
	IReadOnlyCollection<IText>? observances = await observanceService.GetObservances(date);
	if (observances == null) {
		Console.WriteLine("null");
	} else {
		await target.Send(new Observances(observances, date, observanceService.GetSourceUri(date), observanceService.Name));
	}
}

await SendTodaysObservances();

// ReSharper disable once SuspiciousTypeConversion.Global
if (observanceService is IDisposable disposableService) {
	disposableService.Dispose();
}

if (target is IDisposable disposableTarget) {
	disposableTarget.Dispose();
}
