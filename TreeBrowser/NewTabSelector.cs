using System;
using System.Collections.Generic;
using ConsoleUI;
using TreeBrowserPluginInterface;

namespace TreeBrowser {
	public class NewTabSelector : ConsoleTab {
		private static int ITEMS_WIDTH = 30;
		private int selectedProv;
		public NewTabSelector() {
			selectedProv = 0;
			OnKeyPressEvent += (ref ConsoleKeyInfo keyInfo) => {
				switch(keyInfo.Key){
					case ConsoleKey.DownArrow:
						if (selectedProv < TreeProvidersManager.GetProviders().Count - 1) {
							selectedProv++;
							Redraw();
						}
						break;
					case ConsoleKey.UpArrow:
						if (selectedProv > 0) {
							selectedProv--;
							Redraw();
						}
						break;
					case ConsoleKey.Enter:
						try {
							ITreeProvider treeProv = TreeProvidersManager.GetProviders().Values[selectedProv];
							ConsoleManager.ReplaceTab(new TreeConsoleTab(treeProv.GetTree(EndInputText())));
						} catch(Exception e){
							ConsoleManager.ReplaceTab(new TextConsoleTab(e.Message){Title = "Error", ForegroundColor = ConsoleColor.Red});
						}
						break;
				}
			};
			int left = (RightBound - LeftBound - ITEMS_WIDTH) / 2;
			InputText(left + 1, TopBound + 2, 28);
		}
		public override void Draw() {
			base.Draw();
			Console.ForegroundColor = ConsoleManager.ForegroundColor;
			Console.BackgroundColor = ConsoleManager.BackgroundColor;
			int left = (RightBound - LeftBound - ITEMS_WIDTH)/2;

			Console.SetCursorPosition(left, TopBound + 6);

			int i = 0;
			foreach(KeyValuePair<string, ITreeProvider> prov in TreeProvidersManager.GetProviders()){
				if(i == selectedProv){
					Console.ForegroundColor = SelectedForegroundColor;
					Console.BackgroundColor = SelectedBackgroundColor;
				} else {
					Console.ForegroundColor = ForegroundColor;
					Console.BackgroundColor = BackgroundColor;
				}
				string provName = prov.Key.PadLeft((ITEMS_WIDTH-prov.Key.Length)/2 + prov.Key.Length);
				provName = provName.PadRight(ITEMS_WIDTH);
				Console.Write(provName);
				Console.CursorTop++;
				Console.CursorLeft = left;
				i++;
			}
		}
	}
}
