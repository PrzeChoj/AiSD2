using System;
using System.Collections.Generic;
using ASD;
using ASD.Graphs;
using ASD.Graphs.Testing;

namespace Lab10
{
    public class Lab10TestCase : TestCase
    {


        protected readonly Graph G;
        protected readonly List<int> fanclubs;
        protected readonly int[] cost;
        protected readonly int maxBudget;
        protected readonly int correctAnswer;
        protected readonly bool connectedVariant;

        protected List<int> result;

        protected Lab10TestCase(Graph G, List<int> fanclubs, int[] cost, int maxBudget, int correctAnswer, bool connectedVariant, double timeLimit, string description) : base(timeLimit, null, description)
        {
            this.G = G;
            this.fanclubs = fanclubs;
            this.cost = cost;
            this.maxBudget = maxBudget;
            this.correctAnswer = correctAnswer;
            this.result = null;
            this.connectedVariant = connectedVariant;
        }

        private bool doesResultMakeSense(out string msg)
        {
            msg = "";
            bool[] inResult = new bool[G.VertexCount];
            foreach (int v in result)
            {
                if (v < 0 || v >= G.VertexCount)
                {
                    msg = $"Rozwiązanie zawiera wierzchołek {v}, który nie istnieje w grafie!";
                    return false;
                }
                if (inResult[v])
                {
                    msg = $"Rozwiązanie zawiera wierzchołek {v} więcej niż raz!";
                    return false;
                }
                inResult[v] = true;
            }
            return true;
        }

        private bool isResultSeparating(out string errorMsg)
        {
            errorMsg = "";
            bool[] inResult = new bool[G.VertexCount];
            foreach (int v in result)
                inResult[v] = true;
            UnionFind uf = new UnionFind(G.VertexCount);
            foreach (var e in G.DFS().SearchAll())
                if (!inResult[e.From] && !inResult[e.To])
                    uf.Union(e.From, e.To);
            for (int i = 0; i < fanclubs.Count; i++)
                for (int j = i + 1; j < fanclubs.Count; j++)
                    if (uf.Find(fanclubs[i]) == uf.Find(fanclubs[j]))
                    {
                        errorMsg = $"Wierzchołki {fanclubs[i]} oraz {fanclubs[j]} nie są rozdzielone!";
                        return false;
                    }
            return true;
        }

        private bool isResultConnected(out string errorMsg)
        {
            errorMsg = "";
            bool[] inResult = new bool[G.VertexCount];
            foreach (int v in result)
                inResult[v] = true;
            UnionFind uf = new UnionFind(G.VertexCount);
            foreach (var e in G.DFS().SearchAll())
                if (inResult[e.From] && inResult[e.To])
                    uf.Union(e.From, e.To);
            for (int i = 1; i < result.Count; i++)
                if (uf.Find(result[i]) != uf.Find(result[0]))
                {
                    errorMsg = $"Wierzchołki {result[i]} oraz {result[0]} z rozwiązania nie są połączone ścieżką!";
                    return false;
                }
            return true;
        }
        private bool resultHasCorrectCost(out string errorMsg)
        {
            int c = 0;
            foreach (int v in result)
                c += cost[v];
            if(c != correctAnswer)
            {
                errorMsg = $"Rozwiązanie kosztuje w sumie {c}, a powinno kosztować {correctAnswer}!";
                if (c < correctAnswer)
                    errorMsg += " (to może oznaczać błąd w testach, skontaktuj się z szefem!)";
                return false;
            }
            errorMsg = "";
            return true;
        }

        protected override (Result resultCode, string message) VerifyTestCase(object settings)
        {
            if (result == null)
                return (Result.WrongResult, "Jako wynik zwrócono null!");

            string msg;

            if (!doesResultMakeSense(out msg))
                return (Result.WrongResult, msg);

            if (!isResultSeparating(out msg))
                return (Result.WrongResult, msg);

            if (connectedVariant && !isResultConnected(out msg))
                return (Result.WrongResult, msg);

            if (!resultHasCorrectCost(out msg))
                return (Result.WrongResult, msg);

            return OkResult("OK");
        }

        protected override void PerformTestCase(object prototypeObject)
        {
            if (!connectedVariant)
                result = ((Lab10Solution)prototypeObject).FindSeparatingSet(G, fanclubs, cost, maxBudget);
            else
                result = ((Lab10Solution)prototypeObject).FindConnectedSeparatingSet(G, fanclubs, cost, maxBudget);
        }

        protected (Result resultCode, string message) OkResult(string message) => (TimeLimit < PerformanceTime ? Result.LowEfficiency : Result.Success, $"{message} {PerformanceTime.ToString("#0.00")}s");

    }

    public class Lab10Variant1TestCase : Lab10TestCase
    {
        public Lab10Variant1TestCase(Graph G, List<int> fanclubs, int[] cost, int maxBudget, int correctAnswer, double timeLimit, string description) : base(G, fanclubs, cost, maxBudget, correctAnswer, false, timeLimit, description)
        { 
        }
    }

