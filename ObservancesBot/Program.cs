using System.Net.Http.Headers;
using DSharpPlus;
using DSharpPlus.Entities;
using Foxite.Text;
using ObservancesBot;

var observanceService = new WikipediaObservanceService(new HttpClient() {
	DefaultRequestHeaders = {
		UserAgent = {
			new ProductInfoHeaderValue("ObservancesBot", "0.1"),
			new ProductInfoHeaderValue("(https://github.com/Foxite/ObservancesBot)"),
		}
	}
});

DateTime date = DateTime.Today;

IReadOnlyCollection<IText>? observances = await observanceService.GetObservances(date);


if (observances == null) {
	Console.WriteLine("null");
} else {
	var discordFormatter = new DiscordTextFormatter();

	var webhookClient = new DiscordWebhookClient();
	DiscordWebhook webhook = await webhookClient.AddWebhookAsync(new Uri(Environment.GetEnvironmentVariable("WEBHOOK_URL")));

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
