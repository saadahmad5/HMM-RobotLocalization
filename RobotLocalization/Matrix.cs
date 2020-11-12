using System;

namespace RobotLocalization
{
    public partial class Matrix
    {
        private const int ROWS = Constants.ROWS;
        private const int COLS = Constants.COLS;
        private const double WALL = Constants.WALL;
        private const double VALU = Constants.VALUE;

        private readonly double[,] _Matrix;

        public Matrix(double defaultValue = 0)
        {
            _Matrix = new double[ROWS, COLS];
            for (int i = 0; i < ROWS; ++i)
            {
                for (int j = 0; j < COLS; ++j)
                {
                    _Matrix[i, j] = defaultValue;                                   // Defaults to 0
                }
            }
        }

        public void AssignValueToCoordinate(int x, int y, double value)
        {
            if (_Matrix[x, y] != Constants.WALL)                                    // This will only assign the value if [x, y] is not a wall
            {
                _Matrix[x, y] = value;
            }
        }

        public double GetValueAtCoordinate(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < Constants.ROWS && y < Constants.COLS)       // Check for if we're inside the grid
            {
                return _Matrix[x, y];   // This would return either the value or wall/ obstacle
            }
            return Constants.WALL;      // We assume everything outside the grid is wall/ obstacle
        }

        public void PrintMatrix()
        {
            // Uses Unicode charset (Box Drawing characters) 
            Console.WriteLine("┌───────────────────────────────────────┐");
            for (int i = 0; i < ROWS; ++i)
            {
                Console.Write("│ ");
                for (int j = 0; j < COLS; ++j)
                {
                    // If it is a Wall/ Obstacle
                    if (_Matrix[i, j] == Constants.WALL)
                    {
                        // Represents a wall/ obstacle
                        Console.Write("─ ─ ─ │ ");
                    }
                    else
                    {
                        // Show the probability, forcing to show 2 digit number with 2 decimal places
                        Console.Write($"{_Matrix[i, j] * 100:00.00} │ ");           // Multiplying by 100 to show as percentage
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("└───────────────────────────────────────┘");
        }
    }
}
