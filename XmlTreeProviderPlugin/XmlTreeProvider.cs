using System;
using System.IO;
using System.Xml;
using TreeBrowserPluginInterface;

namespace XmlTreeProviderPlugin {
	public class XmlTreeProvider : MarshalByRefObject, ITreeProvider {
		public string GetProviderName() {
			return "XML";
		}

		public ITreeNode GetTree(string path) {
			XmlDocument xdoc = new XmlDocument();
			xdoc.Load(path);
			return new XmlTreeNode(xdoc, Path.GetFileName(path));
		}
	}
}
