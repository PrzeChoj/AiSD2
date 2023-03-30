using System;
using System.Linq;
using System.Text;
using ASD;
using ASD.Graphs;
using System.Collections.Generic;

namespace Lab06
{

    public abstract class Lab06TestCase : TestCase
    {
        protected readonly Graph<int> g;
        protected readonly bool expectedSolutionExists;
        protected readonly int expectedSolutionLength;

        protected readonly (int color, int city)[] keymasterTents;
        protected readonly (int color, int cityA, int cityB)[] borderGates;
        protected readonly int p;

        protected (bool returnedSolutionExists, int returnedSolutionLength) result;

        protected Lab06TestCase(Graph<int> g, bool solutionExists, int solutionLength, (int color, int city)[] keymasterTents, (int color, int cityA, int cityB)[] borderGates, int p, double timeLimit, string description) : 
            base(timeLimit, null, description)
        {
            this.g = (Graph<int>)g.Clone();
            this.expectedSolutionExists = solutionExists;
            this.expectedSolutionLength = solutionLength;
            this.keymasterTents = ((int color, int city)[])keymasterTents.Clone();
            this.borderGates = ((int color, int cityA, int cityB)[])borderGates.Clone();
            this.p = p;
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            var (code, msg) = checkSolution();
            return (code, $"{msg} [{this.Description}]");
        }

