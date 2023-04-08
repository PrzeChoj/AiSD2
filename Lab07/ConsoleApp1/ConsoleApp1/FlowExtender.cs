using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASD
{
    public static class FlowExtender
    {

        /// <summary>
        /// Metoda wylicza minimalny s-t-przekrój.
        /// </summary>
        /// <param name="undirectedGraph">Nieskierowany graf</param>
        /// <param name="s">wierzchołek źródłowy</param>
        /// <param name="t">wierzchołek docelowy</param>
        /// <param name="minCut">minimalny przekrój</param>
        /// <returns>wartość przekroju</returns>
        public static double MinCut(this Graph<double> undirectedGraph, int s, int t, out Edge<double>[] minCut)
        {
            DiGraph<double> directed = new DiGraph<double>(undirectedGraph.VertexCount, undirectedGraph.Representation);
            foreach (var e in undirectedGraph.BFS().SearchAll())
                directed.AddEdge(e.From, e.To, e.Weight); // To sa wobie strony, bo BFS zwraca krawedzie w obie strony

            (var val, var flow) = Flows.FordFulkerson(directed, s, t);

            List<Edge<double>> cut = new List<Edge<double>>();
            bool[] set = new bool[undirectedGraph.VertexCount];

            DiGraph<double> residual = new DiGraph<double>(flow.VertexCount, flow.Representation);

            foreach (var e in directed.BFS().SearchAll())
            {
                double f = flow.HasEdge(e.From, e.To) ? flow.GetEdgeWeight(e.From, e.To) : 0;
                double frev = flow.HasEdge(e.To, e.From) ? flow.GetEdgeWeight(e.To, e.From) : 0;

                double c = e.Weight - f + frev;
                if (c > 0) residual.AddEdge(e.From, e.To, c);

                if (!directed.HasEdge(e.To, e.From) && f > 0)
                {
                    residual.AddEdge(e.To, e.From, f);
                }
               
            }

            set[s] = true;
            foreach (var e in residual.BFS().SearchFrom(s))
            {
                set[e.To] = true;
            }

            // Krawedzie na pograniczu zbiorow S i T
            for (int i = 0; i < undirectedGraph.VertexCount; i++)
            {
                if (!set[i]) continue;

                cut.AddRange(undirectedGraph.OutEdges(i).Where(e => !set[e.To]));
            }

            minCut = cut.ToArray();

            return val;
        }

        /// <summary>
        /// Metada liczy spójność krawędziową grafu oraz minimalny zbiór rozcinający.
        /// </summary>
        /// <param name="undirectedGraph">nieskierowany graf</param>
        /// <param name="cutingSet">zbiór krawędzi rozcinających</param>
        /// <returns>spójność krawędziowa</returns>
        public static int EdgeConnectivity(this Graph<double> undirectedGraph, out Edge<double>[] cutingSet)
        {
            Edge<double>[] minCut = null!;
            double minVal = double.PositiveInfinity;
            for (int i=1; i<undirectedGraph.VertexCount; i++)
            {
                double val = undirectedGraph.MinCut(0, i, out var cut);
                if (val < minVal)
                {
                    minVal = val;
                    minCut = cut;
                }
            }
            cutingSet = minCut;
            return (int)minVal;
        }
    }
}
