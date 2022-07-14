namespace ObservancesBot; 

public abstract class ObservanceService {
	public abstract Task<string[]?> GetObservances(DateTime date);
}