using System;

namespace RobotLocalization
{
    public class Hmm
    {
        private readonly Matrix PriorMatrix;
        private readonly Matrix PosteriorMatrix;

        public Hmm()
        {
            PriorMatrix = new Matrix(AssignDefaultMap: true);
            PosteriorMatrix = new Matrix(AssignDefaultMap: true);
        }

        public void SetStartingValuesInGrid()                                       // Used to equally distribute probability across the grid
        {
            Console.WriteLine("Initial Location Probabilities");
            double probability = 1 / Constants.EMPTYSPOTS;
            for (int i = 0; i < Constants.ROWS; ++i)
            {
                for (int j = 0; j < Constants.COLS; ++j)
                {
                    PriorMatrix.AssignValueToCoordinate(i, j, probability);
                }
            }
        }

        public double Evidence(int west, int north, int east, int south)            // Evidence Cond. Prob P(Zt | St) Used by Filtering, Returns Denominator 
        {
            double[] directedProbability = new double[Constants.NOOFDIRECTIONS];    // Probabilities to be calculated at each directions of a cell
            for (int i = 0; i < Constants.NOOFDIRECTIONS; ++i)
            {
                directedProbability[i] = 0;                                         // Defaults to 0
            }

            double denominator = 0;                                                 // Stores total probability (described below)

            bool[] isWall = new bool[Constants.NOOFDIRECTIONS];                     // a.k.a Obstacles in the different directions ordered by WNES

            for (int i = 0; i < Constants.ROWS; ++i)
            {
                for (int j = 0; j < Constants.COLS; ++j)
                {
                    // Defaulting to NO walls (NO obstacles) at a given coordinate [i, j] at all directions WNES
                    for (int k = 0; k < Constants.NOOFDIRECTIONS; ++k)
                    {
                        isWall[k] = false;
                    }

                    // Check if [i, j] itself is NOT a wall
                    if (PriorMatrix.GetValueAtCoordinate(i, j) != Constants.WALL)
                    {
                        // Identifying if there is the border (edge) of the grid OR is an obstacle to the WEST of [i,j]
                        if ((j - 1 < 0) || (PriorMatrix.GetValueAtCoordinate(i, j - 1) == Constants.WALL))
                        {
                            isWall[(int)Directions.West] = true;
                        }

                        // Identifying if there is the border (edge) of the grid OR is an obstacle to the NORTH of [i,j]
                        if ((i - 1 < 0) || (PriorMatrix.GetValueAtCoordinate(i - 1, j) == Constants.WALL))
                        {
                            isWall[(int)Directions.North] = true;
                        }

                        // Identifying if there is the border (edge) of the grid OR is an obstacle to the EAST of [i,j]
                        if ((j + 1 >= Constants.COLS) || (PriorMatrix.GetValueAtCoordinate(i, j + 1) == Constants.WALL))
                        {
                            isWall[(int)Directions.East] = true;
                        }

                        // Identifying if there is the border (edge) of the grid OR is an obstacle to the SOUTH of [i,j]
                        if ((i + 1 >= Constants.ROWS) || (PriorMatrix.GetValueAtCoordinate(i + 1, j) == Constants.WALL))
                        {
                            isWall[(int)Directions.South] = true;
                        }

                        // If there is a wall to the WEST
                        if (isWall[(int)Directions.West])
                        {
                            // And is given in evidence that WEST is a wall [1, *, *, *],
                            // Assign the probability as Obstacle detected as required
                            if (west == Constants.OBSTACLE)
                            {
                                directedProbability[(int)Directions.West] = Constants.DETECTOBSTACLE;
                            }
                            // And is given in evidence that WEST is NOT a wall [0, *, *, *],
                            // Assign the probability as Obstacle NOT detected as required
                            else if (west == Constants._OBSTACLE)
                            {
                                directedProbability[(int)Directions.West] = Constants._DETECTOBSTACLE;
                            }
                        }
                        // If there is NOT a wall to the WEST
                        else
                        {
                            // And is given in evidence that WEST is NOT a wall [0, *, *, *],
                            // Assign the probability as Available spot detected as required
                            if (west == Constants._OBSTACLE)
                            {
                                directedProbability[(int)Directions.West] = Constants.AVAILABLESPOT;
                            }
                            // And is given in evidence that WEST is a wall [1, *, *, *],
                            // Assign the probability as Available spot NOT detected as required
                            else if (west == Constants.OBSTACLE)
                            {
                                directedProbability[(int)Directions.West] = Constants._AVAILABLESPOT;
                            }
                        }

                        // If there is a wall to the NORTH
                        if (isWall[(int)Directions.North])
                        {
                            // And is given in evidence that NORTH is a wall [*, 1, *, *],
                            // Assign the probability as Obstacle detected as required
                            if (north == Constants.OBSTACLE)
                            {
                                directedProbability[(int)Directions.North] = Constants.DETECTOBSTACLE;
                            }
                            // And is given in evidence that NORTH is NOT a wall [*, 0, *, *],
                            // Assign the probability as Obstacle NOT detected as required
                            else if (north == Constants._OBSTACLE)
                            {
                                directedProbability[(int)Directions.North] = Constants._DETECTOBSTACLE;
                            }
                        }
                        // If there is NOT a wall to the NORTH
                        else
                        {
                            // And is given in evidence that NORTH is NOT a wall [*, 0, *, *],
                            // Assign the probability as Available spot detected as required
                            if (north == Constants._OBSTACLE)
                            {
                                directedProbability[(int)Directions.North] = Constants.AVAILABLESPOT;
                            }
                            // And is given in evidence that NORTH is a wall [*, 1, *, *],
                            // Assign the probability as Available spot NOT detected as required
                            else if (north == Constants.OBSTACLE)
                            {
                                directedProbability[(int)Directions.North] = Constants._AVAILABLESPOT;
                            }
                        }

                        // If there is a wall to the EAST
                        if (isWall[(int)Directions.East])
                        {
                            // And is given in evidence that EAST is a wall [*, *, 1, *],
                            // Assign the probability as Obstacle detected as required
                            if (east == Constants.OBSTACLE)
                            {
                                directedProbability[(int)Directions.East] = Constants.DETECTOBSTACLE;
                            }
                            // And is given in evidence that EAST is NOT a wall [*, *, 0, *],
                            // Assign the probability as Obstacle NOT detected as required
                            else if (east == Constants._OBSTACLE)
                            {
                                directedProbability[(int)Directions.East] = Constants._DETECTOBSTACLE;
                            }
                        }
                        // If there is NOT a wall to the EAST
                        else
                        {
                            // And is given in evidence that EAST is NOT a wall [*, *, 0, *],
                            // Assign the probability as Available spot detected as required
                            if (east == Constants._OBSTACLE)
                            {
                                directedProbability[(int)Directions.East] = Constants.AVAILABLESPOT;
                            }
                            // And is given in evidence that EAST is a wall [*, *, 1, *],
                            // Assign the probability as Available spot NOT detected as required
                            else if (east == Constants.OBSTACLE)
                            {
                                directedProbability[(int)Directions.East] = Constants._AVAILABLESPOT;
                            }
                        }

                        // If there is NOT a wall to the SOUTH
                        if (isWall[(int)Directions.South])
                        {
                            // And is given in evidence that EAST is a wall [*, *, *, 1],
                            // Assign the probability as Obstacle detected as required
                            if (south == Constants.OBSTACLE)
                            {
                                directedProbability[(int)Directions.South] = Constants.DETECTOBSTACLE;
                            }
                            // And is given in evidence that EAST is NOT a wall [*, *, *, 0],
                            // Assign the probability as Obstacle NOT detected as required
                            else if (south == Constants._OBSTACLE)
                            {
                                directedProbability[(int)Directions.South] = Constants._DETECTOBSTACLE;
                            }
                        }
                        // If there is NOT a wall to the SOUTH
                        else
                        {
                            // And is given in evidence that SOUTH is NOT a wall [*, *, *, 0],
                            // Assign the probability as Available spot detected as required
                            if (south == Constants._OBSTACLE)
                            {
                                directedProbability[(int)Directions.South] = Constants.AVAILABLESPOT;
                            }
                            // And is given in evidence that SOUTH is a wall [*, *, *, 1],
                            // Assign the probability as Available spot NOT detected as required
                            else if (south == Constants.OBSTACLE)
                            {
                                directedProbability[(int)Directions.South] = Constants._AVAILABLESPOT;
                            }
                        }

                        // Probability of a cell = P(west) * P(north) * P(east) * P(south) = P(Zt = (W, N, E, S) | St)
                        double probability = directedProbability[(int)Directions.West] *
                                      directedProbability[(int)Directions.North] *
                                      directedProbability[(int)Directions.East] *
                                      directedProbability[(int)Directions.South];

                        PosteriorMatrix.AssignValueToCoordinate(i, j, probability);         // St is this coordinate

                        // Accumulates the Probability to divide by the [total probability] for e.g. P(a|b) = P(a|b) / [P(a|b) + P(-a|b)]
                        // Sigma E i=0 to ROWS ( Sigma E j=0 to COLS ( P of Posterior(i,j) * P of Prior(i,j) ) )
                        denominator += PosteriorMatrix.GetValueAtCoordinate(i, j) * PriorMatrix.GetValueAtCoordinate(i, j);
                    }
                }
            }
            return denominator;
        }

