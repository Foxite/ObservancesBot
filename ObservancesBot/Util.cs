using Foxite.Text;
using HtmlAgilityPack;

namespace ObservancesBot;

public static class Util {
	public static IText GetText(this HtmlNode node, Uri? baseHref = null, Func<HtmlNode, bool>? filter = null) {
		if (node.NodeType == HtmlNodeType.Element) {
			var childTexts = new CompositeText(node.ChildNodes.Where(filter ?? (_ => true)).Select(childNode => GetText(childNode, baseHref, filter)).ToList());

			return node.Name switch {
				"a" => new LinkText(baseHref == null ? new Uri(node.GetAttributeValue("href", null)) : new Uri(baseHref, node.GetAttributeValue("href", null)), childTexts),
				"i" => new StyledText(Style.Italic, childTexts),
				_ => childTexts
			};
		} else { // node.NodeType == HtmlNodeType.Text
			return new LiteralText(HtmlEntity.DeEntitize(node.InnerText));
		}
	}
	
	public static string GetEnv(string name, string? defaultValue = null) {
		return Environment.GetEnvironmentVariable(name) ?? defaultValue ?? throw new Exception("Missing environment variable: " + name);
	}
}
