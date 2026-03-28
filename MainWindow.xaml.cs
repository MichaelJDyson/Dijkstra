using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dijkstra
{
    public partial class DijkstraWindow : Window
    {
        public bool NewRun = true;

        public DijkstraWindow()
        {
            InitializeComponent();
            PopulateCellNames(rowCount, colCount);
            dataGrid.ItemsSource = items;
            canvas.MouseRightButtonDown += CanvasMouseRightButtonDown;
            canvas.Children.Add(dataGrid);
            canvas.Children.Add(mesh);
            NewRun = true;
            Refresh();
        }
        public void Refresh()
        {
            stopwatch.Start();
            startingVertices.Clear();
            if (NewRun) { RandomiseBlockedVertices(35); }
            else // refresh run only 
            {
                foreach (var kvp in userChangedVertices) // for each user-changed vertex, update blocked vertices and weights lists based on the new value of the vertex (X for blocked, 1-3 for open with weight multiplier)
                {
                    if (kvp.Value == "X") // if user changed vertex is now blocked
                    {
                        if (!blockedVertices.Contains(kvp.Key)) { blockedVertices.Add(kvp.Key); } // if list of blocked vertices does not already contain user changed vertex, add it to list
                        if (verticesWeight.ContainsKey(kvp.Key)) { verticesWeight.Remove(kvp.Key); } // if list of vertices weights contains user-changed vertex, delete it 
                    }
                    else 
                    { 
                        verticesWeight[kvp.Key] = int.Parse(kvp.Value); // if user changed vertex is now open, update weight in weights list (this will add it if it is not already there, or update it if it is)
                        //if(blockedVertices.Contains(kvp.Key)) 
                        { blockedVertices.Remove(kvp.Key); } // remove vertices that were previously blocked but are now open
                    }

                }
            }
            if (NewRun) { AssignWeightsToVertices(); } // assign weights to open vertices 
            InitializeEdges();
            PopulateMesh(rowCount, colCount);
            Compute();
            GetShortestPathFromA(stopLocation);
            stopwatch.Stop();
            timerOutput.Text = "Compute mS: " + stopwatch.ElapsedMilliseconds.ToString();
            stopwatch.Reset();
        }
        private void GetShortestPathFromA(string end) // work back towards A1 from end vertex, create route list and string text
        {
            routeList.Clear();
            var route = new StringBuilder();
            string currentVertex = end;
            bool endPoint = true;

            // Use the dictionary for safe predecessor lookups
            while (currentVertex != goLocation)
            {
                if (currentVertex == null)
                {
                    tb.Text = $"No path from {goLocation} to {end} (reached null vertex).";
                    mesh.ClearTrack();
                    items.Clear();
                    dataGrid.Items.Refresh(); // optional — call if UI doesn't update automatically
                    return;
                }

                if (!vertices.Contains(currentVertex))
                {
                    tb.Text = $"No path from {goLocation} to {end} (vertex {currentVertex} unknown).";
                    mesh.ClearTrack();
                    items.Clear();
                    dataGrid.Items.Refresh(); // optional — call if UI doesn't update automatically
                    return;
                }

                if (endPoint) { route.Insert(0, currentVertex); endPoint = false; }
                else { route.Insert(0, currentVertex + "->"); }

                routeList.Add(currentVertex);

                if (!priorVertices.TryGetValue(currentVertex, out string prior) || string.IsNullOrEmpty(prior))
                {
                    // predecessor missing => unreachable from source
                    tb.Text = $"No path from {goLocation} to {end}  (no predecessor for {currentVertex}).";
                    mesh.ClearTrack();
                    items.Clear();
                    dataGrid.Items.Refresh(); // optional — call if UI doesn't update automatically
                    return;
                }
                currentVertex = prior;
            }

            route.Insert(0, goLocation + "->");
            routeList.Add(goLocation);

            int endIdx = vertices.IndexOf(end);
            string distanceText = (endIdx >= 0) ? Math.Round(shortestPaths[endIdx], 4).ToString() : "∞";

            tb.Text = $"Best route from {goLocation} to {end} is: {route}. Total effort: {distanceText}";

            int k = 0;
            GetRowColFromVertex(stopLocation, out int r, out int c);
            mesh.StartPointTrack = new Point(40 * c + 20, 40 * r + 20); // Stop location
            foreach (string visited in routeList)
            {
                int row, col;
                GetRowColFromVertex(visited, out row, out col);
                if (meshText[row, col] != "Go" && meshText[row, col] != "Stop")
                {
                    //meshText[row, col] = "R" + k.ToString();  // this dispays the route as R1, R2, etc on the mesh
                    //k += 1;
                    mesh.EndPointTrack = new Point(40 * col + 20, 40 * row + 20);
                    mesh.StartPointTrack = mesh.EndPointTrack;
                }
            }
            PopulateGrid();
            GetRowColFromVertex(goLocation, out int rr, out int cc);
            mesh.EndPointTrack = new Point(40 * cc + 20, 40 * rr + 20); // Go location
            mesh.Text = meshText;
        }
        public void GetEdges(string startingVertex, double startingDistance)  //  gets the shortest distance from starting Vertex to any other connected "unvisitied" vertex
        {
            int edgeCount = 4 * rowCount * colCount - 3 * (rowCount + colCount) + 2;  //  number of edges = 4rc-3(r+c)+2 
            int priorDirection = 4;
            bool updateList = false;

            foreach (string vertex in vertices) // for each vertex
            {
                bool vertexUsed = false;

                if (!startingVertices.Contains(vertex)) // this for all vertices except starting vertex
                {
                    for (int i = 0; i < edgeCount; i++) // for each edge 
                    {
                        if (edges[i] == null) continue;// guard against null entries in edges array

                        if ((edges[i].Start == startingVertex && edges[i].End == vertex) || (edges[i].Start == vertex && edges[i].End == startingVertex)) // if edge includes the vertex AND starting vertex
                        {
                            Edge e = new Edge();

                            if (shortestPaths[vertices.IndexOf(vertex)] > edges[i].Weight + startingDistance) // if shorter than previous
                            {
                                paths[vertices.IndexOf(vertex), vertices.IndexOf(startingVertex)] = edges[i].Weight + startingDistance; // use it
                                shortestPaths[vertices.IndexOf(vertex)] = edges[i].Weight + startingDistance; // update shortest route
                                priorVertices[vertex] = startingVertex;
                            }
                            else
                            {
                                paths[vertices.IndexOf(vertex), vertices.IndexOf(startingVertex)] = shortestPaths[vertices.IndexOf(vertex)]; // use previous shortest route
                            }
                            vertexUsed = true;
                        }
                    }
                    if (!vertexUsed) { paths[vertices.IndexOf(vertex), vertices.IndexOf(startingVertex)] = shortestPaths[vertices.IndexOf(vertex)]; }  // if vertex doesnt appear with starting vertex assign it the shortest route
                }

                paths[vertices.IndexOf(startingVertex), vertices.IndexOf(startingVertex)] = startingDistance; // update starting priorDistance
                if (!startingVertices.Contains(startingVertex))
                {
                    startingVertices.Add(startingVertex); // add starting vertex to list of starting vertices if not there already

                }
                if (startingVertices.Count > 1)
                {
                    string previousVertex = startingVertices[startingVertices.Count - 2];
                    // Use the computed edgeCount and guard against null entries:
                    for (int j = 0; j < edgeCount; j++)
                    {
                        if (edges[j] == null) continue;
                        if (edges[j].Start == previousVertex && edges[j].End == startingVertex)
                        {
                            ListofDirections.Add(edges[j].Direction);
                        }
                    }
                }
            }
        }
        private void ClosestVertex(string startingVertex) // get the lowest priorDistance in the row  
        {
            minDistance = double.PositiveInfinity; // intialise as infinity
            for (int i = 0; i < countOfVertices; i++) // for each vertex...
            {
                if (!startingVertices.Contains(vertices[i])) // ... except starting vertices (these do not count)
                {
                    if (paths[i, vertices.IndexOf(startingVertex)] < minDistance) // if distance is less than current minimum
                    {
                        minDistance = paths[i, vertices.IndexOf(startingVertex)]; // set new minimum priorDistance
                        minVertex = vertices[i]; // set new minimum vertex
                    }
                }
            }
        }
        public void Compute()  //
        {
            priorVertices.Clear();// reset predecessor dictionary and set source predecessor to null
            priorVertices[goLocation] = null;
            minVertex = goLocation;  // set initial minimum vertex to goLocation
            minDistance = 0;
            ListofDirections.Clear();
            foreach (string vertex in vertices) // for each vertex in the list of vertices
            {
                startingVertex = minVertex;
                GetEdges(minVertex, minDistance);
                ClosestVertex(minVertex);
            }
        }

        bool autoReRoute = true;

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            autoReRoute = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            autoReRoute = false;
        }
    }
}