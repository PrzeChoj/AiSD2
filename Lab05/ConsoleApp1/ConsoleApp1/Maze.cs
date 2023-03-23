using ASD.Graphs;
using System;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;

namespace ASD
{
    public class Maze : MarshalByRefObject
    {

        /// <summary>
        /// Wersje zadania I oraz II
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt bez dynamitów lub z dowolną ich liczbą
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="withDynamite">informacja, czy dostępne są dynamity 
        /// Wersja I zadania -> withDynamites = false, Wersja II zadania -> withDynamites = true</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany (dotyczy tylko wersji II)</param> 
        public int FindShortestPath(char[,] maze, bool withDynamite, out string path, int t = 0)
        {
            DiGraph<int> g = new DiGraph<int>(maze.Length);
            int iMax = maze.GetLength(0);
            int jMax = maze.GetLength(1);
            int startVertex = -1;
            int endVertex = -1;

            for (int i = 0; i < iMax - 1; i++)
            {
                for (int j = 0; j < jMax - 1; j++)
                {
                    if (maze[i, j] == 'X' && !withDynamite)
                    {
                        continue;
                    }
                    else if (maze[i, j] == 'S')
                    {
                        startVertex = Maze.VertexFromCoordinates(i, j, jMax);
                    }
                    else if (maze[i, j] == 'E')
                    {
                        endVertex = Maze.VertexFromCoordinates(i, j, jMax);
                    }
                    
                    int wagaPowrotu = 1;
                    if(maze[i,j] == 'X')
                        wagaPowrotu = t + 1;

                    bool canGoDown = maze[i + 1, j] != 'X';
                    if (canGoDown)
                    {
                        g.AddEdge(Maze.VertexFromCoordinates(i, j, jMax),
                            Maze.VertexFromCoordinates(i + 1, j, jMax), 1);
                        g.AddEdge(Maze.VertexFromCoordinates(i + 1, j, jMax),
                            Maze.VertexFromCoordinates(i, j, jMax), wagaPowrotu);
                    }
                    else if (withDynamite)
                    {
                        g.AddEdge(Maze.VertexFromCoordinates(i, j, jMax),
                            Maze.VertexFromCoordinates(i + 1, j, jMax), t + 1);
                        g.AddEdge(Maze.VertexFromCoordinates(i + 1, j, jMax),
                            Maze.VertexFromCoordinates(i, j, jMax), wagaPowrotu);
                    }
                    
                    bool canGoRight = maze[i, j + 1] != 'X';
                    if (canGoRight)
                    {
                        g.AddEdge(Maze.VertexFromCoordinates(i, j, jMax),
                            Maze.VertexFromCoordinates(i, j + 1, jMax), 1);
                        g.AddEdge(Maze.VertexFromCoordinates(i, j + 1, jMax),
                            Maze.VertexFromCoordinates(i, j, jMax), wagaPowrotu);
                    }
                    else if (withDynamite)
                    {
                        g.AddEdge(Maze.VertexFromCoordinates(i, j, jMax),
                            Maze.VertexFromCoordinates(i, j + 1, jMax), t + 1);
                        g.AddEdge(Maze.VertexFromCoordinates(i, j + 1, jMax),
                            Maze.VertexFromCoordinates(i, j, jMax), wagaPowrotu);
                    }
                }
            }
            
            // Ostatni wiersz
            for (int j = 0; j < jMax - 1; j++)
            {
                if (maze[iMax - 1, j] == 'X')
                {
                    continue;
                }
                else if (maze[iMax - 1, j] == 'S')
                {
                    startVertex = Maze.VertexFromCoordinates(iMax - 1, j, jMax);
                }
                else if (maze[iMax - 1, j] == 'E')
                {
                    endVertex = Maze.VertexFromCoordinates(iMax - 1, j, jMax);
                }
                
                int wagaPowrotu = 1;
                if(maze[iMax - 1, j] == 'X')
                    wagaPowrotu = t + 1;
                
                bool canGoRight = maze[iMax - 1, j + 1] != 'X';
                if (canGoRight)
                {
                    g.AddEdge(Maze.VertexFromCoordinates(iMax - 1, j, jMax),
                        Maze.VertexFromCoordinates(iMax - 1, j + 1, jMax), 1);
                    g.AddEdge(Maze.VertexFromCoordinates(iMax - 1, j + 1, jMax),
                        Maze.VertexFromCoordinates(iMax - 1, j, jMax), wagaPowrotu);
                }
                else if(withDynamite)
                {
                    g.AddEdge(Maze.VertexFromCoordinates(iMax - 1, j, jMax),
                        Maze.VertexFromCoordinates(iMax - 1, j + 1, jMax), t + 1);
                    g.AddEdge(Maze.VertexFromCoordinates(iMax - 1, j + 1, jMax),
                        Maze.VertexFromCoordinates(iMax - 1, j, jMax), wagaPowrotu);
                }
            }
            
            // Ostatnia kolumna
            for (int i = 0; i < iMax - 1; i++)
            {
                if (maze[i, jMax - 1] == 'X' && !withDynamite)
                {
                    continue;
                }
                else if (maze[i, jMax - 1] == 'S')
                {
                    startVertex = Maze.VertexFromCoordinates(i, jMax - 1, jMax);
                }
                else if (maze[i, jMax - 1] == 'E')
                {
                    endVertex = Maze.VertexFromCoordinates(i, jMax - 1, jMax);
                }
                
                int wagaPowrotu = 1;
                if(maze[i, jMax - 1] == 'X')
                    wagaPowrotu = t + 1;
                
                bool canGoDown = maze[i + 1, jMax - 1] != 'X';
                if (canGoDown)
                {
                    g.AddEdge(Maze.VertexFromCoordinates(i, jMax - 1, jMax),
                        Maze.VertexFromCoordinates(i + 1, jMax - 1, jMax), 1);
                    g.AddEdge(Maze.VertexFromCoordinates(i + 1, jMax - 1, jMax),
                        Maze.VertexFromCoordinates(i, jMax - 1, jMax), wagaPowrotu);
                }
                else if(withDynamite)
                {
                    g.AddEdge(Maze.VertexFromCoordinates(i, jMax - 1, jMax),
                        Maze.VertexFromCoordinates(i + 1, jMax - 1, jMax), t+1);
                    g.AddEdge(Maze.VertexFromCoordinates(i + 1, jMax - 1, jMax),
                        Maze.VertexFromCoordinates(i, jMax - 1, jMax), wagaPowrotu);
                }
            }
            
            // Ostatni element macierzy
            if (maze[iMax - 1, jMax - 1] == 'S')
            {
                startVertex = Maze.VertexFromCoordinates(iMax - 1, jMax - 1, jMax);
            }
            else if (maze[iMax - 1, jMax - 1] == 'E')
            {
                endVertex = Maze.VertexFromCoordinates(iMax - 1, jMax - 1, jMax);
            }
            
            // g jest ladnym grafem takim jak chcemy

            PathsInfo<int> pathsInfo = Paths.BellmanFord(g, startVertex);

            if (!pathsInfo.Reachable(startVertex, endVertex))
            {
                path = "";
                return -1;
            }

            path = ""; // TODO String Builder
            int[] pathVertexes = pathsInfo.GetPath(startVertex, endVertex);
            char thisMove = 'X';
            for (int i = 0; i < pathVertexes.Length - 1; i++)
            {
                int change = pathVertexes[i + 1] - pathVertexes[i]; // TODO a co jak labirynt ma jedna kolumne?
                switch (change)
                {
                    case 1:
                        thisMove = 'E';
                        break;
                    case -1:
                        thisMove = 'W';
                        break;
                    case var value when value == jMax: // Powinno byc case jMax, ale kompilator nie pozwala
                        thisMove = 'S';
                        break;
                    case var value when value == -jMax: // Powinno byc case -jMax, ale kompilator nie pozwala
                        thisMove = 'N';
                        break;
                    default:
                        new Exception($"Unexpected change {change}");
                        break;
                }
                path += thisMove;
            }
            
            return path.Length;
        }

        public static int VertexFromCoordinates(int i, int j, int jMax)
        {
            return i * jMax + j;
        }

        public static (int, int) CoordinatesFromVertex(int v, int jMax)
        {
            return (v / jMax, v % jMax);
        }

    /// <summary>
        /// Wersja III i IV zadania
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt z użyciem co najwyżej k lasek dynamitu
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="k">liczba dostępnych lasek dynamitu, dla wersji III k=1</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany</param>
        public int FindShortestPathWithKDynamites(char[,] maze, int k, out string path, int t)
        {
            path = "";
            return -1;
        }
    }
}