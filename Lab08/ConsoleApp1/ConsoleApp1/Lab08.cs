using ASD.Graphs;

namespace Lab08
{

    internal class ColumbExpedition
    {
        private int hours_in_day = 12;
        public ColumbExpedition() { }
        /*

         * :param n: Liczba punktów z których można dokonać obserwacji
         * :param days: Maksymalna liczba dni na dotarcie do punktu
         * :param paths: Tablica krotek reprezentujących dwukierunkowe drogi pomiędzy punktami
         * :param ships: Tablica krotek reprezetująca pozycje i liczba załóg na poszczególnych statkach
         * :return: Maksymalna liczba pomiarów które uda się wykonać załodze Krzysztofa Kolumba
         **/
        public int Stage1(int n, int days, (int from, int to, int distance)[] paths, (int where, int num_of_crews)[] ships)
        {
            // Bedzie sztuczny start (n * days * hours_in_day + n) i sztuczny koniec (n * days * hours_in_day + n + 1)
            // Bedzie days*hours_in_day kopii oryginalnego grafu
            // Jesli krawedz miedzy 1, a 9 bedzie 10, to znaczy, ze bedzie polaczenie o 10 do gory

            int T_max = days * hours_in_day - 1;
            int s = (T_max + 1) * n;
            int t = s + 1;

            int maxWeight = 0;
            foreach ((int where, int num_of_crews) ship in ships)
            {
                maxWeight += ship.num_of_crews;
            }
            
            var my_g = new DiGraph<int>(t + 1);
            
            // Krawedzie ze sztucznego poczatku
            foreach ((int where, int num_of_crews) startingShips in ships)
            {
                my_g.AddEdge(s, startingShips.where, startingShips.num_of_crews);
            }

            foreach ((int from, int to, int distance) edge in paths)
            {
                int T = 0; // Czas w godzinach
                while (T + edge.distance <= T_max)
                {
                    my_g.AddEdge(n * T + edge.from, n * (T + edge.distance) + edge.to, maxWeight);
                    my_g.AddEdge(n * T + edge.to, n * (T + edge.distance) + edge.from, maxWeight);
                    T++;
                }
            }
            
            // Krawedzie dla zalog, ktore czekaja sobie w jakims porcie
            for (int i = 0; i < n; i++)
            {
                for (int T = 0; T < T_max; T++)
                {
                    my_g.AddEdge(T * n + i, T_max * n + i, maxWeight);
                }
            }
            
            // Krawedzie do sztucznego konca
            for (int i = 0; i < n; i++)
            {
                my_g.AddEdge(n * T_max + i, t, 1);
            }

            // Odpalamy Forda Fulkersona
            var (x, y) = ASD.Graphs.Flows.FordFulkerson(my_g, s, t);

            return y.OutEdges(s).Sum(edge => edge.Weight);
        }

        
        public int Stage2(int n, int days, (int from, int to, int distance)[] paths, (int where, int num_of_crews)[] ships)
        {
            // Bedzie sztuczny start (n * days * hours_in_day + n) i sztuczny koniec (n * days * hours_in_day + n + 1)
            // Bedzie days*hours_in_day kopii oryginalnego grafu
            // Jesli krawedz miedzy 1, a 9 bedzie 10, to znaczy, ze bedzie polaczenie o 10 do gory

            int T_max = days * hours_in_day - 1;
            int s = (T_max + 1) * n;
            int t = s + 1;

            int maxWeight = 0;
            foreach ((int where, int num_of_crews) ship in ships)
            {
                maxWeight += ship.num_of_crews;
            }
            
            var my_g = new DiGraph<int>(t + 1);
            
            // Krawedzie ze sztucznego poczatku
            foreach ((int where, int num_of_crews) startingShips in ships)
            {
                my_g.AddEdge(s, startingShips.where, startingShips.num_of_crews);
            }

            foreach ((int from, int to, int distance) edge in paths)
            {
                int T = 0; // Czas w godzinach
                while (T + edge.distance <= T_max)
                {
                    if (T % hours_in_day + edge.distance > hours_in_day) // Nie zdazymy tej nocy
                    {
                        T++;
                        continue;
                    }
                    my_g.AddEdge(n * T + edge.from, n * (T + edge.distance) + edge.to, maxWeight);
                    my_g.AddEdge(n * T + edge.to, n * (T + edge.distance) + edge.from, maxWeight);
                    T++;
                }
            }
            
            // Krawedzie dla zalog, ktore czekaja sobie w jakims porcie
            for (int i = 0; i < n; i++)
            {
                for (int T = 0; T < T_max; T++)
                {
                    my_g.AddEdge(T * n + i, T_max * n + i, maxWeight); // Czekamy do konca
                    
                    // Czekamy do nastepnego dnia:
                    int nextDay = T + (12 - T % hours_in_day);
                    if (nextDay >= T_max)
                    {
                        continue;
                    }
                    my_g.AddEdge(T * n + i, nextDay * n + i, maxWeight);
                }
            }
            
            // Krawedzie do sztucznego konca
            for (int i = 0; i < n; i++)
            {
                my_g.AddEdge(n * T_max + i, t, 1);
            }

            // Odpalamy Forda Fulkersona
            var (x, y) = ASD.Graphs.Flows.FordFulkerson(my_g, s, t);

            return y.OutEdges(s).Sum(edge => edge.Weight);
        }
    }
}
