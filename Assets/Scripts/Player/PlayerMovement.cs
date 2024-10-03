using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    #region member fields
    public bool Moving { get; private set; } = false;
    public PlayerMoveData movedata;
    public Node characterNode;
    public PathIllustrator illustrator;
    public ChangeCharacterControl characterControl;
    

    [SerializeField] LayerMask GroundLayerMask;

    public float influenceStrength;
    public int team = 0;
    public int level;
    public bool moved = false;
    public GlobalAI globalAI;
    #endregion

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindNodeAtStart();
        }
    }*/

    private void Start()
    {
        illustrator = GameObject.Find("Pathfinder").GetComponent<PathIllustrator>();
        characterControl = GameObject.Find("GameplayControl").GetComponent<ChangeCharacterControl>();
        globalAI = GameObject.Find("Global AI").GetComponent<GlobalAI>();
        FindNodeAtStart();
    }

    void FindNodeAtStart()
    {
        Debug.Log("Buscando y asignando nodo");
        if(characterNode != null)
        {
            FinalizePosition(characterNode);
            return;
        }
        if(Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 100f, GroundLayerMask))
        {
           
           FinalizePosition(hit.transform.GetComponent<Node>());
           return;
        }
        Debug.DrawRay(transform.position, -transform.up, Color.blue);
        Debug.Log("Unable to find a start position");
    }

    IEnumerator MoveAlongPath(Path path)
    {
        const float MIN_DISTANCE = 0.05f;
        //const float TERRAIN_PENALTY = 0.5f;

        int currentStep = 0;
        int pathLength = path.waypoints.Length-1;
        Node currentNode = path.waypoints[0];
        float animationTime = 0f;

        while(currentStep <= pathLength)
        {
            yield return null;

            //Move towards next step
            Vector3 nextNodePosition = path.waypoints[currentStep].transform.position;

            float movementTime = animationTime / (movedata.MoveSpeed);
            MoveAndRotate(currentNode.transform.position, nextNodePosition, movementTime);
            animationTime += Time.deltaTime;

            if(Vector3.Distance(transform.position, nextNodePosition) > MIN_DISTANCE)
            {
                continue;
            }

            //Min distance reached, look to next step
            currentNode = path.waypoints[currentStep];
            currentStep++;
            animationTime = 0f;
        }
        FinalizePosition(path.waypoints[pathLength]);
        moved = true;
    }

    public void StartMove(Path _path)
    {
        Moving = true;
        characterNode.available = true;
        characterNode.onTopGmObj = null;
        characterNode.occupyingCharacter = null;
        globalAI.updateInfluence(_path.waypoints[0], -influenceStrength, false);
        StartCoroutine(MoveAlongPath(_path));
        globalAI.updateInfluence(_path.waypoints[_path.waypoints.Length - 1], influenceStrength, false);
    }

    void FinalizePosition(Node node)
    {
        illustrator.DissapiredIllustration();
        transform.position = node.transform.position;
        characterNode = node;
        Moving = false;
        node.available = false;
        if (node.team == 1 && node.onTopGmObj == null)
        {
            globalAI.minHeapConqueredNodes.Remove(node.HeapIndexInfluence);
            globalAI.maxHeapConqueredNodes.Remove(node.HeapIndexMaxInfluence);
        }
        Killing(node);
        node.occupyingCharacter = this;
        node.team = 0;
        characterControl.playerProfile.fluxMoney += 1;
        node.ChangeToTeamColor();
        characterControl.ChangeUnit();
        
        node.onTopGmObj = this.gameObject; // pasarle lo que hay encima
    }

    void  MoveAndRotate(Vector3 origin, Vector3 destination, float duration)
    {
        transform.position = Vector3.Lerp(origin, destination, duration);
        transform.rotation = Quaternion.LookRotation(origin.DirectionTo(destination).Flat(), Vector3.up);
    }

    public void Killing(Node node)
    {
        if (node.onTopGmObj != null && node.team == 1)
        {
            if (node.enemyCharacter != null)
            {
                if (level < node.enemyCharacter.level)
                {
                    characterControl.allUnits.Remove(this.gameObject);
                    globalAI.updateInfluence(node, -influenceStrength, false);
                    globalAI.moneyFlux += 1;
                    characterControl.playerProfile.fluxMoney -= 1;
                    Destroy(this.gameObject);
                }
                else
                {
                    globalAI.troops.Remove(node.enemyCharacter.enemyDecisionMaking);
                    globalAI.updateInfluence(node, -node.enemyCharacter.influenceStrength, false);
                    node.enemyCharacter = null;
                    globalAI.moneyFlux -= 1;
                    Destroy(node.onTopGmObj);
                }
            }
            else if (node.onTopGmObj.CompareTag("Castillo"))
            {
                //Win
            }
            else
            {
                globalAI.moneyFlux -= 3;
                Destroy(node.onTopGmObj);
            }
        }
    }
}