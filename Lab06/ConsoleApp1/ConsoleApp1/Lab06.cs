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

            foreach (var edge in g.BFS().SearchAll())
            {
                for (int i = 0; i < p2; i++)
                { // TODO Dodawac tylko te, co nie maja blokad
                    myMap.AddEdge(i * n + edge.From, i * n + edge.To, edge.Weight);
                    myMap.AddEdge(i * n + edge.To, i * n + edge.From, edge.Weight);
                }
            }
            // Kopje mapy gotowe
            
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
                    myMap.AddEdge(j * n + i, (j | keysList[i]) * n + i, 0); // OPTIMIZE zapisac, ze z tego wierzcholka jest krawedz o wadze 0 i tylko nia pozniej isc w Djikstr-ze
                }
            }
            
            // zakazac przechodzenia na bramach bede robil w Djikstr-ie
            var q = new Queue();
            q.Enqueue(1);
            var odwiedzone = new HashSet<int> { 1 };

            while (q.Count != 0)
            {
                var v = (int)q.Dequeue()!;
                if (v % n == 0)
                {
                    return true;
                }

                foreach (int outNeighbor in myMap.OutNeighbors(v))
                {
                    bool juzTuBylem = odwiedzone.Contains(outNeighbor);
                    if (juzTuBylem)
                        continue;
                    if(JestBramaZamknieta(n, v, outNeighbor, borderGates))
                        continue;
                    // OPTIMIZE Tu bym mogl sprawdzac, czy doszedlem do konca
                    odwiedzone.Add(outNeighbor);
                    q.Enqueue(outNeighbor);
                    // TODO: Zapisywac skad przyszedlem dla ustalenia scieszki
                }
            }

            return false;
        }

        private static bool JestBramaZamknieta(int n, int from, int to, (int color, int cityA, int cityB)[] borderGates)
        {
            foreach ((int color, int cityA, int cityB) borderGate in borderGates)
            {
                if (borderGate.cityA != from || borderGate.cityB != to)
                    continue;
                if (!CanGo((from - 1) / n, borderGate.color))
                    return true;
            }

            return false;
        }

        private static bool CanGo(int ownedKeys, int forbiddenColor)
        {
            return (ownedKeys & forbiddenColor) > 0; // Jesli moge przejsc, to (ownedKeys & forbiddenColor) == forbiddenColor
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

            return (false, 0);
        }
    }
}
