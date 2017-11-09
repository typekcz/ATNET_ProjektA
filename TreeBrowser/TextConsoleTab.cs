using System;
using ConsoleUI;

namespace TreeBrowser {
	public class TextConsoleTab : ConsoleTab {
		private string str;
		public TextConsoleTab(string str) {
			this.str = str;
		}

		public override void Draw() {
			base.Draw();

			Console.SetCursorPosition(LeftBound + 1, TopBound + 1);
			foreach (string row in str.Split('\n')) {
				int maxLength = RightBound - LeftBound - 2;
				if (row.Length < maxLength){
					Console.Write(row);
					Console.CursorTop++;
					Console.CursorLeft = LeftBound + 1;
				} else {
					string s = row;
					while (s.Length > 0) {
						Console.Write(s.Substring(0, maxLength));
						Console.CursorTop++;
						s = s.Remove(0, maxLength);
					}
				}
			}
		}
	}
}
