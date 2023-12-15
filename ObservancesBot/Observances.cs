using Foxite.Text;

namespace ObservancesBot;

public record Observances(
	IReadOnlyCollection<IText> Items,
	DateTime Date,
	Uri SourceUri,
	string SourceName
);
