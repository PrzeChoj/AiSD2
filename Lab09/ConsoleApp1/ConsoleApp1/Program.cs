
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

/// <summary>
/// Klasa rozszerzająca klasę Graph o rozwiązania problemów największej kliki i izomorfizmu grafów metodą pełnego przeglądu (backtracking)
/// </summary>
public static class Lab10GraphExtender
{
    /// <summary>
    /// Wyznacza największą klikę w grafie i jej rozmiar metodą pełnego przeglądu (backtracking)
    /// </summary>
    /// <param name="g">Badany graf</param>
    /// <param name="clique">Wierzchołki znalezionej największej kliki - parametr wyjściowy</param>
    /// <returns>Rozmiar największej kliki</returns>
    /// <remarks>
    /// Nie wolno modyfikować badanego grafu.
    /// </remarks>
    public static int MaxClique(this Graph g, out int[] clique)
    {
        int n = g.VertexCount;
        List<int> biggestClique, currentClique;
        biggestClique = new List<int>();
        currentClique = new List<int>();

        void expandClique(int currentIndex)
        {
            if (currentClique.Count > biggestClique.Count)
            {
                biggestClique.Clear();
                biggestClique.AddRange(currentClique);
            }
            for (int i = currentIndex; i < n; i++)
            {
                if (n - (i - 1) + currentClique.Count < biggestClique.Count) break;
                bool canExpand = true;
                
                // check if can expand
                foreach (int element in currentClique)
                {
                    if (!g.HasEdge(element, i))
                    {
                        canExpand = false;
                        break;
                    }
                }
                if (canExpand)
                {
                    currentClique.Add(i);
                    expandClique(i + 1);
                    currentClique.RemoveAt(currentClique.Count - 1);
                }
            }
        }
        
        expandClique(0);

        clique = biggestClique.ToArray();
        return biggestClique.Count;
    }

    /// <summary>
    /// Bada izomorfizm grafów metodą pełnego przeglądu (backtracking)
    /// </summary>
    /// <param name="g">Pierwszy badany graf</param>
    /// <param name="h">Drugi badany graf</param>
    /// <param name="map">Mapowanie wierzchołków grafu h na wierzchołki grafu g (jeśli grafy nie są izomorficzne to null) - parametr wyjściowy</param>
    /// <returns>Informacja, czy grafy g i h są izomorficzne</returns>
    /// <remarks>
    /// 1) Uwzględniamy wagi krawędzi
    /// 3) Nie wolno modyfikować badanych grafów.
    /// </remarks>
    /// 
    public static bool IsomorphismTest(this Graph<int> g, Graph<int> h, out int[] map)
    {
        map = null;
        if (g.VertexCount != h.VertexCount)
            return false;
        map = new int[g.VertexCount];
        bool[] used = new bool[g.VertexCount];

        bool FindMapping(int vh, int[] localMap)
        {
            if (vh == g.VertexCount)
                return true;
            for (int vg = 0; vg < g.VertexCount; ++vg)
            {
                bool bylemWWierzcholku = used[vg];
                bool vhMaTenSamStopienNizvg = h.OutNeighbors(vh).Count() == g.OutNeighbors(vg).Count();
                if (bylemWWierzcholku || !vhMaTenSamStopienNizvg)
                {
                    continue;
                }

                bool solutionExists = true;
                foreach (var edge in h.OutEdges(vh))
                {
                    if (edge.To >= vh || (g.HasEdge(localMap[edge.To], vg) && edge.Weight == g.GetEdgeWeight(localMap[edge.To], vg)))
                        continue;
                    solutionExists = false;
                    break;
                }

                if (!solutionExists) continue;
                
                used[vg] = true;
                localMap[vh] = vg;
                if (FindMapping(vh + 1, localMap)) return true;
                
                used[vg] = false;
            }

            return false;
        }

        if (FindMapping(0, map))
            return true;
        map = null;
        return false;
    }


    //public static bool IsomorphismTest(this Graph<int> g, Graph<int> h, out int[] map)
    //{
    //    map = null;
    //    if (g.VertexCount != h.VertexCount)
    //        return false;
    //    map = new int[g.VertexCount];
    //    bool[] used = new bool[g.VertexCount];

    //    bool FindMapping(int vh, int[] localMap)
    //    {
    //        int v;
    //        if (vh == g.VertexCount)
    //            return true;
    //        for (int vg = 0; vg < g.VertexCount; ++vg)
    //            if (!used[vg] && h.OutNeighbors(vh).Count() == g.OutNeighbors(vg).Count())
    //            {
    //                for (v = 0; v < vh; ++v)
    //                    if (g.HasEdge(localMap[v], vg) != h.HasEdge(v, vh) || (g.HasEdge(localMap[v], vg) && g.GetEdgeWeight(localMap[v], vg) != h.GetEdgeWeight(v, vh)))
    //                        break;
    //                if (v < vh)
    //                    continue;
    //                used[vg] = true;
    //                localMap[vh] = vg;
    //                if (FindMapping(vh + 1, localMap)) return true;
    //                used[vg] = false;
    //            }
    //        return false;
    //    }

    //    if (FindMapping(0, map))
    //        return true;
    //    map = null;
    //    return false;
    //}
}
