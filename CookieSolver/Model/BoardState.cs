using System.Runtime.ConstrainedExecution;

namespace CookieSolver.Model
{
	public class BoardState
	{
		public double EmptyCellMultiplier { get; set; }
		public double CellValueMultiplier { get; set; }
		public double PossibleMergeMultiplier { get; set; }
		public double MergeChainMultiplier { get; set; }
		public int NextPiece { get; set; }
		public bool GameLost { get; set; }
		public int Size { get; set; }
		public Cell[,] Cells { get; set; }
		public List<Cell> CellList => Cells.Cast<Cell>().ToList(); // Cast 2d array to single list for easier iteration
		public double BoardValue { get; set; } // Used to evaluate the value of each board state, allowing algo to choose which board states to go to easier
		public BoardState(int size) : this(size, new Cell[size, size], -1) { }
		public BoardState(int size, Cell[,] cells) : this(size, cells, -1) { }
		public BoardState(int size, Cell[,] cells, int previousRandomPiece)
		{  // Copying from a previous board state
			Size = size;
			Cells = cells;
			EmptyCellMultiplier = 1;
			CellValueMultiplier = 1;
			PossibleMergeMultiplier = 1;
			MergeChainMultiplier = 1;

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
			var neighbours = new List<Cell>();

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

		public List<Cell> GetMergeableNeighbour(Cell cell)
		{
			// Note: I simplify the code with Linq
			return GetNeighbours(cell)
				.Where(neighbour => IsMergeable(cell, neighbour))
				.ToList();
		}

		public List<Cell> GetMergeableCells()
		{
			//Note .Any() is same as Count > 0
			return CellList.Where(cell => GetMergeableNeighbour(cell).Any())
				.ToList();
		}

		public bool IsMergeable(Cell firstCell, Cell secondCell)
		{
			// Note: Given the logic is to return false after failing a case, we can directly return false when the condition is not met
			if (firstCell == secondCell)
				return false;

			if (firstCell.Value != secondCell.Value)
				return false;

			if (firstCell.Value == 0)
				return false;

			if (!GetNeighbours(firstCell).Contains(secondCell))
				return false;

			return true;
		}

		public void Merge(Cell mergingCell, Cell targetCell)
		{
			//Note: Since you can already access the cell objects, you can directly update the values without reinitializing them
			mergingCell.Value = 0;
			targetCell.Value += 1;
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

			BoardValue += GetEmptyCellPositions()
				.Count * EmptyCellMultiplier;

			BoardValue += CellList
				.Select(cell => cell.Value * CellValueMultiplier)
				.Sum();

			BoardValue += GetMergeableCells()
				.Select(cell => cell.Value + 1)
				.Sum() * PossibleMergeMultiplier;

			BoardValue += GetHighestMergeChain() * MergeChainMultiplier;
		}

		public List<Cell> GetEmptyCellPositions()
		{
			// Note: you can use LINQ to filter the cells with value 0
			// Note2: Tuple<int,int> is less favourable due to the lack of context, reusing the cell class here is totally fine
			return CellList
				.Where(cell => cell.Value == 0)
				.ToList();
		}

		public void UpdateCell(Cell cell, int value)
		{
			// Note: you can directly replace the object value without reinitializing it, it also helps on the performance
			// Old: Cells[posY, posX] = new Cell(posY, posX, value);
			// New: Cells[posY, posX].Value = value;

			// Update: i replaced the array lookup and use the cell as param directly to make it cleaner
			cell.Value = value;
			CalculateBoardValue();
		}

		public void UpdateGameState()
		{
			GameLost = GetEmptyCellPositions().Count == 0 && GetMergeableCells().Count == 0;
		}

		public int GetHighestMergeChain()
		{
			// Merge chains can be get by the difference of the largest number in a chain and the smallest number in the chain
			// Edge cases where a chain can be exended by merging a cell in the middle is not considered as it requires going layers deep, being very expensive for calculating for every board state

			var highestMergeChain = 0;

			foreach (Cell cell in GetMergeableCells())
			{
				var baseValue = cell.Value;
				var highestValue = cell.Value;
				var checkedCells = new List<Cell>();
				var cellsToCheck = new Queue<Cell> ([cell]);

				// Get a list of neighbours of the current cell that is not already included in the queue or has not been checked yet
				var getNextNeighbours = (Cell cellToCheck) => GetNeighbours(cellToCheck)
					.Where(otherCell => !checkedCells.Contains(otherCell) && !cellsToCheck.Contains(otherCell))
					.Where(otherCell => otherCell.Value == cellToCheck.Value + 1)
					.ToList();

				do
				{
					var currentCell = cellsToCheck.Dequeue();
					checkedCells.Add(currentCell);
					var nextNeighbours = getNextNeighbours(currentCell);
					if (nextNeighbours.Count > 0)
						highestValue = Math.Max(highestValue, nextNeighbours.Max(otherCell => otherCell.Value));
					nextNeighbours.ForEach(otherCell => cellsToCheck.Enqueue(otherCell));
				} while (cellsToCheck.Count > 0);

				highestMergeChain = Math.Max(highestValue - baseValue + 1, highestMergeChain);
			}

			return highestMergeChain;
		}
	}
}
