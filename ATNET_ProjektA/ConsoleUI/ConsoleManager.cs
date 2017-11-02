using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleUI {
    public class ConsoleManager {
		static private Thread eventThread;

		static private List<ConsoleTab> tabs = new List<ConsoleTab>();
		static private int activeTab = 0;
		static private bool running = true;
		static public void AddTab(ConsoleTab tab) {
			tabs.Add(tab);
		}
		static public void RunUI() {
			Console.BufferHeight = Console.WindowHeight;
			Console.BufferWidth = Console.WindowWidth;

			Console.CursorVisible = false;

			eventThread = new Thread(handleEvents);
			eventThread.Start();

			if (tabs.Count == 0)
				throw new ApplicationException("No tabs.");

			while (running) {
				if(tabs[activeTab].HasChanged())
				tabs[activeTab].Draw();
			}
		}
		static private void handleEvents() {
			try {
				ConsoleKeyInfo keyInfo;
				while (true) {
					keyInfo = Console.ReadKey();
					tabs[activeTab].FireKeyPressEvent(ref keyInfo);
					Thread.Sleep(0);
				}
			} catch (ThreadInterruptedException) {}
		}
		static public void Exit() {
			running = false;
			if (eventThread != null && eventThread.IsAlive) {
				eventThread.Interrupt();
			}
		}
    }
}
