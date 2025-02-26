using System.Runtime.ConstrainedExecution;

namespace CookieSolver.Model
{
	public class BoardState
	{
		public const int MergeValueMultiplier = 4;  // Merges are considered to be worth more
		public int NextPiece { get; set; }
		public bool GameLost { get; set; }
		public int Size { get; set; }
		public Cell[,] Cells {  get; set; }
		public double BoardValue { get; set; } // Used to evaluate the value of each board state, allowing algo to choose which board states to go to easier
		public BoardState(int size) : this(size, new Cell[size, size], -1) { }
		public BoardState(int size, Cell[,] cells) : this(size, cells, -1) { }
		public BoardState(int size, Cell[,] cells, int previousRandomPiece) {  // Copying from a previous board state
			Size = size;
			Cells = cells;

			// Initialize the board
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					if (Cells[i, j] == null)
						Cells[i, j] = new Cell(i, j);
				}
			}

			UpdateGameState();
			NextPiece = previousRandomPiece == -1 ? PieceRandomizer.GetRandomPiece() : previousRandomPiece;
			CalculateBoardValue();
		}

		public List<Cell> GetNeighbours(Cell cell)
		{
			List<Cell> neighbours = new List<Cell>();

			if (cell.PosX + 1 < Size)
				neighbours.Add(Cells[cell.PosY, cell.PosX + 1]);
			if (cell.PosY + 1 < Size)
				neighbours.Add(Cells[cell.PosY + 1, cell.PosX]);
			if (cell.PosX - 1 >= 0)
				neighbours.Add(Cells[cell.PosY, cell.PosX - 1]);
			if (cell.PosY - 1 >= 0)
				neighbours.Add(Cells[cell.PosY - 1, cell.PosX]);

			return neighbours;
		}
		public List<Cell> GetMergeableCells()
		{
			List<Cell> mergeableCells = new List<Cell>();

			foreach (Cell cell in Cells)
			{
				List<Cell> neighbours = GetNeighbours(cell);

				foreach (Cell neighbour in neighbours)
				{
					if (IsMergeable(cell, neighbour))
					{
						mergeableCells.Add(cell);
						break;
					}
				}
			}

			return mergeableCells;
		}

		public bool IsMergeable(Cell firstCell,  Cell secondCell)
		{
			bool mergeable = true;

			if (firstCell == secondCell)
				mergeable = false;

			if (firstCell.Value != secondCell.Value)
				mergeable = false;

			if (firstCell.Value == 0)
				mergeable = false;

			if (!GetNeighbours(firstCell).Contains(secondCell))
				mergeable = false;

			return mergeable;
		}

		public void Merge(Cell mergingCell, Cell targetCell)
		{
			Cells[mergingCell.PosY, mergingCell.PosX] = new Cell(mergingCell.PosY, mergingCell.PosX);  // Resets old cell to an empty cell
			Cells[targetCell.PosY, targetCell.PosX].Value++;

			CalculateBoardValue();
		}

		public void CalculateBoardValue()
		{
			if (GameLost)
			{
				BoardValue = -1;
				return;
			}

			BoardValue = 0;

			foreach (Cell cell in Cells)
			{
				BoardValue += cell.Value;
			}

			foreach (Cell cell in GetMergeableCells())
			{
				BoardValue += MergeValueMultiplier * (cell.Value + 1);
			}
		}

		public List<Tuple<int, int>> GetEmptyCellPositions()
		{
			List<Tuple<int, int>> emptyCellPositions = new List<Tuple<int, int>>();

			foreach (Cell cell in Cells)
			{
				if (cell.Value == 0)
					emptyCellPositions.Add(new Tuple<int, int>(cell.PosY, cell.PosX));
			}

			return emptyCellPositions;
		}

		public void UpdateCell(int posY, int posX, int value)
		{
			Cells[posY, posX] = new Cell(posY, posX, value);
			CalculateBoardValue();
		}

		public void UpdateGameState()
		{
			GameLost = GetEmptyCellPositions().Count == 0 && GetMergeableCells().Count == 0;
		}
	}
}
