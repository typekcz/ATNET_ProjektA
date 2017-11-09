using System;
using System.IO;
using System.Diagnostics;
using ConsoleUI;
using TreeBrowserPluginInterface;

namespace TreeBrowser {
	class Program {
		public static void Main(string[] args) {
			Console.Title = "TreeBrowser";
			Debug.Listeners.Add(new TextWriterTraceListener("debug.log"));
			Trace.Listeners.Add(new TextWriterTraceListener("trace.log"));

			Trace.WriteLine("Start Trace " + DateTime.Now);
			Debug.WriteLine("Start Debug " + DateTime.Now);

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

			Trace.WriteLine("End " + DateTime.Now);
			Trace.Flush();
			Debug.WriteLine("End " + DateTime.Now);
			Debug.Flush();
		}
	}
}
