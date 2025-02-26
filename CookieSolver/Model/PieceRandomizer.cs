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

		public static Tuple<int, int> GetRandomPosition(List<Tuple<int, int>> possiblePositions)
		{
			return possiblePositions[random.Next(possiblePositions.Count)];
		}
	}
}
