namespace CookieSolver.Model
{
    public class Tile
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public Tile(int size)
        {
            Row = size;
            Column = size;

            // Initializing the cells
            for (var i = 0; i < size; i++)
            {
                Cells[i] = new Dictionary<int, int>();
                for (var j = 0; j < size; j++)
                {
                    Cells[i][j] = 0;
                }
            }
        }

        public Dictionary<int, Dictionary<int, int>> Cells = new();

        public void Solve()
        {
            // Printing test
            foreach (var cellRow in Cells)
            {
                foreach (var cell in cellRow.Value)
                {
                    Console.WriteLine($"[{cellRow.Key}, {cell.Key}] = {cell.Value}");
                }
            }

            //todo something here
        }
    }
}
