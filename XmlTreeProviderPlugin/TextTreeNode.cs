using System;
using System.Collections.Generic;
using TreeBrowserPluginInterface;

namespace XmlTreeProviderPlugin {
	public class TextTreeNode : MarshalByRefObject, ITreeNode {
		string text;
		public TextTreeNode(string text) {
			this.text = text;
		}

		public List<ITreeNode> GetChildren() {
			return null;
		}

		public string GetName() {
			return text;
		}

		public bool HasChildren() {
			return false;
		}
	}
}
