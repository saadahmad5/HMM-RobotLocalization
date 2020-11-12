using System;

namespace RobotLocalization
{
    class Program
    {
        static void Main(string[] args)
        {
            // Driver code
            Console.WriteLine("Welcome to HMM-Robot Localization Program");
            Hmm hmm = new Hmm();

            // Equally distributed (Initial Location Probabilities)
            hmm.SetStartingValuesInGrid();
            hmm.ShowCurrentGrid();

            // Filtering after evidence of NO Obstacles at West, North, East and South respectively (Sensing)
            hmm.Filter(0, 0, 0, 0);
            hmm.ShowCurrentGrid();

            // Predicting after an action towards West (Moving)
            hmm.Predict(Directions.West);
            hmm.ShowCurrentGrid();

            // Filtering after evidence of Obstacles at West, North and South respectively (Sensing)
            hmm.Filter(1, 1, 0, 1);
            hmm.ShowCurrentGrid();

            // Predicting after an action towards North (Moving)
            hmm.Predict(Directions.North);
            hmm.ShowCurrentGrid();

            // Filtering after evidence of Obstacles at West, North and South respectively (Sensing)
            hmm.Filter(1, 1, 0, 1);
            hmm.ShowCurrentGrid();
        }
    }
}
