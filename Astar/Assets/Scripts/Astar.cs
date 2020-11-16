using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.CompilerServices;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
    /// Note that you will probably need to add some helper functions
    /// from the startPos to the endPos
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    /// 

    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        List<Node> closedList = new List<Node>();  // Makes closed node list
        List<Node> openList = new List<Node>();    // Open node list

        Node startNode = new Node(startPos, null, startPos.x, startPos.y);    //start node position, 
        Node targetNode = new Node(endPos, null, endPos.x, endPos.y);         //Target Pos
        openList.Add(startNode);

        while (openList.Count > 0) //stops this loop when hit destination, it will clear the list when it does.
        {
            Node currentNode = openList[0]; //First node from the openlist

            //loops through the open list.
            //Open list gets filled by neighbour foreach
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FScore <= currentNode.FScore)
                {
                    currentNode = openList[i]; //Node with lowest f score
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode.position == endPos)
            {
                targetNode = currentNode; //set this node as the end node, break the loop and retrace the path back from this node.
                //break;
                openList.Clear();
            }

            foreach (Cell neighbour in GetNeighbours(grid[currentNode.position.x, currentNode.position.y], grid))
            {
                int gCost = (int)currentNode.GScore + 1;

                //Calculates hCost
                int hCost =
                    Mathf.Abs(neighbour.gridPosition.x - endPos.x) +
                    Mathf.Abs(neighbour.gridPosition.y - endPos.y);

                //set selectedneighbournodes properties
                Node selectedNeighbourNode = new Node(neighbour.gridPosition, currentNode, gCost, hCost);

                if (closedList.Any(node => node.position == neighbour.gridPosition))
                {
                    continue;
                }

                //Calculates the move cost for the neighbour cells.
                int _moveCost =
                    (int)currentNode.GScore +
                    Mathf.Abs(currentNode.position.x - selectedNeighbourNode.position.x) +
                    Mathf.Abs(currentNode.position.y - selectedNeighbourNode.position.y);

                //Checks if the cost is lower and if the node is already on one of the neighbours position
                if (_moveCost < selectedNeighbourNode.GScore || !openList.Any(node => node.position == neighbour.gridPosition))
                {
                    selectedNeighbourNode.GScore = _moveCost;

                    //Calculates and sets the HScore of the node
                    selectedNeighbourNode.HScore =
                        Mathf.Abs(currentNode.position.x - selectedNeighbourNode.position.x) +
                        Mathf.Abs(currentNode.position.y - selectedNeighbourNode.position.y);

                    //If the open list doesnt already contain the neighbour node add it.
                    if (!openList.Contains(selectedNeighbourNode))
                    {
                        openList.Add(selectedNeighbourNode);
                    }
                }
            }
        }
        return NodesPath(startNode, targetNode);
    }

    List<Vector2Int> NodesPath(Node startNode, Node endNode)
    {
        List<Vector2Int> nodeHistoryPostions = new List<Vector2Int>();
        Node currentNode = endNode;

        //while the current node isnt the start node it will keep looping and adding the positions.
        while (currentNode != startNode)
        {
            nodeHistoryPostions.Add(currentNode.position);
            currentNode = currentNode.parent;
        }

        nodeHistoryPostions.Reverse(); //reverses the list.
        return nodeHistoryPostions;    //Returns the postions.
    }

    private List<Cell> GetNeighbours(Cell _cell, Cell[,] _grid)
    {

        List<Cell> result = new List<Cell>();

        int wallLeft = -1;
        int wallRight = 1;

        //If the cell has a wall to the right or left it will set that count to 0
        //The code will know later on.
        if (_cell.HasWall(Wall.LEFT))
        {
            wallLeft = 0;
        }

        if (_cell.HasWall(Wall.RIGHT))
        {
            wallRight = 0;
        }

        int wallDown = -1;
        int wallUp = 1;

        //If the cell has a wall to the up or down it will set that count to 0
        //The code will know later on.
        if (_cell.HasWall(Wall.UP))
        {
            wallUp = 0;
        }

        if (_cell.HasWall(Wall.DOWN))
        {
            wallDown = 0;
        }

        //check if there are walls left and or right
        for (int x = wallLeft; x <= wallRight; x++)
        {
            //Checkf if there are wall up and or down
            for (int y = wallDown; y <= wallUp; y++)
            {
                //Set cell positions for next node position
                int cellX = _cell.gridPosition.x + x;
                int cellY = _cell.gridPosition.y + y;

                //if cellx is smaller than 0 it means it can go left
                if (cellX < 0)
                {
                    continue;
                }

                //if it is bigger or the same as the max nodes on the x axis it moves to the right
                if (cellX >= _grid.GetLength(0))//checks x amount of grid nodes.
                {
                    continue;
                }

                //if celly is smaller than 0 it means it can go down
                if (cellY < 0)
                {
                    continue;
                }

                //if it is bigger or the same as the max nodes on the y axis it moves to the top
                if (cellY >= _grid.GetLength(1))//checks y amount of grid nodes.
                {
                    continue;
                }

                if (Mathf.Abs(x) == Mathf.Abs(y))
                {
                    continue;
                }

                //sets neighbour sells as canditates for shortest travel route
                Cell canditateCell = _grid[cellX, cellY];
                result.Add(canditateCell);
            }
        }
        return result;
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore
        { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
