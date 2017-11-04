using System;

namespace ConsoleUI {
	public class ConsoleTab {
		static public int TopBound { 
			get {
				return 3;
			} 
		}

		static public int LeftBound { 
			get {
				return 1;
			}
		}

		static public int RightBound {
			get {
				return Console.WindowWidth - 2;
			}
		}

		static public int BottomBound {
			get {
				return Console.WindowHeight - 2;
			}
		}

		public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;
		public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
		public ConsoleColor SelectedForegroundColor { get; set; } = ConsoleColor.White;
		public ConsoleColor SelectedBackgroundColor { get; set; } = ConsoleColor.DarkRed;
		public delegate void OnKeyPressListener(ref ConsoleKeyInfo keyInfo);
		protected bool changed;
		public event OnKeyPressListener OnKeyPressEvent;
		public string Title { get; set; }

		public ConsoleTab() {
			changed = true;
			Title = "NewTab";
		}

		virtual public void Draw() {
			changed = false;

			Console.BackgroundColor = BackgroundColor;
			for (int y = TopBound; y <= BottomBound; y++) {
				Console.SetCursorPosition(1, y);
				for (int x = LeftBound; x <= RightBound; x++) {
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
