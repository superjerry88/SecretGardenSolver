namespace CookieSolver.Model
{
	public class Cell
	{
		public int Value {  get; set; }
		public int PosX { get; set; }
		public int PosY { get; set; }
		public bool Selected { get; set; }

		public Cell(int y, int x) { 
			Value = 0;
			PosX = x;
			PosY = y;
			Selected = false;
		}

		public Cell(int y, int x, int value)
		{
			Value = value;
			PosX = x;
			PosY = y;
			Selected = false;
		}
	}
}
