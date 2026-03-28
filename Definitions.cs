using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Dijkstra
{
    public partial class DijkstraWindow : Window
    {
        readonly int rowCount = 10, colCount = 10;
        Edge[] edges = new Edge[500]; // edge array (max 500 edges for a 10x10 grid with 8 possible directions minus blocked vertices)
        List<string> vertices = new List<string>(); // list of vertex names (A1, A2, ..., J10) = Dijkstra UNVISITED NODES
        List<string> blockedVertices = new List<string>(); // the vertices that are blocked and cannot be used in the route
        List<string> startingVertices = new List<string>(); // the vertices that have been used as starting points in the algorithm = Dijkstra VISITED NODES
        List<string> initPriorVertices = new List<string>(); // the vertices that have been used as starting points in the initialization of the edges (this is to ensure each vertex is only mapped once in the edges array)
        List<string> routeList = new List<string>(); // the list of vertices in the final route from end to start
        List<string> reversedRouteList = new List<string>(); // the list of vertices in the final route from start to end (this is for display purposes, the routeList is reversed to create this)
        List<int> ListofDirections = new List<int>();
        Dictionary<string, string> priorVertices = new Dictionary<string, string>(); // predecessor map
        Dictionary<string, int> verticesWeight = new Dictionary<string, int>(); // weights map
        Dictionary<string, string> userChangedVertices = new Dictionary<string, string>(); // dictionary of user changed vertices and their new values, this is used to update the mesh and the data grid when the user changes a vertex value (e.g. blocks a vertex or changes its weight)
        int countOfVertices; // the total number of vertices, this is used to limit loops that iterate through vertices
        double[,] paths = new double[200, 200]; // this is the main data array for the algorithm, it stores the priorDistance from each vertex to each starting vertex, this is updated as the algorithm progresses and is used to determine the closest vertex in each iteration (the closest vertex is the one with the lowest priorDistance to any starting vertex)
        double[] shortestPaths = new double[500]; //
        string startingVertex;
        readonly double root2 = Math.Sqrt(2);
        readonly int[,] Offsets = new int[,] // relative offsets for 8 directions (row delta, col delta), clockwise from "NW" (directions 0 thru 7)
        {
            {-1, -1}, {-1, 0}, {-1, 1},
            {0, 1},   {1, 1},
            {1, 0},  {1, -1},  {0, -1}
        };
        string[,] meshText = new string[10, 10]; // text to display in mesh, this is updated as the mesh is populated and the route is tracked
        double minDistance = double.PositiveInfinity;
        string minVertex;// the vertex with the minimum priorDistance in the ClosestVertex method, this becomes the starting vertex for the next iteration of the algorithm
        string goLocation = "A1";
        string stopLocation = "H9";
        bool doPopulateMesh = true;
        Stopwatch stopwatch = new Stopwatch();

        DataGrid dataGrid = new DataGrid
        {
            //AutoGenerateColumns = true,
            Margin = new Thickness(440, 20, 0, 0),
            Height = 395,
            Width = 130
        };

   
        List<dynamic> items = new List<dynamic>();

        Mesh mesh = new Mesh(10, 10)
        {
            RowCount = 10,
            ColCount = 10,
            Margin = new Thickness(20, 20, 0, 0),
            Height = 400,
            Width = 400
        };


    }
}
