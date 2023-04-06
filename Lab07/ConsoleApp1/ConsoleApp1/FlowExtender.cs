using ASD.Graphs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASD
{
    public static class FlowExtender
    {

        /// <summary>
        /// Metod wylicza minimalny s-t-przekrój.
        /// </summary>
        /// <param name="undirectedGraph">Nieskierowany graf</param>
        /// <param name="s">wierzchołek źródłowy</param>
        /// <param name="t">wierzchołek docelowy</param>
        /// <param name="minCut">minimalny przekrój</param>
        /// <returns>wartość przekroju</returns>
        public static double MinCut(this Graph<double> undirectedGraph, int s, int t, out Edge<double>[] minCut)
        {
            DiGraph<double> directedGraph = new DiGraph<double>(undirectedGraph.VertexCount);
            foreach (Edge<double> edge in undirectedGraph.DFS().SearchAll())
            {
                undirectedGraph.AddEdge(edge.From, edge.To, edge.Weight);
                undirectedGraph.AddEdge(edge.To, edge.From, edge.Weight);
            }

            var (maxFlowValue, maxFlow) = Flows.FordFulkerson(undirectedGraph, s, t);
            
            // Mam maksymalny przeplyw
            // Zbuduje siec rezydualna
            
            DiGraph<double> residualNet = new DiGraph<double>(undirectedGraph.VertexCount);
            foreach (Edge<double> edge in undirectedGraph.DFS().SearchAll())
            {
                residualNet.AddEdge(edge.From, edge.To, edge.Weight);
            }

            foreach (Edge<double> edge in maxFlow.DFS().SearchAll())
            {
                bool jestForward = residualNet.HasEdge(edge.From, edge.To);
                if (!jestForward)
                {
                    throw new Exception("Nie powinno byc takich krawedzi. Jesli usunalem, to nie wrocilem do niej");
                } 
                
                bool jestBackward = residualNet.HasEdge(edge.To, edge.From);
                if (jestBackward)
                {
                    double oldEdgeWeightForward = residualNet.GetEdgeWeight(edge.From, edge.To);
                    double oldEdgeWeightBackward = residualNet.GetEdgeWeight(edge.To, edge.From);
                    if (oldEdgeWeightForward - edge.Weight == 0)
                    {
                        residualNet.RemoveEdge(edge.From, edge.To);
                    }
                    else
                    {
                        residualNet.SetEdgeWeight(edge.From, edge.To, oldEdgeWeightForward - edge.Weight);                        
                    }
                    residualNet.SetEdgeWeight(edge.To, edge.From, oldEdgeWeightBackward + edge.Weight);
                }
                else
                {
                    double oldEdgeWeightForward = residualNet.GetEdgeWeight(edge.From, edge.To);
                    residualNet.SetEdgeWeight(edge.From, edge.To, oldEdgeWeightForward - edge.Weight);
                    residualNet.AddEdge(edge.To, edge.From, edge.Weight);
                }
            }
            
            
            HashSet<int> SSet = new HashSet<int>(s);
            foreach (Edge<double> edge in residualNet.DFS().SearchFrom(s))
            {
                SSet.Add(edge.From);
            }
            
            HashSet<int> TSet = new HashSet<int>(s);
            for (int i = 0; i < directedGraph.VertexCount; i++)
            {
                if (!SSet.Contains(i))
                {
                    TSet.Add(i);
                }
            }
            
            // Wiem, ze przekroj to wrzystkie krawedzie między TSet, a SSet

            double outValue = 0;
            var minCut2 = new List<Edge<double>>();
            foreach (Edge<double> edge in residualNet.DFS().SearchFrom(s))
            {
                bool fromIsInS = SSet.Contains(edge.From);
                bool fromIsInT = TSet.Contains(edge.From);
                bool toIsInS = SSet.Contains(edge.From);
                bool toIsInT = TSet.Contains(edge.From);
                if ((fromIsInS && toIsInT) || (toIsInS && fromIsInT))
                {
                    outValue += edge.Weight;
                    minCut2.Add(edge);
                }
            }

            minCut = minCut2.ToArray();
            return outValue;
        }

        /// <summary>
        /// Metada liczy spójność krawędziową grafu oraz minimalny zbiór rozcinający.
        /// </summary>
        /// <param name="undirectedGraph">nieskierowany graf</param>
        /// <param name="cutingSet">zbiór krawędzi rozcinających</param>
        /// <returns>spójność krawędziowa</returns>
        public static int EdgeConnectivity(this Graph<double> undirectedGraph, out Edge<double>[] cutingSet)
        {
            cutingSet = null;
            return 0;
        }
        
    }
}
