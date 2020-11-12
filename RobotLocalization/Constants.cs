
namespace RobotLocalization
{
    public static class Constants
    {
        // Directions
        public const int NOOFDIRECTIONS = 4;

        // Matrix/ Grid
        public const int ROWS = 6;
        public const int COLS = 5;
        public const double WALL = -1;                                      // -1 in the grid represents a wall/ obstacle
        public const double VALUE = 0;                                      // 0 is the default value of a cell

        public const double NOOFWALLS = 6;                                  // Obstacles inside (used to calculate empty spots)

        public const double NOOFCELLS = ROWS * COLS;
        public const double EMPTYSPOTS = NOOFCELLS - NOOFWALLS;             // Needed to calculate equally distributed probability

        // Sensing
        public const double DETECTOBSTACLE = 0.75;                          // Given Probability to detect obstacle if there is
        public const double _DETECTOBSTACLE = 1 - DETECTOBSTACLE;           // Complementary

        public const double _AVAILABLESPOT = 0.2;                           // Given (MISTAKE which would occur) in analyzing an empty spot
        public const double AVAILABLESPOT = 1 - _AVAILABLESPOT;             // Complementary

        // Motion
        public const double MOVEPROBABILITY = 0.7;                          // Probability to move in the certain described direction
        public const double SLIPPROBABILITY = 0.15;                         // Probability to slip left or right of certain direction due to Windy situation

        // Obstacle
        public const int OBSTACLE = 1;                                      // IOW: Wall; an Obstacle
        public const int _OBSTACLE = 0;                                     // Spot/ Available Cell (NOT Obstacle)
    }

    public partial class Matrix
    {
        public Matrix(bool AssignDefaultMap) : this()
        {
            if (AssignDefaultMap)
            {
                _Matrix = new double[ROWS, COLS]                            // Initializing with the default map/ grid as specified
                {
                   {VALU, VALU, VALU, VALU, VALU},
                   {VALU, WALL, WALL, VALU, VALU},
                   {VALU, WALL, VALU, VALU, VALU},
                   {VALU, WALL, WALL, VALU, VALU},
                   {VALU, WALL, VALU, VALU, VALU},
                   {VALU, VALU, VALU, VALU, VALU}
                };
            }
        }
    }
}
