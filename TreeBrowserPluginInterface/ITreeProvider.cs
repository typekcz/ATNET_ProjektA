using System;
namespace TreeBrowserPluginInterface {
	public interface ITreeProvider {
		string GetProviderName();
		ITreeNode GetTree(string path);
	}
}
