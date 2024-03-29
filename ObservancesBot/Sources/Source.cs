using Foxite.Text;

namespace ObservancesBot; 

public abstract class Source {
	public abstract string Name { get; }
	
	public abstract Uri GetSourceUri(DateTime date);
	public abstract Task<IReadOnlyCollection<IText>?> GetObservances(DateTime date);
}
