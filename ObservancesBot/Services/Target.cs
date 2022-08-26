using Discord;
using Discord.Webhook;
using Foxite.Text;

namespace ObservancesBot; 

public abstract class Target {
	public abstract Task Send(Observances observances1);
}

public class DiscordWebhookTarget : Target {
	private readonly DiscordTextFormatter m_Formatter;
	private readonly DiscordWebhookClient m_WebhookClient;

	public DiscordWebhookTarget() {
		string webhookUrl = Util.GetEnv("WEBHOOK_URL");
		
		m_Formatter = new DiscordTextFormatter();

		m_WebhookClient = new DiscordWebhookClient(webhookUrl);
	}

	public async override Task Send(Observances observances) {
		var fields = new List<EmbedFieldBuilder>();
		
		int i = 1;
		foreach (IText observance in observances.Items) {
			if (i >= 25) {
				fields.Add(new EmbedFieldBuilder() {
					Name = "And more",
					Value = m_Formatter.Format(new LinkText(observances.SourceUri, new LiteralText("Check the source!")))
				});
			} else {
				fields.Add(new EmbedFieldBuilder() {
					Name = i.ToString(),
					Value = m_Formatter.Format(observance)
				});
			}

			i++;
		}

		await m_WebhookClient.SendMessageAsync(embeds: new[] {
			new EmbedBuilder()
				.WithTitle($"{observances.Date:MMMM} {observances.Date:dd}")
				.WithDescription("Good morning! Around the world, these celebrations will happen today:")
				.WithFooter($"Source: {observances.SourceName}")
				.WithUrl(observances.SourceUri.ToString())
				.WithFields(fields)
				.Build()
		});
	}
}