    public class Lab10Variant2TestCase : Lab10TestCase
    {
        public Lab10Variant2TestCase(Graph G, List<int> fanclubs, int[] cost, int maxBudget, int correctAnswer, double timeLimit, string description) : base(G, fanclubs, cost, maxBudget, correctAnswer, true, timeLimit, description)
        {
        }
    }

    public class Lab08Tests : TestModule
    {
        TestSet Variant1 = new TestSet(prototypeObject: new Lab10Solution(), description: "Wariant 1", settings: true);
        TestSet Variant2 = new TestSet(prototypeObject: new Lab10Solution(), description: "Wariant 2", settings: true);
        //TestSet HomeVariant1 = new TestSet(prototypeObject: new Lab10Solution(), description: "Wariant 1 - Domowe", settings: true);
        //TestSet HomeVariant2 = new TestSet(prototypeObject: new Lab10Solution(), description: "Wariant 2 - Domowe", settings: true);

        public override void PrepareTestSets()
        {
            TestSets["Variant1"] = Variant1;
            TestSets["Variant2"] = Variant2;
            //TestSets["HomeVariant1"] = HomeVariant1;
            //TestSets["HomeVariant2"] = HomeVariant2;

            prepare();
        }

        private void prepare()
        {

            {
                string title = "Krótka ścieżka";
                int[] cost = { 10, 6, 4, 3, 5, 10 };
                int[] fanclubs = { 0, 5};
                Graph G = new Graph(6);
                for (int i = 0; i < 5; i++)
                    G.AddEdge(i, i + 1);
                Variant1.TestCases.Add(new Lab10Variant1TestCase(G, new List<int>(fanclubs), cost, 99, 3, 1, title));
                Variant2.TestCases.Add(new Lab10Variant2TestCase(G, new List<int>(fanclubs), cost, 99, 3, 1, title));
            }


            {
                string title = "Krótki cykl";
                int[] cost = { 10, 6, 4, 5, 3, 10, 2, 3 };
                int[] fanclubs = { 0, 3, 5 };
                Graph G = GraphExamples.Cycle(8);
                Variant1.TestCases.Add(new Lab10Variant1TestCase(G, new List<int>(fanclubs), cost, 99, 7, 1, title));
                Variant2.TestCases.Add(new Lab10Variant2TestCase(G, new List<int>(fanclubs), cost, 99, 18, 1, title));
            }

            {
                string title = "Przykład z zadania";
                int[] cost = { 4, 3, 4, 3, 4, 3, 4, 3, 4 };
                int[] fanclubs = { 0, 2, 6, 8 };
                Graph G = new Graph(@"
0:1 3
1:0 2 4 5
2:1 5
3:0 6 7
4:0 1 7
5:1 2 8
6:3 7
7:3 4 6 8
8:5 7");
                Variant1.TestCases.Add(new Lab10Variant1TestCase(G, new List<int>(fanclubs), cost, 99, 10, 1, title));
                Variant2.TestCases.Add(new Lab10Variant2TestCase(G, new List<int>(fanclubs), cost, 99, 14, 1, title));
            }

            {
                string title = "Mały przykład 1";
                int[] cost = { 4, 1, 1, 3, 1, 1, 4, 1, 1, 4 };
                int[] fanclubs = { 0, 6, 9 };
                Graph G = new Graph(@"
0:1 2
1:0 3
2:0 3
3:1 2 4 5 7 8
4:3 6
5:3 6
6:4 5
7:3 9
8:3 9
9:7 8");
                Variant1.TestCases.Add(new Lab10Variant1TestCase(G, new List<int>(fanclubs), cost, 99, 3, 1, title));
                Variant2.TestCases.Add(new Lab10Variant2TestCase(G, new List<int>(fanclubs), cost, 99, 3, 1, title));
            }


            {
                string title = "Mała potęga ściezki";
                int n = 10;
                int p = 3;
                Random random = new Random(13);
                int[] cost = new int[n];
                int[] fanclubs = { 0, n / 2, n - 1 };
                Graph G = new Graph(n);
                for (int i = 0; i < n; i++)
                {
                    cost[i] = 90 + random.Next(21);
                    if (5 * i > 2 * n && 5 * i < 3 * n)
                        cost[i] += 70;
                    for (int j = 1; j <= p && i + j < n; j++)
                        G.AddEdge(i, i + j);
                }
                cost[0] = cost[n - 1] = cost[n / 2] = 200 * p;
                Variant1.TestCases.Add(new Lab10Variant1TestCase(G, new List<int>(fanclubs), cost, 200 * (p + 1), 611, 1, title));
                Variant2.TestCases.Add(new Lab10Variant2TestCase(G, new List<int>(fanclubs), cost, 200 * (p + 1), 611, 1, title));
            }



            {
                string title = "Duża potęga ściezki";
                int n = 18;
                int p = 4;
                Random random = new Random(13);
                int[] cost = new int[n];
                int[] fanclubs = { 0, n / 2, n - 1 };
                Graph G = new Graph(n);
                for (int i = 0; i < n; i++)
                {
                    cost[i] = 90 + random.Next(21);
                    if (5 * i > 2 * n && 5 * i < 3 * n)
                        cost[i] += 70;
                    for (int j = 1; j <= p && i + j < n; j++)
                        G.AddEdge(i, i + j);
                }
                cost[0] = cost[n - 1] = cost[n / 2] = 200 * p;
                Variant1.TestCases.Add(new Lab10Variant1TestCase(G, new List<int>(fanclubs), cost, 800, 771, 5, title));
                Variant2.TestCases.Add(new Lab10Variant2TestCase(G, new List<int>(fanclubs), cost, 1000, 803, 5, title));
            }

            {
                string title = "Losowy graf";
                int n = 20;
                double p = 0.2;
                Random random = new Random(13);
                RandomGraphGenerator rgg = new RandomGraphGenerator(13);
                
                int[] cost = new int[n];
                List<int> fanclubs = new List<int>();
                Graph G = rgg.Graph(n, p);
                for (int i = 0; i < n; i++)
                {
                    cost[i] = random.Next(20) + 1;
                    if (random.Next(5) == 0)
                    {
                        fanclubs.Add(i);
                        cost[i] += 5;
                    }

                }
                Variant1.TestCases.Add(new Lab10Variant1TestCase(G, fanclubs, cost, 800, 23, 2, title));
                Variant2.TestCases.Add(new Lab10Variant2TestCase(G, fanclubs, cost, 1000, 34, 1, title));
            }

            {
                string title = "Losowy graf 2";
                int n = 18;
                double p = 0.15;
                Random random = new Random(1);
                RandomGraphGenerator rgg = new RandomGraphGenerator(1);

                int[] cost = new int[n];
                List<int> fanclubs = new List<int>();
                Graph G = rgg.Graph(n, p);
                for (int i = 0; i < n; i++)
                {
                    cost[i] = random.Next(20) + 1;
                    if (random.Next(4) == 0)
                    {
                        fanclubs.Add(i);
                        cost[i] += 5;
                    }

                }
                Variant1.TestCases.Add(new Lab10Variant1TestCase(G, fanclubs, cost, 100, 35, 2, title));
                Variant2.TestCases.Add(new Lab10Variant2TestCase(G, fanclubs, cost, 100, 35, 1, title));
            }

            {
                string title = "Specjalny test, mały budżet";
                int[] cost = { 20, 5, 5, 5, 9, 7, 8, 6, 5, 8, 6, 5, 4, 20, 4, 5, 5, 6, 7, 6, 2, 3, 4, 20, 9, 5, 6, 5, 6, 9, 20 };
                int[] fanclubs = { 0, 13, 23, 30 };
                Graph G = new Graph(@"
0:1 2
1:0 2 3
2:0 1 3 4
3:1 2 4 5
4:2 3 5
5:3 4 6
6:4 5 7
7:6 8
8:7 9
9:8 10 18
10:9 11
11:7 10 12 14 16 17 18
12:11 13
13:12 14
14:11 13 15
15:14 16
16:11 15 17
17:11 16 18
18:9 11 17 19
19:18 20 21 27
20:7 19 21 23 25
21:19 20 22 24
22:21 23
23:20 22 24 26
24:21 23 25
25:20 24 26
26:23 25 27
27:19 26 28
28:27 29 30
29:28 30
30:28 29");
                Variant1.TestCases.Add(new Lab10Variant1TestCase(G, new List<int>(fanclubs), cost, 16, 16, 8, title));
                Variant2.TestCases.Add(new Lab10Variant2TestCase(G, new List<int>(fanclubs), cost, 21, 19, 1, title));
            }


            {
                string title = "Specjalny test, duży budżet";
                int[] cost = { 20, 5, 5, 5, 9, 7, 8, 6, 5, 8, 6, 5, 4, 20, 4, 5, 5, 6, 7, 6, 2, 3, 4, 20, 9, 5, 6, 5, 6, 9, 20 };
                int[] fanclubs = { 0, 13, 23, 30 };
                Graph G = new Graph(@"
0:1 2
1:0 2 3
2:0 1 3 4
3:1 2 4 5
4:2 3 5
5:3 4 6
6:4 5 7
7:6 8
8:7 9
9:8 10 18
10:9 11
11:7 10 12 14 16 17 18
12:11 13
13:12 14
14:11 13 15
15:14 16
16:11 15 17
17:11 16 18
18:9 11 17 19
19:18 20 21 27
20:7 19 21 23 25
21:19 20 22 24
22:21 23
23:20 22 24 26
24:21 23 25
25:20 24 26
26:23 25 27
27:19 26 28
28:27 29 30
29:28 30
30:28 29");
                Variant1.TestCases.Add(new Lab10Variant1TestCase(G, new List<int>(fanclubs), cost, 100, 16, 30, title));
                Variant2.TestCases.Add(new Lab10Variant2TestCase(G, new List<int>(fanclubs), cost, 100, 19, 2, title));
            }
        }

    }



    class Program
    {
        static void Main(string[] args)
        {
            var tests = new Lab08Tests();
            tests.PrepareTestSets();
            foreach (var ts in tests.TestSets)
            {
                ts.Value.PerformTests(verbose: true, checkTimeLimit: false);
            }
        }
    }
}