using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleUI {
	public class ConsoleTab {
		public delegate void OnKeyPressListener(ref ConsoleKeyInfo keyInfo);
		private bool changed;
		public event OnKeyPressListener OnKeyPressEvent;
		public ConsoleTab() {
			changed = true;
		}
		virtual public void Draw() {
			changed = false;

			for (int y = 0; y < Console.WindowHeight; y++) {
				Console.SetCursorPosition(0, y);
				for (int x = 0; x < Console.WindowWidth; x++) {
					if ((x == 1 || x == Console.WindowWidth - 2) || (y == 1 || y == Console.WindowHeight - 2)) {
						Console.BackgroundColor = ConsoleColor.DarkGreen;
					} else {
						Console.BackgroundColor = ConsoleColor.Gray;
					}
					Console.Write(" ");
				}
			}

			Console.SetCursorPosition(0, 0);
		}
		public bool HasChanged() {
			return changed;
		}

		public void Redraw() {
			changed = true;
		}
		public void FireKeyPressEvent(ref ConsoleKeyInfo keyInfo) {
			OnKeyPressEvent.Invoke(ref keyInfo);
		}
	}
}
