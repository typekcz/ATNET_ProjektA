using System;
using System.Text;

namespace ConsoleUI {
	public class ConsoleTab {
		static public int TopBound { 
			get {
				return 2;
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
		protected bool changed;
		public event OnKeyPressListener OnKeyPressEvent;
		private string title;
		public string Title { get {
				return title;
			} 
			set {
				title = value;
				ConsoleManager.TabsChanged();
			}
		}
		private StringBuilder inputText;
		private int inputPositionX;
		private int inputPositionY;
		private int inputLength;
		private int inputOffset;
		private int cursorPosition;
		private int CursorPosition {
			get {
				return cursorPosition;
			}
			set {
				if (value > cursorPosition) {
					cursorPosition = value;
					if (cursorPosition - inputOffset >= inputLength) {
						inputOffset = cursorPosition - inputLength;
					}
				} else {
					cursorPosition = value;
					if(cursorPosition - inputOffset < 0){
						inputOffset = cursorPosition;
					}
				}
			}
		}
		private OnKeyPressListener inputListener;

		public ConsoleTab() {
			changed = true;
			Title = "NewTab";
		}

		protected void InputText(int positionX, int positionY, int length){
			inputPositionX = positionX;
			inputPositionY = positionY;
			inputLength = length;
			inputOffset = 0;
			CursorPosition = 0;
			inputText = new StringBuilder();
			Console.SetCursorPosition(positionX, positionY);
			Console.CursorVisible = true;
			inputListener = (ref ConsoleKeyInfo keyInfo) => {
				switch(keyInfo.Key){
					case ConsoleKey.Backspace:
						if (CursorPosition > 0) {
							inputText.Remove(CursorPosition - 1, 1);
							CursorPosition--;
						}
						break;
					case ConsoleKey.Delete:
						if(CursorPosition < inputText.Length)
							inputText.Remove(CursorPosition, 1);
						break;
					case ConsoleKey.LeftArrow:
						if(CursorPosition > 0)
							CursorPosition--;
						break;
					case ConsoleKey.RightArrow:
						if (CursorPosition < inputText.Length)
							CursorPosition++;
						break;
					case ConsoleKey.Home:
						CursorPosition = 0;
						break;
					case ConsoleKey.End:
						CursorPosition = inputText.Length;
						break;
					default:
						if (keyInfo.KeyChar > ' ') {
							inputText.Insert(CursorPosition, keyInfo.KeyChar);
							CursorPosition++;
						}
						break;
				}
				DrawInput();
			};
			OnKeyPressEvent += inputListener;
		}

		protected string EndInputText() {
			if (inputText == null)
				return null;
			OnKeyPressEvent -= inputListener;
			Console.CursorVisible = false;
			string input = inputText.ToString();
			inputText = null;
			return input;
		}

		public StringBuilder PeekInputText(){
			return inputText;
		}

		public void PlaceInputCursor(){
			if (inputText == null) {
				Console.CursorVisible = false;
				Console.SetCursorPosition(0, 1);
				Console.ForegroundColor = ConsoleManager.BackgroundColor;
				Console.BackgroundColor = ConsoleManager.BackgroundColor;
				return;
			}
			Console.CursorVisible = true;
			if (!(Console.CursorTop == inputPositionY && Console.CursorLeft >= inputPositionX && Console.CursorLeft < inputPositionX + inputLength))
				Console.SetCursorPosition(inputPositionX + CursorPosition - inputOffset, inputPositionY);
		}

		virtual public void Draw() {
			changed = false;
			Console.CursorVisible = false;

			Console.BackgroundColor = BackgroundColor;
			Console.ForegroundColor = ForegroundColor;
			for (int y = TopBound; y <= BottomBound; y++) {
				Console.SetCursorPosition(1, y);
				for (int x = LeftBound; x <= RightBound; x++) {
					Console.Write(" ");
				}
			}

			DrawInput();
		}

		virtual public void DrawInput(){
			if(inputText == null){
				return;
			}

			Console.ForegroundColor = ConsoleManager.ForegroundColor;
			Console.BackgroundColor = ConsoleManager.BackgroundColor;

			Console.SetCursorPosition(inputPositionX - 1, inputPositionY - 1);
			Console.Write('┌');
			for (int i = 0; i < inputLength; i++)
				Console.Write('─');
			Console.Write('┐');

			Console.SetCursorPosition(inputPositionX - 1, inputPositionY);
			Console.Write('│');
			int charsToDraw = Math.Min(inputText.Length - inputOffset, inputLength);
			Console.Write(inputText.ToString().Substring(inputOffset, charsToDraw));
			for (int i = 0; i < (inputLength - charsToDraw); i++)
				Console.Write(' ');
			Console.Write('│');

			Console.SetCursorPosition(inputPositionX - 1, inputPositionY + 1);
			Console.Write('└');
			for (int i = 0; i < inputLength; i++)
				Console.Write('─');
			Console.Write('┘');

			PlaceInputCursor();
		}

		public bool HasChanged() {
			return changed;
		}

		public void Redraw() {
			changed = true;
		}

		public void FireKeyPressEvent(ref ConsoleKeyInfo keyInfo) {
			if(OnKeyPressEvent != null)
				OnKeyPressEvent.Invoke(ref keyInfo);
		}
	}
}
