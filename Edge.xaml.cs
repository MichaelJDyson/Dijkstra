using System;
using System.Collections.Generic;
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
    /// <summary>
    /// The Edge class represents a graphical edge in a graph with properties for weight, start, and end nodes.
    /// </summary>
    public partial class Edge : UserControl
    {
        public Edge()
        {
            InitializeComponent();
        }

        private double weight;
        public double Weight
        {
            get { return weight; }
            set
            {
                weight = value;
            }
        }

        private string start;
        public string Start
        {
            get { return start; }
            set
            {
                start = value;
            }
        }

        private string end;
        public string End
        {
            get { return end; }
            set
            {
                end = value;
            }
        }

        private int direction;
        public int Direction

        {
            get { return direction; }
            set
            {
                direction = value;
            }
        }

        public int DirectionChange(int priorDirection, int newDirection)  // compute direction change weighting THIS IS NOT USED ȦT PRESENT BUT COULD BE USED TO ADD A TURNING COST TO THE ALGORITHM
        {
            if (priorDirection > 4) priorDirection -= 8;
            if (newDirection > 4) newDirection -= 8;
            int turn = Math.Abs(newDirection - priorDirection);
            if (turn > 4) turn = 8 - turn;
            return turn;
        }
    }
}
