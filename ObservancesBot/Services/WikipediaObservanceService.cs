using Foxite.Text;
using HtmlAgilityPack;

namespace ObservancesBot;

public class WikipediaObservanceService : ObservanceService {
	private readonly HttpClient m_Http;

	public override string Name => "Wikipedia";

	public WikipediaObservanceService(HttpClient http) {
		m_Http = http;
	}

	public override Uri GetSourceUri(DateTime date) {
		return new Uri($"https://en.wikipedia.org/wiki/{date:MMMM}_{date:dd}");
	}

	public async override Task<IReadOnlyCollection<IText>?> GetObservances(DateTime date) {
		string htmlString = await m_Http.GetStringAsync(GetSourceUri(date));
		var htmlDocument = new HtmlDocument();
		htmlDocument.LoadHtml(htmlString);
		HtmlNode? header = htmlDocument.GetElementbyId("Holidays_and_observances");
		HtmlNode? section = header.ParentNode.NextSibling;
		while (section != null && !(section.NodeType == HtmlNodeType.Element && section.Name == "ul")) {
			section = section.NextSibling;
		}

		if (section == null) {
			return null;
		}

		return section.ChildNodes
			.Where(child => child.NodeType == HtmlNodeType.Element && child.Name == "li")
			.Where(li => !li.ChildNodes.Any(child => child.NodeType == HtmlNodeType.Element && child.Name == "ul")) // filter sublists
			.Select(li => li.GetText(new Uri("https://en.wikipedia.org/"), node => !node.HasClass("reference")))
			.ToArray();
	}
}
