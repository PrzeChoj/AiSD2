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
            char[,] dynamicPath  = new char[n, m];
            
            /// Trasa zapisywana jako znak char.
            /// Jesli w dol,    to 'D'
            /// Jesli w prawo,  to 'R'
            /// Jak sie nie da, to 'N'

            dynamicPath[0, 0] = 'N';
            
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
                    dynamicPath[0, j] = 'N';
                    continue;
                }

                dynamicPossible[0, j] = dynamicPossible[0, j - 1];
                dynamicPath[0, j] = 'R';
            }
            
            /// Pierwszy wiersz jest ok
            /// Teraz pierwsza kolumna
            for (int i = 1; i < n; i++)
            {
                if (!dynamicPossible[i, 0])
                {
                    // dynamicPossible[i, 0] juz ma false
                    dynamicPath[i, 0] = 'N';
                    continue;
                }

                dynamicPossible[i, 0] = dynamicPossible[i - 1, 0];
                dynamicPath[i, 0] = 'D';
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
                        dynamicPath[i, j] = 'N';
                        continue;
                    }

                    if (dynamicPossible[i - 1, j])
                    {
                        // dynamicPossible[i, j] juz ma true
                        dynamicPath[i, j] = 'D';
                        continue;
                    }
                    
                    if (dynamicPossible[i, j - 1])
                    {
                        // dynamicPossible[i, j] juz ma true
                        dynamicPath[i, j] = 'R';
                        continue;
                    }
                    
                    // nie da sie dojsc
                    dynamicPossible[i, j] = false;
                    dynamicPath[i, j] = 'N';
                }
            }

            if (!dynamicPossible[n - 1, m - 1])
            {
                return (false, "");
            }
            
            /// Zostalo spisac cala trase w string
            StringBuilder trasaString = new StringBuilder();
            int ii = n - 1;
            int jj = m - 1;
            while (ii != 0 || jj != 0)
            {
                if (dynamicPath[ii, jj] == 'R')
                {
                    trasaString.Append("R");
                    jj--;
                }
                else
                {
                    trasaString.Append("D");
                    ii--;
                }
            }
            
            /// Odwrocic stringa
            char[] chars = trasaString.ToString().ToCharArray();
            Array.Reverse(chars);
            string reversedString = new string(chars);
            
            return (dynamicPossible[n-1, m-1], reversedString);
        }

        /// <summary>
        /// Etap 2 - wyznaczenie trasy realizującej zadany wzorzec, zgodnie z którą robot przemieści się z pozycji poczatkowej (0,0) na pozycję docelową (n-1, m-1)
        /// </summary>
        /// <param name="n">wysokość prostokąta</param>
        /// <param name="m">szerokość prostokąta</param>
        /// <param name="pattern">zadany wzorzec</param>
        /// <param name="obstacles">tablica ze współrzędnymi przeszkód</param>
        /// <returns>krotka (bool result, string path) - result ma wartość true jeżeli trasa istnieje, false wpp., path to wynikowa trasa</returns>
        public (bool result, string path) Lab02Stage2(int n, int m, string pattern, (int, int)[] obstacles)
        {
            /// W tej tablicy na (k, i, j) bedzie informacja, czy da sie
            /// na patternie do k dojsc do miejsca (i, j)
            bool[,,] dynamicPossible = new bool[pattern.Length + 1, n, m];
            char[,,] dynamicPath  = new char[pattern.Length + 1, n, m];
            
            /// Trasa zapisywana jako znak char.
            /// Jesli w dol,                   to 'D'
            /// Jesli w dol z gwiazki,         to 'd'
            /// Jesli w prawo,                 to 'R'
            /// Jesli w prawo z gwiazki,       to 'r'
            /// Jesli z przeszlosci z gwiazki, to 'p'
            /// Jak sie nie da,                to 'N'
            ///
            /// dynamicPath[0, *, *] nic nie oznacza i jest zainicjowana dla wygody

            /// Przepiszmy zakazane miejsca do dynamicPossible
            for (int k = 1; k < pattern.Length + 1; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        dynamicPossible[k, i, j] = true;
                    }
                }
            }
            for (int k = 1; k < pattern.Length + 1; k++)
            {
                for (int o = 0; o < obstacles.Length; o++)
                {
                    dynamicPossible[k, obstacles[o].Item1, obstacles[o].Item2] = false;
                }
            }
            /// Teraz w dynamicPossible na [k, i, j] jest info, czy na [i, j] jest obstacle (poza k == 0)
            
            /// Ustawny dla k = 0
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    dynamicPossible[0, i, j] = false; // Byl tak zainicjowany juz na poczatku, ale napisze explicite, ze tak chce
                }
            }
            dynamicPossible[0, 0, 0] = true;
            
            for (int k = 1; k < pattern.Length + 1; k++)
            {
                /// W dynamicPossible[k - 1, *, *] jest info takie jak trzeba
                /// TODO(Zrobimy dla dynamicPossible[k, *, *] takie jak trzeba)
                
                /// Mamy 4 opcje na nastepny znak
                /// 1. D
                /// 2. R
                /// 3. *
                /// 4. ?

                switch (pattern[k - 1]) /// Pattern dlugosci k konczy sie ta litera
                {
                    case 'D':
                        /// Najpierw pierwszy wiersz bedzie false
                        for (int j = 0; j < m; j++)
                        {
                            dynamicPossible[k, 0, j] = false;
                            dynamicPath[k, 0, j] = 'N';
                        }
                        /// Pozostale wiersze
                        for (int i = 1; i < n; i++)
                        {
                            for (int j = 0; j < m; j++)
                            {
                                if (!dynamicPossible[k, i, j])
                                {
                                    /// Tu jest przeszkoda
                                    dynamicPath[k, i, j] = 'N';
                                    continue;
                                }

                                /// Da sie dostac na [k-1, i-1, j] iff da sie na [k, i, j]
                                dynamicPossible[k, i, j] = dynamicPossible[k - 1, i - 1, j];
                                dynamicPath[k, i, j] = 'D';
                            }
                        }
                        break;
                    case 'R':
                        /// Najpierw pierwsza kolumna bedzie false
                        for (int i = 0; i < n; i++)
                        {
                            dynamicPossible[k, i, 0] = false;
                            dynamicPath[k, i, 0] = 'N';
                        }
                        /// Pozostale kolumny
                        for (int i = 0; i < n; i++)
                        {
                            for (int j = 1; j < m; j++)
                            {
                                if (!dynamicPossible[k, i, j])
                                {
                                    /// Tu jest przeszkoda
                                    dynamicPath[k, i, j] = 'N';
                                    continue;
                                }

                                /// Da sie dostac na [k-1, i, j-1] iff da sie na [k, i, j]
                                dynamicPossible[k, i, j] = dynamicPossible[k - 1, i, j - 1];
                                dynamicPath[k, i, j] = 'R';
                            }
                        }
                        break;
                    case '?':
                        /// Nie da sie dostac na [0, 0]
                        dynamicPossible[k, 0, 0] = false;
                        dynamicPath[k, 0, 0] = 'N';
                        
                        /// Pierwszy wiersz tylko z lewej moze
                        for (int j = 1; j < m; j++)
                        {
                            if (!dynamicPossible[k, 0, j])
                            {
                                /// Tu jest przeszkoda
                                dynamicPath[k, 0, j] = 'N';
                                continue;
                            }
                            
                            /// Da sie dostac na [k-1, 0, j-1] iff da sie na [k, 0, j]
                            dynamicPossible[k, 0, j] = dynamicPossible[k - 1, 0, j - 1];
                            dynamicPath[k, 0, j] = 'R';
                        }
                        
                        /// Pierwsza kolumna tylko z gory moze
                        for (int i = 1; i < n; i++)
                        {
                            if (!dynamicPossible[k, i, 0])
                            {
                                /// Tu jest przeszkoda
                                dynamicPath[k, i, 0] = 'N';
                                continue;
                            }
                            
                            /// Da sie dostac na [k-1, i-1, 0] iff da sie na [k, i, 0]
                            dynamicPossible[k, i, 0] = dynamicPossible[k - 1, i - 1, 0];
                            dynamicPath[k, i, 0] = 'D';
                        }
                        
                        /// Pozostale wiersze i kolumny
                        for (int i = 1; i < n; i++)
                        {
                            for (int j = 1; j < m; j++)
                            {
                                if (!dynamicPossible[k, i, j])
                                {
                                    /// Tu jest przeszkoda
                                    dynamicPath[k, i, j] = 'N';
                                    continue;
                                }

                                /// Da sie dostac na [k, i, j] iff da sie na
                                ///     [k-1, i, j-1] albo [k-1, i-1, j]:
                                if (dynamicPossible[k - 1, i, j - 1])
                                {
                                    dynamicPossible[k, i, j] = true;
                                    dynamicPath[k, i, j] = 'R';
                                }
                                else if (dynamicPossible[k - 1, i - 1, j])
                                {
                                    dynamicPossible[k, i, j] = true;
                                    dynamicPath[k, i, j] = 'D';
                                }
                                else
                                {
                                    dynamicPossible[k, i, j] = false;
                                    dynamicPath[k, i, j] = 'N';
                                }
                            }
                        }
                        break;
                    case '*':
                        /// Pierwszy element
                        dynamicPossible[k, 0, 0] = dynamicPossible[k - 1, 0, 0];
                        if (dynamicPossible[k, 0, 0])
                        {
                            dynamicPath[k, 0, 0] = 'p';
                        }
                        else
                        {
                            dynamicPath[k, 0, 0] = 'N';
                        }
                        
                        /// Pierwszy wiersz tylko z lewej moze
                        for (int j = 1; j < m; j++)
                        {
                            if (!dynamicPossible[k, 0, j])
                            {
                                /// Tu jest przeszkoda
                                dynamicPath[k, 0, j] = 'N';
                                continue;
                            }
                            
                            /// Da sie dostac na [k, 0, j] iff da sie na
                            ///     [k, 0, j-1] albo [k-1, 0, j]
                            if (dynamicPossible[k, 0, j - 1])
                            {
                                dynamicPossible[k, 0, j] = true;
                                dynamicPath[k, 0, j] = 'r';
                            }
                            else if (dynamicPossible[k - 1, 0, j])
                            {
                                dynamicPossible[k, 0, j] = true;
                                dynamicPath[k, 0, j] = 'p';
                            }
                            else
                            {
                                dynamicPossible[k, 0, j] = false;
                                dynamicPath[k, 0, j] = 'N';
                            }
                        }
                        
                        /// Pierwsza kolumna tylko z gory moze
                        for (int i = 1; i < n; i++)
                        {
                            if (!dynamicPossible[k, i, 0])
                            {
                                /// Tu jest przeszkoda
                                dynamicPath[k, i, 0] = 'N';
                                continue;
                            }
                            
                            /// Da sie dostac na [k, i, 0] iff da sie na
                            ///     [k-1, i, 0] albo [k, i-1, 0]
                            if (dynamicPossible[k - 1, i, 0])
                            {
                                dynamicPossible[k, i, 0] = true;
                                dynamicPath[k, i, 0] = 'p';
                            }
                            else if (dynamicPossible[k, i - 1, 0])
                            {
                                dynamicPossible[k, i, 0] = true;
                                dynamicPath[k, i, 0] = 'd';
                            }
                            else
                            {
                                dynamicPossible[k, i, 0] = false;
                                dynamicPath[k, i, 0] = 'N';
                            }
                        }
                        
                        /// Pozostale wiersze i kolumny
                        for (int i = 1; i < n; i++)
                        {
                            for (int j = 1; j < m; j++)
                            {
                                if (!dynamicPossible[k, i, j])
                                {
                                    /// Tu jest przeszkoda
                                    dynamicPath[k, i, j] = 'N';
                                    continue;
                                }

                                /// Da sie dostac na [k, i, j] iff da sie na
                                ///     [k-1, i, j] albo [k, i-1, j] albo [k, i, j-1]
                                if (dynamicPossible[k - 1, i, j])
                                {
                                    dynamicPossible[k, i, j] = true;
                                    dynamicPath[k, i, j] = 'p';
                                }
                                else if (dynamicPossible[k, i - 1, j])
                                {
                                    dynamicPossible[k, i, j] = true;
                                    dynamicPath[k, i, j] = 'd';
                                }
                                else if(dynamicPossible[k, i, j - 1])
                                {
                                    dynamicPossible[k, i, j] = true;
                                    dynamicPath[k, i, j] = 'r';
                                }
                                else
                                {
                                    dynamicPossible[k, i, j] = false;
                                    dynamicPath[k, i, j] = 'N';
                                }
                            }
                        }
                        break;
                    default:
                        throw new Exception("Impropper pattern");
                }
            }

            /// Pozostalo odczytac trase
            /// Zapisana ona jest w macierzy dynamicPath
            /// Zaczniemy od dynamicPath[pattern.Length, n - 1, m - 1]
            /// Gdy trafimy na odpowiednia litere to odpowiednio sie zachowamy
            
            /// dynamicPossible[pattern.Length, n - 1, m - 1] jest false
            /// iff
            /// dynamicPath[pattern.Length, n - 1, m - 1] jest 'N'
            
            if (!dynamicPossible[pattern.Length, n - 1, m - 1])
            {
                return (false, "");
            }
            
            StringBuilder trasaString = new StringBuilder();
            int kk = pattern.Length;
            int ii = n - 1;
            int jj = m - 1;
            while (ii != 0 || jj != 0)
            {
                switch (dynamicPath[kk, ii, jj])
                {
                    case 'D':
                        trasaString.Append("D");
                        kk--;
                        ii--;
                        break;
                    case 'd':
                        trasaString.Append("D");
                        ii--;
                        break;
                    case 'R':
                        trasaString.Append("R");
                        kk--;
                        jj--;
                        break;
                    case 'r':
                        trasaString.Append("R");
                        jj--;
                        break;
                    case 'p':
                        kk--;
                        break;
                    case 'N':
                    default:
                        throw new Exception("Bug of the algorithm :<");
                }
            }
            
            /// Odwrocic stringa
            char[] chars = trasaString.ToString().ToCharArray();
            Array.Reverse(chars);
            string reversedString = new string(chars);
            
            return (dynamicPossible[pattern.Length, n - 1, m - 1], reversedString);
        }
    }
}