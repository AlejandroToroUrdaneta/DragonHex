using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node :MonoBehaviour, IHeapItem<Node>, IHeapItemInfluence<Node>, IHeapItemMaxInfluence<Node>
{

    #region atributes
    //public Vector3 worldPosition;
    public Node parent;
    public Node connectedNode;
    public bool walkable = true;

    public LayerMask nodeMask;
    public float gCost, hCost;
    public int team = -1;
    public bool available { get; set; } = true;
    public PlayerMovement occupyingCharacter;
    public EnemyMovement enemyCharacter;
    int heapIndex, heapIndexInfluence, heapIndexMaxInfluence;

    public float influence = 0f;

    public GameObject onTopGmObj;
    public GameObject onTeamGmObj;


    public Node[] validNorthNeighbours;
    public Node[] validNorthEastNeighbours;
    public Node[] validSouthEastNeighbours;
    public Node[] validSouthNeighbours;
    public Node[] validSouthWestNeighbours;
    public Node[] validNorthWestNeighbours;

    private Color thisColor;

    #endregion

    void Awake()
    {
        if(this.gameObject.transform.childCount > 0)
        {
            onTeamGmObj = this.gameObject.transform.GetChild(0).gameObject;
        }

        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutElastic);
        thisColor = this.GetComponent<MeshRenderer>().material.color;

    }


    public Node()
    {

    }


    public float fCost
    {
        get { return gCost + hCost; }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int HeapIndexInfluence
    {
        get
        {
            return heapIndexInfluence;
        }
        set
        {
            heapIndexInfluence = value;
        }
    }

    public int HeapIndexMaxInfluence
    {
        get
        {
            return heapIndexMaxInfluence;
        }
        set
        {
            heapIndexMaxInfluence = value;
        }
    }

    public int CompareByAttributeIndex(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }

    public int CompareByAttributeInfluence(Node nodeToCompare)
    {
        int compare = influence.CompareTo(nodeToCompare.influence);
        
        return -compare;
    }


    #region Neighbors
    public List<Node> GetNeighbors()
    {
        return GetNeighbors(1, false);
    }

    public List<Node> GetNeighbors(int range)
    {
        return GetNeighbors(range, false);
    }

    public List<Node> GetNeighbors(int range, bool filter)
    {
        Node origin = this;
        HashSet<Node> nodeNeighbors = new HashSet<Node>();
        Vector3 direction = Vector3.forward;
        float rayLength = 1.816f * range;

        //Rotate a raycast in 60/range degree steps and find all adjacent nodes
        for (int i = 0; i < 6 * range; i++)
        {
            direction = Quaternion.Euler(0f, 60f / range, 0f) * direction;

            RaycastHit[] hits;
            hits = Physics.RaycastAll(origin.transform.position, direction, rayLength, nodeMask);

            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    Node hitNode = hit.transform.GetComponent<Node>();
                    if (hitNode.available == true && filter? this.team == hitNode.team : true)
                        nodeNeighbors.Add(hitNode);
                }

            }

            Debug.DrawRay(direction, direction * rayLength, Color.blue);
        }

        return nodeNeighbors.ToList();
    }
    #endregion

    public void ChangeToTeamColor() 
    {
        onTeamGmObj.GetComponent<TeamLigth>().ChangeColor(team);
    }

    private void SetColor(Color color)
    {
        this.GetComponent<MeshRenderer>().material.color = color;
    }

    public void Highlight()
    {
        SetColor(Color.white);
    }

    
    public void ClearHighlight()
    {
        SetColor(thisColor);
    }

    void OnDrawGizmos()
    {
        if(influence != 0)
        {
            Gizmos.color = new Color(1, 0, 0, influence);
            Gizmos.DrawSphere(transform.position, 1f);
            //Debug.Log("dibuja");
        }
    }

}
