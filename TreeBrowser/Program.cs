using System;
using ConsoleUI;
using FilesystemTreeProviderPlugin;

namespace TreeBrowser {
	class Program {
		public static void Main(string[] args) {
			FilesystemTreeProvider fsProv = new FilesystemTreeProvider();
			ConsoleTab tab = new TreeConsoleTab(fsProv.GetTree("../..")) {
				Title = "File system"
			};
			tab.OnKeyPressEvent += (ref ConsoleKeyInfo keyInfo) => {
				if (keyInfo.Key == ConsoleKey.Escape)
					ConsoleManager.Exit();
			};
			ConsoleManager.AddTab(tab);
			ConsoleManager.AddTab(new ConsoleTab());
			ConsoleManager.RunUI();
		}
	}
}
