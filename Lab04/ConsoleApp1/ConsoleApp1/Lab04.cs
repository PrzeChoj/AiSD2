using System;
using System.Collections;
using ASD.Graphs;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ASD
{
    public class Lab04 : MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - wyznaczanie numerów grup, które jest w stanie odwiedzić Karol, zapisując się na początku do podanej grupy
        /// </summary>
        /// <param name="graph">Ważony graf skierowany przedstawiający zasady dołączania do grup</param>
        /// <param name="start">Numer grupy, do której początkowo zapisuje się Karol</param>
        /// <returns>Tablica numerów grup, które może odwiedzić Karol, uporządkowana rosnąco</returns>
        public int[] Lab04Stage1(DiGraph<int> graph, int start)
        {
            var canGetIntoSet = new HashSet<(int, int)>(); // Mozemy sie dostac do item1 zaczynajac w item2
            var canGetIntoSetOnlyVertex = new HashSet<int>();
            var canGetIntoSetOld = new HashSet<(int, int)>(); // W poprzednim kroku
            var canGetIntoSetNew = new HashSet<(int, int)>(); // W tym kroku
            
            canGetIntoSet.Add((start, -1));
            canGetIntoSetOld.Add((start, graph.VertexCount)); // Nie -1, bo sztuczka optymalizacyjna, bo on jest szukany w edgesList
            canGetIntoSetOnlyVertex.Add(start);
            
            /// W glownej petli bede uzywal edgesList zamiast .OutNeighbors()
            /// edgesList[i,j] posiada info do ktorych wierzcholkow mozna pojsc z i, jeli wczesniej bylem w j
            var edgesList = new List<int>[graph.VertexCount,graph.VertexCount+1]; // +1, bo mozna przyjsc z "-1"
            for (int i = 0; i < graph.VertexCount; i++)
            {
                for (int j = 0; j < graph.VertexCount+1; j++)
                {
                    edgesList[i,j] = new List<int>();
                }
            }
            
            /// To wpisanie ponizej nie jestem pewny, czy jest optymalne, bo uzywam .Add()
            /// Jesli nie przejdzie, to bede kombinowal...
            foreach (Edge<int> edge in graph.DFS().SearchAll())
            {
                int vertexFromMinus1 = graph.GetEdgeWeight(edge.From, edge.To);
                if (vertexFromMinus1 == -1)
                {
                    vertexFromMinus1 = graph.VertexCount;
                }
                edgesList[edge.From, vertexFromMinus1].Add(edge.To);
            }

            while (canGetIntoSetOld.Count != 0)
            {
                // Dodaj znalezione ostatnio do calosci
                foreach (var newElemToCanGetIntoSet in canGetIntoSetOld)
                {
                    canGetIntoSet.Add(newElemToCanGetIntoSet);
                }

                // Dodaj nowe
                foreach (var canGetIntoSetOldElement in canGetIntoSetOld)
                {
                    foreach (int outNeighbor in edgesList[canGetIntoSetOldElement.Item1, canGetIntoSetOldElement.Item2])
                    {
                        bool wasThereAlready = canGetIntoSet.Contains((outNeighbor, canGetIntoSetOldElement.Item1));
                        if (!wasThereAlready)
                        {
                            canGetIntoSetNew.Add((outNeighbor, canGetIntoSetOldElement.Item1));
                            canGetIntoSet.Add((outNeighbor, canGetIntoSetOldElement.Item1));
                            canGetIntoSetOnlyVertex.Add(outNeighbor);
                        }
                    }
                }
                
                canGetIntoSetOld = canGetIntoSetNew;
                canGetIntoSetNew = new HashSet<(int, int)>();
            }

            int[] outTable = new int[canGetIntoSetOnlyVertex.Count];
            int outTableCount = 0;
            foreach (var valueCanGetInto in canGetIntoSetOnlyVertex)
            {
                outTable[outTableCount] = valueCanGetInto;
                outTableCount++;
            }
            
            Array.Sort(outTable);
            
            return outTable;
        }
        
        /// <summary>
        /// Etap 2 - szukanie możliwości przejścia z jednej z grup z `starts` do jednej z grup z `goals`
        /// </summary>
        /// <param name="graph">Ważony graf skierowany przedstawiający zasady dołączania do grup</param>
        /// <param name="starts">Tablica z numerami grup startowych (trasę należy zacząć w jednej z nich)</param>
        /// <param name="goals">Tablica z numerami grup docelowych (trasę należy zakończyć w jednej z nich)</param>
        /// <returns>(possible, route) - `possible` ma wartość true gdy istnieje możliwość przejścia, wpp. false, 
        /// route to tablica z numerami kolejno odwiedzanych grup (pierwszy numer to numer grupy startowej, ostatni to numer grupy docelowej),
        /// jeżeli possible == false to route ustawiamy na null</returns>
        public (bool possible, int[] route) Lab04Stage2(DiGraph<int> graph, int[] starts, int[] goals)
        {
            var goalsSet = new HashSet<int>(goals);
            var startsSet = new HashSet<int>(starts);
            
            var canGetIntoSet = new HashSet<(int, int)>(); // Mozemy sie dostac do item1 zaczynajac w item2
            var canGetIntoSetOld = new HashSet<(int, int)>(); // W poprzednim kroku
            var canGetIntoSetNew = new HashSet<(int, int)>(); // W tym kroku

            foreach (int START in starts)
            {
                if (goalsSet.Contains(START))
                {
                    return (true, new[] { START });
                }

                canGetIntoSet.Add((START, -1));
                canGetIntoSetOld.Add((START, graph.VertexCount)); // Nie -1, bo sztuczka optymalizacyjna, bo on jest szukany w edgesList
            }

            /// W glownej petli bede uzywal edgesHash zamiast .OutNeighbors()
            /// edgesHash[i,j] posiada info do ktorych wierzcholkow mozna pojsc z i, jeli wczesniej bylem w j
            var edgesList = new List<int>[graph.VertexCount,graph.VertexCount+1]; // +1, bo mozna przyjsc z "-1"
            for (int i = 0; i < graph.VertexCount; i++)
            {
                for (int j = 0; j < graph.VertexCount+1; j++)
                {
                    edgesList[i,j] = new List<int>();
                }
            }

            /// To wpisanie ponizej nie jestem pewny, czy jest optymalne, bo uzywam .Add()
            /// Jesli nie przejdzie, to bede kombinowal...
            foreach (Edge<int> edge in graph.DFS().SearchAll())
            {
                int vertexFromMinus1 = graph.GetEdgeWeight(edge.From, edge.To);
                if (vertexFromMinus1 == -1)
                {
                    vertexFromMinus1 = graph.VertexCount;
                }
                edgesList[edge.From, vertexFromMinus1].Add(edge.To);
            }


            while (canGetIntoSetOld.Count != 0)
            {
                // Dodaj znalezione ostatnio do calosci
                foreach (var newElemToCanGetIntoSet in canGetIntoSetOld)
                {
                    canGetIntoSet.Add(newElemToCanGetIntoSet);
                }

                // Dodaj nowe
                foreach (var canGetIntoSetOldElement in canGetIntoSetOld)
                {
                    foreach (int outNeighbor in edgesList[canGetIntoSetOldElement.Item1, canGetIntoSetOldElement.Item2])
                    {
                        bool wasThereAlready = canGetIntoSet.Contains((outNeighbor, canGetIntoSetOldElement.Item1));
                        if (!wasThereAlready)
                        {
                            if (goalsSet.Contains(outNeighbor))
                            {
                                // Znalazlem trase. Ignacy bedzie zadowolony
                                return (true, getPathAlt(graph, outNeighbor, canGetIntoSetOldElement.Item1, startsSet));
                            }
                            
                            canGetIntoSetNew.Add((outNeighbor, canGetIntoSetOldElement.Item1));
                            canGetIntoSet.Add((outNeighbor, canGetIntoSetOldElement.Item1));
                        }
                    }
                }
                
                canGetIntoSetOld = canGetIntoSetNew;
                canGetIntoSetNew = new HashSet<(int, int)>();
            }

            return (false, null);
        }

        private int[] getPathAlt(DiGraph<int> graph, int goal, int goalMinus1, HashSet<int> startsSet)
        {
            int dlugoscTrasy = 2;
            int vertexNow = goal;
            int vertexNowMinus1 = goalMinus1;
            int tmp;

            while (!startsSet.Contains(vertexNowMinus1) || -1 != graph.GetEdgeWeight(vertexNowMinus1, vertexNow))
            {
                tmp = vertexNowMinus1;
                vertexNowMinus1 = graph.GetEdgeWeight(vertexNowMinus1, vertexNow);
                vertexNow = tmp;
                dlugoscTrasy++;
            }

            
            int[] outPath = new int[dlugoscTrasy];

            dlugoscTrasy = outPath.Length-1;
            vertexNow = goal;
            vertexNowMinus1 = goalMinus1;
            while (dlugoscTrasy != 0)
            {
                outPath[dlugoscTrasy] = vertexNow;
                tmp = vertexNowMinus1;
                vertexNowMinus1 = graph.GetEdgeWeight(vertexNowMinus1, vertexNow);
                vertexNow = tmp;
                dlugoscTrasy--;
            }

            if (outPath.Length >= 3)
            {
                outPath[0] = graph.GetEdgeWeight(outPath[1], outPath[2]);                
            }
            else
            {
                outPath[0] = goalMinus1;
            }

            return outPath;
        }
    }
}
