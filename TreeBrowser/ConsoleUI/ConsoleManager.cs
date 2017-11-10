using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TreeBrowser;

namespace ConsoleUI {
	public delegate void OnKeyPressListener(ref ConsoleKeyInfo keyInfo);

    static public class ConsoleManager {
		static private Thread eventThread;
		static private List<ConsoleTab> tabs = new List<ConsoleTab>();
		static public event OnKeyPressListener OnKeyPressEvent;
		static private bool tabsChanged = true;
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

		static public void TabsChanged(){
			tabsChanged = true;
		}

		static public void AddTab(ConsoleTab tab) {
			tabs.Add(tab);
		}

		static public void ReplaceTab(ConsoleTab tab){
			tabs[activeTab] = tab;
		}

		static public void RunUI() {
			Console.CursorVisible = false;
			
			eventThread = new Thread(HandleEvents);
			eventThread.Start();

			if (tabs.Count == 0)
				throw new ApplicationException("No tabs.");

			Draw();
			Console.CursorTop = 0;

			while (running) {
				ConsoleTab tab;
				lock (tabs) {
					if (tabsChanged) {
						DrawTabs();
						tab = tabs[activeTab];
						lock (Console.Out) {
							tab.Draw();
							tab.PlaceInputCursor();
						}
					}
					tab = tabs[activeTab];
					if (tab.HasChanged()) {
						lock (Console.Out) {
							tab.Draw();
							tab.PlaceInputCursor();
						}
					}
				}
			}

			if (eventThread != null && eventThread.IsAlive) {
				eventThread.Interrupt();
			}
			eventThread.Join();
			Console.SetCursorPosition(Console.WindowWidth - 1, Console.WindowHeight - 1);
			Console.CursorVisible = true;
			Console.ResetColor();
			Console.WriteLine();
		}

		static private void WriteAt(int x, int y, string str){
			Console.SetCursorPosition(x, y);
			Console.Write(str);
		}

		static private void Draw() {
			DrawTabs();
			lock (Console.Out) {
				Console.BackgroundColor = ActiveTabBackgroundColor;
				Console.ForegroundColor = ActiveTabForegroundColor;
				for (int y = 1; y < Console.WindowHeight; y++) {
					WriteAt(0, y, " ");
					WriteAt(Console.WindowWidth - 1, y, " ");
				}
				Console.SetCursorPosition(0, 1);
				for (int x = 1; x < Console.WindowWidth; x++) {
					Console.Write(' ');
				}
				Console.SetCursorPosition(0, Console.WindowHeight - 1);
				for (int x = 1; x < Console.WindowWidth; x++) {
					Console.Write(' ');
				}
			}
		}

		static private void DrawTabs() {
			lock (Console.Out) {
				tabsChanged = false;
				Console.CursorVisible = false;
				Console.BackgroundColor = BackgroundColor;
				Console.ForegroundColor = ForegroundColor;
				List<ConsoleTab>.Enumerator tab = tabs.GetEnumerator();
				Console.SetCursorPosition(0, 0);
				for (int i = 0; i < tabs.Count; i++) {
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
				}
				int fillFrom = Console.CursorLeft;
				Console.BackgroundColor = BackgroundColor;
				for (int i = fillFrom; i < Console.WindowWidth; i++) {
					Console.Write(" ");
				}
			}
		}

		static private void HandleEvents() {
			try {
				ConsoleKeyInfo keyInfo;
				while (Thread.CurrentThread.IsAlive) {
					if (!Console.KeyAvailable) {
						Thread.Sleep(0);
						continue;
					}
					lock (Console.Out) {
						keyInfo = Console.ReadKey(tabs[activeTab].PeekInputText() == null);
						OnKeyPressEvent.Invoke(ref keyInfo);
						bool propagate = true;
						switch (keyInfo.Key) {
							case ConsoleKey.Tab:
								if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift)) {
									if (activeTab > 0)
										activeTab--;
									else
										activeTab = tabs.Count - 1;
								} else {
									if (activeTab < tabs.Count - 1)
										activeTab++;
									else
										activeTab = 0;
								}
								tabsChanged = true;
								propagate = false;
								break;
							case ConsoleKey.T:
								if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control)) {
									AddTab(new NewTabSelector());
									activeTab = tabs.Count - 1;
									tabsChanged = true;
									propagate = false;
								}
								break;
							case ConsoleKey.W:
								if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control)) {
									lock (tabs) {
										if (activeTab > 0)
											activeTab--;
										tabs.RemoveAt(activeTab);
										tabsChanged = true;
										propagate = false;
									}
								}
								break;
						}
						if (propagate)
							tabs[activeTab].FireKeyPressEvent(ref keyInfo);

						tabs[activeTab].PlaceInputCursor();
					}
					Thread.Sleep(0);
				}
			} catch (ThreadInterruptedException) {}
		}

		static public void ProvidersChanged(){
			if (tabs[activeTab].GetType() == typeof(NewTabSelector))
				tabs[activeTab].Redraw();
		}

		static public void Exit() {
			running = false;
		}
    }
}
