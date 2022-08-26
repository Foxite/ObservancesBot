using System.Net.Http.Headers;
using DSharpPlus;
using DSharpPlus.Entities;
using Foxite.Text;
using ObservancesBot;

string GetEnv(string name, string? defaultValue = null) {
	return Environment.GetEnvironmentVariable(name) ?? defaultValue ?? throw new ArgumentNullException(nameof(name), "Missing environment variable: " + name);
}

string webhookUrl = GetEnv("WEBHOOK_URL");
string source = GetEnv("SOURCE", "wikipedia");

var http = new HttpClient() {
	DefaultRequestHeaders = {
		UserAgent = {
			new ProductInfoHeaderValue("ObservancesBot", "0.2"),
			new ProductInfoHeaderValue("(https://github.com/Foxite/ObservancesBot)")
		}
	}
};

ObservanceService observanceService = source switch {
	"wikipedia" => new WikipediaObservanceService(http),
	"daysoftheyear.com" => new DaysOfTheYearDotComObservanceService(http)
};

DateTime date = DateTime.Today;

IReadOnlyCollection<IText>? observances = await observanceService.GetObservances(date);


if (observances == null) {
	Console.WriteLine("null");
} else {
	var discordFormatter = new DiscordTextFormatter();

	var webhookClient = new DiscordWebhookClient();
	DiscordWebhook webhook = await webhookClient.AddWebhookAsync(new Uri(webhookUrl));

	var webhookBuilder = new DiscordWebhookBuilder();

	var embedBuilder = new DiscordEmbedBuilder()
		.WithTitle($"{date:MMMM} {date:dd}")
		.WithDescription("Good morning! Around the world, these celebrations will happen today:")
		.WithFooter($"Source: {observanceService.Name}")
		.WithUrl(observanceService.GetSourceUri(date));

	int i = 0;
	foreach (IText observance in observances) {
		embedBuilder.AddField((++i).ToString(), discordFormatter.Format(observance));
	}
	
	webhookBuilder.AddEmbed(embedBuilder);
	
	await webhook.ExecuteAsync(webhookBuilder);
}
