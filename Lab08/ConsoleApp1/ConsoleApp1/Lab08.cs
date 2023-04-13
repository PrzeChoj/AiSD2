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

            int s = n * days * hours_in_day + n;
            int t = n * days * hours_in_day + n + 1;

            int maxWeight = 0;
            foreach ((int where, int num_of_crews) ship in ships)
            {
                maxWeight += ship.num_of_crews;
            }
            
            var my_g = new DiGraph<int>(n * days * hours_in_day + n + 2); // TODO pomyslec, czy powinno byc to + n
            
            // Krawedzie ze sztucznego poczatku
            foreach ((int where, int num_of_crews) startingShips in ships)
            {
                my_g.AddEdge(s, startingShips.where, startingShips.num_of_crews);
            }

            foreach ((int from, int to, int distance) edge in paths)
            {
                int T = 0; // Czas w godzinach
                while (T + edge.distance <= days * hours_in_day)
                {
                    my_g.AddEdge(n * T + edge.from, n * (T + edge.distance) + edge.to, maxWeight);
                    my_g.AddEdge(n * T + edge.to, n * (T + edge.distance) + edge.from, maxWeight);
                    T++;
                }
            }
            
            // Krawedzie dla zalog, ktore czekaja sobie w jakims porcie
            for (int i = 0; i < n; i++)
            {
                for (int T = 0; T <= days * hours_in_day; T++)
                {
                    my_g.AddEdge(T * n + i, days * hours_in_day + i, maxWeight);
                }
            }
            
            // Krawedzie do sztucznego konca
            for (int i = 0; i < n; i++)
            {
                my_g.AddEdge(n * days * hours_in_day + i, t, 1);
            }

            // Odpalamy Forda Fulkersona
            (var x, var y) = ASD.Graphs.Flows.FordFulkerson(my_g, s, t);

            int outNumber = 0;
            foreach (Edge<int> edge in y.OutEdges(s))
            {
                outNumber += edge.Weight;
            }
            
            return outNumber;
        }


        public int Stage2(int n, int days, (int from, int to, int distance)[] paths, (int where, int num_of_crews)[] ships)
        {
            return 0;
        }
    }
}
