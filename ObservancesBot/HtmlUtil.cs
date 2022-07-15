using Foxite.Text;
using HtmlAgilityPack;

namespace ObservancesBot;

public static class HtmlUtil {
	public static IText GetText(this HtmlNode node, Uri baseHref, Func<HtmlNode, bool> filter) {
		if (node.NodeType == HtmlNodeType.Element) {
			var childTexts = new CompositeText(node.ChildNodes.Where(filter).Select(childNode => GetText(childNode, baseHref, filter)).ToList());

			return node.Name switch {
				"a" => new LinkText(new Uri(baseHref, node.GetAttributeValue("href", null)), childTexts),
				"i" => new StyledText(Style.Italic, childTexts),
				_ => childTexts
			};
		} else { // node.NodeType == HtmlNodeType.Text
			return new LiteralText(node.InnerText);
		}
	}
}
