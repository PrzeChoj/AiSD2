using ASD.Graphs.Testing;
using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ASD;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Lab08
{
    public abstract class Lab08TestCase : TestCase
    {


        protected readonly int n;
        protected readonly int days;
        protected readonly (int, int, int)[] paths;
        protected readonly (int, int)[] ships;

        protected readonly int expectedResult;
        protected int result;

        protected Lab08TestCase(int n, int days, (int from, int to, int weight)[] paths, (int where, int num_of_crews)[] ships, int solution, double timeLimit, string description) : base(timeLimit, null, description)
        {
            this.n = n;
            this.days = days;
            this.paths = ((int from, int to, int weight)[])paths.Clone();
            this.ships = ((int where, int num_of_crews)[])ships.Clone();
            this.expectedResult = solution;
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            var (code, msg) = checkSolution();
            return (code, $"{msg} [{this.Description}]");
        }

        protected (Result resultCode, string message) checkSolution()
        {
            if (!result.Equals(expectedResult))
                return (Result.WrongResult, $"Zwrócono {result} podczas gdy {this.expectedResult} było oczekiwanym wynikiem.");

            return OkResult("OK");
        }

        protected (Result resultCode, string message) OkResult(string message) => (TimeLimit < PerformanceTime ? Result.LowEfficiency : Result.Success, $"{message} {PerformanceTime.ToString("#0.00")}s");

    }

    public class Stage1TestCase : Lab08TestCase
    {
        public Stage1TestCase(int n, int days, (int from, int to, int weight)[] paths, (int where, int num_of_crews)[] ships, int solution, double timeLimit, string description) : base(n, days, paths, ships, solution, timeLimit, description) { }

        protected override void PerformTestCase(object prototypeObject)
        {
            result = (((ColumbExpedition)prototypeObject).Stage1(n, days, paths, ships));
            Console.WriteLine(result.ToString());
        }
    }

    public class Stage2TestCase : Lab08TestCase
    {
        public Stage2TestCase(int n, int days, (int from, int to, int weight)[] paths, (int where, int num_of_crews)[] ships, int solution, double timeLimit, string description) : base(n, days, paths, ships, solution, timeLimit, description) { }

        protected override void PerformTestCase(object prototypeObject)
        {
            result = (((ColumbExpedition)prototypeObject).Stage2(n, days, paths, ships));
            Console.WriteLine(result.ToString());
        }

    }

    public class Lab08Tests : TestModule
    {
        TestSet Stage1 = new TestSet(prototypeObject: new ColumbExpedition(), description: "Etap 1", settings: true);
        TestSet Stage2 = new TestSet(prototypeObject: new ColumbExpedition(), description: "Etap 1", settings: true);

        public override void PrepareTestSets()
        {
            TestSets["Stage1"] = Stage1;
            TestSets["Stage2"] = Stage2;

            prepare();
        }

        private void prepare()
        {
            //Przykład z zadania
            (int where, int num_of_crews)[] ships = { (0, 3), (1, 7) };
            (int from, int to, int weight)[] paths =
            {
                (1, 9, 10), (7, 9, 8), (8, 9, 3), (6, 8, 12), (5, 8, 5), (5, 7, 1), (4, 7, 5), (2, 4, 13), (2, 5, 8), (3, 5, 11), (3, 6, 7), (0, 3, 2), (0, 2, 11)
            };
            int n = 10;
            int daysStage1 = 2;
            int daysStage2 = 3;
            int resultStage1 = 9;
            int resultStage2 = 7;
            Stage1.TestCases.Add(new Stage1TestCase(n, daysStage1, paths, ships, resultStage1, 2, "Przykład z zadania"));
            Stage2.TestCases.Add(new Stage2TestCase(n, daysStage2, paths, ships, resultStage2, 2, "Przykład z zadania"));

            (int where_2, int num_of_crews_2)[] ships_2 = { (0, 10), (1, 10), (2, 10) };
            (int from_2, int to_2, int weight_2)[] paths_2 =
            {
            (0, 3, 11), (1, 3, 11), (2, 3, 11), (3, 4, 5), (3, 5, 5), (3, 6, 5), (3, 7, 5), (3, 8, 5), (3, 9, 5), (3, 10, 5), (3, 11, 5), (3, 12, 5)
            };
            int n_2 = 13;
            int daysStage1_2 = 5;
            int daysStage2_2 = 5;
            int resultStage1_2 = 13;
            int resultStage2_2 = 8;
            Stage1.TestCases.Add(new Stage1TestCase(n_2, daysStage1_2, paths_2, ships_2, resultStage1_2, 2, "Gwiazda"));
            Stage2.TestCases.Add(new Stage2TestCase(n_2, daysStage2_2, paths_2, ships_2, resultStage2_2, 2, "Gwiazda"));

            (int where_3, int num_of_crews_3)[] ships_3 = { (0, 20) };
            (int from_3, int to_3, int weight_3)[] paths_3 =
            {
            (0, 1, 0), (1, 2, 0), (2, 3, 0), (3, 4, 0), (4, 5, 0), (5, 6, 0), (6, 7, 0), (7, 8, 0), (8, 9, 0), (9, 10, 0), (10, 11, 0), (11, 12, 0), (12, 13, 0), (13, 14, 0), (14, 15, 0), (15, 16, 0), (16, 17, 0), (17, 18, 0), (18, 19, 13)
            };
            int n_3 = 20;
            int daysStage1_3 = 2;
            int daysStage2_3 = 2;
            int resultStage1_3 = 20;
            int resultStage2_3 = 19;
            Stage1.TestCases.Add(new Stage1TestCase(n_3, daysStage1_3, paths_3, ships_3, resultStage1_3, 2, "Ścieżka"));
            Stage2.TestCases.Add(new Stage2TestCase(n_3, daysStage2_3, paths_3, ships_3, resultStage2_3, 2, "Ścieżka"));

            (int where_5, int num_of_crews_5)[] ships_5 = { (0, 30), (20, 10), (40, 15), (60, 20) };
            (int from_5, int to_5, int weight_5)[] paths_5 =
            {
            (0, 1, 4), (0, 2, 6), (1, 3, 6), (1, 4, 8), (2, 5, 10), (3, 6, 3), (3, 7, 5), (4, 8, 7), (5, 9, 9), (6, 10, 2), (6, 11, 8), (7, 12, 6), (8, 13, 4), (9, 14, 5), (10, 15, 7), (11, 16, 9), (12, 17, 11), (13, 18, 3), (14, 19, 5), (15, 20, 7),
            (20, 21, 4), (20, 22, 6), (21, 23, 6), (21, 24, 8), (22, 25, 10), (23, 26, 3), (23, 27, 5), (24, 28, 7), (25, 29, 9), (26, 30, 2), (26, 31, 8), (27, 32, 6), (28, 33, 4), (29, 34, 5), (30, 35, 7), (31, 36, 9), (32, 37, 11), (33, 38, 3), (34, 39, 5), (35, 40, 7),
            (40, 41, 4), (40, 42, 6), (41, 43, 6), (41, 44, 8), (42, 45, 10), (43, 46, 3), (43, 47, 5), (44, 48, 7), (45, 49, 9), (46, 50, 2), (46, 51, 8), (47, 52, 6), (48, 53, 4), (49, 54, 5), (50, 55, 7), (51, 56, 9), (52, 57, 11), (53, 58, 3), (54, 59, 5), (55, 60, 7),
            (60, 61, 4), (60, 62, 6), (61, 63, 6), (61, 64, 8), (62, 65, 10), (63, 66, 3), (63, 67, 5), (64, 68, 7), (65, 69, 9), (66, 70, 2), (66, 71, 8), (67, 72, 6), (68, 73, 4), (69, 74, 5), (70, 75, 7), (71, 76, 9), (72, 77, 11), (73, 78, 3), (74, 79, 5)
};
            int n_5 = 80;
            int daysStage1_5 = 4;
            int daysStage2_5 = 4;
            int resultStage1_5 = 74;
            int resultStage2_5 = 61;
            Stage1.TestCases.Add(new Stage1TestCase(n_5, daysStage1_5, paths_5, ships_5, resultStage1_5, 2, "Ciekawy rzadko połączony graf z 80 wierzchołkami"));
            Stage2.TestCases.Add(new Stage2TestCase(n_5, daysStage2_5, paths_5, ships_5, resultStage2_5, 2, "Ciekawy rzadko połączony graf z 80 wierzchołkami"));

            

            (int where_5, int num_of_crews_5)[] ships_7 = { (0, 100), (1, 100), (2, 100), (32, 4)};
            int n_7 = 33;
            List<(int from_5, int to_5, int weight_5)> paths_list_7 = new List<(int from_5, int to_5, int weight_5)>();

            for (int i = 12; i < 22; i++)
            {
                for (int j = i + 1; j < 22; j++)
                {
                    paths_list_7.Add((i, j, 1));
                }
            }

            for (int i = 22; i < 32; i++)
            {
                for (int j = i + 1; j < 32; j++)
                {
                    paths_list_7.Add((i, j, 1));
                }
            }

            paths_list_7.Add((13, 22, 12));
            paths_list_7.Add((32, 30, 12));

            for (int i = 0; i < 3; i++)
                for(int j = 0; j < 3; j++)
                    paths_list_7.Add((i, 3 + 3 * i + j, 1));

            for (int i = 3; i < 12; i++)
                paths_list_7.Add((i, 12, 12));


            int daysStage1_7 = 2;
            int daysStage2_7 = 5;
            int resultStage1_7 = 26;
            int resultStage2_7 = 20;
            Stage1.TestCases.Add(new Stage1TestCase(n_7, daysStage1_7, paths_list_7.ToArray(), ships_7, resultStage1_7, 2, ""));
            Stage2.TestCases.Add(new Stage2TestCase(n_7, daysStage2_7, paths_list_7.ToArray(), ships_7, resultStage2_7, 2, ""));

            int daysStage1_8 = 1;
            int daysStage2_8 = 3;
            int resultStage1_8 = 13;
            int resultStage2_8 = 17;
            Stage1.TestCases.Add(new Stage1TestCase(n_7, daysStage1_8, paths_list_7.ToArray(), ships_7, resultStage1_8, 2, ""));
            Stage2.TestCases.Add(new Stage2TestCase(n_7, daysStage2_8, paths_list_7.ToArray(), ships_7, resultStage2_8, 2, ""));

            (int where_9, int num_of_crews_9)[] ships_9 = { (0, 2), (5, 5), (10, 3), (15, 1) };
            (int from_9, int to_9, int weight_9)[] paths_9 =
            {
            (19, 15, 2), (14, 12, 12), (13, 17, 12), (5, 8, 12), (9, 19, 12), (17, 5, 12), (10, 12, 12), (17, 19, 2), (17, 3, 12), (16, 3, 12), (12, 7, 12), (1, 14, 12), (16, 14, 12), (16, 7, 12), (10, 2, 12), (18, 19, 12), (9, 3, 12), (2, 8, 12), (17, 7, 12), (6, 4, 12),
            (9, 16, 12), (8, 15, 12), (4, 5, 12), (14, 3, 12), (18, 2, 12), (14, 4, 12), (18, 12, 12), (12, 8, 12), (2, 7, 12), (12, 5, 12), (6, 8, 12), (0, 13, 12), (9, 4, 12), (16, 11, 12), (14, 6, 12), (4, 12, 12), (6, 3, 12), (9, 13, 12), (14, 19, 12)
            };
            int n_9 = 20;
            int daysStage1_9 = 3;
            int daysStage2_9 = 3;
            int resultStage1_9 = 11;
            int resultStage2_9 = 9;
            Stage1.TestCases.Add(new Stage1TestCase(n_9, daysStage1_9, paths_9, ships_9, resultStage1_9, 2, ""));
            Stage2.TestCases.Add(new Stage2TestCase(n_9, daysStage2_9, paths_9, ships_9, resultStage2_9, 2, ""));

            (int where_9, int num_of_crews_9)[] ships_10 = { (1, 5), (9, 5), (11, 5) };
            (int from_9, int to_9, int weight_9)[] paths_10 =
            {
            (1, 2, 2), (2, 3, 4), (2, 4, 10), (4,5,6), (5, 6,3), (5,7,7), (7, 8,13), (5, 9, 4), (5, 11, 12), (11, 10, 7), (9, 10, 8), (9, 0, 2), (0, 13, 5), (10, 13, 10), (13, 12, 10)
            };
            int n_10 = 14;
            int daysStage1_10 = 3;
            int daysStage2_10 = 3;
            int resultStage1_10 = 14;
            int resultStage2_10 = 13;
            Stage1.TestCases.Add(new Stage1TestCase(n_10, daysStage1_10, paths_10, ships_10, resultStage1_10, 2, ""));
            Stage2.TestCases.Add(new Stage2TestCase(n_10, daysStage2_10, paths_10, ships_10, resultStage2_10, 2, ""));


            (int where_5, int num_of_crews_5)[] ships_6 = { (0, 1), (1, 100), (2, 100), (3, 100) };
            int n_6 = 500;
            List<(int from_5, int to_5, int weight_5)> paths_list = new List<(int from_5, int to_5, int weight_5)>();

            for (int i = 0; i < n_6; i++)
            {
                for (int j = i + 1; j < n_6; j++)
                {
                    paths_list.Add((i, j, 12));
                }
            }

            int daysStage1_6 = 2;
            int daysStage2_6 = 2;
            int resultStage1_6 = 301;
            int resultStage2_6 = 7;
            Stage1.TestCases.Add(new Stage1TestCase(n_6, daysStage1_6, paths_list.ToArray(), ships_6, resultStage1_6, 15, ""));
            Stage2.TestCases.Add(new Stage2TestCase(n_6, daysStage2_6, paths_list.ToArray(), ships_6, resultStage2_6, 10, ""));

        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            var tests = new Lab08Tests();
            tests.PrepareTestSets();
            foreach(var ts in tests.TestSets)
            {
                ts.Value.PerformTests(verbose: true, checkTimeLimit: false);
            }
        }
    }


}