        private double Action(Directions direction, int x, int y)                           // Returns Transition Probability P(St | St-1) used by Prediction 
        {
            double probability = 0;

            if (direction == Directions.West)
            {
                // It calculates the probability of bumping into a wall towards the WEST
                if ((y - 1 < 0) || (PriorMatrix.GetValueAtCoordinate(x, y - 1) == Constants.WALL))
                {
                    probability += Constants.MOVEPROBABILITY * PriorMatrix.GetValueAtCoordinate(x, y);
                }
                // It calculates the probabilty if robot really could move towards the WEST
                if ((y + 1 < Constants.COLS) && (PriorMatrix.GetValueAtCoordinate(x, y + 1) != Constants.WALL))
                {
                    probability += Constants.MOVEPROBABILITY * PriorMatrix.GetValueAtCoordinate(x, y + 1);
                }

                // It calculates the probabilty if robot bounces back because of an obstacle at the NORTH
                if ((x - 1 < 0) || (PriorMatrix.GetValueAtCoordinate(x - 1, y) == Constants.WALL))
                {
                    probability += Constants.SLIPPROBABILITY * PriorMatrix.GetValueAtCoordinate(x, y);
                }
                // If it really could move there because there is available spot at the NORTH
                else
                {
                    probability += Constants.SLIPPROBABILITY * PriorMatrix.GetValueAtCoordinate(x - 1, y);
                }

                // It calculates the probabilty if robot bounces back because of an obstacle at the SOUTH
                if ((x + 1 >= Constants.ROWS) || (PriorMatrix.GetValueAtCoordinate(x + 1, y) == Constants.WALL))
                {
                    probability += Constants.SLIPPROBABILITY * PriorMatrix.GetValueAtCoordinate(x, y);
                }
                // If it really could move there because there is available spot at the SOUTH
                else
                {
                    probability += Constants.SLIPPROBABILITY * PriorMatrix.GetValueAtCoordinate(x + 1, y);
                }
            }

            else if (direction == Directions.North)
            {
                // It calculates the probability of bumping into a wall towards the NORTH
                if ((x - 1 < 0) || (PriorMatrix.GetValueAtCoordinate(x - 1, y) == Constants.WALL))
                {
                    probability += Constants.MOVEPROBABILITY * PriorMatrix.GetValueAtCoordinate(x, y);
                }
                // It calculates the probabilty if robot really could move towards the NORTH
                if ((x + 1 < Constants.ROWS) && (PriorMatrix.GetValueAtCoordinate(x + 1, y) != Constants.WALL))
                {
                    probability += Constants.MOVEPROBABILITY * PriorMatrix.GetValueAtCoordinate(x + 1, y);
                }

                // It calculates the probabilty if robot bounces back because of an obstacle at the EAST
                if ((y - 1 < 0) || (PriorMatrix.GetValueAtCoordinate(x, y - 1) == Constants.WALL))
                {
                    probability += Constants.SLIPPROBABILITY * PriorMatrix.GetValueAtCoordinate(x, y);
                }
                // If it really could move there because there is available spot at the EAST
                else
                {
                    probability += Constants.SLIPPROBABILITY * PriorMatrix.GetValueAtCoordinate(x, y - 1);
                }

                // It calculates the probabilty if robot bounces back because of an obstacle at the WEST
                if ((y + 1 >= Constants.COLS) || (PriorMatrix.GetValueAtCoordinate(x, y + 1) == Constants.WALL))
                {
                    probability += Constants.SLIPPROBABILITY * PriorMatrix.GetValueAtCoordinate(x, y);
                }
                // If it really could move there because there is available spot at the WEST
                else
                {
                    probability += Constants.SLIPPROBABILITY * PriorMatrix.GetValueAtCoordinate(x, y + 1);
                }
            }

            /* Although we are not using East and South in our driver code and also it is not required but we just 
             * implemented since these are analogous to the West and North which are already implemented */

            else if (direction == Directions.East)
            {
                // It calculates the probability of bumping into a wall towards the EAST
                if ((y + 1 >= Constants.COLS) || (PriorMatrix.GetValueAtCoordinate(x, y + 1) == Constants.WALL))
                {
                    probability += Constants.MOVEPROBABILITY * PriorMatrix.GetValueAtCoordinate(x, y);
                }
                // It calculates the probabilty if robot really could move towards the EAST
                if ((y - 1 >= 0) && (PriorMatrix.GetValueAtCoordinate(x, y - 1) != Constants.WALL))
                {
                    probability += Constants.MOVEPROBABILITY * PriorMatrix.GetValueAtCoordinate(x, y - 1);
                }

                // It calculates the probabilty if robot bounces back because of an obstacle at the NORTH
                if ((x - 1 < 0) || (PriorMatrix.GetValueAtCoordinate(x - 1, y) == Constants.WALL))
                {
                    probability += Constants.SLIPPROBABILITY * PriorMatrix.GetValueAtCoordinate(x, y);
                }
                // If it really could move there because there is available spot at the NORTH
                else
                {
                    probability += Constants.SLIPPROBABILITY * PriorMatrix.GetValueAtCoordinate(x - 1, y);
                }

                // It calculates the probabilty if robot bounces back because of an obstacle at the SOUTH
                if ((x + 1 >= Constants.ROWS) || (PriorMatrix.GetValueAtCoordinate(x + 1, y) == Constants.WALL))
                {
                    probability += Constants.SLIPPROBABILITY * PriorMatrix.GetValueAtCoordinate(x, y);
                }
                // If it really could move there because there is available spot at the SOUTH
                else
                {
                    probability += Constants.SLIPPROBABILITY * PriorMatrix.GetValueAtCoordinate(x + 1, y);
                }
            }

            else if (direction == Directions.South)
            {
                // It calculates the probability of bumping into a wall towards the SOUTH
                if ((x + 1 >= Constants.ROWS) || (PriorMatrix.GetValueAtCoordinate(x + 1, y) == Constants.WALL))
                {
                    probability += Constants.MOVEPROBABILITY * PriorMatrix.GetValueAtCoordinate(x, y);
                }
                // It calculates the probabilty if robot really could move towards the SOUTH
                if ((x - 1 >= 0) && (PriorMatrix.GetValueAtCoordinate(x - 1, y) != Constants.WALL))
                {
                    probability += Constants.MOVEPROBABILITY * PriorMatrix.GetValueAtCoordinate(x - 1, y);
                }

                // It calculates the probabilty if robot bounces back because of an obstacle at the EAST
                if ((y - 1 < 0) || (PriorMatrix.GetValueAtCoordinate(x, y - 1) == Constants.WALL))
                {
                    probability += Constants.SLIPPROBABILITY * PriorMatrix.GetValueAtCoordinate(x, y);
                }
                // If it really could move there because there is available spot at the EAST
                else
                {
                    probability += Constants.SLIPPROBABILITY * PriorMatrix.GetValueAtCoordinate(x, y - 1);
                }

                // It calculates the probabilty if robot bounces back because of an obstacle at the WEST
                if ((y + 1 >= Constants.COLS) || (PriorMatrix.GetValueAtCoordinate(x, y + 1) == Constants.WALL))
                {
                    probability += Constants.SLIPPROBABILITY * PriorMatrix.GetValueAtCoordinate(x, y);
                }
                // If it really could move there because there is available spot at the WEST
                else
                {
                    probability += Constants.SLIPPROBABILITY * PriorMatrix.GetValueAtCoordinate(x, y + 1);
                }
            }

            return probability;
        }

