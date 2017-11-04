using System;
using System.IO;
using TreeBrowserPluginInterface;

namespace FilesystemTreeProviderPlugin {
	public class FilesystemTreeProvider :  ITreeProvider {
		public FilesystemTreeProvider(){
		}

		public ITreeNode GetTree(string path) {
			return new DirectoryTreeNode(path);
		}
	}
}
