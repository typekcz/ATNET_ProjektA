using System;
using System.Collections.Generic;

namespace TreeBrowserPluginInterface {
	public interface ITreeNode {
		ITreeNode GetParent();
		bool HasChildren();
		List<ITreeNode> GetChildren();
		string GetName();
	}
}
