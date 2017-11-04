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
		public TreeConsoleTab(ITreeNode root) {
			this.root = root;
			rows = new LinkedList<TreeRow>();
			rows.AddFirst(new TreeRow() {
				node = root,
				level = 0
			});
			selectedRow = rows.First;
			OnKeyPressEvent += (ref ConsoleKeyInfo keyInfo) => {
				switch(keyInfo.Key){
					case ConsoleKey.DownArrow:
						if (selectedRow.Next != null) {
							selectedRow = selectedRow.Next;
							/*if (selectedRow + scrollOffset - 1 > BottomBound - TopBound)
								scrollOffset--;*/
							changed = true;
						}
						break;
					case ConsoleKey.UpArrow:
						if (selectedRow.Previous != null) {
							selectedRow = selectedRow.Previous;
							/*if (selectedRow + scrollOffset + 1 > BottomBound - TopBound)
								scrollOffset++;*/
							changed = true;
						}
						break;
					case ConsoleKey.Enter:
						if(selectedRow.Next != null && selectedRow.Next.Value.level > selectedRow.Value.level){
							CollapseRow(selectedRow);
						} else {
							ExpandRow(selectedRow);
						}
						changed = true;
						break;
				}
			};
		}

		private void ExpandRow(LinkedListNode<TreeRow> parent){
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

		public override void Draw() {
			base.Draw();
			Console.SetCursorPosition(LeftBound, TopBound);

			for(LinkedListNode<TreeRow> row = rows.First; row != null; row = row.Next){
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
				Console.WriteLine(row.Value.node.GetName());
				Console.CursorLeft = LeftBound;
			}
		}
	}
}
