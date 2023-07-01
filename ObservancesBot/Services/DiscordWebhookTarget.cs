using Discord;
using Discord.Webhook;
using Foxite.Text;

namespace ObservancesBot;

public class DiscordWebhookTarget : Target {
	private readonly ITextFormatter m_Formatter;
	private readonly List<DiscordWebhookClient> m_WebhookClients;

	public DiscordWebhookTarget() {
		string webhookUrl = Util.GetEnv("WEBHOOK_URL");
		
		m_Formatter = ModularTextFormatter.Markdown();

		m_WebhookClients = webhookUrl.Split(';').Select(url => new DiscordWebhookClient(url)).ToList();
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
				break;
			} else {
				fields.Add(new EmbedFieldBuilder() {
					Name = i.ToString(),
					Value = m_Formatter.Format(observance)
				});
			}

			i++;
		}

		Embed[] embeds = new[] {
			new EmbedBuilder()
				.WithTitle($"{observances.Date:MMMM} {observances.Date:dd}")
				.WithDescription("Good morning! Around the world, these celebrations will happen today:")
				.WithFooter($"Source: {observances.SourceName}")
				.WithUrl(observances.SourceUri.ToString())
				.WithFields(fields)
				.Build()
		};
		
		foreach (DiscordWebhookClient webhook in m_WebhookClients) {
			await webhook.SendMessageAsync(embeds: embeds);
		}
	}
}
