using System;
using System.Collections.Generic;

namespace TreeBrowserPluginInterface {
	public interface ITreeNode {
		bool HasChildren();
		List<ITreeNode> GetChildren();
		string GetName();
	}
}
