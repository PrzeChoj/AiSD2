using System;
using System.Collections.Generic;
using System.Text;

namespace Lab02
{
    public class PatternMatching : MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - wyznaczenie trasy, zgodnie z którą robot przemieści się z pozycji poczatkowej (0,0) na pozycję docelową (n-1, m-1)
        /// </summary>
        /// <param name="n">wysokość prostokąta</param>
        /// <param name="m">szerokość prostokąta</param>
        /// <param name="obstacles">tablica ze współrzędnymi przeszkód</param>
        /// <returns>krotka (bool result, string path) - result ma wartość true jeżeli trasa istnieje, false wpp., path to wynikowa trasa</returns>
        public (bool result, string path) Lab02Stage1(int n, int m, (int, int)[] obstacles)
        {
            bool[,] dynamicPossible = new bool[n, m];
            string[,] dynamicPath  = new string[n, m];

            dynamicPath[0, 0] = "";
            
            /// Na poczatek wpisze w dynamicPossible False jesli jest przeszkoda i False jesli nie ma
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    dynamicPossible[i, j] = true;
                }
            }
            for (int k = 0; k < obstacles.Length; k++)
            {
                dynamicPossible[obstacles[k].Item1, obstacles[k].Item2] = false;
            }

            /// Teraz zaczne dynamicznie wypelniac obie macierze
            /// Na poczatek pierwszy wiersz oddzielnie
            for (int j = 1; j < m; j++)
            {
                if (!dynamicPossible[0, j])
                {
                    // dynamicPossible[0, j] juz ma false
                    dynamicPath[0, j] = "";
                    continue;
                }

                dynamicPossible[0, j] = dynamicPossible[0, j - 1];
                dynamicPath[0, j] = dynamicPath[0, j - 1] + "R";
            }
            
            /// Pierwszy wiersz jest ok
            /// Teraz pierwsza kolumna
            for (int i = 1; i < n; i++)
            {
                if (!dynamicPossible[i, 0])
                {
                    // dynamicPossible[i, 0] juz ma false
                    dynamicPath[i, 0] = "";
                    continue;
                }

                dynamicPossible[i, 0] = dynamicPossible[i - 1, 0];
                dynamicPath[i, 0] = dynamicPath[i - 1, 0] + "D";
            }
            
            /// Pierwszy wiersz i pierwsza kolumna juz sa ok
            /// Teraz dynamicznie reszte macierzy
            /// WIERSZOWO
            for (int i = 1; i < n; i++)
            {
                for (int j = 1; j < m; j++)
                {
                    if (!dynamicPossible[i, j])
                    {
                        // dynamicPossible[i, j] juz ma false
                        dynamicPath[i, j] = "";
                        continue;
                    }

                    if (dynamicPossible[i - 1, j])
                    {
                        // dynamicPossible[i, j] juz ma true
                        dynamicPath[i, j] = dynamicPath[i - 1, j] + "D";
                        continue;
                    }
                    
                    if (dynamicPossible[i, j - 1])
                    {
                        // dynamicPossible[i, j] juz ma true
                        dynamicPath[i, j] = dynamicPath[i, j - 1] + "R";
                        continue;
                    }
                    
                    // nie da sie dojsc
                    dynamicPossible[i, j] = false;
                    dynamicPath[i, j] = "";
                }
            }
            
            return (dynamicPossible[n-1, m-1], dynamicPath[n-1, m-1]);
        }

        /// <summary>
        /// Etap 2 - wyznaczenie trasy realizującej zadany wzorzec, zgodnie z którą robot przemieści się z pozycji poczatkowej (0,0) na pozycję docelową (-n-1, m-1)
        /// </summary>
        /// <param name="n">wysokość prostokąta</param>
        /// <param name="m">szerokość prostokąta</param>
        /// <param name="pattern">zadany wzorzec</param>
        /// <param name="obstacles">tablica ze współrzędnymi przeszkód</param>
        /// <returns>krotka (bool result, string path) - result ma wartość true jeżeli trasa istnieje, false wpp., path to wynikowa trasa</returns>
        public (bool result, string path) Lab02Stage2(int n, int m, string pattern, (int, int)[] obstacles)
        {
            return (false, "");
        }
    }
}