using ASD.Graphs;
using System;
using System.Collections.Generic;

namespace Lab10
{
    public class Lab10Solution : MarshalByRefObject
    {
        /// <summary>
        /// Wariant 1: Znajdź najtańszy zbiór wierzchołków grafu G 
        /// rozdzielający wszystkie pary wierzchołków z listy fanclubs 
        /// </summary>
        /// <param name="G">Graf prosty</param>
        /// <param name="fanclubs">Lista wierzchołków, które należy rozdzielić</param>
        /// <param name="cost">cost[v] to koszt użycia wierzchołka v; koszty są nieujemne</param>
        /// <param name="maxBudget">Górne ograniczenie na koszt rozwiązania</param>
        /// <returns></returns>
        public List<int> FindSeparatingSet(Graph G, List<int> fanclubs, int[] cost, int maxBudget)
        {
            int foundBestCost = Int32.MaxValue;
            bool[] foundBestStations = new bool[G.VertexCount];
            
            int currentCost = 0;
            bool[] currentStations = new bool[G.VertexCount];

            void FindCost(int v, int currentCost, bool[] currentStations)
            {
                if (v == G.VertexCount - 1) // Wszystkie sa ogarniete
                {
                    if (thisAreCorrectFanclubs(G, currentStations, fanclubs))
                    {
                        // Na pewno currentCost < foundBestCost, bo tu doszlismy 
                        foundBestCost = currentCost;
                        foundBestStations = (bool[])currentStations.Clone();
                    }

                    return;
                }
                
                // Zalozmy, ze do v (razem v) są juz ogarniete
                int potentialNewCost = currentCost + cost[v + 1];
                if (potentialNewCost > maxBudget || potentialNewCost >= foundBestCost)
                {
                    return;
                }

                int oldCost = currentCost;
                currentCost = potentialNewCost;
                currentStations[v + 1] = true;
                FindCost(v + 1, currentCost, currentStations);
                    
                currentCost = oldCost;
                currentStations[v + 1] = false;
                FindCost(v + 1, currentCost, currentStations);
            }

            FindCost(-1, currentCost, currentStations);

            if (foundBestCost == Int32.MaxValue)
            {
                return null;
            }

            var outStations = new List<int>();
            for (int i = 0; i < G.VertexCount; i++)
            {
                if (foundBestStations[i])
                {
                    outStations.Add(i);
                }
            }
            
            return outStations;
        }

        private static bool thisAreCorrectFanclubs(Graph G, bool[] currentStations, List<int> fanclubs)
        {
            // moj DFS, ktory szuka scierzek
            bool DFSOnlyOneFanclub(int funclub)
            {
                bool[] visited = (bool[])currentStations.Clone(); // Nie mozemy przejsc przez posterunki, wiec to tak jakbysmy juz w nich byli
                Stack<int> stack = new Stack<int>();

                visited[funclub] = true;
                stack.Push(funclub);

                int v;

                while (stack.Count != 0)
                {
                    v = stack.Pop();
                    
                    if (fanclubs.Contains(v) && v != funclub)
                        return false;

                    foreach (int i in G.OutNeighbors(v))
                    {
                        if (visited[i])
                            continue;
                        visited[i] = true;
                        stack.Push(i);
                    }
                }

                return true;
            }

            foreach (int fanclub in fanclubs)
            {
                if (!DFSOnlyOneFanclub(fanclub))
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Wariant 2: Znajdź najtańszy spójny zbiór wierzchołków grafu G 
        /// rozdzielający wszystkie pary wierzchołków z listy fanclubs 
        /// </summary>
        /// <param name="G">Graf prosty</param>
        /// <param name="fanclubs">Lista wierzchołków, które należy rozdzielić</param>
        /// <param name="cost">cost[v] to koszt użycia wierzchołka v; koszty są nieujemne</param>
        /// <param name="maxBudget">Górne ograniczenie na koszt rozwiązania</param>
        /// <returns></returns>
        public List<int> FindConnectedSeparatingSet(Graph G, List<int> fanclubs, int[] cost, int maxBudget)
        {
            return null;
        }
    }
}
