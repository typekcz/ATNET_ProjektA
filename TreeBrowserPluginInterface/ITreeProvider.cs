using System;
namespace TreeBrowserPluginInterface {
	public interface ITreeProvider {
		ITreeNode GetTree(string path);
	}
}
