namespace CookieSolver.Model
{
	public class PieceRandomizer
    {
        // To make predictable game for comparing logics
        public static int Seed { get; set; } = 69420; 

        // Weight is to make certain pieces less likely to appear
        public static readonly WeightedValue[] PossiblePieces =
        [
            new(1, 3),
            new(2, 3), 
            new(3, 1)
        ];

        // Make RNG seeded
        static Random _random = new Random(Seed);

		public static void SetSeed(int seed)
        {
            Seed = seed;
            _random = new Random(seed);
        }

        public static int GetRandomPiece()
        {
            var totalWeight = PossiblePieces.Sum(piece => piece.Weight);
            var randomValue = _random.Next(totalWeight);

            var currentWeightSum = 0;
            foreach (var piece in PossiblePieces)
            {
                currentWeightSum += piece.Weight;
                if (randomValue < currentWeightSum)
                {
                    return piece.Value;
                }
            }

            // Shouldn't reach here but just in-case
            throw new Exception("GetRandomPiece() logic may be broken ?");
        }

        public static Cell GetRandomPosition(List<Cell> possiblePositions)
		{
			return possiblePositions[_random.Next(possiblePositions.Count)];
		}
	}

    public struct WeightedValue(int value, int weight)
    {
        public int Value = value;
        public int Weight = weight;
    }
}
