using System;
using System.Collections.Generic;
using ConsoleUI;
using TreeBrowserPluginInterface;

namespace TreeBrowser {
	public class TreeConsoleTab : ConsoleTab {
		private class TreeRow {
			public ITreeNode node;
			public int level;
			public bool lastChild = false;
		}
		private ITreeNode root;
		private LinkedList<TreeRow> rows;
		private LinkedListNode<TreeRow> selectedRow;
		private int scrollOffset;
		public TreeConsoleTab(ITreeNode root) {
			this.root = root;
			Title = root.GetName();
			rows = new LinkedList<TreeRow>();
			rows.AddFirst(new TreeRow() {
				node = root,
				level = 0
			});
			selectedRow = rows.First;
			scrollOffset = 0;
			OnKeyPressEvent += (ref ConsoleKeyInfo keyInfo) => {
				switch(keyInfo.Key){
					case ConsoleKey.DownArrow:
						if (selectedRow.Next != null) {
							selectedRow = selectedRow.Next;
							if (GetSelectedRowIndex() - scrollOffset > BottomBound - TopBound)
								scrollOffset++;
							else {
								drawFrom = selectedRow.Previous;
								drawToEnd = false;
							}
							changed = true;
						}
						break;
					case ConsoleKey.UpArrow:
						if (selectedRow.Previous != null) {
							selectedRow = selectedRow.Previous;
							if (GetSelectedRowIndex() - scrollOffset < 0)
								scrollOffset--;
							else {
								drawFrom = selectedRow;
								drawToEnd = false;
							}
							changed = true;
						}
						break;
					case ConsoleKey.Enter:
						if(selectedRow.Next != null && selectedRow.Next.Value.level > selectedRow.Value.level){
							CollapseRow(selectedRow);
						} else {
							ExpandRow(selectedRow);
						}
						drawFrom = selectedRow;
						drawToEnd = true;
						changed = true;
						break;
				}
			};
		}

		private void ExpandRow(LinkedListNode<TreeRow> parent){
			if (!parent.Value.node.HasChildren())
				return;
			LinkedListNode<TreeRow> row = parent;
			foreach(ITreeNode child in parent.Value.node.GetChildren()){
				row = rows.AddAfter(row, new TreeRow() {
					node = child,
					level = parent.Value.level + 1
				});
			}
			row.Value.lastChild = true;
		}

		private void CollapseRow(LinkedListNode<TreeRow> parent){
			LinkedListNode<TreeRow> row = parent.Next;
			while(row != null && row.Value.level > parent.Value.level){
				LinkedListNode<TreeRow> rowToDelete = row;
				row = rowToDelete.Next;
				rows.Remove(rowToDelete);
			}
		}

		private int GetSelectedRowIndex(){
			LinkedListNode<TreeRow> start = selectedRow;
			int i;
			for (i = 0; start.Previous != null;i++){
				start = start.Previous;
			}
			return i;
		}

		private LinkedListNode<TreeRow> drawFrom = null;
		private bool drawToEnd = true;
		public override void Draw() {
			Draw(drawFrom, drawToEnd);
			drawFrom = null;
			drawToEnd = true;
		}
		private void Draw(LinkedListNode<TreeRow> from = null, bool toEnd = true) {
			//base.Draw();
			changed = false;
			Console.SetCursorPosition(LeftBound, TopBound);

			int rowIndex = 0;
			int maxRows = BottomBound - TopBound;
			bool fromNodeFound = false;
			for(LinkedListNode<TreeRow> row = rows.First; row != null; row = row.Next){
				if (rowIndex - scrollOffset > maxRows)
					break;

				if (from != null) {
					if (fromNodeFound) {
						if (!toEnd && row.Previous != from) {
							break;
						}
					} else {
						if (row == from) {
							fromNodeFound = true;
							Console.CursorTop = Math.Max(TopBound, rowIndex + TopBound - scrollOffset);

						} else {
							rowIndex++;
							continue;
						}
					}
				}

				if (rowIndex < scrollOffset){
					rowIndex++;
					continue;
				}
				Console.ForegroundColor = ForegroundColor;
				Console.BackgroundColor = BackgroundColor;
				LinkedListNode<TreeRow> ancestor;
				for (int i = row.Value.level; i > 1; i--) {
					for (ancestor = row.Previous; ancestor != null; ancestor = ancestor.Previous){
						if (ancestor.Value.level < i)
							break;
					}
					Console.CursorLeft = LeftBound + 4 * (i-2);
					if(ancestor.Value.lastChild)
						Console.Write("   ");
					else
						Console.Write(" │ ");
				}
				if (row.Value.level > 0) {
					Console.CursorLeft = LeftBound + 4 * (row.Value.level-1);
					if (row.Value.lastChild)
						Console.Write(" └──");
					else
						Console.Write(" ├──");
				}
				if (row == selectedRow) {
					Console.ForegroundColor = SelectedForegroundColor;
					Console.BackgroundColor = SelectedBackgroundColor;
				}
				if (row.Next != null && row.Next.Value.level > row.Value.level) {
					Console.Write("[-] ");
				} else {
					if (row.Value.node.HasChildren())
						Console.Write("[+] ");
					else
						Console.Write("[ ] ");
				}
				string name = row.Value.node.GetName().Replace('\n', ' ').Trim();
				name = name.Substring(0, Math.Min(name.Length, RightBound - Console.CursorLeft));
				Console.Write(row.Value.node.GetName());

				Console.ForegroundColor = ForegroundColor;
				Console.BackgroundColor = BackgroundColor;
				while (Console.CursorLeft <= RightBound)
					Console.Write(' ');
				Console.CursorLeft = LeftBound;
				Console.CursorTop++;

				rowIndex++;
			}
			if (toEnd) {
				while (Console.CursorTop <= BottomBound) {
					while (Console.CursorLeft <= RightBound)
						Console.Write(' ');
					Console.CursorLeft = LeftBound;
					Console.CursorTop++;
				}
			}
		}
	}
}
