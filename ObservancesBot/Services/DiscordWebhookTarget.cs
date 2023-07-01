using Discord;
using Discord.Webhook;
using Foxite.Text;

namespace ObservancesBot;

public class DiscordWebhookTarget : Target {
	private readonly ITextFormatter m_Formatter;
	private readonly List<DiscordWebhookClient> m_WebhookClients;
	private readonly bool m_UseFields;

	public DiscordWebhookTarget(string webhookUrl, bool useFields) {
		m_Formatter = ModularTextFormatter.Markdown();
		m_WebhookClients = webhookUrl.Split(';').Select(url => new DiscordWebhookClient(url)).ToList();
		m_UseFields = useFields;
	}

	public async override Task Send(Observances observances) {
		var fields = new List<EmbedFieldBuilder>();
		string description;

		if (m_UseFields) {
			description = "Good morning! Around the world, these celebrations will happen today:";
			
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
		} else {
			string RenderDescription(IReadOnlyList<IText> items) {
				return m_Formatter.Format(new CompositeText(new LiteralText("Good morning! Around the world, these celebrations will happen today:\n"), new ListText(true, items)));
			}

			List<IText> items = new List<IText>();

			bool ExceedsLimit() {
				const int maxEmbedDescriptionLength = 4096;
				return RenderDescription(items).Length > maxEmbedDescriptionLength;
			}

			items.AddRange(observances.Items);

			IText andMoreText = new CompositeText(new LiteralText("And more - "), new LinkText(observances.SourceUri, new LiteralText("check the source!")));

			bool notFirstIteration = false;
			while (ExceedsLimit()) {
				if (notFirstIteration) {
					// Remove "and more" text
					items.RemoveAt(items.Count - 1);
				}

				items.RemoveAt(items.Count - 1);
				items.Add(andMoreText);

				notFirstIteration = true;
			}

			description = RenderDescription(items);
		}

		Embed[] embeds = new[] {
			new EmbedBuilder()
				.WithTitle($"{observances.Date:MMMM} {observances.Date:dd}")
				.WithDescription(description)
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
