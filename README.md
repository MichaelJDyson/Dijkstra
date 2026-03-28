This app demonstrates the use of the Dijkstra's algorithm to find the shortest path between two vertices in a network. 

In this case the vertices are points in a 10 x 10 matrix and the edges are the connections between the points. 
The app allows the user to select a start and end point (in code), and then calculate and display the best route between them using Dijkstra's algorithm.
The best route is the lowest sum of the products of distance between the vertices and their mean weights.  
The code randonly assigns 35 vertices as "blocked" - ie cannot be used.  The remaining "open" vertices are randomly assigned weights 1, 2 or 3 to indicate how easy they are to pass through.  The user can change the state of a vertex by right clicking on it - toggling through the weights in ascending order.  The app automatically recomputes the best route.  

I found this video very helpful in understanding Dijkstra's algorithm: https://www.youtube.com/watch?v=bZkzH5x0SKU

The app is written in C# WPF using Visual Studio 2026.  