        public void Filter(int west, int north, int east, int south)
        {
            Console.WriteLine($"Filtering after Evidence: [{west}, {north}, {east}, {south}]");
            var Denominator = Evidence(west, north, east, south);
            for (int i = 0; i < Constants.ROWS; ++i)
            {
                for (int j = 0; j < Constants.COLS; ++j)
                {
                    // Calculating new probability
                    double value = (PosteriorMatrix.GetValueAtCoordinate(i, j) * PriorMatrix.GetValueAtCoordinate(i, j)) / Denominator;
                    // Assigning the new probability to the same matrix (Updating it)
                    PriorMatrix.AssignValueToCoordinate(i, j, value);
                }
            }
        }

        public void Predict(Directions direction)
        {
            Console.WriteLine($"Prediction after Action towards: {direction}");
            for (int i = 0; i < Constants.ROWS; ++i)
            {
                for (int j = 0; j < Constants.COLS; ++j)
                {
                    if (PriorMatrix.GetValueAtCoordinate(i, j) != Constants.WALL)
                    {
                        double value = Action(direction, i, j);
                        PosteriorMatrix.AssignValueToCoordinate(i, j, value);                   // Just using this matrix to store new values
                    }
                }
            }
            for (int i = 0; i < Constants.ROWS; ++i)
            {
                for (int j = 0; j < Constants.COLS; ++j)
                {
                    // Putting the values back to same grid (Updating it)
                    PriorMatrix.AssignValueToCoordinate(i, j, PosteriorMatrix.GetValueAtCoordinate(i, j));
                }
            }
        }

        public void ShowCurrentGrid()
        {
            PriorMatrix.PrintMatrix();
            Console.WriteLine();
        }
    }
}
