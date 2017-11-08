using System;
using System.IO;
using TreeBrowserPluginInterface;

namespace FilesystemTreeProviderPlugin {
	public class FilesystemTreeProvider :  MarshalByRefObject, ITreeProvider {
		public FilesystemTreeProvider(){
		}

		public string GetProviderName() {
			return "Filesystem";
		}

		public ITreeNode GetTree(string path) {
			return new DirectoryTreeNode(path);
		}
	}
}
