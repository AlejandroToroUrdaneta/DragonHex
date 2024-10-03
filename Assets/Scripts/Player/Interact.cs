using UnityEngine;
using UnityEngine.TextCore.Text;
using System;

[RequireComponent(typeof(AudioSource))]
public class Interact : MonoBehaviour
{
    #region member fields
    public LayerMask interactMask;

    //Debug purposes only
    [SerializeField]
    bool debug;
    Path Lastpath;

    Camera mainCam;

    Node currentNode;
    PlayerMovement selectedCharacter;

    public ChangeCharacterControl changeCharacterControl;

    public Pathfinder pathfinder;
    #endregion

    private void Start()
    {
        mainCam = gameObject.GetComponent<Camera>();

        if (pathfinder == null)
            pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
    }

    private void Update()
    {
        Clear();
        MouseUpdate();
    }

    private void MouseUpdate()
    {
        
        if (!Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200f, interactMask))
            return;

        currentNode = hit.transform.GetComponent<Node>();
        InspectNode();
    }

    private void InspectNode()
    {
        if (!currentNode.available)
            InspectCharacter();
        else
            NavigateToNode();
    }

    private void InspectCharacter()
    {
        if ((currentNode.team == 0 && currentNode.occupyingCharacter.Moving) || currentNode.occupyingCharacter.moved || currentNode.occupyingCharacter.gameObject != changeCharacterControl.FocusDragon)
        {
            return;
        }

        currentNode.Highlight();

        if (Input.GetMouseButtonDown(0))
        {
            SelectCharacter();
        }
    }

    private void Clear()
    {
        if (currentNode == null || currentNode.available == true)
        {
            return;
        }

        //currentNode.ModifyCost(currentNode.terrainCost-1);//Reverses to previous cost and color after being highlighted
        currentNode.ClearHighlight();
        //currentNode.onTopGmObj = null;
        currentNode = null;
    }

    private void SelectCharacter()
    {
        selectedCharacter = currentNode.occupyingCharacter;
        //GetComponent<AudioSource>().PlayOneShot(pop);
    }

    private void NavigateToNode()
    {
        if (selectedCharacter == null || selectedCharacter.Moving == true)
            return;

        if (RetrievePath(out Path newPath))
        {
            if (Input.GetMouseButtonDown(0))
            {
                //GetComponent<AudioSource>().PlayOneShot(click);
                selectedCharacter.StartMove(newPath);

                selectedCharacter = null;
            }
        }
    }

    bool RetrievePath(out Path path)
    {
        if (Vector3.Distance(selectedCharacter.characterNode.transform.position, currentNode.transform.position) <= 5.7f || currentNode.team == selectedCharacter.characterNode.team)
        {
            path = pathfinder.FindPath(selectedCharacter.characterNode, currentNode);
            if (path == null)
                return false;
            return true;
        }

        path = null;
        return false;
    }
}