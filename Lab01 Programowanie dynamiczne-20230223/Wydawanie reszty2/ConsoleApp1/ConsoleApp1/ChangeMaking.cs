
using System;
using System.Runtime.InteropServices;

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
            int[,] myDynamicChange = new int[amount+1, coins.Length];
            int[,] myDynamicCount = new int[amount+1, coins.Length];
            
            // Zerowy wiersz ma same zera

            int i, j, newCoinId;
            // Wypelnijmy -1 jako wiersze nieprzetworzone
            for (i = 1; i <= amount; i++) // Dla i = 0 jest 0 monet i to jest ok
            {
                for (j = 0; j < coins.Length; j++)
                {
                    myDynamicCount[i, j] = -1;
                }
            }
            
            //for (i = coins[0]; i <= amount && i <= limits[0]; i += coins[0])
            for (i = 0; i <= amount; i += 1)
            {
                // Mamy tylko jedna monete
                if (i > limits[0] * coins[0] || (i % coins[0]) != 0 || i / coins[0] > limits[0])
                {
                    myDynamicCount[i, 0] = -2; // Nie da sie
                    continue;
                }
                myDynamicCount[i, 0] = i / coins[0]; // Tyle monet uzylismy; to dzielenie daje liczbe calkowita
                myDynamicChange[i, 0] = myDynamicCount[i,0];
            }

            // Mamy wypelnione dla uzywania dokladnie jednej monety
            for (newCoinId = 1; newCoinId < coins.Length; newCoinId++)
            {
                // Zalozmy, ze mamy do dyspozycji monety o indeksach od 0 do maxMonetId
                // i mamy dobrze policzone dla dyspozycji monet o indeksach od 0 do maxMonetId - 1
                for (i = 1; i <= amount; i++)
                {
                    // Max ponizej jest tylko po to, zeby nie probowal odczywac pomieci do ktorej nie ma dostepu
                    // Jesli max jest osiagany w 0, to i tak nie przejdzie
                    bool amountSmallerThanNewCoin = i - coins[newCoinId] < 0;
                    bool impossibleToMakeOneLessNewCoin = myDynamicCount[Math.Max(0, i - coins[newCoinId]), newCoinId] == -2;
                    bool alreadyUsedAllNewCoins = myDynamicChange[Math.Max(0, i - coins[newCoinId]), newCoinId] == limits[newCoinId];
                    if (i == 80 && newCoinId == 2 && amount == 123 && limits[4] == 3 && limits[5] == limits[6] && limits[6] == 0)
                    {
                        ;
                    }
                    if (amountSmallerThanNewCoin || impossibleToMakeOneLessNewCoin || alreadyUsedAllNewCoins)
                    {
                        // Moneta newCoinId nam nie pomoze
                        // Wystarczy tyle monet, co bez niej, myDynamicChange juz jest ok
                        myDynamicCount[i, newCoinId] = myDynamicCount[i, newCoinId - 1];
                        continue;
                    }
                    
                    // Mamy 2 opcje:
                    // Albo przepisujemy rozwiazanie bez monety newCoinId
                    // Albo do rozwiazania dla i - coins[newCoinId] dodajemy jenda monete

                    int wouldUseThatManyCoins = myDynamicCount[i - coins[newCoinId], newCoinId] + 1;
                    int usedThatManyCoins = myDynamicCount[i, newCoinId - 1];
                    bool nowICanButUsedToCannot = (wouldUseThatManyCoins != -2 && usedThatManyCoins == -2);
                    if (wouldUseThatManyCoins < usedThatManyCoins || nowICanButUsedToCannot)
                    {
                        // Skoro mozemy uzyc mniej, to to zrobmy; Przepiszmy stare wartosci
                        for (j = 0; j <= newCoinId; j++)
                        {
                            myDynamicChange[i, j] = myDynamicChange[i - coins[newCoinId], j];
                        }
                        // Dodajmy nowa monete
                        myDynamicChange[i, newCoinId]++;
                        myDynamicCount[i, newCoinId] = wouldUseThatManyCoins;
                    }
                    else
                    {
                        myDynamicCount[i, newCoinId] = usedThatManyCoins;
                    }

                    if (i == 80 && amount == 123 && limits[4] == 3 && limits[5] == limits[6] && limits[6] == 0)
                    {
                        ;
                    }
                }
            }
            
            if (amount == 123 && limits[4] == 3 && limits[5] == limits[6] && limits[6] == 0)
            {
                var myList = Array.Empty<int>();
                for (int k = 0; k < 124; k++)
                {
                    if (myDynamicChange[k, 6] == 1)
                    {
                        Console.WriteLine(k);
                        myList.Append(k);
                    }
                }
                ;
            }
            
            change = new int[coins.Length];
            for (j = 0; j < coins.Length; j++)
            {
                change[j] = myDynamicChange[amount, j];
            }
            
            return myDynamicCount[amount, coins.Length-1];
        }
    }

}
