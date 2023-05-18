using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab12
{
    public class Transport : MarshalByRefObject
    {
        public const double EPS = 10e-9;

        /// <summary>
        /// Dwuwymiarowy punkt na płaszczyźnie
        /// </summary>
        private struct Point
        {
            /// <summary>
            /// Pierwsza współrzędna
            /// </summary>
            public double X { get; }

            /// <summary>
            /// Druga współrzędna
            /// </summary>
            public double Y { get; }

            public Point(double x, double y)
            {
                this.X = x;
                this.Y = y;
            }

            /// <summary>
            /// Zwraca dystans między dwoma punktami
            /// </summary>
            /// <param name="p2">Drugi punkt</param>
            public double DistTo(Point p2)
            {
                return Math.Sqrt((X - p2.X) * (X - p2.X) + (Y - p2.Y) * (Y - p2.Y));
            }
        }

        /// <summary>
        /// Odcinek opisany za pomocą dwóch punktów będących jego końcami
        /// </summary>
        private class Segment
        {
            /// <summary>
            /// Punkt początkowy
            /// </summary>
            public Point Start;

            /// <summary>
            /// Punkt końcowy
            /// </summary>
            public Point End;

            public Segment(Point start, Point end)
            {
                this.Start = start;
                this.End = end;
            }

            public Segment(double startX, double startY, double endX, double endY)
            {
                this.Start = new Point(startX, startY);
                this.End = new Point(endX, endY);
            }

            private static double CrossProduct(Point p1, Point p2, Point p3)
            {
                return (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
            }

            /// <summary>
            /// Sprawdza czy dwa odcinki się przecinają
            /// </summary>
            /// <param name="b">Drugi odcinek</param>
            public bool Intersects(Segment b)
            {
                double d1 = CrossProduct(Start, End, b.Start);
                double d2 = CrossProduct(Start, End, b.End);
                double d3 = CrossProduct(b.Start, b.End, Start);
                double d4 = CrossProduct(b.Start, b.End, End);

                return (d1 * d2 < 0 && d3 * d4 < 0);
            }

            /// <summary>
            /// Zwraca punkt przecięcia dwóch odcinków
            /// </summary>
            /// <param name="b">Drugi odcinek</param>
            public Point IntersectionPoint(Segment b)
            {
                if (!Intersects(b))
                    throw new InvalidOperationException("Szukany punkt przecięcia nie istnieje (lub jest końcem któregoś z odcinków)!");

                double determinant = (Start.X - End.X) * (b.Start.Y - b.End.Y) - (Start.Y - End.Y) * (b.Start.X - b.End.X);

                double x = ((Start.X * End.Y - Start.Y * End.X) * (b.Start.X - b.End.X) - (Start.X - End.X) * (b.Start.X * b.End.Y - b.Start.Y * b.End.X)) / determinant;
                double y = ((Start.X * End.Y - Start.Y * End.X) * (b.Start.Y - b.End.Y) - (Start.Y - End.Y) * (b.Start.X * b.End.Y - b.Start.Y * b.End.X)) / determinant;

                return new Point(x, y);
            }

            /// <summary>
            /// Sprawdza czy dwa odcinki są takie same
            /// </summary>
            /// <param name="s2">Drugi odcinek</param>
            /// <returns></returns>
            public bool Equals(Segment s2)
            {
                if (s2 == null)
                    return false;

                if (Start.X != s2.Start.X || Start.Y != s2.Start.Y || End.X != s2.End.X || End.Y != s2.End.Y)
                    return false;

                return true;
            }

            public override string ToString()
            {
                return $"({Start.X}, {Start.Y}) - ({End.X}, {End.Y})";
            }
        }

        /// <summary>
        /// Wykorzystany do sortowania odcinków według współrzędnej y punktu przecięcia z pionową prostą o zadanej współrzędnej x
        /// </summary>
        private class SegmentComparer : IComparer<Segment>
        {
            /// <summary>
            /// Współrzędna x pionowej prostej
            /// </summary>
            public double verticalLineXCoordinate;

            /// <summary>
            /// Porównuje dwa odcinki i zwraca ich kolejność według współrzędnej y punktu przecięcia z pionową prostą o zadanej współrzędnej x
            /// </summary>
            /// <param name="a">Pierwszy odcinek</param>
            /// <param name="b">Drugi odcinek</param>
            /// <returns>
            /// <para>-1 gdy pierwszy odcinek przecina prostą niżej niż drugi odcinek</para>
            /// <para>0 gdy oba odcinki przecinają prostą w tym samym punkcie</para>
            /// <para>1 gdy pierwszy odcinek przecina prostą wyżej niż drugi odcinek</para>
            /// </returns>
            public int Compare(Segment a, Segment b)
            {
                if (a == null || b == null)
                    throw new InvalidOperationException("Jeden z porównywanych odcinków to null!");

                if (a.Start.X == a.End.X || b.Start.X == b.End.X)
                    throw new InvalidOperationException("Jeden z porównywanych odcinków jest pionowy!");

                Point s1, s2, e1, e2;
                if (a.Start.X < a.End.X)
                {
                    s1 = a.Start;
                    e1 = a.End;
                }
                else
                {
                    s1 = a.End;
                    e1 = a.Start;
                }
                if (b.Start.X < b.End.X)
                {
                    s2 = b.Start;
                    e2 = b.End;
                }
                else
                {
                    s2 = b.End;
                    e2 = b.Start;
                }

                double aIntersection = s1.Y + (verticalLineXCoordinate - s1.X) * (e1.Y - s1.Y) / (e1.X - s1.X);
                double bIntersection = s2.Y + (verticalLineXCoordinate - s2.X) * (e2.Y - s2.Y) / (e2.X - s2.X);
                return aIntersection.CompareTo(bIntersection);
            }
        }

        /// <summary>
        /// Y-struktura wykorzystana do trzymania odcinków posortowanych
        /// według współrzędnej y ich punktu przecięcia z pionową prostą o zadanej współrzędnej x.
        /// Kolejność sortowania to od najmniejszej do największej współrzędnej y przecięcia z pionową prostą.
        /// W czasie wykonywanych operacji na strukturze współrzędna x pionowej prostej jest ustawiania automatycznie 
        /// w zależności od operacji. Przyjęto założenie, że pionowa prosta przesuwa się od lewej do prawej strony.
        /// </summary>
        private class YStructure
        {
            private SortedSet<Segment> segments;
            private SegmentComparer segmentComparer;

            public YStructure()
            {
                segmentComparer = new SegmentComparer();
                segments = new SortedSet<Segment>(segmentComparer);
            }

            /// <summary>
            /// Wstawia odcinek do struktury. Współrzędna x pionowej prostej ustawiana jest jako
            /// mniejsza współrzędna x jednego z końców odcinka.
            /// </summary>
            /// <param name="s">Dodawany odcinek</param>
            public void Insert(Segment s)
            {
                if (s == null)
                    throw new ArgumentNullException("Argument s nie może być nullem!");

                segmentComparer.verticalLineXCoordinate = Math.Min(s.Start.X, s.End.X);

                if (segments.Contains(s))
                    throw new InvalidOperationException("Podany odcinek już znajduje się w strukturze!");

                segments.Add(s);
            }

            /// <summary>
            /// Usuwa odcinek ze struktury. Współrzędna x pionowej prostej ustawiana jest jako
            /// większa współrzędna x jednego z końców odcinka.
            /// </summary>
            /// <param name="s">Usuwany odcinek</param>
            public void Delete(Segment s)
            {
                if (s == null)
                    throw new ArgumentNullException("Argument s nie może być nullem!");

                segmentComparer.verticalLineXCoordinate = Math.Max(s.Start.X, s.End.X);

                if (!segments.Contains(s))
                    throw new InvalidOperationException("Podany odcinek nie znajduje się w strukturze!");

                segments.Remove(s);
            }

            /// <summary>
            /// Zwraca najbliższego sąsiada nad sprawdzanym odcinkiem zgodnie z kolejnością sortowania odcinków.
            /// Gdy sąsiad nie istnieje zwracany jest null.
            /// </summary>
            /// <param name="s">Odcinek</param>
            public Segment Above(Segment s)
            {
                if (s == null)
                    throw new ArgumentNullException("Argument s nie może być nullem!");

                if (!segments.Contains(s))
                    throw new InvalidOperationException("Podany odcinek nie znajduje się w strukturze!");

                Segment upperBound = segments.Reverse().First();
                foreach (Segment segmentAbove in segments.GetViewBetween(s, upperBound))
                {
                    if (!s.Equals(segmentAbove))
                        return segmentAbove;
                }
                return null;
            }

            /// <summary>
            /// Zwraca najbliższego sąsiada pod sprawdzanym odcinkiem zgodnie z kolejnością sortowania odcinków.
            /// Gdy sąsiad nie istnieje zwracany jest null.
            /// </summary>
            /// <param name="s">Odcinek</param>
            public Segment Below(Segment s)
            {
                if (s == null)
                    throw new ArgumentNullException("Argument s nie może być nullem!");

                if (!segments.Contains(s))
                    throw new InvalidOperationException("Podany odcinek nie znajduje się w strukturze!");

                Segment lowerBound = segments.First();
                foreach (Segment segmentBelow in segments.GetViewBetween(lowerBound, s).Reverse())
                {
                    if (!s.Equals(segmentBelow))
                        return segmentBelow;
                }
                return null;
            }

            /// <summary>
            /// Zamienia kolejność dwóch sąsiadujących odcinków. Kolejność odpowiada kolejności sortowania odcinków na prawo od ich przecięcia.
            /// </summary>
            /// <param name="s1">Pierwszy odcinek</param>
            /// <param name="s2">Drugi odcinek</param>
            public void Interchange(Segment s1, Segment s2)
            {
                if (!segments.Contains(s1) || !segments.Contains(s2))
                    throw new InvalidOperationException("Przynajmniej jeden z zamienianych segmentów nie należy do struktury!");

                Segment s1a = Above(s1);
                Segment s2a = Above(s2);
                if (!s2.Equals(s1a) && !s1.Equals(s2a))
                    throw new InvalidOperationException("Próba zamiany kolejności niesąsiednich odcinków!");

                segments.Remove(s1);
                segments.Remove(s2);

                segmentComparer.verticalLineXCoordinate = s1.IntersectionPoint(s2).X + EPS;

                segments.Add(s1);
                segments.Add(s2);
            }
        }

        /// <summary>
        /// Rodzaj zdarzenia
        /// </summary>
        enum EventType
        {

        }

        /// <summary>
        /// Opisuje zdarzenie
        /// </summary>
        class SweepEvent
        {

        }

        /// <summary>
        /// Wykorzystany do sortowania zdarzeń w odpowiedniej kolejności
        /// </summary>
        class SweepEventComparer : IComparer<SweepEvent>
        {
            /// <summary>
            /// Zwraca kolejność dwóch zdarzeń
            /// </summary>
            /// <param name="e1">Pierwsze zdarzenie</param>
            /// <param name="e2">Drugie zdarzenie</param>
            /// <returns>
            /// <para>-1 gdy pierwsze zdarzenie jest przed drugim zdarzeniem</para>
            /// <para>0 gdy oba zdarzenia mają tę samą kolejność</para>
            /// <para>1 gdy pierwsze zdarzenie jest za drugim zdarzeniem</para>
            /// </returns>
            public int Compare(SweepEvent e1, SweepEvent e2)
            {
                return 0;
            }
        }

        /// <summary>
        /// Etap 1 - znaleźć liczbę wszystkich stacji przesiadkowych
        /// </summary>
        /// <param name="lines">tablica z liniami metra, gdzie każda linia to współrzędne stacji początkowej i końcowej</param>
        /// <returns>int - liczba wszystkich stacji przesiadkowych</returns>
        public int Lab12Stage1(((int, int), (int, int))[] lines)
        {
            return 0;
        }

        /// <summary>
        /// Etap 2 - znaleźć maksymalną długość nieprzerwanego fragmentu metra między dwoma dowolnymi stacjami
        /// </summary>
        /// <param name="lines">tablica z liniami metra, gdzie każda linia to współrzędne stacji początkowej i końcowej</param>
        /// <returns>double - maksymalna długość nieprzerwanego fragmentu metra między dwoma dowolnymi stacjami</returns>
        public double Lab12Stage2(((int, int), (int, int))[] lines)
        {
            return 0.0;
        }
    }
}
