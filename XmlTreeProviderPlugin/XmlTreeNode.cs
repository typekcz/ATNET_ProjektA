using System;
using System.Collections.Generic;
using System.Xml;
using TreeBrowserPluginInterface;

namespace XmlTreeProviderPlugin {
	public class XmlTreeNode : MarshalByRefObject, ITreeNode{
		private XmlNode node;
		private string name;

		public XmlTreeNode(XmlNode node, string name = null) {
			this.node = node;
			this.name = name;
		}

		public List<ITreeNode> GetChildren() {
			List<ITreeNode> children = new List<ITreeNode>();
			if (node.Name == "#text") {
				foreach (string row in node.InnerText.Split('\n')) {
					children.Add(new TextTreeNode(row));
				}
			} else {
				foreach (XmlNode child in node.ChildNodes) {
					children.Add(new XmlTreeNode(child));
				}
			}
			return children;
		}

		public string GetName() {
			if (name != null)
				return name;
			else
				return node.Name;
		}

		public bool HasChildren() {
			return node.HasChildNodes || node.Name == "#text";
		}
	}
}
