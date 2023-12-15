using Foxite.Text;
using Foxite.Text.Parsers;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using ObservancesBot;

public class IcsSource : Source {
	private readonly HttpClient m_Http;
	private readonly string m_IcsPath;
	private readonly string m_SourceUriFormat;
	private readonly MarkdownParser m_MarkdownParser;

	public override string Name { get; }
	
	public IcsSource(HttpClient http, string icsPath, string name, string sourceUriFormat) {
		Name = name;
		m_Http = http;
		m_IcsPath = icsPath;
		m_SourceUriFormat = sourceUriFormat;
		m_MarkdownParser = new MarkdownParser();
	}

	public override Uri GetSourceUri(DateTime date) {
		return new Uri(string.Format(m_SourceUriFormat, date));
	}
	
	public async override Task<IReadOnlyCollection<IText>?> GetObservances(DateTime date) {
		Calendar calendar;
		
		await using (Stream icsStream = await OpenIcsFile()) {
			calendar = Calendar.Load(icsStream);
		}
		
		HashSet<Occurrence> occurrences = calendar.GetOccurrences(date.AddSeconds(-1), date.AddSeconds(1)); // No, i don't know why it's that way
		List<IText> observances = occurrences
			.Select(occurrence => occurrence.Source)
			.Cast<CalendarEvent>()
			.Select(calevent => m_MarkdownParser.Parse(calevent.Summary))
			.ToList();

		return observances;
	}

	private async Task<Stream> OpenIcsFile() {
		var icsUri = new Uri(m_IcsPath);
		if (icsUri.Scheme == "file") {
			return File.OpenRead(m_IcsPath);
		} else {
			return await m_Http.GetStreamAsync(icsUri);
		}
	}
}
