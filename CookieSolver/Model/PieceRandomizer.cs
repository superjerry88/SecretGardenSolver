namespace CookieSolver.Model
{
	public class PieceRandomizer
	{
		public static readonly int[] PossibleValues = { 1, 2, 3 };
		public static Random random = new Random();

		public static int GetRandomPiece()
		{
			return PossibleValues[random.Next(PossibleValues.Length)];
		}

		public static Cell GetRandomPosition(List<Cell> possiblePositions)
		{
			return possiblePositions[random.Next(possiblePositions.Count)];
		}
	}
}
