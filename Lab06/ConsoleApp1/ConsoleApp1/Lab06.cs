using ASD;
using ASD.Graphs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Lab06
{
    public class HeroesSolver : MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - stwierdzenie, czy rozwiązanie istnieje
        /// </summary>
        /// <param name="g">graf przedstawiający mapę</param>
        /// <param name="keymasterTents">tablica krotek zawierająca pozycje namiotów klucznika - pierwsza liczba to kolor klucznika, druga to numer skrzyżowania</param>
        /// <param name="borderGates">tablica krotek zawierająca pozycje bram granicznych - pierwsza liczba to kolor bramy, dwie pozostałe to numery skrzyżowań na drodze między którymi znajduje się brama</param>
        /// <param name="p">ilość występujących kolorów (występujące kolory to 1,2,...,p)</param>
        /// <returns>bool - wartość true jeśli rozwiązanie istnieje i false wpp.</returns>
        public bool Lab06Stage1(Graph<int> g, (int color, int city)[] keymasterTents, (int color, int cityA, int cityB)[] borderGates, int p)
        {
            int n = g.VertexCount - 1; // wierzchołek 0 nie występuje w zadaniu
            int p2 = (int)Math.Pow(2, p); // UWAGA, bo kolory tez sa od 1 XD

            var myMap = new DiGraph<int>(n * p2 + 1); // p2 kopii mapy. Kazda kopia mowi jakiego rodzaje klucze mamy

            var youCanCollectKey = new HashSet<int>();
            
            // Zrobic przjscia gdy zbieramy klucz
            int[] keysList = new int[n+1]; // Mowi jakie klucze dostaniemy w i-tym

            foreach (var tent in keymasterTents)
            { // OPTIMIZE Móglbym uzyc przestawien bitowych, bo (1 << (tent.color-1)) == (int)Math.Pow(2, tent.color - 1), ale tak jest czytelniej
                keysList[tent.city] |= (int)Math.Pow(2, tent.color - 1); // -1, bo klucze bede kodowal od 0 pierwszy bit mowi o pierwszym kluczu
            }

            for (int i = 1; i < n+1; i++)
            {
                if (keysList[i] == 0)
                    continue;
                
                for (int j = 0; j < p2; j++) // OPTIMIZE tu bym mogl tylko pomowe sprawdzic tych, tak, zeby nie bylo tego ifa 2 linijki nizej
                {
                    if (j == (j | keysList[i]))
                        continue;
                    youCanCollectKey.Add(j * n + i);
                    myMap.AddEdge(j * n + i, (j | keysList[i]) * n + i, 0);
                }
            }
            
            foreach (var edge in g.BFS().SearchAll())
            {
                for (int i = 0; i < p2; i++)
                {
                    int vertex1 = i * n + edge.From;
                    int vertex2 = i * n + edge.To;
                    if(!youCanCollectKey.Contains(vertex1))
                        myMap.AddEdge(vertex1, vertex2, edge.Weight);
                    if(!youCanCollectKey.Contains(vertex2))
                        myMap.AddEdge(vertex2, vertex1, edge.Weight);
                }
            }
            
            // Usunac krawedzie z bramami
            foreach ((int color, int cityA, int cityB) borderGate in borderGates)
            {
                for (int i = 0; i < p2; i++)
                {
                    int vertex1 = i * n + borderGate.cityA;
                    int vertex2 = i * n + borderGate.cityB;

                    if(CanGo((vertex1 - 1) / n, borderGate.color))
                        continue;
                    // Ustawe flage waga == -1, ze nie mozna przejsc
                    // Ale, tak zrobie tylko jesli jest taka krawedz
                      // A moze jej nie byc, bo jest klucznik w vartexi
                    if(myMap.HasEdge(vertex1, vertex2))
                        myMap.SetEdgeWeight(vertex1, vertex2, -1);
                    if(myMap.HasEdge(vertex2, vertex1))
                        myMap.SetEdgeWeight(vertex2, vertex1, -1);
                }
            }

            // Kopje mapy gotowe
            
            var q = new Queue();
            q.Enqueue(1);
            var odwiedzone = new HashSet<int> { 1 };

            while (q.Count != 0)
            {
                var v = (int)q.Dequeue();
                if (v % n == 0)
                {
                    return true;
                }

                foreach (int outNeighbor in myMap.OutNeighbors(v))
                {
                    bool juzTuBylem = odwiedzone.Contains(outNeighbor);
                    bool zamknietaBrama = myMap.GetEdgeWeight(v, outNeighbor) == -1;
                    if (juzTuBylem || zamknietaBrama)
                        continue;
                    // OPTIMIZE Tu bym mogl sprawdzac, czy doszedlem do konca
                    odwiedzone.Add(outNeighbor);
                    q.Enqueue(outNeighbor);
                }
            }

            return false;
        }

        private static bool CanGo(int ownedKeys, int forbiddenColor)
        {
            return (ownedKeys & (int)Math.Pow(2, forbiddenColor - 1)) > 0; // Jesli moge przejsc, to (ownedKeys & 2^(forbiddenColor-1)) == 2^(forbiddenColor-1)
        }
        
        /// <summary>
        /// Etap 2 - stwierdzenie, czy rozwiązanie istnieje
        /// </summary>
        /// <param name="g">graf przedstawiający mapę</param>
        /// <param name="keymasterTents">tablica krotek zawierająca pozycje namiotów klucznika - pierwsza liczba to kolor klucznika, druga to numer skrzyżowania</param>
        /// <param name="borderGates">tablica krotek zawierająca pozycje bram granicznych - pierwsza liczba to kolor bramy, dwie pozostałe to numery skrzyżowań na drodze między którymi znajduje się brama</param>
        /// <param name="p">ilość występujących kolorów (występujące kolory to 1,2,...,p)</param>
        /// <returns>krotka (bool solutionExists, int solutionLength) - solutionExists ma wartość true jeśli rozwiązanie istnieje i false wpp. SolutionLenth zawiera długość optymalnej trasy ze skrzyżowania 1 do n</returns>
        public (bool solutionExists, int solutionLength) Lab06Stage2(Graph<int> g, (int color, int city)[] keymasterTents, (int color, int cityA, int cityB)[] borderGates, int p)
        {
            int n = g.VertexCount - 1; // wierzchołek 0 nie występuje w zadaniu
            int p2 = (int)Math.Pow(2, p); // UWAGA, bo kolory tez sa od 1 XD

            var myMap = new DiGraph<int>(n * p2 + 1); // p2 kopii mapy. Kazda kopia mowi jakiego rodzaje klucze mamy

            var youCanCollectKey = new HashSet<int>();

            // Zrobic przjscia gdy zbieramy klucz
            int[] keysList = new int[n+1]; // Mowi jakie klucze dostaniemy w i-tym

            foreach (var tent in keymasterTents)
            { // OPTIMIZE Móglbym uzyc przestawien bitowych, bo (1 << (tent.color-1)) == (int)Math.Pow(2, tent.color - 1), ale tak jest czytelniej
                keysList[tent.city] |= (int)Math.Pow(2, tent.color - 1); // -1, bo klucze bede kodowal od 0 pierwszy bit mowi o pierwszym kluczu
            }

            for (int i = 1; i < n+1; i++)
            {
                if (keysList[i] == 0)
                    continue;
                
                for (int j = 0; j < p2; j++) // OPTIMIZE tu bym mogl tylko pomowe sprawdzic tych, tak, zeby nie bylo tego ifa 2 linijki nizej
                {
                    if (j == (j | keysList[i]))
                        continue;
                    youCanCollectKey.Add(j * n + i);
                    myMap.AddEdge(j * n + i, (j | keysList[i]) * n + i, 0);
                }
            }
            
            foreach (var edge in g.BFS().SearchAll())
            {
                for (int i = 0; i < p2; i++)
                {
                    int vertex1 = i * n + edge.From;
                    int vertex2 = i * n + edge.To;
                    if(!youCanCollectKey.Contains(vertex1))
                        myMap.AddEdge(vertex1, vertex2, edge.Weight);
                    if(!youCanCollectKey.Contains(vertex2))
                        myMap.AddEdge(vertex2, vertex1, edge.Weight);
                }
            }
            
            // Usunac krawedzie z bramami
            foreach ((int color, int cityA, int cityB) borderGate in borderGates)
            {
                for (int i = 0; i < p2; i++)
                {
                    int vertex1 = i * n + borderGate.cityA;
                    int vertex2 = i * n + borderGate.cityB;

                    if(CanGo((vertex1 - 1) / n, borderGate.color))
                        continue;
                    // Ustawe flage waga == -1, ze nie mozna przejsc
                    // Ale, tak zrobie tylko jesli jest taka krawedz
                    // A moze jej nie byc, bo jest klucznik w vartexi
                    if(myMap.HasEdge(vertex1, vertex2))
                        myMap.SetEdgeWeight(vertex1, vertex2, -1);
                    if(myMap.HasEdge(vertex2, vertex1))
                        myMap.SetEdgeWeight(vertex2, vertex1, -1);
                }
            }
            // Kopje mapy gotowe

            var q = new Queue();
            q.Enqueue(1);
            var odwiedzone = new HashSet<int> { 1 };
            var krokWstecz = new Hashtable();

            while (q.Count != 0)
            {
                var v = (int)q.Dequeue();
                if (v % n == 0)
                {
                    return (true, GetTrasaLeangth(v, krokWstecz, myMap));
                }

                foreach (int outNeighbor in myMap.OutNeighbors(v))
                {
                    bool juzTuBylem = odwiedzone.Contains(outNeighbor);
                    bool zamknietaBrama = myMap.GetEdgeWeight(v, outNeighbor) == -1;
                    if (juzTuBylem || zamknietaBrama)
                        continue;
                    // OPTIMIZE Tu bym mogl sprawdzac, czy doszedlem do konca
                    odwiedzone.Add(outNeighbor);
                    q.Enqueue(outNeighbor);
                    krokWstecz.Add(outNeighbor, v); // Dla ustalenia scieszki
                }
            }

            return (false, 0);
        }

        private int GetTrasaLeangth(int outVertex, Hashtable krokWstecz, DiGraph<int> myMap)
        {
            int trasaLength = 0;
            int vertex = outVertex;

            while (vertex != 1)
            {
                int vertexPrev = (int)krokWstecz[vertex];
                trasaLength += myMap.GetEdgeWeight(vertexPrev, vertex);
                vertex = vertexPrev;
            }
            
            return trasaLength;
        }
    }
}
