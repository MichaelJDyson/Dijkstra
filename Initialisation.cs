using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Dijkstra
{
    public partial class DijkstraWindow : Window
    {
        public void RandomiseBlockedVertices(int numberToBlock) // block out a number of vertices at random, ensuring start and end are not blocked
        {
            Random rand = new Random();
            blockedVertices.Clear();
            while (blockedVertices.Count < numberToBlock)
            {
                int row = rand.Next(0, 10);
                int col = rand.Next(0, 10);
                string cellName = GetVertexfromRowCol(row, col);
                if (cellName != goLocation && cellName != stopLocation && !blockedVertices.Contains(cellName)) // do not block start or end or already blocked
                {
                    blockedVertices.Add(cellName);
                                    }
            }
        }
        public void AssignWeightsToVertices() // assign weights to "open" vertices 
        {
            Random rand = new Random();
            verticesWeight.Clear();
            foreach (string vertex in vertices)
            {
                if (vertex != goLocation && vertex != stopLocation && !blockedVertices.Contains(vertex)) // do not assign weights to go or end or blocked vertices
                {
                    int weight = rand.Next(1, 4); // weights between 1 and 3
                    verticesWeight[vertex] = weight;
                }
            }
        }
        private void PopulateCellNames(int rows, int cols) // put cell names into vertices list
        {
            vertices.Clear(); // clear vertices list
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    vertices.Add(GetVertexfromRowCol(row, col));
                }
            }
            countOfVertices = vertices.Count;
        }
        public void InitializeEdges() // initialize edges array with start, end and weight properties
        {
            int row, col;
            int edgesCount = 0;

            edges = new Edge[500]; // effective reset
            initPriorVertices.Clear(); // clear prior vertices list
            foreach (string vertex in vertices)
            {
                GetRowColFromVertex(vertex, out row, out col);
                for (int i = 0; i < 8; i++) // for each possible direction
                {
                    int newRow = row + Offsets[i, 0];
                    int newCol = col + Offsets[i, 1];
                    string neighbourVertex = GetVertexfromRowCol(newRow, newCol); // get neighbour vertex name
                    double weight = (Offsets[i, 0] != 0 && Offsets[i, 1] != 0) ? root2 : 1;// diagonal movement has root2 weight
                    weight = weight * 0.5 * ((verticesWeight.ContainsKey(vertex) ? verticesWeight[vertex] : 1) + (verticesWeight.ContainsKey(neighbourVertex) ? verticesWeight[neighbourVertex] : 1)); // weight multiplier takes average of vertex weights

                    if (blockedVertices.Contains(vertex) || blockedVertices.Contains(neighbourVertex)) weight = double.PositiveInfinity; // if either vertex is blocked, set weight to infinity
                    if (newRow >= 0 && newRow < rowCount && newCol >= 0 && newCol < colCount && !initPriorVertices.Contains(neighbourVertex)) // if within bounds and not already mapped
                    {
                        edges[edgesCount++] = new Edge() { Start = vertex, End = neighbourVertex, Weight = weight, Direction = i }; // add edge  (we dont acually use the direction Property)
                    }
                }
                initPriorVertices.Add(vertex); // mark vertex as mapped
            }

            for (int i = 0; i < 500; i++) // initialize shortest paths array with infinity
            {
                shortestPaths[i] = double.PositiveInfinity;
            }
        }
        private void PopulateMesh(int rows, int cols)  // fill mesh text with vertex names, mark start, stop and blocked 
        {
            meshText = new string[10, 10];// clear mesh text
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    meshText[row, col] = vertices[cols * row + col];
                }
            }
            GetRowColFromVertex(goLocation, out int goRow, out int goCol);
            meshText[goRow, goCol] = "Go";
            GetRowColFromVertex(stopLocation, out int stopRow, out int stopCol);
            meshText[stopRow, stopCol] = "Stop";

            foreach (string blocked in blockedVertices)
            {
                GetRowColFromVertex(blocked, out int row, out int col);
                meshText[row, col] = "X"; // mark blocked vertices with X
            }

            foreach (var kvp in verticesWeight)// mark open vertices with their assigned weight
            {
                string openVertex = kvp.Key;
                int weight = kvp.Value;
                GetRowColFromVertex(openVertex, out int row, out int col);
                meshText[row, col] = weight.ToString();
            }
            mesh.Text = meshText;
        }
    }
}
