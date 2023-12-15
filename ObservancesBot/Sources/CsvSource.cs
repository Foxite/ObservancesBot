using System.Globalization;
using CsvHelper;
using Foxite.Text;
using Foxite.Text.Parsers;

namespace ObservancesBot; 

public class CsvSource : Source {
	private readonly string m_Path;
	private readonly string m_SourceUriFormat;
	private readonly MarkdownParser m_MarkdownParser;

	public override string Name { get; }

	public CsvSource(string path, string name, string sourceUriFormat) {
		m_Path = path;
		m_SourceUriFormat = sourceUriFormat;
		m_MarkdownParser = new MarkdownParser();

		Name = name;
	}
	
	public override Uri GetSourceUri(DateTime date) {
		//return new Uri($"https://en.wikipedia.org/wiki/{date:MMMM}_{date:dd}")
		
		return new Uri(string.Format(m_SourceUriFormat, date));
	}

	public async override Task<IReadOnlyCollection<IText>?> GetObservances(DateTime date) {
		using var sr = new StreamReader(m_Path);
		using var csv = new CsvReader(sr, CultureInfo.InvariantCulture, true);
		//await csv.ReadAsync();
		//csv.ReadHeader();

		bool Predicate(CsvRow row) => row.Date.HasValue && row.Date.Value.Month == date.Month && row.Date.Value.Day == date.Day;

		IText Selector(CsvRow row) => m_MarkdownParser.Parse(row.Text);

		return await csv.GetRecordsAsync<CsvRow>()
			.Where(Predicate)
			.Select(Selector).ToListAsync();
	}
}
