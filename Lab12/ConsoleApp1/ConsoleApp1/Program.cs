using System;
using ASD;

namespace Lab12
{
    public abstract class Lab12TestCase : TestCase
    {
        protected readonly ((int, int), (int, int))[] lines;

        protected Lab12TestCase(((int, int), (int, int))[] lines, double timeLimit, string description) : base(timeLimit, null, description)
        {
            this.lines = lines;
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            var (code, msg) = checkSolution();
            return (code, $"{msg} [{this.Description}]");
        }

        protected abstract (Result resultCode, string message) checkSolution();

        public (Result resultCode, string message) OkResult(string message) => (TimeLimit < PerformanceTime ? Result.LowEfficiency : Result.Success, $"{message} {PerformanceTime.ToString("#0.00")}s");
    }

    public class Stage1TestCase : Lab12TestCase
    {
        protected readonly int expectedNumCrossings;
        protected int result;

        public Stage1TestCase(((int, int), (int, int))[] lines, int expectedNumCrossings, double timeLimit, string description) : base(lines, timeLimit, description)
        {
            this.expectedNumCrossings = expectedNumCrossings;
        }

        protected override void PerformTestCase(object prototypeObject)
        {
            result = ((Transport)prototypeObject).Lab12Stage1(lines);
        }

        protected override (Result resultCode, string message) checkSolution()
        {
            if (this.expectedNumCrossings != this.result)
            {
                return (Result.WrongResult, $"Zwrócono ({result}), powinno być ({expectedNumCrossings})");
            }

            return OkResult("OK");
        }
    }

    public class Stage2TestCase : Lab12TestCase
    {
        protected readonly double expectedMaximumDistance;
        protected readonly double eps;
        protected double result;

        public Stage2TestCase(((int, int), (int, int))[] lines, double expectedMaximumDistance, double eps, double timeLimit, string description) : base(lines, timeLimit, description)
        {
            this.expectedMaximumDistance = expectedMaximumDistance;
            this.eps = eps;
        }

        protected override void PerformTestCase(object prototypeObject)
        {
            result = ((Transport)prototypeObject).Lab12Stage2(lines);
        }

        protected override (Result resultCode, string message) checkSolution()
        {
            if (this.result < 0)
            {
                return (Result.WrongResult, $"Zwrócono negatywny dystans ({result}), powinno być ({expectedMaximumDistance})");
            }

            double absoluteDiff = Math.Abs(this.expectedMaximumDistance - this.result);
            if (absoluteDiff > this.eps)
            {
                return (Result.WrongResult, $"Zwrócono niepoprawny wynik ({result}), powinno być ({expectedMaximumDistance}), wartość bezwzględna różnicy ({absoluteDiff})");
            }

            return OkResult("OK");
        }
    }



    public class Lab12Tests : TestModule
    {
        TestSet Stage1 = new TestSet(prototypeObject: new Transport(), description: "Etap 1", settings: true);
        TestSet Stage2 = new TestSet(prototypeObject: new Transport(), description: "Etap 2", settings: true);

        public override void PrepareTestSets()
        {
            TestSets["Stage1"] = Stage1;
            TestSets["Stage2"] = Stage2;

            prepare();
        }

        private void addStage1(Stage1TestCase s1TestCase)
        {
            Stage1.TestCases.Add(s1TestCase);
        }

        private void addStage2(Stage2TestCase s2TestCase)
        {
            Stage2.TestCases.Add(s2TestCase);
        }


