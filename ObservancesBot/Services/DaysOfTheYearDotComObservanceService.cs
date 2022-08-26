using Foxite.Text;
using HtmlAgilityPack;

namespace ObservancesBot; 

public class DaysOfTheYearDotComObservanceService : ObservanceService {
	private readonly HttpClient m_Http;

	public DaysOfTheYearDotComObservanceService(HttpClient http) {
		m_Http = http;
	}

	public override string Name => "DaysOfTheYear.com";
	
	public override Uri GetSourceUri(DateTime date) {
		string month = date.Month switch {
			1  => "jan",
			2  => "feb",
			3  => "mar",
			4  => "apr",
			5  => "may",
			6  => "jun",
			7  => "jul",
			8  => "aug",
			9  => "sep",
			10 => "oct",
			11 => "nov",
			12 => "dec",
		};

		return new Uri($"https://www.daysoftheyear.com/days/{month}/{date.Day}/");
	}

	public async override Task<IReadOnlyCollection<IText>?> GetObservances(DateTime date) {
		string htmlString = await m_Http.GetStringAsync(GetSourceUri(date));
		var htmlDocument = new HtmlDocument();
		htmlDocument.LoadHtml(htmlString);

		var ret = new List<IText>();
		HtmlNodeCollection cards = htmlDocument.DocumentNode.SelectNodes("/html/body/div[1]/main/section[1]/div/div");
		foreach (HtmlNode card in cards) {
			HtmlNode content = card.Elements("div").First(node => node.HasClass("card__content"));
			HtmlNode text = content.Elements("div").First(node => node.HasClass("card__text"));
			HtmlNode title = text.Elements("h3").First(node => node.HasClass("card__title"));
			HtmlNode link = title.Element("a");
			ret.Add(link.GetText());
		}

		return ret;
	}
}
