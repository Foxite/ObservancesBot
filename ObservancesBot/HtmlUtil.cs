using System.Text;
using HtmlAgilityPack;

namespace ObservancesBot;

public static class HtmlUtil {
	public static string GetInnerText(this HtmlNode node, Predicate<HtmlNode> filter) {
		var sb = new StringBuilder();
		GetInnerTextInternal(node, filter, sb);
		return sb.ToString();
	}

	private static void GetInnerTextInternal(HtmlNode node, Predicate<HtmlNode> filter, StringBuilder sb) {
		if (node.NodeType == HtmlNodeType.Element) {
			foreach (HtmlNode childNode in node.ChildNodes) {
				if (filter(childNode)) {
					GetInnerTextInternal(childNode, filter, sb);
				}
			}
		} else if (node.NodeType == HtmlNodeType.Text) {
			sb.Append(node.InnerText);
		}
	}
}
