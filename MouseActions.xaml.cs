using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dijkstra;

public partial class DijkstraWindow : Window

{
    bool firstMouseClick = true;
    int clickCounter = 0;
    string previouslyChangedVertex = string.Empty;

    private void CanvasMouseRightButtonDown(object sender, MouseEventArgs e)
    {
        Point position = e.GetPosition(canvas); // get mouse position relative to canvas
        string[,]? newMeshText = new string[10, 10]; // create a new mesh text array to modify
        Array.Copy(mesh.Text, 0, newMeshText, 0, 100); // copy current mesh text to new array to preserve existing text
        int row = (int)((position.Y + 20) / 40 - 1); // calculate row and column based on mouse position, adjusting for cell size (40) and margin (20)
        int col = (int)((position.X + 20) / 40 - 1);
        string changedVertex = GetVertexfromRowCol(row, col);
        if (changedVertex == goLocation || changedVertex == stopLocation) { return; } // prevent changes to start and stop vertices
        if (previouslyChangedVertex != changedVertex) firstMouseClick = true;// reset click counter and first mouse click flag if the user clicks on a different vertex to allow toggling of X, 1, 2 and 3 states for each vertex independently

        if (row >= 0 && row < mesh.RowCount && col >= 0 && col < mesh.ColCount) // ✅ Guard ALL array accesses with the bounds check first
        {
            string currentValue = mesh.Text[row, col]; // safe — now inside the bounds check
            if (firstMouseClick) // on the first mouse click, populate clickCounter with existing value of the cell to allow toggling between X, 1, 2 and 3 states based on the current state of the cell (e.g. if the cell is currently "1", the first click will change it to "2" instead of "X")
            {
                firstMouseClick = false;
                switch (currentValue)
                {
                    case "X": clickCounter = 1; break;
                    case "1": clickCounter = 2; break;
                    case "2": clickCounter = 3; break;
                    case "3": clickCounter = 0; break;
                    default: clickCounter = 0; break;// if the cell is empty or has an unexpected value, start with "X"
                }
            }

            switch (clickCounter)
            {
                case 0: newMeshText[row, col] = "X"; break;
                case 1: newMeshText[row, col] = "1"; break;
                case 2: newMeshText[row, col] = "2"; break;
                case 3: newMeshText[row, col] = "3"; break;
            }
            clickCounter += 1;
            if (clickCounter > 3) clickCounter = 0; // reset click counter after 4 clicks to allow toggling of X, 1, 2 and 3 states for each vertex

            mesh.Text = newMeshText;  // update Text property of the mesh with the modified array to reflect the change in the grid
            meshText = mesh.Text; // update meshText variable with the new mesh text to ensure that the changes are preserved for future reference and to allow the algorithm to access the updated values of the cells when it runs

            userChangedVertices.Remove(changedVertex); // remove vertex from user changed vertices dictionary to ensure it is not duplicated if the user changes the same vertex multiple times
            userChangedVertices.Add(changedVertex, newMeshText[row, col]);

            previouslyChangedVertex = changedVertex;
        }

        if (autoReRoute) // if auto reroute is enabled, run the algorithm and refresh the grid on every right mouse click to provide real time feedback to the user on how their changes are affecting the route
        {
            NewRun = false;
            Refresh();
        }
    }
}

