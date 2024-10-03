using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool collapsed = false;
    public Node[] nodeOptions;

    public void CreateCell(bool collapseState, Node[] nodes)
    {
        collapsed = collapseState;
        nodeOptions = nodes;
    }

    public void RecreateCell(Node[] nodes)
    {
        nodeOptions = nodes;
    }
}
