using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Foxite.Text;

namespace ObservancesBot;

public class SaveToCsvTarget : Target, IDisposable {
	private readonly CsvWriter m_CsvWriter;

	public SaveToCsvTarget(string path) {
		var sw = new StreamWriter(path);
		m_CsvWriter = new CsvWriter(sw, new CsvConfiguration(CultureInfo.InvariantCulture) {
			Delimiter = ","
		}, false);

		m_CsvWriter.WriteHeader<CsvRow>();
	}

	public async override Task Send(Observances observances) {
		var formatter = ModularTextFormatter.Markdown();
		foreach (IText item in observances.Items) {
			await m_CsvWriter.NextRecordAsync();
			m_CsvWriter.WriteRecord(new CsvRow(observances.Date, formatter.Format(item)));
		}
	}

	public void Dispose() {
		m_CsvWriter.Dispose();
	}
}

public record CsvRow(DateTime? Date, string Text);
