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
            
            
            return null;
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
