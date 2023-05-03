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
            
            bool[] currentStations = new bool[G.VertexCount];

            void FindCost(int v, int currentCost)
            {
                // Zalozmy, ze do v (bez v) są juz ogarniete
                
                if (v == G.VertexCount) // Wszystkie sa ogarniete; v to nie wierzcholek
                {
                    if (currentCost < foundBestCost && ThisAreCorrectStations(G, currentStations, fanclubs))
                    {
                        foundBestCost = currentCost;
                        foundBestStations = (bool[])currentStations.Clone();
                    }

                    return;
                }
                
                int potentialNewCost = currentCost + cost[v];
                if (potentialNewCost <= maxBudget && potentialNewCost < foundBestCost)
                {
                    currentStations[v] = true;
                    FindCost(v + 1, potentialNewCost);
                    currentStations[v] = false;
                }
                
                FindCost(v + 1, currentCost);
            }

            FindCost(0, 0);

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

        private static bool ThisAreCorrectStations(Graph G, bool[] currentStations, List<int> fanclubs)
        {
            // moj DFS, ktory szuka scierzek
            bool DFSOnlyOneFanclub(int funclub)
            {
                if (currentStations[funclub])
                    return true; // On jest poza grafem
                
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
            // Heuristic on the order of the vertices
            // (the fanclubs will be last on the list, therefore will be the first to be checked):
            int[] verticesOrder = new int[G.VertexCount];

            bool[] fanclubsBoolTable = new bool[G.VertexCount];
            int myIndex = G.VertexCount - 1;
            foreach (int fanclub in fanclubs)
            {
                fanclubsBoolTable[fanclub] = true;
                verticesOrder[myIndex] = fanclub;
                myIndex--;
            }

            for (int i = 0; i < G.VertexCount; i++)
            {
                if (fanclubsBoolTable[i])
                    continue;
                verticesOrder[myIndex] = i;
                myIndex--;
            }

            // Start of the solution
            int foundBestCost = Int32.MaxValue;
            bool[] foundBestStations = new bool[G.VertexCount];
            
            bool[] currentStations = new bool[G.VertexCount];

            void FindCost(int v, int currentCost)
            {
                // Zalozmy, ze do v (bez v) są juz ogarniete
                
                if (v == G.VertexCount) // Wszystkie sa ogarniete; v to nie wierzcholek
                {
                    if (currentCost >= foundBestCost)
                        return;
                    if (!ThisSIsConnected(G, currentStations))
                        return;
                    if (!ThisAreCorrectStations(G, currentStations, fanclubs))
                        return;
                    
                    foundBestCost = currentCost;
                    foundBestStations = (bool[])currentStations.Clone();

                    return;
                }
                
                int potentialNewCost = currentCost + cost[verticesOrder[v]];
                if (potentialNewCost <= maxBudget && potentialNewCost < foundBestCost)
                {
                    currentStations[verticesOrder[v]] = true;
                    FindCost(v + 1, potentialNewCost);
                    currentStations[verticesOrder[v]] = false;
                }
                
                FindCost(v + 1, currentCost); // This has to be checked first. Last Lab test is 10 times faster
            }

            FindCost(0, 0);

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

        private static bool ThisSIsConnected(Graph G, bool[] stations)
        {
            int? firstStation = null;
            int numOfStations = 0;

            for (int i = stations.Length - 1; i >= 0; i--)
            {
                if (stations[i])
                {
                    numOfStations++;
                    firstStation = i;
                }
            }
            
            if (firstStation == null)
                return true;
            
            bool[] visited = new bool[G.VertexCount];
            Stack<int> stack = new Stack<int>();
            
            visited[(int)firstStation] = true;
            stack.Push((int)firstStation);

            int numOfVisitedStations = 1;
            int v;

            while (stack.Count != 0)
            {
                v = stack.Pop();

                for (int i = 0; i < G.VertexCount; i++)
                {
                    if(!G.HasEdge(v, i) || visited[i] || !stations[i])
                        continue;
                    numOfVisitedStations++;
                    visited[i] = true;
                    stack.Push(i);
                }
            }
            
            return numOfVisitedStations == numOfStations;
        }
    }
}
