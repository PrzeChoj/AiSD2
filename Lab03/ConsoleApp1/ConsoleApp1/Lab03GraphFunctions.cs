using System;
using ASD.Graphs;
using ASD;
using System.Collections.Generic;

namespace ASD
{

    public class Lab03GraphFunctions : System.MarshalByRefObject
    {

        // Część 1
        // Wyznaczanie odwrotności grafu
        //   0.5 pkt
        // Odwrotność grafu to graf skierowany o wszystkich krawędziach przeciwnie skierowanych niż w grafie pierwotnym
        // Parametry:
        //   g - graf wejściowy
        // Wynik:
        //   odwrotność grafu
        // Uwagi:
        //   1) Graf wejściowy pozostaje niezmieniony
        //   2) Graf wynikowy musi być w takiej samej reprezentacji jak wejściowy
        public DiGraph Lab03Reverse(DiGraph g)
        {
            DiGraph outGraph = new DiGraph(g.VertexCount, g.Representation);

            foreach (Edge e in g.DFS().SearchAll())
            {
                outGraph.AddEdge(e.To, e.From); // Reversed order
            }

            return outGraph;
        }

        // Część 2
        // Badanie czy graf jest dwudzielny
        //   0.5 pkt
        // Graf dwudzielny to graf nieskierowany, którego wierzchołki można podzielić na dwa rozłączne zbiory
        // takie, że dla każdej krawędzi jej końce należą do róźnych zbiorów
        // Parametry:
        //   g - badany graf
        //   vert - tablica opisująca podział zbioru wierzchołków na podzbiory w następujący sposób
        //          vert[i] == 1 oznacza, że wierzchołek i należy do pierwszego podzbioru
        //          vert[i] == 2 oznacza, że wierzchołek i należy do drugiego podzbioru
        // Wynik:
        //   true jeśli graf jest dwudzielny, false jeśli graf nie jest dwudzielny (w tym przypadku parametr vert ma mieć wartość null)
        // Uwagi:
        //   1) Graf wejściowy pozostaje niezmieniony
        //   2) Podział wierzchołków może nie być jednoznaczny - znaleźć dowolny
        //   3) Pamiętać, że każdy z wierzchołków musi być przyporządkowany do któregoś ze zbiorów
        //   4) Metoda ma mieć taki sam rząd złożoności jak zwykłe przeszukiwanie (za większą będą kary!)
        public bool Lab03IsBipartite(Graph g, out int[] vert)
        {
            vert = new int[g.VertexCount];

            for (int i = 0; i < g.VertexCount; i++)
            {
                vert[i] = 1;
            }
            
            vert[0] = 1;
            foreach (Edge e in g.DFS().SearchFrom(0))
            {
                vert[e.To] = reverseVertex(vert[e.From]);
            }

            if (IsProperPartition(g, vert))
            {
                return true;
            }

            vert = null;
            return false;
        }
        
        private static bool IsProperPartition(Graph g, int[] part)
        {
            if (part == null || part.Length != g.VertexCount) return false;
            for (int v = 0; v < g.VertexCount; ++v)
                if (part[v] != 1 && part[v] != 2)
                    return false;
            for (int v = 0; v < g.VertexCount; ++v)
                foreach (int u in g.OutNeighbors(v))
                    if (part[u] == part[v])
                        return false;
            return true;
        }

        private int reverseVertex(int vFrom)
        {
            if (vFrom == 1)
            {
                return 2;
            }
            else if (vFrom == 2)
            {
                return 1;
            }

            throw new Exception("wrong input vFrom");
        }

        // Część 3
        // Wyznaczanie minimalnego drzewa rozpinającego algorytmem Kruskala
        //   1 pkt
        // Schemat algorytmu Kruskala
        //   1) wrzucić wszystkie krawędzie do "wspólnego worka"
        //   2) wyciągać z "worka" krawędzie w kolejności wzrastających wag
        //      - jeśli krawędź można dodać do drzewa to dodawać, jeśli nie można to ignorować
        //      - punkt 2 powtarzać aż do skonstruowania drzewa (lub wyczerpania krawędzi)
        // Parametry:
        //   g - graf wejściowy
        //   mstw - waga skonstruowanego drzewa (lasu)
        // Wynik:
        //   skonstruowane minimalne drzewo rozpinające (albo las)
        // Uwagi:
        //   1) Graf wejściowy pozostaje niezmieniony
        //   2) Wykorzystać klasę UnionFind z biblioteki Graph
        //   3) Jeśli graf g jest niespójny to metoda wyznacza las rozpinający
        //   4) Graf wynikowy (drzewo) musi być w takiej samej reprezentacji jak wejściowy
        public Graph<int> Lab03Kruskal(Graph<int> g, out int mstw)
        {
            var myPriorityQueue = new PriorityQueue<int, (int, int)>();

            for (int i = 0; i < g.VertexCount; i++)
            {
                foreach (var edge in g.OutEdges(i))
                {
                    myPriorityQueue.Insert((edge.From, edge.To), g.GetEdgeWeight(edge.From, edge.To));
                }
            }

            var myUnionFind = new ASD.UnionFind(g.VertexCount);
            Graph<int> outG = new Graph<int>(g.VertexCount);
            int wagaOutG = 0;
            
            try
            {
                while (true)
                {
                    var edge = myPriorityQueue.Extract();

                    if (myUnionFind.Find(edge.Item1) == myUnionFind.Find(edge.Item2))
                    {
                        continue; // Po dodaniu edge do outG mialbym cykl
                    }
                    
                    outG.AddEdge(edge.Item1, edge.Item2);
                    outG.AddEdge(edge.Item2, edge.Item1);
                    wagaOutG += g.GetEdgeWeight(edge.Item1, edge.Item2);
                    
                    myUnionFind.Union(edge.Item1, edge.Item2);
                }
            }
            catch (System.InvalidOperationException e)
            {
                // Kolejka jest pusta. Nic nie rob, wszystko jest ok
            }

            mstw = wagaOutG;
            return outG;
        }

        // Część 4
        // Badanie czy graf nieskierowany jest acykliczny
        //   0.5 pkt
        // Parametry:
        //   g - badany graf
        // Wynik:
        //   true jeśli graf jest acykliczny, false jeśli graf nie jest acykliczny
        // Uwagi:
        //   1) Graf wejściowy pozostaje niezmieniony
        //   2) Najpierw pomysleć jaki, prosty do sprawdzenia, warunek spełnia acykliczny graf nieskierowany
        //      Zakodowanie tego sprawdzenia nie powinno zająć więcej niż kilka linii!
        //      Zadanie jest bardzo łatwe (jeśli wydaje się trudne - poszukać prostszego sposobu, a nie walczyć z trudnym!)
        public bool Lab03IsUndirectedAcyclic(Graph g)
        {
            // Wystarczy policzyć ile spójnych składowych ma graf
            int numComponents = CountConnectedComponents(g);
            
            return (g.VertexCount - numComponents == g.EdgeCount);
        }
        
        public int CountConnectedComponents(Graph g)
        {
            HashSet<int> visited = new HashSet<int>();
            int numComponents = 0;
            for (int node = 0; node < g.VertexCount; node++)
            {
                if (!visited.Contains(node))
                {
                    numComponents++;
                    DFS(node, visited, g);
                }
            }
            return numComponents;
        }

        private void DFS(int node, HashSet<int> visited, Graph g)
        {
            visited.Add(node);
            foreach (int neighbor in g.OutNeighbors(node))
            {
                if (!visited.Contains(neighbor))
                {
                    DFS(neighbor, visited, g);
                }
            }
        }


    }

}
