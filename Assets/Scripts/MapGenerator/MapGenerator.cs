using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class MapGenerator : MonoBehaviour
{
    public Vector2Int dimensions = new Vector2Int(20, 20);
    public Node[] nodeObjects;
    public List<Cell> gridComponents;
    public Cell cellObj;

    public Node backupNode;
    public LayerMask layerMask;

    public int numOfTowns;

    private int iteration;

    //pathfinder
    public Pathfinder pathfinder;
    public GlobalAI globalAI;

    //other
    public GameObject Castle;
    public GameObject parentNodes;
    public GameObject parentCells;
    public GameObject townNode;
    public GameObject pathNode;
    public List<GameObject> typeOfHouses = new List<GameObject>();

    const int maxNumberOfHouses = 9;


    List<Node> towns = new List<Node>();

    Vector3 gridPosition;
    //public GameObject character;

    private readonly Vector3 North = new Vector3(0f, 0f, 1f);
    private readonly Vector3 NorthEast = new Vector3(1f, 0f, 1f).normalized;
    private readonly Vector3 SouthEast = new Vector3(1f, 0f, -1f).normalized;
    private readonly Vector3 South = new Vector3(0f, 0f, -1f);
    private readonly Vector3 SouthWest = new Vector3(-1f, 0f, -1f).normalized;
    private readonly Vector3 NorthWest = new Vector3(-1f, 0f, 1f).normalized;



    private void Awake()
    {
        gridComponents = new List<Cell>();
        globalAI = GameObject.Find("Global AI").GetComponent<GlobalAI>();
        globalAI.minHeapConqueredNodes = new MinHeapOfInfluence<Node>(gridMaxSize);
        globalAI.maxHeapConqueredNodes = new MaxHeapOfInfluence<Node>(gridMaxSize);
        GenerateGrid();
        //SetCharacterStartNode();

    }

    void GenerateGrid()
    {
        if (parentNodes == null)
            CreateNewParent();

        parentNodes.transform.position = gridPosition;

        InitializeGrid();
    }
    public int gridMaxSize { get { return dimensions.x * dimensions.y; } }
    Vector2 DetermineHexSize(Bounds hexBounds)
    {
        return new Vector2((hexBounds.extents.x * 2) * 0.75f, (hexBounds.extents.z * 2));
    }

    float UnevenRowOffset(float x, float y)
    {
        return x % 2 == 0 ? y / 2 : 0f;
    }

    void CreateNewParent()
    {
        parentNodes = new GameObject("Grid");
    }

    void InitializeGrid()
    {

        Vector2 hexSize = DetermineHexSize(backupNode.GetComponent<MeshFilter>().sharedMesh.bounds);
        Vector3 position = transform.position;
        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                position.x = transform.position.x + hexSize.x * x;
                position.z = transform.position.z + hexSize.y * y;

                position.z += UnevenRowOffset(x, hexSize.y);

                Cell newCell = Instantiate(cellObj, position, Quaternion.identity, parentCells.transform);
                newCell.name = "cell" + "(" + (x * dimensions.y + y) + ")" + "( x: " + x + ", y: " + y + ")";
                newCell.CreateCell(false, nodeObjects);
                gridComponents.Add(newCell);
            }
        }

        CollapseTowns();
        StartCoroutine(CheckEntropy());
    }

    void CollapseTowns()
    {
        Vector2 hexSize = DetermineHexSize(backupNode.GetComponent<MeshFilter>().sharedMesh.bounds);
        float maxMapAreaX = (dimensions.x * hexSize.x) - (hexSize.x * 2), maxMapAreaZ = (dimensions.y * hexSize.y) - (hexSize.y * 2);
        float minMapAreaX = hexSize.x * 2, minMapAreaZ = hexSize.y * 2;
        float distMinTowns = Mathf.Sqrt(maxMapAreaX * maxMapAreaX + maxMapAreaZ * maxMapAreaZ) / numOfTowns;


        while (numOfTowns != towns.Count)
        {
            int numberOfHouses = 0;
            Cell cell = parentCells.transform.GetChild(UnityEngine.Random.Range(0, parentCells.transform.childCount)).gameObject.GetComponent<Cell>();
            if (cell.transform.position.x > minMapAreaX && cell.transform.position.x < maxMapAreaX && cell.transform.position.z > minMapAreaZ && cell.transform.position.z < maxMapAreaZ)
            {
                if (towns.Count != 0)
                {
                    bool insertar = true;
                    foreach (Node town in towns)
                    {
                        if (Vector3.Distance(town.transform.position, cell.transform.position) < distMinTowns) insertar = false;
                    }
                    if (insertar)
                    {
                        cell.collapsed = true;
                        cell.nodeOptions = new Node[] { townNode.GetComponent<Node>() };
                        Node town = Instantiate(townNode, cell.transform.position, townNode.transform.rotation, parentNodes.transform).GetComponent<Node>();
                        town.team = 1;
                        town.ChangeToTeamColor();
                        List<Cell> neighbors = GetCellNeighbors(cell, 2);
                        foreach (Cell neighbor in neighbors)
                        {
                            neighbor.collapsed = true;
                            neighbor.nodeOptions = new Node[] { townNode.GetComponent<Node>() };
                            Node nodeNeighbor = Instantiate(townNode, neighbor.transform.position, townNode.transform.rotation, parentNodes.transform).GetComponent<Node>();
                            nodeNeighbor.team = 1;
                            nodeNeighbor.ChangeToTeamColor();
                            int built = UnityEngine.Random.Range(0, 2);
                            if (numberOfHouses <= maxNumberOfHouses && built % 2 == 0)
                            {
                                GameObject house = typeOfHouses[UnityEngine.Random.Range(0, typeOfHouses.Count)];
                                Vector3 OffsetPosition = new Vector3(neighbor.transform.position.x, neighbor.transform.position.y + 0.5f, neighbor.transform.position.z);
                                nodeNeighbor.onTopGmObj = Instantiate(house, OffsetPosition, house.transform.rotation, nodeNeighbor.transform);
                                nodeNeighbor.walkable = false;
                                numberOfHouses++;
                            }
                            if(nodeNeighbor.onTopGmObj == null)
                            {
                                globalAI.minHeapConqueredNodes.Add(nodeNeighbor);
                                globalAI.maxHeapConqueredNodes.Add(nodeNeighbor);
                            }
                        }
                        towns.Add(town);
                    }
                }
                else
                {
                    cell.collapsed = true;
                    cell.nodeOptions = new Node[] { townNode.GetComponent<Node>() };
                    Node town = Instantiate(townNode, cell.transform.position, townNode.transform.rotation, parentNodes.transform).GetComponent<Node>();
                    town.team = 0;
                    town.ChangeToTeamColor();
                    List<Cell> neighbors = GetCellNeighbors(cell, 2);
                    foreach (Cell neighbor in neighbors)
                    {
                        neighbor.collapsed = true;
                        neighbor.nodeOptions = new Node[] { townNode.GetComponent<Node>() };
                        Node nodeNeighbor = Instantiate(townNode, neighbor.transform.position, townNode.transform.rotation, parentNodes.transform).GetComponent<Node>();
                        nodeNeighbor.team = 0;
                        nodeNeighbor.ChangeToTeamColor();
                        int built = UnityEngine.Random.Range(0, 2);
                        if (numberOfHouses <= maxNumberOfHouses && built % 2 == 0)
                        {
                            GameObject house = typeOfHouses[UnityEngine.Random.Range(0, typeOfHouses.Count)];
                            Vector3 OffsetPosition = new Vector3(neighbor.transform.position.x, neighbor.transform.position.y + 0.5f, neighbor.transform.position.z);
                            nodeNeighbor.onTopGmObj = Instantiate(house, OffsetPosition, house.transform.rotation, nodeNeighbor.transform);
                            nodeNeighbor.walkable = false;
                            numberOfHouses++;
                        }
                    }
                    towns.Add(town);

                }
            }
        }
        iteration = 19 * numOfTowns;
    }

    void ConnectTowns()
    {
        Path path = pathfinder.FindPath(towns[0], towns[1], false, false);
        if (path == null) { path = pathfinder.FindPath(towns[0], towns[1], false, true); }
        if (path != null)
        {
            foreach (Node node in path.waypoints)
            {
                if (node.name != "P(Clone)")
                {
                    Vector3 position = node.transform.position;
                    Destroy(node.transform.gameObject);
                    Instantiate(pathNode, position, pathNode.transform.rotation, parentNodes.transform);
                }
                else
                {
                    if (node.onTopGmObj != null)
                    {
                        Destroy(node.onTopGmObj);
                    }
                }
            }
        }
        foreach (Node town in towns)
        {
            town.onTopGmObj = Instantiate(Castle, town.transform.position, Castle.transform.rotation, town.transform);
        }
    }

    IEnumerator CheckEntropy()
    {
        List<Cell> tempGrid = new List<Cell>(gridComponents);
        tempGrid.RemoveAll(c => c.collapsed);
        tempGrid.Sort((a, b) => a.nodeOptions.Length - b.nodeOptions.Length);
        tempGrid.RemoveAll(a => a.nodeOptions.Length != tempGrid[0].nodeOptions.Length);

        yield return null;

        CollapseCell(tempGrid);
    }

    void CollapseCell(List<Cell> tempGrid)
    {
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);

        Cell cellToCollapse = tempGrid[randIndex];

        cellToCollapse.collapsed = true;
        try
        {
            Node selectedNode = cellToCollapse.nodeOptions[UnityEngine.Random.Range(0, cellToCollapse.nodeOptions.Length)];
            cellToCollapse.nodeOptions = new Node[] { selectedNode };
        }
        catch
        {
            Node selectedNode = backupNode;
            cellToCollapse.nodeOptions = new Node[] { selectedNode };
            Debug.LogError("AQUI ESTA EL ERROR" + cellToCollapse.name);
        }

        Node foundNode = cellToCollapse.nodeOptions[0];
        Instantiate(foundNode, cellToCollapse.transform.position, foundNode.transform.rotation, parentNodes.transform);

        UpdateGeneration();
    }

    void UpdateGeneration()
    {
        List<Cell> newGenerationCell = new List<Cell>(gridComponents);

        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                var index = x * dimensions.y + y;
                var cell = gridComponents[index];

                if (gridComponents[index].collapsed)
                {
                    newGenerationCell[index] = gridComponents[index];
                }
                else
                {
                    List<Node> options = new List<Node>();
                    foreach (Node t in nodeObjects)
                    {
                        options.Add(t);
                    }

                    /* Checkin Neighbours rules to update actualCell rules*/

                    //North
                    if (y < dimensions.y - 1)
                    {

                        Cell north = GetCellInDirection(cell.transform.position, North);
                        List<Node> validOptions = new List<Node>();

                        foreach (Node possibleOptions in north.nodeOptions)
                        {
                            var validOption = Array.FindIndex(nodeObjects, obj => obj == possibleOptions);
                            var valid = nodeObjects[validOption].validSouthNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);


                    }

                    //NorthEast
                    if (!((x == dimensions.x - 1) ^ (y == dimensions.y - 1 && (x % 2 == 0))))
                    {
                        Cell northEast = GetCellInDirection(cell.transform.position, NorthEast);
                        List<Node> validOptions = new List<Node>();

                        foreach (Node possibleOptions in northEast.nodeOptions)
                        {
                            var validOption = Array.FindIndex(nodeObjects, obj => obj == possibleOptions);
                            var valid = nodeObjects[validOption].validSouthWestNeighbours;
                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //SouthEast
                    if ((x != dimensions.x - 1) && ((y > 0) || (y == 0 && x % 2 == 0)))
                    {
                        Cell southEast = GetCellInDirection(cell.transform.position, SouthEast);

                        List<Node> validOptions = new List<Node>();

                        foreach (Node possibleOptions in southEast.nodeOptions)
                        {
                            var validOption = Array.FindIndex(nodeObjects, obj => obj == possibleOptions);
                            var valid = nodeObjects[validOption].validNorthWestNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //South
                    if (y > 0)
                    {
                        Cell south = GetCellInDirection(cell.transform.position, South);
                        List<Node> validOptions = new List<Node>();

                        foreach (Node possibleOptions in south.nodeOptions)
                        {
                            var validOption = Array.FindIndex(nodeObjects, obj => obj == possibleOptions);
                            var valid = nodeObjects[validOption].validNorthNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //SouthWest
                    if (!(x == 0 ^ (y == 0 && (x % 2 != 0))))
                    {

                        Cell southWest = GetCellInDirection(cell.transform.position, SouthWest);
                        List<Node> validOptions = new List<Node>();

                        foreach (Node possibleOptions in southWest.nodeOptions)
                        {
                            var validOption = Array.FindIndex(nodeObjects, obj => obj == possibleOptions);
                            var valid = nodeObjects[validOption].validNorthEastNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //NorthWest
                    if ((x != 0) && ((y < dimensions.y - 1) || (y >= dimensions.y - 1) && (x % 2 != 0)))
                    {

                        Cell northWest = GetCellInDirection(cell.transform.position, NorthWest);
                        List<Node> validOptions = new List<Node>();

                        foreach (Node possibleOptions in northWest.nodeOptions)
                        {
                            var validOption = Array.FindIndex(nodeObjects, obj => obj == possibleOptions);
                            var valid = nodeObjects[validOption].validSouthEastNeighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    Node[] newNodeList = new Node[options.Count];

                    for (int i = 0; i < options.Count; i++)
                    {
                        newNodeList[i] = options[i];
                    }

                    newGenerationCell[index].RecreateCell(newNodeList);
                }
            }
        }

        gridComponents = newGenerationCell;
        iteration++;
        if ((iteration) < gridMaxSize)
        {
            StartCoroutine(CheckEntropy());
        }
        else
        {
            Destroy(parentCells);
            ConnectTowns();
            //SetCharacterStartNode();
        }
    }

    void CheckValidity(List<Node> optionList, List<Node> validOption)
    {
        for (int x = optionList.Count - 1; x >= 0; x--)
        {
            var element = optionList[x];
            if (!validOption.Contains(element))
            {
                optionList.RemoveAt(x);
            }
        }
    }

    Cell GetCellInDirection(Vector3 origen, Vector3 direction)
    {

        Cell cell = null;
        RaycastHit hit;
        if (Physics.Raycast(origen, direction, out hit, 20f, layerMask))
        {
            cell = hit.transform.gameObject.GetComponent<Cell>();
        }
        return cell;
    }


    public List<Cell> GetCellNeighbors(Cell origin, int range)
    {
        HashSet<Cell> cellNeighbors = new HashSet<Cell>();
        Vector3 direction = Vector3.forward;
        float rayLength = 2f * range;

        //Rotate a raycast in 60/range degree steps and find all adjacent nodes
        for (int i = 0; i < 6 * range; i++)
        {
            direction = Quaternion.Euler(0f, 60f / range, 0f) * direction;

            RaycastHit[] hits;
            hits = Physics.RaycastAll(origin.transform.position, direction, rayLength, layerMask);

            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    Cell hitCell = hit.transform.GetComponent<Cell>();
                    cellNeighbors.Add(hitCell);
                }

            }

            Debug.DrawRay(direction, direction * rayLength, Color.blue);
        }

        return cellNeighbors.ToList();
    }

}

