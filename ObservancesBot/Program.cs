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

Source source = sourceName switch {
	"wikipedia" => new WikipediaSource(http),
	"daysoftheyear.com" => new DaysOfTheYearDotComSource(http),
	"csv" => new CsvSource(Util.GetEnv("CSV_PATH"), Util.GetEnv("SOURCE_NAME"), Util.GetEnv("SOURCE_URI_FORMAT")),
	"ics" => new IcsSource(http, Util.GetEnv("ICS_PATH"), Util.GetEnv("SOURCE_NAME"), Util.GetEnv("SOURCE_URI_FORMAT")),
};

Target target = targetName switch {
	"discord" => new DiscordWebhookTarget(Util.GetEnv("WEBHOOK_URL"), Util.GetEnv("DISCORD_USE_FIELDS", "true") == "true"),
	"csv" => new SaveToCsvTarget(Util.GetEnv("CSV_PATH")),
};

bool onlyToday = string.IsNullOrEmpty(Util.GetEnv("ENUMERATE_ALL", ""));

async Task SendAllObservances() {
	const int year = 2020;
	for (var date = new DateTime(year, 1, 1); date.Year == year; date = date.AddDays(1)) {
		Console.WriteLine(date.ToString("yyyy-MM-dd"));
		IReadOnlyCollection<IText>? observances = await source.GetObservances(date);

		if (observances == null) {
			Console.WriteLine("null");
		} else {
			await target.Send(new Observances(observances, date, source.GetSourceUri(date), source.Name));
		}
	}
}

async Task SendTodaysObservances() {
	string mockDate = Util.GetEnv("MOCK_DATE", "");
	DateTime date = string.IsNullOrWhiteSpace(mockDate) ? DateTime.Today : DateTime.Parse(mockDate);
	IReadOnlyCollection<IText>? observances = await source.GetObservances(date);
	if (observances == null) {
		Console.WriteLine("null");
	} else {
		await target.Send(new Observances(observances, date, source.GetSourceUri(date), source.Name));
	}
}

if (onlyToday) {
	await SendTodaysObservances();
} else {
	await SendAllObservances();
}

// ReSharper disable once SuspiciousTypeConversion.Global
if (source is IDisposable disposableSource) {
	disposableSource.Dispose();
}

if (target is IDisposable disposableTarget) {
	disposableTarget.Dispose();
}
