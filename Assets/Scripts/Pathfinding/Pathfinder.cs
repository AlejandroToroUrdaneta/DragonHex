using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;

public class Pathfinder : MonoBehaviour
{
    #region member fields
    public PathIllustrator illustrator;
    public MapGenerator mapGenerator;

    [SerializeField]
    LayerMask nodeMask;
    #endregion

    private void Start()
    {
        if (illustrator == null)
            illustrator = GetComponent<PathIllustrator>();
    }

    /// <summary>
    /// Main pathfinding function, marks tiles as being in frontier, while keeping a copy of the frontier
    /// in "currentFrontier" for later clearing
    /// </summary>
    /// <param name="character"></param>
    /// 
    public Path FindPath(Node origin, Node destination)
    {
        return FindPath(origin, destination, true, false);
    }
    public Path FindPath(Node origin, Node destination, bool illustrate, bool asolated)
    {

        if (origin.walkable && destination.walkable)
        {
            MinHeap<Node> openSet = new MinHeap<Node>(mapGenerator.gridMaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(origin);
            origin.gCost = 0;

            float nodeDistance = origin.GetComponent<MeshFilter>().sharedMesh.bounds.extents.z * 2;

            while (openSet.Count > 0)
            {
                
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                //Destination reached
                if (currentNode == destination)
                {
                    return PathBetween(destination, origin, illustrate);
                }

                foreach (Node neighbor in currentNode.GetNeighbors())
                {
                    if (((!asolated)? !neighbor.walkable : false) || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    float costToNeighbor = currentNode.gCost + nodeDistance;
                    if (costToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = costToNeighbor;
                        neighbor.hCost = Vector3.Distance(destination.transform.position, neighbor.transform.position);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                        else
                        {
                            openSet.UpdateItem(neighbor);
                        }
                    }

                }
            }
 
        }

        return null;
    }


    /// <summary>
    /// Called by Interact.cs to create a path between two tiles on the grid 
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public Path PathBetween(Node dest, Node source, bool illustrate)
    {
        Path path = MakePath(dest, source);
        if(illustrate) illustrator.IllustratePath(path);
        return path;
    }


    /// <summary>
    /// Creates a path between two tiles
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    private Path MakePath(Node destination, Node origin)
    {
        List<Node> nodes = new List<Node>();
        Node current = destination;

        while (current != origin)
        {
            nodes.Add(current);
            if (current.parent != null)
                current = current.parent;
            else
                break;
        }

        nodes.Add(origin);
        nodes.Reverse();

        Path path = new Path();
        path.waypoints = nodes.ToArray();

        return path;
    }

    int GetDistance(Node nodeA, Node nodeB) { 
        int distX = (int)Mathf.Abs(nodeA.transform.position.x - nodeB.transform.position.x);
        int distY = (int)Mathf.Abs(nodeA.transform.position.y - nodeB.transform.position.y);
        if(distX > distY)
        {
            return 14 * distY + 10 * (distX-distY);
        }
        return 14 * distX + 10 * (distY-distX);
    }


}
