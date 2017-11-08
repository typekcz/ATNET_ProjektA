using System;
using System.Collections.Generic;
using System.IO;
using TreeBrowserPluginInterface;

namespace FilesystemTreeProviderPlugin {
	public class DirectoryTreeNode : MarshalByRefObject, ITreeNode {
		private DirectoryInfo dirInfo;
		public DirectoryTreeNode(string path) {
			dirInfo = new DirectoryInfo(path);
		}

		public DirectoryTreeNode(DirectoryInfo dirInfo){
			this.dirInfo = dirInfo;
		}

		public List<ITreeNode> GetChildren() {
			List<ITreeNode> children = new List<ITreeNode>();
			foreach(DirectoryInfo dir in dirInfo.GetDirectories()){
				children.Add(new DirectoryTreeNode(dir));
			}
			foreach (FileInfo file in dirInfo.GetFiles()){
				children.Add(new FileTreeNode(file));
			}
			return children;
		}

		public string GetName() {
			return dirInfo.Name;
		}

		public bool HasChildren() {
			try {
				return (dirInfo.GetDirectories().Length + dirInfo.GetFiles().Length) > 0;
			} catch(Exception){
				return false;
			}
		}
	}
}