        private void prepare()
        {
            const double timeLimit = 1;
            const double eps = Transport.EPS;
            ((int, int), (int, int))[] lines;
            int expectedNumCrossings;
            double expectedMaximumDistance;
            string description;


            // TEST 1
            lines = new ((int, int), (int, int))[]
            {
                ((148, 167), (41, 22)), // AB
                ((124, 86), (65, 103)), // CD
                ((3, 60), (102, 45)), // EF
                ((40, 2), (25, 101)), // GH
                ((47, 142), (66, 123)), // IJ
            };
            expectedNumCrossings = 3;
            expectedMaximumDistance = 90.1027191598567;
            description = "Przykład z zadania";
            addStage1(new Stage1TestCase(lines, expectedNumCrossings, timeLimit, description));
            addStage2(new Stage2TestCase(lines, expectedMaximumDistance, eps, timeLimit, description));


            // TEST 2
            lines = new ((int, int), (int, int))[]
            {
                ((41, 22), (148, 167)),
                ((65, 103), (124, 86)),
                ((102, 45), (3, 60)),
                ((25, 101), (40, 2)),
                ((66, 123), (47, 142)),
            };
            expectedNumCrossings = 3;
            expectedMaximumDistance = 90.1027191598567;
            description = "Odwrócony przykład z zadania";
            addStage1(new Stage1TestCase(lines, expectedNumCrossings, timeLimit, description));
            addStage2(new Stage2TestCase(lines, expectedMaximumDistance, eps, timeLimit, description));


            // TEST 3
            lines = new ((int, int), (int, int))[]
            {
                ((0, 0), (1, 1)),
            };
            expectedNumCrossings = 0;
            expectedMaximumDistance = 1.4142135623731;
            description = "Jedna linia metra";
            addStage1(new Stage1TestCase(lines, expectedNumCrossings, timeLimit, description));
            addStage2(new Stage2TestCase(lines, expectedMaximumDistance, eps, timeLimit, description));


            // TEST 4
            lines = new ((int, int), (int, int))[]
            {
                ((0, 0), (5, 5)),
                ((20, 15), (45, 30)),
            };
            expectedNumCrossings = 0;
            expectedMaximumDistance = 29.1547594742265;
            description = "Dwie linie metra bez przecięcia";
            addStage1(new Stage1TestCase(lines, expectedNumCrossings, timeLimit, description));
            addStage2(new Stage2TestCase(lines, expectedMaximumDistance, eps, timeLimit, description));


            // TEST 5
            lines = new ((int, int), (int, int))[]
            {
                ((58, 362), (324, 96)),
                ((42, 42), (180, 408)),
                ((408, 180), (44, 82)),
            };
            expectedNumCrossings = 3;
            expectedMaximumDistance = 224.806152676015;
            description = "Trójkąt";
            addStage1(new Stage1TestCase(lines, expectedNumCrossings, timeLimit, description));
            addStage2(new Stage2TestCase(lines, expectedMaximumDistance, eps, timeLimit, description));


            // TEST 6
            lines = new ((int, int), (int, int))[]
            {
                ((88, 164), (242, 52)),
                ((84, 84), (340, 416)),
                ((162, 48), (418, 380)),
                ((220, 410), (412, 260)),
            };
            expectedNumCrossings = 4;
            expectedMaximumDistance = 273.55240539977;
            description = "Gmina Manhattan";
            addStage1(new Stage1TestCase(lines, expectedNumCrossings, timeLimit, description));
            addStage2(new Stage2TestCase(lines, expectedMaximumDistance, eps, timeLimit, description));


            // TEST 7
            lines = new ((int, int), (int, int))[]
            {
                ((0, 0), (2, 2)),
                ((4, 4), (8, 8)),
                ((10, 10), (12, 12))
            };
            expectedNumCrossings = 0;
            expectedMaximumDistance = 5.656854249492;
            description = "Współliniowe metro";
            addStage1(new Stage1TestCase(lines, expectedNumCrossings, timeLimit, description));
            addStage2(new Stage2TestCase(lines, expectedMaximumDistance, eps, timeLimit, description));


            // TEST 8
            lines = new ((int, int), (int, int))[]
            {
                ((2, 3), (8, 9)),
                ((1, 8), (9, 16)),
                ((3, 6), (10, 13)),
                ((0, 5), (7, 12)),
            };
            expectedNumCrossings = 0;
            expectedMaximumDistance = 11.3137084989848;
            description = "Równoległe metro";
            addStage1(new Stage1TestCase(lines, expectedNumCrossings, timeLimit, description));
            addStage2(new Stage2TestCase(lines, expectedMaximumDistance, eps, timeLimit, description));


            // TEST 9
            lines = new ((int, int), (int, int))[]
            {
                ((23, 61), (42, 42)),
                ((62, 43), (41, 22)),
                ((80, 4), (61, 23)),
                ((21, 21), (40, 2)),
                ((63, 63), (82, 44)),
                ((2, 40), (100, 5)),
                ((122, 46), (101, 25)),
            };
            expectedNumCrossings = 2;
            expectedMaximumDistance = 44.5982062419555;
            description = "Mała gmina";
            addStage1(new Stage1TestCase(lines, expectedNumCrossings, timeLimit, description));
            addStage2(new Stage2TestCase(lines, expectedMaximumDistance, eps, timeLimit, description));


            // TEST 10
            lines = new ((int, int), (int, int))[]
            {
                ((44, 82), (86, 124)),
                ((46, 122), (122, 46)),
                ((165, 108), (81, 24)),
                ((125, 106), (163, 68)),
                ((110, 205), (129, 186)),
                ((63, 63), (120, 6)),
                ((104, 85), (161, 28)),
                ((132, 246), (180, 9)),
                ((88, 164), (145, 107)),
                ((130, 206), (87, 144)),
                ((152, 247), (111, 225)),
                ((127, 146), (85, 104)),
                ((182, 49), (60, 3)),
                ((49, 182), (70, 203)),
                ((89, 184), (28, 161)),
                ((189, 189), (167, 148)),
            };
            expectedNumCrossings = 13;
            expectedMaximumDistance = 136.131149545367;
            description = "Sen o Skaryszewie";
            addStage1(new Stage1TestCase(lines, expectedNumCrossings, timeLimit, description));
            addStage2(new Stage2TestCase(lines, expectedMaximumDistance, eps, timeLimit, description));


            // TEST 11
            lines = new ((int, int), (int, int))[]
            {
                ((0, 0), (63, 63)),
                ((20, 1), (83, 64)),
                ((40, 2), (103, 65)),
                ((60, 3), (3, 60)),
                ((80, 4), (23, 61)),
                ((100, 5), (43, 62)),
            };
            expectedNumCrossings = 9;
            expectedMaximumDistance = 44.5477272147525;
            description = "Siatka";
            addStage1(new Stage1TestCase(lines, expectedNumCrossings, timeLimit, description));
            addStage2(new Stage2TestCase(lines, expectedMaximumDistance, eps, timeLimit, description));
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            var tests = new Lab12Tests();
            tests.PrepareTestSets();
            foreach (var ts in tests.TestSets)
            {
                ts.Value.PerformTests(verbose: true, checkTimeLimit: false);
            }

            Console.ReadKey();
        }
    }
}
