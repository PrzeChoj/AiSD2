
using System;

namespace ASD
{

    class ChangeMaking
    {

        /// <summary>
        /// Metoda wyznacza rozwiązanie problemu wydawania reszty przy pomocy minimalnej liczby monet
        /// bez ograniczeń na liczbę monet danego rodzaju
        /// </summary>
        /// <param name="amount">Kwota reszty do wydania</param>
        /// <param name="coins">Dostępne nominały monet</param>
        /// <param name="change">Liczby monet danego nominału użytych przy wydawaniu reszty</param>
        /// <returns>Minimalna liczba monet potrzebnych do wydania reszty</returns>
        /// <remarks>
        /// coins[i]  - nominał monety i-tego rodzaju
        /// change[i] - liczba monet i-tego rodzaju (nominału) użyta w rozwiązaniu
        /// Jeśli dostepnymi monetami nie da się wydać danej kwoty to change = null,
        /// a metoda również zwraca null
        ///
        /// Wskazówka/wymaganie:
        /// Dodatkowa uzyta pamięć powinna (musi) być proporcjonalna do wartości amount ( czyli rzędu o(amount) )
        /// </remarks>
        public int? NoLimitsDynamic(int amount, int[] coins, out int[] change)
        {
            int[,] my_dynamic_list = new int[amount+1, coins.Length]; // Zerowy wiersz mnie nie interesuje
            int[] my_count = new int[amount+1];

            int i;
            int j;
            // Wypelnijmy -1 jako wiersze nieprzetworzone
            for (i = 1; i <= amount; i++) // Dla i = 0 jest 0 monet i to jest ok
            {
                my_count[i] = -1;
            }
            
            // Wypelnijmy tymi, ktore wiemy, ze sa najlepsze. Mozemy uzyc jednej monety
            for (j = 0; j < coins.Length; j++)
            {
                if (coins[j] > amount) // Moze moneta jest wieksza niz reszta do wydania
                {
                    continue;
                }
                my_dynamic_list[coins[j], j] = 1; // Posoztale sa 0 by default
                my_count[coins[j]] = 1; // 1 moneta wystarczy
            }

            int best_moneta;
            int best_count;
            for (i = 1; i <= amount; i++)
            {
                if (my_count[i] != -1)
                {
                    continue; // Juz ten wiersz jest przetworzony
                }

                best_moneta = -1;
                best_count = int.MaxValue;
                for (j = 0; j < coins.Length; j++)
                {
                    if (i - coins[j] < 0)
                    {
                        continue; // ta moneta jest nieuzywalna
                    }
                    if (my_count[i - coins[j]] == -2)
                    {
                        continue; // nie da sie ulozyc i - coins[j]
                    }
                    
                    // Teraz i > i - coins[j] > 0
                    if (my_count[i - coins[j]] < best_count)
                    {
                        best_count = my_count[i - coins[j]];
                        best_moneta = j;
                    }
                }

                if (best_moneta == -1)
                {
                    my_count[i] = -2;
                }
                else
                {
                    // Znalazlem rozwiazanie dla i. Zapiszmy je
                    my_count[i] = my_count[i - coins[best_moneta]] + 1;
                    for (int jj = 0; jj < coins.Length; jj++)
                    { // Przepisz wiersz
                        my_dynamic_list[i, jj] = my_dynamic_list[i - coins[best_moneta], jj];
                    }
                    // Dopisz jedna monete
                    my_dynamic_list[i, best_moneta] += 1;
                }
            }

            change = new int[coins.Length];
            for (j = 0; j < coins.Length; j++)
            {
                change[j] = my_dynamic_list[amount, j];
            }
            //change = null;  // zmienić
            return my_count[amount];      // zmienić
        }

