using HtmlAgilityPack;

namespace ObservancesBot;

public class WikipediaObservanceService : ObservanceService {
	private readonly HttpClient m_Http;

	public WikipediaObservanceService(HttpClient http) {
		m_Http = http;
	}
	
	public async override Task<string[]?> GetObservances(DateTime date) {
		string wikipediaUrl = $"https://en.wikipedia.org/wiki/{date:MMMM}_{date:dd}";
		string htmlString = await m_Http.GetStringAsync(wikipediaUrl);
		var htmlDocument = new HtmlDocument();
		htmlDocument.LoadHtml(htmlString);
		HtmlNode? header = htmlDocument.GetElementbyId("Holidays_and_observances");
		HtmlNode? section = header.ParentNode.NextSibling;
		while (!(section.NodeType == HtmlNodeType.Element && section.Name == "ul")) {
			section = section.NextSibling;
		}

		if (section == null) {
			return null;
		}

		return section.ChildNodes
			.Where(child => child.NodeType == HtmlNodeType.Element && child.Name == "li")
			.Where(li => !li.ChildNodes.Any(child => child.NodeType == HtmlNodeType.Element && child.Name == "ul")) // filter sublists
			.Select(li => li.GetInnerText(node => !node.HasClass("reference")))
			.ToArray();
	}
}
