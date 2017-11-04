using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleUI {
    static public class ConsoleManager {
		static private Thread eventThread;
		static private List<ConsoleTab> tabs = new List<ConsoleTab>();
		static private int activeTab = 0;
		static public int ActiveTab {
			get {
				return activeTab;
			}
			set {
				if (value >= 0 && value < tabs.Count)
					activeTab = value;
			}
		}

		static private bool running = true;
		static public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;
		static public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
		static public ConsoleColor ActiveTabForegroundColor { get; set; } = ConsoleColor.White;
		static public ConsoleColor ActiveTabBackgroundColor { get; set; } = ConsoleColor.DarkGreen;
		static public void AddTab(ConsoleTab tab) {
			tabs.Add(tab);
		}

		static public void RunUI() {
			Console.CursorVisible = false;

			eventThread = new Thread(HandleEvents);
			eventThread.Start();

			if (tabs.Count == 0)
				throw new ApplicationException("No tabs.");

			while (running) {
				if (tabs[activeTab].HasChanged()) {
					Draw();
					tabs[activeTab].Draw();
				}
			}
		}

		static private void WriteAt(int x, int y, string str){
			Console.SetCursorPosition(x, y);
			Console.Write(str);
		}

		static private void Draw() {
			Console.BackgroundColor = BackgroundColor;
			Console.ForegroundColor = ForegroundColor;
			List<ConsoleTab>.Enumerator tab = tabs.GetEnumerator();
			Console.SetCursorPosition(0, 1);
			for (int i = 0; i < tabs.Count; i++){
				tab.MoveNext();
				if (activeTab == i) {
					Console.BackgroundColor = ActiveTabBackgroundColor;
					Console.ForegroundColor = ActiveTabForegroundColor;
				} else {
					Console.BackgroundColor = BackgroundColor;
					Console.ForegroundColor = ForegroundColor;
				}
				string str = "  " + tab.Current.Title + "  ";
				Console.Write(str);
				Console.CursorTop--;
				Console.CursorLeft -= str.Length;

				Console.BackgroundColor = BackgroundColor;
				if(activeTab == i){
					Console.ForegroundColor = ActiveTabBackgroundColor;
					for (int j = 0; j < str.Length; j++)
						Console.Write('▃');
				} else {
					for (int j = 0; j < str.Length; j++)
						Console.Write(' ');
				}
				Console.CursorTop++;
			}
			int fillFrom = Console.CursorLeft;
			for (int i = fillFrom; i < Console.WindowWidth; i++) {
				Console.Write(" ");
			}
			Console.SetCursorPosition(fillFrom, 0);
			for (int i = fillFrom; i < Console.WindowWidth; i++) {
				Console.Write(" ");
			}

			Console.BackgroundColor = ActiveTabBackgroundColor;
			Console.ForegroundColor = ActiveTabForegroundColor;
			for (int y = 2; y < Console.WindowHeight; y++){
				WriteAt(0, y, " ");
				WriteAt(Console.WindowWidth-1, y, " ");
			}
			Console.SetCursorPosition(0, 2);
			for (int x = 1; x < Console.WindowWidth; x++) {
				Console.Write(' ');
			}
			Console.SetCursorPosition(0, Console.WindowHeight - 1);
			for (int x = 1; x < Console.WindowWidth; x++) {
				Console.Write(' ');
			}
		}

		static private void HandleEvents() {
			try {
				ConsoleKeyInfo keyInfo;
				while (Thread.CurrentThread.IsAlive) {
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
			Console.SetCursorPosition(Console.WindowWidth-1, Console.WindowHeight-1);
			Console.WriteLine();
			Console.ResetColor();
		}
    }
}
