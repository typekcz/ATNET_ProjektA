using System;
using System.IO;
using ConsoleUI;
using TreeBrowserPluginInterface;

namespace TreeBrowser {
	class Program {
		private static FileStream logFile;
		public static StreamWriter log;
		public static void Main(string[] args) {
			Console.Title = "TreeBrowser";

			logFile = File.Open("log.txt", FileMode.Create);
			log = new StreamWriter(logFile);

			TreeProvidersManager.LoadPlugins();
			TreeProvidersManager.StartFSWatcher();
			//TreeProvidersManager.RegisterProvider("plugins/FilesystemTreeProvider.dll");

			ConsoleManager.OnKeyPressEvent += (ref ConsoleKeyInfo keyInfo) => {
				if (keyInfo.Key == ConsoleKey.Escape) {
					ConsoleManager.Exit();
				}
			};

			ConsoleManager.AddTab(new NewTabSelector());
			ConsoleManager.RunUI();

			log.Close();
			logFile.Close();
		}
	}
}
