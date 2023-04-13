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
            // Bedzie sztuczny start (n * days * hours_in_day) i sztuczny koniec (n * days * hours_in_day + 1)
            // Bedzie days*hours_in_day kopii oryginalnego grafu
            // Jesli krawedz miedzy 1, a 9 bedzie 10, to znaczy, ze bedzie polaczenie o 10 do gory
            
            var my_g = new Graph<int>(n * days * hours_in_day + 2);

            foreach ((int from, int to, int distance) edge in paths)
            {
                my_g.AddEdge(edge.from, edge.to, edge.distance);
            }

            // Krawedzie ze sztucznego poczatku
            foreach ((int where, int num_of_crews) startingShips in ships)
            {
                my_g.AddEdge(n, startingShips.where, 0);
            }

            // Krawedzie do sztucznego konca
            for (int i = 0; i < n; i++)
            {
                my_g.AddEdge(i, n + 1, 1);
            }
            
        
            return 0;
        }


        public int Stage2(int n, int days, (int from, int to, int distance)[] paths, (int where, int num_of_crews)[] ships)
        {
            return 0;
        }
    }
}
