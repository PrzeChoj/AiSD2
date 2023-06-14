using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab15
{
    /// <summary>
    /// Struktura danych obsługująca rezerwacje miejsc w pociągu.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class ReservationManager : MarshalByRefObject
    {
        private class TicketsComparer : IComparer<int>
        {
            public int Compare(int e1, int e2)
            {
                if (e1 < e2) return 1;
                if (e1 == e2) return 0;
                return -1;
            }
        }
        
        private class Node
        {
            public readonly int Leftmost;
            public readonly int Rightmost;
            public int Opened;
            public int Closed;
            public Node LeftChild;
            public Node RightChild;

            public int MyMax;
            public SortedDictionary<int, int> TicketsFromStation;

            public Node(int leftmost, int rightmost)
            {
                Leftmost = leftmost;
                Rightmost = rightmost;
                Opened = 0;
                Closed = 0;
                LeftChild = null;
                RightChild = null;

                MyMax = -1;
                TicketsFromStation = null;
            }
        }

        private Node _root;
        private readonly Hashtable _tickets = new Hashtable(); // For a tuple, return an int of how many there is of such a tickets
        
        /// <summary>
        /// Tworzy pustą strukturę danych dla pociągu zatrzymującego się na zadanej liczbie stacji.
        /// </summary>
        /// <param name="N">Całkowita liczba stacji</param>
        /// <remarks>
        /// Metoda może być wywołana wielokrotnie, z różną wartością N, i za każdym razem powinna 
        /// przywracać domyślny stan obiektu (tzn. brak jakichkolwiek rezerwacji)
        /// </remarks>
        public void Initialize(int N)
        {
            int newN = (int)Math.Pow(2, Math.Ceiling(Math.Log(N, 2)));
            int height = (int)Math.Log(newN, 2);

            _root = new Node(0,  newN- 1);
            MakeChildren(_root, height);
        }

        private static void MakeChildren(Node node, int height)
        {
            int leftLeftmost = node.Leftmost;
            int leftRightmost = node.Leftmost + (node.Rightmost - node.Leftmost)/2; // division is intentionally integer to lose 0.5
            int rightLeftmost = leftRightmost + 1;
            int rightRightmost = node.Rightmost;
            
            Node leftChild = new Node(leftLeftmost, leftRightmost);
            Node rightChild = new Node(rightLeftmost, rightRightmost);

            if (height > 1)
            {
                MakeChildren(leftChild, height - 1);
                MakeChildren(rightChild, height - 1);
            }
            
            node.LeftChild = leftChild;
            node.RightChild = rightChild;
        }

        /// <summary>
        /// Dodaje rezerwację pomiędzy zadanymi stacjami.
        /// </summary>
        /// <param name="start">Stacja poczatkowa</param>
        /// <param name="end">Stacja końcowa</param>
        /// <remarks>
        /// Zawsze zachodzi 0 <= start < end < N, gdzie N jest liczbą stacji.
        /// </remarks>
        public void AddReservation(int start, int end)
        {
            if (_root == null)
                throw new Exception("Tree was not initiated!");
            
            _tickets[(start, end)] = (int)(_tickets[(start, end)] ?? 0) + 1;

            AddStart(_root, start);
            AddEnd(_root, end);
            AddMax(_root, start, end);
        }

        private static void AddStart(Node node, int start)
        {
            node.Opened += 1;
            while (node.Leftmost != node.Rightmost)
            {
                if (node.LeftChild == null || node.RightChild == null) throw new Exception("Tree has an error in architecture!");

                node = start <= node.LeftChild.Rightmost ? node.LeftChild : node.RightChild;
                node.Opened += 1;
            }
        }
        
        private static void AddEnd(Node node, int end)
        {
            node.Closed += 1;
            while (node.Leftmost != node.Rightmost)
            {
                if (node.LeftChild == null || node.RightChild == null) throw new Exception("Tree has an error in architecture!");

                node = end <= node.LeftChild.Rightmost ? node.LeftChild : node.RightChild;
                node.Closed += 1;
            }
        }

        private static void AddMax(Node node, int start, int end)
        {
            if (node.MyMax < end) // -1 < end zawsze
                node.MyMax = end;
            
            while (node.Leftmost != node.Rightmost)
            {
                node = node.RightChild.Leftmost > start ? node.LeftChild : node.RightChild;
                if (node.MyMax < end) // -1 < end zawsze
                    node.MyMax = end;
            }

            if (node.TicketsFromStation == null)
                node.TicketsFromStation = new SortedDictionary<int, int>();

            if (node.TicketsFromStation.ContainsKey(end))
                node.TicketsFromStation[end] += 1;
            else
                node.TicketsFromStation.Add(end, 1);
        }

        /// <summary>
        /// Usuwa rezerwację pomiędzy zadanymi stacjami.
        /// </summary>
        /// <param name="start">Stacja poczatkowa</param>
        /// <param name="end">Stacja końcowa</param>
        /// <remarks>
        /// Zawsze zachodzi 0 <= start < end < N, gdzie N jest liczbą stacji.
        /// Można założyć, że podana rezerwacja została wcześniej dodana.
        /// </remarks>
        public void RemoveReservation(int start, int end)
        {
            if (_root == null)
                throw new Exception("Tree was not initiated!");
            if (_tickets[(start, end)] == null)
                throw new Exception("There was no such a ticket!");
            int oldTicketNumber = (int)_tickets[(start, end)];
            if (oldTicketNumber == 1)
                _tickets.Remove((start, end));
            else
                _tickets[(start, end)] = oldTicketNumber - 1;

            RemoveStart(_root, start);
            RemoveEnd(_root, end);
            RemoveMax(_root, start, end);
        }
        
        private static void RemoveStart(Node node, int start)
        {
            node.Opened -= 1;
            while (node.Leftmost != node.Rightmost)
            {
                if (node.LeftChild == null || node.RightChild == null) throw new Exception("Tree has an error in architecture!");

                node = start <= node.LeftChild.Rightmost ? node.LeftChild : node.RightChild;
                node.Opened -= 1;
            }
        }
        
        private static void RemoveEnd(Node node, int end)
        {
            node.Closed -= 1;
            while (node.Leftmost != node.Rightmost)
            {
                if (node.LeftChild == null || node.RightChild == null) throw new Exception("Tree has an error in architecture!");

                node = end <= node.LeftChild.Rightmost ? node.LeftChild : node.RightChild;
                node.Closed -= 1;
            }
        }
        
        private static void RemoveMax(Node node, int start, int end)
        {
            if (node.Leftmost != node.Rightmost)
            {
                RemoveMax(node.RightChild.Leftmost > start ? node.LeftChild : node.RightChild, start, end);

                node.MyMax = Math.Max(node.RightChild.MyMax, node.LeftChild.MyMax);
            }
            else
            {
                if (node.TicketsFromStation[end] == 1)
                {
                    node.TicketsFromStation.Remove(end);
                    node.MyMax = node.TicketsFromStation.Count == 0 ? -1 : node.TicketsFromStation.Keys.Max(); // TODO(Czy to nie jest zbyt wolne?)
                }
                else
                    node.TicketsFromStation[end] -= 1;
            }
        }

        /// <summary>
        /// Zwraca liczbę miejsc zajętych na zadanej stacji, tzn. liczbę rezerwacji (s, e) takich, że s < station < e
        /// (uwaga: nie liczymy rezerwacji zaczynających lub kończących się na zadanej stacji)
        /// </summary>
        /// <param name="station"></param>
        /// <remarks>
        /// Zawsze zachodzi 0 <= station < N
        /// </remarks>
        /// <returns></returns>
        public int FilledSeats(int station)
        {
            if (_root == null)
                throw new Exception("Tree was not initiated!");

            Node node = _root;
            int filledSeats = 0; 
            
            while (node.LeftChild != null && node.RightChild != null) // I not yet in a leaf
            {
                if (station > node.LeftChild.Rightmost)
                {
                    // Jesli ide w prawo, to musze dodac to, co jest otwierane po lewej
                    filledSeats += node.LeftChild.Opened - node.LeftChild.Closed;
                    node = node.RightChild;
                }
                else
                {
                    // Jesli ide w lewo, to nic sie nie dzieje xd
                    node = node.LeftChild;
                }
            }
            
            // Na koniec odejmij to, co sie zamyka w lisciu
            filledSeats -= node.Closed;

            return filledSeats;
        }

        /// <summary>
        /// Zwraca najwcześniej zaczynającą się rezerwację zawierającą zadaną stację,
        /// tzn. rezerwację (s, e) taką, że s <= station < e oraz s jest najmniejsze możliwe
        /// w przypadku remisu należy maksymalizować e.
        /// 
        /// Jeśli taka rezerwacja nie istnieje, należy zwrócić krotkę (-1, -1)
        /// </summary>
        /// <param name="start">Stacja</param>
        /// <remarks>
        /// Zawsze zachodzi 0 <= station < N, gdzie N jest liczbą stacji.
        /// </remarks>
        /// <returns></returns>
        public (int s, int e) FirstOverlappingReservation(int station)
        {
            Node node = _root;
            
            if (node.MyMax <= station)
                return (-1, -1);

            while (node.Leftmost != node.Rightmost)
            {
                node = node.LeftChild.MyMax > station ? node.LeftChild : node.RightChild;
                if (station < node.Leftmost)
                    return (-1, -1);
            }
            
            return (node.Leftmost, node.MyMax);
        }
    }
}
