using Foxite.Text;

namespace ObservancesBot; 

public abstract class ObservanceService {
	public abstract string Name { get; }
	
	public abstract Uri GetSourceUri(DateTime date);
	public abstract Task<IReadOnlyCollection<IText>?> GetObservances(DateTime date);
}
