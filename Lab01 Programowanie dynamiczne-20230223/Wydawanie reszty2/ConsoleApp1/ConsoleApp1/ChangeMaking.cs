
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
        public static void CopyFormNewToOld(int[,] oldMatrix, out int[,] newMatrix)
        {
            newMatrix = new int[oldMatrix.GetLength(0),oldMatrix.GetLength(1)];
            for (int i = 0; i < oldMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < oldMatrix.GetLength(1); j++)
                {
                    newMatrix[i, j] = oldMatrix[i, j];
                }
            }
        }
        
        public int? Dynamic(int amount, int[] coins, int[] limits, out int[] change)
        {
            int[,] myDynamicChangeOld, myDynamicChange = new int[amount+1, coins.Length];
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
                // Mamy tylko jeden rodzaj monety
                if (i > limits[0] * coins[0] || (i % coins[0]) != 0)
                {
                    myDynamicCount[i, 0] = -2; // Nie da sie
                    continue;
                }
                myDynamicCount[i, 0] = i / coins[0]; // Tyle monet uzylismy; to dzielenie daje liczbe calkowita
                myDynamicChange[i, 0] = myDynamicCount[i,0];
            }

            CopyFormNewToOld(myDynamicChange, out myDynamicChangeOld);
            
            // Mamy wypelnione dla uzywania dokladnie jednej monety
            for (newCoinId = 1; newCoinId < coins.Length; newCoinId++)
            {
                // Zalozmy, ze mamy do dyspozycji monety o indeksach od 0 do maxMonetId
                // i mamy dobrze policzone dla dyspozycji monet o indeksach od 0 do maxMonetId - 1
                // i tamto jest zapisane poprawnie w obu macierzach myDynamicCount oraz myDynamicChangeOld
                for (i = 1; i <= amount; i++)
                {
                    // Max ponizej jest tylko po to, zeby nie probowal odczywac pomieci do ktorej nie ma dostepu
                    // Jesli max jest osiagany w 0, to i tak nie przejdzie

                    int bestNewCoinFullAmount = int.MaxValue;
                    int bestNewCoinAmount = -1;
                    int newCoinAmount = 0;
                    int valueWithoutNewCoin = i - coins[newCoinId] * newCoinAmount;
                    while (valueWithoutNewCoin >= 0 && newCoinAmount <= limits[newCoinId])
                    {
                        bool daSieZTylomaNowymiMonetami = myDynamicCount[valueWithoutNewCoin, newCoinId - 1] != -2;
                        bool jestLepiejZTylomaNowymiMonetami =
                            myDynamicCount[valueWithoutNewCoin, newCoinId - 1] + newCoinAmount < bestNewCoinFullAmount;
                        if (daSieZTylomaNowymiMonetami && jestLepiejZTylomaNowymiMonetami)
                        {
                            bestNewCoinFullAmount = myDynamicCount[valueWithoutNewCoin, newCoinId - 1] + newCoinAmount;
                            bestNewCoinAmount = newCoinAmount;
                        }
                        newCoinAmount++;
                        valueWithoutNewCoin = i - coins[newCoinId] * newCoinAmount;
                    }

                    if (bestNewCoinAmount == -1)
                    {
                        // Nie da sie tylu monet wydac
                        myDynamicCount[i, newCoinId] = -2;
                        continue;
                    }
                    valueWithoutNewCoin = i - coins[newCoinId] * bestNewCoinAmount;
                    // Najlepiej uzyc bestNewCoinAmount nowych monet

                    myDynamicCount[i, newCoinId] =
                        myDynamicCount[valueWithoutNewCoin, newCoinId - 1] + bestNewCoinAmount;
                    
                    for (int k = 0; k < newCoinId; k++)
                    {
                        myDynamicChange[i, k] = myDynamicChangeOld[valueWithoutNewCoin, k];
                    }
                    myDynamicChange[i, newCoinId] = bestNewCoinAmount;
                }
                
                // Udalo sie policzyc dla tych monet
                // Teraz bedziemy liczyc dla kolejnych
                // wiec nowopowstala tablica jest juz stara
                CopyFormNewToOld(myDynamicChange, out myDynamicChangeOld);
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
