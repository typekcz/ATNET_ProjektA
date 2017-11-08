using System;
using System.Collections.Generic;
using System.IO;
using TreeBrowserPluginInterface;

namespace FilesystemTreeProviderPlugin {
	public class FileTreeNode : MarshalByRefObject, ITreeNode {
		private FileInfo fileInfo;
		public FileTreeNode(string path) {
			fileInfo = new FileInfo(path);
		}

		public FileTreeNode(FileInfo fileInfo){
			this.fileInfo = fileInfo;
		}

		public List<ITreeNode> GetChildren() {
			return new List<ITreeNode>();
		}

		public string GetName() {
			return fileInfo.Name;
		}

		public bool HasChildren() {
			return false;
		}
	}
}
