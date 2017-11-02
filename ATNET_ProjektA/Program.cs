using System;
using ConsoleUI;

namespace TreeBrowser {
	class Program {
		public static void Main(string[] args) {
			ConsoleTab tab = new ConsoleTab();
			tab.OnKeyPressEvent += (ref ConsoleKeyInfo keyInfo) => {
				if (keyInfo.Key == ConsoleKey.Escape)
					ConsoleManager.Exit();
			};
			ConsoleManager.AddTab(tab);
			ConsoleManager.RunUI();
		}
	}
}