        /// <summary>
        /// Metoda wyznacza rozwiązanie problemu wydawania reszty przy pomocy minimalnej liczby monet
        /// z uwzględnieniem ograniczeń na liczbę monet danego rodzaju
        /// </summary>
        /// <param name="amount">Kwota reszty do wydania</param>
        /// <param name="coins">Dostępne nominały monet</param>
        /// <param name="limits">Liczba dostępnych monet danego nomimału</param>
        /// <param name="change">Liczby monet danego nominału użytych przy wydawaniu reszty</param>
        /// <returns>Minimalna liczba monet potrzebnych do wydania reszty</returns>
        /// <remarks>
        /// coins[i]  - nominał monety i-tego rodzaju
        /// limits[i] - dostepna liczba monet i-tego rodzaju (nominału)
        /// change[i] - liczba monet i-tego rodzaju (nominału) użyta w rozwiązaniu
        /// Jeśli dostepnymi monetami nie da się wydać danej kwoty to change = null,
        /// a metoda również zwraca null
        ///
        /// Wskazówka/wymaganie:
        /// Dodatkowa uzyta pamięć powinna (musi) być proporcjonalna do wartości iloczynu amount*(liczba rodzajów monet)
        /// ( czyli rzędu o(amount*(liczba rodzajów monet)) )
        /// </remarks>
        public int? Dynamic(int amount, int[] coins, int[] limits, out int[] change)
        {
            int[,] my_dynamic_list = new int[amount+1, coins.Length]; // Zerowy wiersz mnie nie interesuje
            int[] my_count = new int[amount+1];

            int i;
            int j;
            // Wypelnijmy -1 jako wiersze nieprzetworzone
            for (i = 1; i <= amount; i++) // Dla i = 0 jest 0 monet i to jest ok
            {
                my_count[i] = -1;
            }
            
            // Wypelnijmy tymi, ktore wiemy, ze sa najlepsze. Mozemy uzyc jednej monety
            for (j = 0; j < coins.Length; j++)
            {
                if (coins[j] > amount) // Moze moneta jest wieksza niz reszta do wydania
                {
                    continue;
                }
                if (limits[j] < 1) // Nie mozemy uzyc nawet jednej monety
                {
                    continue;
                }
                my_dynamic_list[coins[j], j] = 1; // Posoztale sa 0 by default
                my_count[coins[j]] = 1; // 1 moneta wystarczy
            }

            int best_moneta;
            int best_count;
            for (i = 1; i <= amount; i++)
            {
                if (my_count[i] != -1)
                {
                    continue; // Juz ten wiersz jest przetworzony
                }

                best_moneta = -1;
                best_count = int.MaxValue;
                for (j = 0; j < coins.Length; j++)
                {
                    if (i - coins[j] < 0)
                    {
                        continue; // ta moneta jest nieuzywalna
                    }
                    if (my_count[i - coins[j]] == -2)
                    {
                        continue; // nie da sie ulozyc i - coins[j]
                    }
                    
                    // Teraz i > i - coins[j] > 0
                    if (my_dynamic_list[i - coins[j], j] == limits[j]) // nie mozemy uzyc juz wiecej takich monet
                    {
                        continue;
                    }
                    
                    if (my_count[i - coins[j]] < best_count)
                    {
                        best_count = my_count[i - coins[j]];
                        best_moneta = j;
                    }
                }

                if (best_moneta == -1)
                {
                    my_count[i] = -2;
                }
                else
                {
                    // Znalazlem rozwiazanie dla i. Zapiszmy je
                    my_count[i] = my_count[i - coins[best_moneta]] + 1;
                    for (int jj = 0; jj < coins.Length; jj++)
                    { // Przepisz wiersz
                        my_dynamic_list[i, jj] = my_dynamic_list[i - coins[best_moneta], jj];
                    }
                    // Dopisz jedna monete
                    my_dynamic_list[i, best_moneta] += 1;
                }
                
                if(amount == 141 && new []{65, 100, 130, 139, 141}.Contains(i))
                {
                    ;
                }
            }

            change = new int[coins.Length];
            for (j = 0; j < coins.Length; j++)
            {
                change[j] = my_dynamic_list[amount, j];
            }
            //change = null;  // zmienić
            return my_count[amount];      // zmienić
        }
        

    }

}
