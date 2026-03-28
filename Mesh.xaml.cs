using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
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

namespace Dijkstra;
// THis is a change made from the GIT portal
// THis is a change made 1631 on the GIT portal
// THis is a change made 1635 on the App
public partial class Mesh : UserControl
{
    Label[,] cell; // do not allocate here
    List<Line> Track = new List<Line>();
    private int rowCount;
    bool MouseIsOverCell = false;

    public int RowCount
    {
        get { return rowCount; }
        set { rowCount = value; }
    }

    private int colCount;

    public int ColCount
    {
        get { return colCount; }
        set { colCount = value; }
    }

    public Mesh(int rows, int cols)
    {
        InitializeComponent();
        rowCount = rows;
        colCount = cols;
        cell = new Label[rowCount, colCount]; //instantiate cell label here
        border = new Thickness[rowCount, colCount];
        setupCells();
    }

    private void setupCells()  // setup cells (vertices) and add to canvas 
    {
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                cell[row, col] = new Label
                {
                    Width = 35,
                    Height = 35,
                    VerticalAlignment = VerticalAlignment.Top,
                    VerticalContentAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(40 * col, 40 * row, 0, 0),
                    FontSize = 12,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1)
                };
                canvas.Children.Add(cell[row, col]);
            }
        }
        CellMouseEvents();  // Register per-cell mouse handlers after cells are created.
    }

    private void CellMouseEvents() // Register per-cell mouse handlers
    {
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                int r = row, c = col;
                cell[r, c].MouseEnter += (s, e) => // red border on mouseover
                {
                    cell[r, c].BorderBrush = Brushes.Red;
                    cell[r, c].BorderThickness = new Thickness(3);
                    MouseIsOverCell = true;
                };
                cell[r, c].MouseLeave += (s, e) => // reset border on mouse leave
                {
                    cell[r, c].BorderBrush = Brushes.Black;
                    cell[r, c].BorderThickness = new Thickness(1);
                    MouseIsOverCell = false;
                };
                cell[r, c].MouseRightButtonDown += (s, e) =>  // green border on right click if mouse is over cell
                {
                    if (MouseIsOverCell)
                    {
                        cell[r, c].BorderBrush = Brushes.Green;
                    }
                };
            }
        }
    }

    private string[,]? text; // declaration of 'text' nullable to satisfy CS8618
    public string[,]? Text  // Update property type to match
    {
        get { return text; }
        set
        {
            text = value;
            OnPropertyChanged();
        }
    }

    private Thickness[,]? border; //  currently this property is not actually called
    public Thickness[,]? Border    // Update property type to match
    {
        get { return border; }
        set
        {
            border = value;
            OnPropertyChanged();
        }
    }

    private Point? startPointTrack;
    public Point? StartPointTrack
    {
        get { return startPointTrack; }
        set
        {
            startPointTrack = value;
        }
    }

    private Point? endPointTrack;
    public Point? EndPointTrack
    {
        get { return endPointTrack; }
        set
        {
            endPointTrack = value;
            CreateTrack(); // create track when endpoint provided
        }
    }

    public void CreateTrack()
    {
        Line line = new Line();
        line.Stroke = new SolidColorBrush(Colors.Purple); line.StrokeThickness = 5;
        line.StrokeEndLineCap = PenLineCap.Round;
        line.X1 = startPointTrack?.X ?? 0; // Use null-coalescing operator to provide default value
        line.Y1 = startPointTrack?.Y ?? 0;

        line.X2 = endPointTrack?.X ?? 0; // Use null-coalescing operator to provide default value
        line.Y2 = endPointTrack?.Y ?? 0;
        Track.Add(line);
        if (endPointTrack == new Point(40 * 0 + 20, 40 * 0 + 20)) // if at the start point, plot and clear the Track list
        {
            PlotTrack();
            Track.Clear();
        }
    }

    private void PlotTrack()
    {
        trackCanvas.Children.Clear();  // clear existing lines before plotting new ones
        foreach (Line line in Track)
        {
            trackCanvas.Children.Add(line);
        }
    }

    public void ClearTrack()   // method to clear the track from the canvas
    {
        trackCanvas.Children.Clear();
    }

    public void OnPropertyChanged()
    {
        if (text == null) return; // Add null check to avoid NullReferenceException              

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                cell[row, col].Content = text[row, col];
                switch (text[row, col])
                {
                    case "X": cell[row, col].Background = Brushes.Black; cell[row, col].Foreground = Brushes.White; break;// black background white text for blocked vertices
                    case "Go": cell[row, col].Background = Brushes.Green; cell[row, col].Foreground = Brushes.White; break;// green background white text for start vertex
                    case "Stop": cell[row, col].Background = Brushes.Red; cell[row, col].Foreground = Brushes.White; break;// red background white text for end vertex
                    case "R": cell[row, col].Background = Brushes.Blue; cell[row, col].Foreground = Brushes.White; break;// blue background white text for route vertices - not currently used
                    case "1": cell[row, col].Background = Brushes.White; cell[row, col].Foreground = Brushes.Black; break; // white background black text for weight 1
                    case "2": cell[row, col].Background = Brushes.LightGray; cell[row, col].Foreground = Brushes.Black; break; // light gray background black text for weight 2
                    case "3": cell[row, col].Background = Brushes.Gray; cell[row, col].Foreground = Brushes.White; break;// gray background white text for weight 3

                    default: cell[row, col].Background = Brushes.White; cell[row, col].Foreground = Brushes.Black; break;
                }
            }
        }
    }
}
