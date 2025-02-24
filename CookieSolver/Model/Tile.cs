using System.Drawing;

namespace CookieSolver.Model
{
    public class Tile
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public int[,] Cells { get; }

        public Tile(int size)
        {
            Row = size;
            Column = size;
            Cells = new int[size, size];

            // Initializing the cells
            for (var row = 0; row < size; row++)
            {
                for (var col = 0; col < size; col++)
                {
                    Cells[row, col] = 0;
                }
            }
        }

        public void Solve()
        {
            // Printing test
            for (var row = 0; row < Row; row++)
            {
                for (var col = 0; col < Column; col++)
                {
                    var value = Cells[row, col];
                    Console.WriteLine($"[{row}, {col}] = {value}");
                }
            }

            //todo something here
        }
    }
}
