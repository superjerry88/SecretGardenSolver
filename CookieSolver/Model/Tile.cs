namespace CookieSolver.Model
{
    public class Tile
    {
        public int Size { get; set; }
        public BoardState CurrentBoard { get; set; }
		public Stack<string> BoardStateHistoryPrevious { set; get; }
		public Stack<string> BoardStateHistoryNext { set; get; }
		public Cell[] CellsToMerge { get; set; }

		// Handle game logic if manual clicking needs to be done
		public enum ClickState { FirstCell, SecondCell, Cancel };
		public ClickState CurrentState = ClickState.FirstCell;

		public Tile(int size)
        {
            CurrentBoard = new BoardState(size);
            Size = size;
			CellsToMerge = new Cell[2];
			BoardStateHistoryPrevious = new Stack<string>();
			BoardStateHistoryNext = new Stack<string>();
		}

		public void ResolveClick(Cell cell)
		{
			switch (CurrentState)
			{
				case ClickState.FirstCell:
					CurrentState = ClickState.SecondCell;

					cell.Selected = true;
					CellsToMerge[0] = cell;

					break;

				case ClickState.SecondCell:
					CurrentState = ClickState.FirstCell;

					cell.Selected = true;
					CellsToMerge[1] = cell;

					Cell firstCell = CellsToMerge[0], secondCell = CellsToMerge[1];

					if (CurrentBoard.IsMergeable(firstCell, secondCell))
					{
						BoardStateHistoryPrevious.Push(BoardStateHelper.EncodeBoardState(CurrentBoard));
						CurrentBoard = new BoardState(Size, CurrentBoard.Cells, CurrentBoard.NextPiece);
						CurrentBoard.Merge(firstCell, secondCell);

						// If an action is done while on a previous board state, erase the future stack (effectively an undo button)
						BoardStateHistoryNext = new Stack<string>();
					}

					firstCell.Selected = false;
					secondCell.Selected = false;

					break;
			}
		}

		public void PlaceNextPiece()
		{
			var emptyCells = CurrentBoard.GetEmptyCellPositions();
			var randomEmptyCell = PieceRandomizer.GetRandomPosition(emptyCells);
			var pieceValue = CurrentBoard.NextPiece;

			BoardStateHistoryPrevious.Push(BoardStateHelper.EncodeBoardState(CurrentBoard));
			CurrentBoard = new BoardState(Size, CurrentBoard.Cells);
			CurrentBoard.UpdateCell(randomEmptyCell, pieceValue);
			CurrentBoard.UpdateGameState();

			// If an action is done while on a previous board state, erase the future stack (effectively an undo button)
			BoardStateHistoryNext = new Stack<string>();
		}

		public void GoToPreviousState()
		{
			BoardStateHistoryNext.Push(BoardStateHelper.EncodeBoardState(CurrentBoard));
			CurrentBoard = BoardStateHelper.ParseBoardString(BoardStateHistoryPrevious.Pop());
			CurrentBoard.UpdateGameState();
		}

		public void GoToNextState()
		{
			BoardStateHistoryPrevious.Push(BoardStateHelper.EncodeBoardState(CurrentBoard));
			CurrentBoard = BoardStateHelper.ParseBoardString(BoardStateHistoryNext.Pop());
			CurrentBoard.UpdateGameState();
		}
	}
}