        protected (Result resultCode, string message) checkSolution()
        {
            if (!result.returnedSolutionExists.Equals(expectedSolutionExists))
            {
                if (expectedSolutionExists)
                {
                    return (Result.WrongResult, $"Podano, że trasa nie istnieje, co nie jest prawdą");
                }
                else
                {
                    return (Result.WrongResult, $"Podano, że trasa istnieje, co nie jest prawdą");
                }
            }
            var resultStatus = OkResult("OK");
            if (expectedSolutionExists)
            {
                resultStatus = solutionLengthMatch();
            }
            if (resultStatus.resultCode == Result.Success || resultStatus.resultCode == Result.LowEfficiency)
            {
                if (PerformanceTime > TimeLimit)
                {
                    return (Result.WrongResult, $"Podany wynik jest poprawny ale przekroczono limit czasowy {TimeLimit.ToString("#0.00")}s (Twoj czas: {PerformanceTime.ToString("#0.00")}s)");
                }
            }
            return resultStatus;
        }

        protected abstract (Result resultCode, string message) solutionLengthMatch();
        public (Result resultCode, string message) OkResult(string message) => (TimeLimit < PerformanceTime ? Result.LowEfficiency : Result.Success, $"{message} {PerformanceTime.ToString("#0.00")}s");
    }

    public class Stage1TestCase : Lab06TestCase
    {
        public Stage1TestCase(Graph<int> g, bool solutionExists, (int color, int city)[] keymasterTents, (int color, int cityA, int cityB)[] borderGates, int p, double timeLimit, string description) : 
            base(g, solutionExists, 0, keymasterTents, borderGates, p, timeLimit, description) { }

        protected override void PerformTestCase(object prototypeObject)
        {
            result = (((HeroesSolver)prototypeObject).Lab06Stage1(g, keymasterTents, borderGates, p), 0);
        }

        protected override (Result resultCode, string message) solutionLengthMatch()
        {
            return OkResult("OK");
        }
    }

    public class Stage2TestCase : Lab06TestCase
    {
        private readonly string pattern;
        public Stage2TestCase(Graph<int> g, bool solutionExists, int solutionLength, (int color, int city)[] keymasterTents, (int color, int cityA, int cityB)[] borderGates, int p, double timeLimit, string description) :
            base(g, solutionExists, solutionLength, keymasterTents, borderGates, p, timeLimit, description)
        { }

        protected override void PerformTestCase(object prototypeObject)
        {
            result = ((HeroesSolver)prototypeObject).Lab06Stage2(g, keymasterTents, borderGates, p);
        }

        protected override (Result resultCode, string message) solutionLengthMatch()
        {
   
            if (result.returnedSolutionLength.Equals(expectedSolutionLength)) 
            {
                return OkResult("OK");
            } 
            else 
            {
                return (Result.WrongResult, $"Zwrócona długość trasy ({result.returnedSolutionLength}) nie jest zgodna z oczekiwaną ({expectedSolutionLength})");
            }
        }
    }



    public class Lab06Tests : TestModule
    {
        TestSet Stage1 = new TestSet(prototypeObject: new HeroesSolver(), description: "Etap 1", settings: true);
        TestSet Stage2 = new TestSet(prototypeObject: new HeroesSolver(), description: "Etap 2", settings: true);

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
            //// Przykłady z zadania
            Graph<int> g1 = new Graph<int>(7);
            g1.AddEdge(1, 2, 2);
            g1.AddEdge(1, 4, 2);
            g1.AddEdge(2, 3, 9);
            g1.AddEdge(2, 5, 3);
            g1.AddEdge(4, 5, 5);
            g1.AddEdge(4, 6, 18);
            g1.AddEdge(5, 6, 3);
            (int color, int city)[] keymasterTents1 = new (int color, int city)[] { (1, 3), (3, 3), (2, 2), (1, 6) };
            (int color, int cityA, int cityB)[] borderGates1 = new (int color, int cityA, int cityB)[] { (2, 1, 4), (2, 4, 5), (3, 4, 5), (1, 5, 6), (2, 5, 6) };
            addStage1(new Stage1TestCase(g1, true, keymasterTents1, borderGates1, 3, 1, "TRASA ISTNIEJE: przykład 1. z zadania"));
            addStage2(new Stage2TestCase(g1, true, 24, keymasterTents1, borderGates1, 3, 1, "TRASA ISTNIEJE: przykład 1. z zadania"));

            Graph<int> g2 = new Graph<int>(7);
            g1.AddEdge(1, 2, 2);
            g1.AddEdge(1, 4, 2);
            g1.AddEdge(2, 3, 9);
            g1.AddEdge(2, 5, 3);
            g1.AddEdge(4, 5, 5);
            g1.AddEdge(4, 6, 18);
            g1.AddEdge(5, 6, 3);
            (int color, int city)[] keymasterTents2 = new (int color, int city)[] { (1, 3), (3, 3), (2, 2), (1, 6), (4, 4) };
            (int color, int cityA, int cityB)[] borderGates2 = new (int color, int cityA, int cityB)[] { (4, 1, 2), (3, 1, 4), (2, 4, 5), (3, 4, 5), (1, 5, 6), (2, 5, 6) };
            addStage1(new Stage1TestCase(g2, false, keymasterTents2, borderGates2, 4, 1, "TRASA NIE ISTNIEJE: przykład 2. z zadania"));
            addStage2(new Stage2TestCase(g2, false, 0, keymasterTents2, borderGates2, 4, 1, "TRASA NIE ISTNIEJE: przykład 2. z zadania"));

            Graph<int> g3 = new Graph<int>(5);
            g3.AddEdge(1, 4, 2);
            g3.AddEdge(3, 4, 5);
            g3.AddEdge(1, 2, 7);
            g3.AddEdge(2, 3, 3);
            (int color, int city)[] keymasterTents3 = new (int color, int city)[] { (3, 2), (4, 3), (1, 4), (2, 1) };
            (int color, int cityA, int cityB)[] borderGates3 = new (int color, int cityA, int cityB)[] { (1, 1, 4), (2, 3, 4), (3, 3, 4), (4, 3, 4), (2, 1, 2), (2, 2, 3), (3, 2, 3) };
            addStage1(new Stage1TestCase(g3, true, keymasterTents3, borderGates3, 4, 1, ""));
            addStage2(new Stage2TestCase(g3, true, 15, keymasterTents3, borderGates3, 4, 1, ""));

            addStage1(new Stage1TestCase(new Graph<int>(2), true, new (int color, int city)[0], new (int color, int cityA, int cityB)[0], 10, 1, "1 skrzyzowanie"));
            addStage2(new Stage2TestCase(new Graph<int>(2), true, 0, new (int color, int city)[0], new (int color, int cityA, int cityB)[0], 10, 1, "1 skrzyzowanie"));

            Graph<int> g4 = new Graph<int>(6);
            g4.AddEdge(5, 2, 1);
            g4.AddEdge(2, 3, 1);
            g4.AddEdge(3, 4, 1);
            g4.AddEdge(4, 5, 3);
            (int color, int city)[] keymasterTents4 = new (int color, int city)[] { (1, 2), (1, 3), (4, 3) };
            (int color, int cityA, int cityB)[] borderGates4 = new (int color, int cityA, int cityB)[] { (3, 5, 2), (1, 3, 4), (1, 4, 5), (2, 4, 5), (3, 4, 5) };
            addStage1(new Stage1TestCase(g4, false, keymasterTents4, borderGates4, 4, 1, ""));
            addStage2(new Stage2TestCase(g4, false, 0, keymasterTents4, borderGates4, 4, 1, ""));

            Graph<int> g5 = new Graph<int>(9);
            g5.AddEdge(1, 2, 4);
            g5.AddEdge(1, 3, 1);
            g5.AddEdge(1, 4, 3);
            g5.AddEdge(2, 5, 5);
            g5.AddEdge(2, 6, 6);
            g5.AddEdge(3, 6, 9);
            g5.AddEdge(4, 6, 1);
            g5.AddEdge(4, 7, 3);
            g5.AddEdge(5, 8, 1);
            g5.AddEdge(6, 8, 1);
            g5.AddEdge(7, 8, 4);
            (int color, int city)[] keymasterTents5 = new (int color, int city)[] { (3, 3), (3, 5), (4, 5), (2, 5), (2, 6) };
            (int color, int cityA, int cityB)[] borderGates5 = new (int color, int cityA, int cityB)[] { (1, 1, 3), (4, 1, 3), (1, 1, 4), (3, 3, 6), (1, 4, 6), (2, 4, 6), (3, 5, 8), (4, 5, 8), (2, 6, 8), (1, 7, 8), (4, 7, 8) };
            addStage1(new Stage1TestCase(g5, true, keymasterTents5, borderGates5, 4, 1, ""));
            addStage2(new Stage2TestCase(g5, true, 10, keymasterTents5, borderGates5, 4, 1, ""));

            Graph<int> g7 = new Graph<int>(13);
            g7.AddEdge(1, 3, 1);
            g7.AddEdge(7, 8, 3);
            g7.AddEdge(5, 6, 3);
            g7.AddEdge(3, 5, 2);
            g7.AddEdge(11, 12, 5);
            g7.AddEdge(1, 2, 3);
            g7.AddEdge(3, 7, 3);
            g7.AddEdge(2, 9, 4);
            g7.AddEdge(4, 5, 1);
            g7.AddEdge(1, 4, 1);
            g7.AddEdge(8, 11, 3);
            g7.AddEdge(6, 11, 2);
            g7.AddEdge(9, 10, 3);
            g7.AddEdge(10, 11, 4);
            (int color, int city)[] keymasterTents7 = new (int color, int city)[] { (2, 1), (3, 11), (1, 4), (2, 3), (4, 3), (2, 9), (2, 12), (3, 12), (4, 12), (2, 7), (2, 12) };
            (int color, int cityA, int cityB)[] borderGates7 = new (int color, int cityA, int cityB)[] { (3, 1, 3), (2, 7, 8), (3, 7, 8), (4, 3, 5), (1, 11, 12), (2, 11, 12), (3, 11, 12), (4, 11, 12), (2, 1, 3), (2, 3, 7), (3, 3, 7), (4, 3, 7), (2, 4, 5), (3, 4, 5), (4, 4, 5), (1, 1, 4), (2, 6, 11) };
            addStage1(new Stage1TestCase(g7, true, keymasterTents7, borderGates7, 4, 1, ""));
            addStage2(new Stage2TestCase(g7, true, 43, keymasterTents7, borderGates7, 4, 1, ""));

            Graph<int> g6 = new Graph<int>(201);
            g6.AddEdge(1, 2, 2);
            g6.AddEdge(2, 3, 3);
            g6.AddEdge(3, 4, 4);
            g6.AddEdge(4, 5, 5);
            g6.AddEdge(5, 6, 6);
            g6.AddEdge(6, 7, 7);
            g6.AddEdge(7, 8, 8);
            g6.AddEdge(8, 9, 9);
            g6.AddEdge(9, 10, 10);
            g6.AddEdge(10, 11, 11);
            g6.AddEdge(11, 12, 12);
            g6.AddEdge(12, 13, 13);
            g6.AddEdge(13, 14, 14);
            g6.AddEdge(14, 15, 15);
            g6.AddEdge(15, 16, 16);
            g6.AddEdge(16, 17, 17);
            g6.AddEdge(17, 18, 18);
            g6.AddEdge(18, 19, 19);
            g6.AddEdge(19, 20, 20);
            g6.AddEdge(20, 21, 21);
            g6.AddEdge(21, 22, 22);
            g6.AddEdge(22, 23, 23);
            g6.AddEdge(23, 24, 24);
            g6.AddEdge(24, 25, 25);
            g6.AddEdge(25, 26, 26);
            g6.AddEdge(26, 27, 27);
            g6.AddEdge(27, 28, 28);
            g6.AddEdge(28, 29, 29);
            g6.AddEdge(29, 30, 30);
            g6.AddEdge(30, 31, 31);
            g6.AddEdge(31, 32, 32);
            g6.AddEdge(32, 33, 33);
            g6.AddEdge(33, 34, 34);
            g6.AddEdge(34, 35, 35);
            g6.AddEdge(35, 36, 36);
            g6.AddEdge(36, 37, 37);
            g6.AddEdge(37, 38, 38);
            g6.AddEdge(38, 39, 39);
            g6.AddEdge(39, 40, 40);
            g6.AddEdge(40, 41, 41);
            g6.AddEdge(41, 42, 42);
            g6.AddEdge(42, 43, 43);
            g6.AddEdge(43, 44, 44);
            g6.AddEdge(44, 45, 45);
            g6.AddEdge(45, 46, 46);
            g6.AddEdge(46, 47, 47);
            g6.AddEdge(47, 48, 48);
            g6.AddEdge(48, 49, 49);
            g6.AddEdge(49, 50, 50);
            g6.AddEdge(50, 51, 51);
            g6.AddEdge(51, 52, 52);
            g6.AddEdge(52, 53, 53);
            g6.AddEdge(53, 54, 54);
            g6.AddEdge(54, 55, 55);
            g6.AddEdge(55, 56, 56);
            g6.AddEdge(56, 57, 57);
            g6.AddEdge(57, 58, 58);
            g6.AddEdge(58, 59, 59);
            g6.AddEdge(59, 60, 60);
            g6.AddEdge(60, 61, 61);
            g6.AddEdge(61, 62, 62);
            g6.AddEdge(62, 63, 63);
            g6.AddEdge(63, 64, 64);
            g6.AddEdge(64, 65, 65);
            g6.AddEdge(65, 66, 66);
            g6.AddEdge(66, 67, 67);
            g6.AddEdge(67, 68, 68);
            g6.AddEdge(68, 69, 69);
            g6.AddEdge(69, 70, 70);
            g6.AddEdge(70, 71, 71);
            g6.AddEdge(71, 72, 72);
            g6.AddEdge(72, 73, 73);
            g6.AddEdge(73, 74, 74);
            g6.AddEdge(74, 75, 75);
            g6.AddEdge(75, 76, 76);
            g6.AddEdge(76, 77, 77);
            g6.AddEdge(77, 78, 78);
            g6.AddEdge(78, 79, 79);
            g6.AddEdge(79, 80, 80);
            g6.AddEdge(80, 81, 81);
            g6.AddEdge(81, 82, 82);
            g6.AddEdge(82, 83, 83);
            g6.AddEdge(83, 84, 84);
            g6.AddEdge(84, 85, 85);
            g6.AddEdge(85, 86, 86);
            g6.AddEdge(86, 87, 87);
            g6.AddEdge(87, 88, 88);
            g6.AddEdge(88, 89, 89);
            g6.AddEdge(89, 90, 90);
            g6.AddEdge(90, 91, 91);
            g6.AddEdge(91, 92, 92);
            g6.AddEdge(92, 93, 93);
            g6.AddEdge(93, 94, 94);
            g6.AddEdge(94, 95, 95);
            g6.AddEdge(95, 96, 96);
            g6.AddEdge(96, 97, 97);
            g6.AddEdge(97, 98, 98);
            g6.AddEdge(98, 99, 99);
            g6.AddEdge(99, 100, 100);
            g6.AddEdge(100, 101, 101);
            g6.AddEdge(101, 102, 102);
            g6.AddEdge(102, 103, 103);
            g6.AddEdge(103, 104, 104);
            g6.AddEdge(104, 105, 105);
            g6.AddEdge(105, 106, 106);
            g6.AddEdge(106, 107, 107);
            g6.AddEdge(107, 108, 108);
            g6.AddEdge(108, 109, 109);
            g6.AddEdge(109, 110, 110);
            g6.AddEdge(110, 111, 111);
            g6.AddEdge(111, 112, 112);
            g6.AddEdge(112, 113, 113);
            g6.AddEdge(113, 114, 114);
            g6.AddEdge(114, 115, 115);
            g6.AddEdge(115, 116, 116);
            g6.AddEdge(116, 117, 117);
            g6.AddEdge(117, 118, 118);
            g6.AddEdge(118, 119, 119);
            g6.AddEdge(119, 120, 120);
            g6.AddEdge(120, 121, 121);
            g6.AddEdge(121, 122, 122);
            g6.AddEdge(122, 123, 123);
            g6.AddEdge(123, 124, 124);
            g6.AddEdge(124, 125, 125);
            g6.AddEdge(125, 126, 126);
            g6.AddEdge(126, 127, 127);
            g6.AddEdge(127, 128, 128);
            g6.AddEdge(128, 129, 129);
            g6.AddEdge(129, 130, 130);
            g6.AddEdge(130, 131, 131);
            g6.AddEdge(131, 132, 132);
            g6.AddEdge(132, 133, 133);
            g6.AddEdge(133, 134, 134);
            g6.AddEdge(134, 135, 135);
            g6.AddEdge(135, 136, 136);
            g6.AddEdge(136, 137, 137);
            g6.AddEdge(137, 138, 138);
            g6.AddEdge(138, 139, 139);
            g6.AddEdge(139, 140, 140);
            g6.AddEdge(140, 141, 141);
            g6.AddEdge(141, 142, 142);
            g6.AddEdge(142, 143, 143);
            g6.AddEdge(143, 144, 144);
            g6.AddEdge(144, 145, 145);
            g6.AddEdge(145, 146, 146);
            g6.AddEdge(146, 147, 147);
            g6.AddEdge(147, 148, 148);
            g6.AddEdge(148, 149, 149);
            g6.AddEdge(149, 150, 150);
            g6.AddEdge(150, 151, 151);
            g6.AddEdge(151, 152, 152);
            g6.AddEdge(152, 153, 153);
            g6.AddEdge(153, 154, 154);
            g6.AddEdge(154, 155, 155);
            g6.AddEdge(155, 156, 156);
            g6.AddEdge(156, 157, 157);
            g6.AddEdge(157, 158, 158);
            g6.AddEdge(158, 159, 159);
            g6.AddEdge(159, 160, 160);
            g6.AddEdge(160, 161, 161);
            g6.AddEdge(161, 162, 162);
            g6.AddEdge(162, 163, 163);
            g6.AddEdge(163, 164, 164);
            g6.AddEdge(164, 165, 165);
            g6.AddEdge(165, 166, 166);
            g6.AddEdge(166, 167, 167);
            g6.AddEdge(167, 168, 168);
            g6.AddEdge(168, 169, 169);
            g6.AddEdge(169, 170, 170);
            g6.AddEdge(170, 171, 171);
            g6.AddEdge(171, 172, 172);
            g6.AddEdge(172, 173, 173);
            g6.AddEdge(173, 174, 174);
            g6.AddEdge(174, 175, 175);
            g6.AddEdge(175, 176, 176);
            g6.AddEdge(176, 177, 177);
            g6.AddEdge(177, 178, 178);
            g6.AddEdge(178, 179, 179);
            g6.AddEdge(179, 180, 180);
            g6.AddEdge(180, 181, 181);
            g6.AddEdge(181, 182, 182);
            g6.AddEdge(182, 183, 183);
            g6.AddEdge(183, 184, 184);
            g6.AddEdge(184, 185, 185);
            g6.AddEdge(185, 186, 186);
            g6.AddEdge(186, 187, 187);
            g6.AddEdge(187, 188, 188);
            g6.AddEdge(188, 189, 189);
            g6.AddEdge(189, 190, 190);
            g6.AddEdge(190, 191, 191);
            g6.AddEdge(191, 192, 192);
            g6.AddEdge(192, 193, 193);
            g6.AddEdge(193, 194, 194);
            g6.AddEdge(194, 195, 195);
            g6.AddEdge(195, 196, 196);
            g6.AddEdge(196, 197, 197);
            g6.AddEdge(197, 198, 198);
            g6.AddEdge(198, 199, 199);
            g6.AddEdge(199, 200, 200);
            addStage1(new Stage1TestCase(g6, true, new (int color, int city)[0], new (int color, int cityA, int cityB)[0], 13, 15, "Duzy test czasowy"));
            addStage2(new Stage2TestCase(g6, true, 20099, new (int color, int city)[0], new (int color, int cityA, int cityB)[0], 13, 15, "Duzy test czasowy"));

        }

    }


    class Program
    {
        static void Main(string[] args)
        {
            var tests = new Lab06Tests();
            tests.PrepareTestSets();
            foreach (var ts in tests.TestSets)
            {
                ts.Value.PerformTests(verbose: true, checkTimeLimit: false);
            }
        }
    }
}
