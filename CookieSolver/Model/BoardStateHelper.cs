namespace CookieSolver.Model
{
	public class BoardStateHelper
	{
		public static BoardState ParseBoardString(string boardString)
		{
			// String Format:
			// 2 characters depicting the dimensions of the board (04 for 4x4 board, 10 for 10x10 board)
			// 1 character depicting the next piece
			// next dimension * dimension characters represent the cell values (ASCII values - 48, '0' for 0, ':' for 10)

			var size = int.Parse(boardString.Substring(0, 2));
			var nextPiece = int.Parse(boardString[2].ToString());
			var cells = ParseCellString(size, boardString.Substring(3));

			return new BoardState(size, cells, nextPiece);
		}

		public static Cell[,] ParseCellString(int size, string cellString)
		{
			var cells = new Cell[size, size];
			for (int i = 0; i < size * size; i++)
			{
				var posY = i / size;
				var posX = i % size;
				var value = (int)char.GetNumericValue(cellString[i]);
				cells[i / size, i % size] = new Cell(posY, posX, value);
			}

			return cells;
		}

		public static string EncodeBoardState(BoardState boardState)
		{
			var boardString = "";

			boardString += boardState.Size.ToString().PadLeft(2, '0');
			boardString += boardState.NextPiece.ToString();
			boardString += string.Join("", boardState.CellList.Select(cell => (char)(cell.Value + 48)));

			return boardString;
		}
	}
}
