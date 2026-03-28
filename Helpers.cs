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
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            NewRun = true;
            Array.Clear(meshText);  // clears the mesh text array for new run
            Refresh();
        }

        public void Button2_Click(object sender, RoutedEventArgs e)
        {
            NewRun = false;
            Refresh();
        }
        private void GetRowColFromVertex(string vertex, out int row, out int col)  // get row and column from vertex name
        {
            byte[] asciiBytes = Encoding.ASCII.GetBytes(vertex);
            col = asciiBytes[0] - 65; // col is an int (0 to 9) >>  A=0, B=1, C=2, D=3
            if (asciiBytes.Length <= 2) // row is an int (0 to 9) 
            {
                row = asciiBytes[1] - 49; // 1=0, 2=1, 3=2, 4=3
            }
            else // when double digit row
            {
                row = 10 * (asciiBytes[1] - 48) + (asciiBytes[2] - 48) - 1; // 10=9, 11=10 etc 
            }
        }
        private string GetVertexfromRowCol(int row, int col)  // get vertex name from row and column
        {
            return ((char)(col + 65)).ToString() + (row + 1).ToString(); // A=65 in ASCII
        }
        private void PopulateGrid() // provide data to the datagrid
        {
            reversedRouteList = routeList.ToList();
            reversedRouteList.Reverse(); // create a reversed (Go to Stop) route list
            items.Clear(); // clear the grid
            double cummDistance = 0;
            double priorDistance = 0;
            foreach (string visited in reversedRouteList)
            {
                cummDistance = shortestPaths[vertices.IndexOf(visited)]; // get cummulative distance to visited node
                if (cummDistance == double.PositiveInfinity) cummDistance = 0; // fix starting cumm distance to zero
                priorDistance = cummDistance - priorDistance;
                DataToGrid(visited, priorDistance, cummDistance);
                priorDistance = cummDistance;
            }
        }
        private void DataToGrid(string startingVertex, double dist, double cummDist) // update data grid from data array
        {
            items.Add(new
            {
                Vertex = startingVertex,
                Step= dist.ToString("0.00"), // was EffortFromPrevious 
                Cumm= cummDist.ToString("0.0") // was EffortFromGo
            });
            dataGrid.Items.Refresh();
        }
    }
}
