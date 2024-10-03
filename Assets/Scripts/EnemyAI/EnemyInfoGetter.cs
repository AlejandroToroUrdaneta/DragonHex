using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class EnemyInfoGetter : MonoBehaviour
{
    public Pathfinder pathfinder;
    Path path;
    public GlobalAI globalAI;
    
    [SerializeField] public EnemyMovement thisEnemy;
    //public Node enemyNode;

    //[SerializeField] private MinHeap<Node> priorityQueue;
    public List<Node> myNeighbours = new List<Node>();
    private List<Node> explorableNodes = new List<Node>();

    //public GameObject tower;
    public GameObject farm;
    public GameObject city;

    public bool safeState = true;
    Node defaultNode;

    private void Awake()
    {
        defaultNode = GameObject.Find("defaultNode").GetComponent<Node>();
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
        globalAI = GameObject.Find("Global AI").GetComponent<GlobalAI>();
        if (thisEnemy.enemyNode != null)
        {
            //myNeighbours = enemyNode.GetNeighbors(2, false);
            //EnemyPlay();
        }
    }

    public async Task EnemyPlay()
    {
        myNeighbours = ObserveSurroundings();
        await DecideNextMovement();
        //StartEnemyMove(path);
        //FinalizePosition(node);
    }

    public List<Node> ObserveSurroundings()
    {
        List<Node> neigbours = new List<Node>();
        neigbours = thisEnemy.enemyNode.GetNeighbors(3);

        return neigbours;
    }

    public async Task DecideNextMovement()
    {
        Node safestNode = defaultNode;
        Node dangerNode = defaultNode;
        Node explorableNode = defaultNode;

        Node cityNode = defaultNode;
        Node farmNode = defaultNode;
        Node playerNode = defaultNode;

        float totalEnemyPresence = 0;
        float totalAllyPresence = 0;

        explorableNodes = new List<Node>();

        if (myNeighbours.Count > 0)
        {

            foreach (Node node in myNeighbours)
            {
                //node controlled by enemy forces (player)
                if (node.influence < 0)
                {
                    totalEnemyPresence += node.influence;

                    if (node.onTopGmObj == city) //destroy city Max priority
                    {
                        cityNode = node;
                        continue;
                    }
                    /*else if (node.onTopGmObj == tower) //destroy tower priority
                    {
                        towerNode = node;
                        continue;
                    }*/
                    else if (node.onTopGmObj == farm) //destroy farm priority
                    {
                        farmNode = node;
                        continue;
                    }
                    else if (node.occupyingCharacter != null) //there is a enemy in this node
                    {
                        if(thisEnemy.level >= node.occupyingCharacter.level)
                        {
                            playerNode = node;
                        }
                        else
                        {
                            continue;
                        }
                        
                    }


                    if (dangerNode == defaultNode) //declare dangerNode if it wasnt declared before
                    {
                        dangerNode = node;
                    }
                    if (node.influence < dangerNode.influence && dangerNode != defaultNode) //update dangerNode if theres something better
                    {
                        dangerNode = node;
                    }

                }

                //node controlled by ally forces (ai)
                if (node.influence > 0)
                {
                    totalAllyPresence += node.influence;

                    if (safestNode == defaultNode && node.onTopGmObj == null) //declare safestNode if it wasnt declared before, and isnt
                                                                           //occupied by a ally
                    {
                        safestNode = node;
                    }
                    else if (node.influence > safestNode.influence && safestNode != defaultNode && node.onTopGmObj == null) //update safestNode if theres something better
                                                                                                                         //and isnt ocuppied by a ally
                    {
                        safestNode = node;
                    }
                }

                if (node.team == -1)
                {
                    explorableNodes.Add(node);
                }


            } //end of node search

            //go to random explorable Node
            explorableNode = ChooseRandomExplorableNode(explorableNode);

            //Declaration of total ally or enemy presence
            if (totalAllyPresence > -totalEnemyPresence)
            {
                safeState = true;
                if (explorableNode == defaultNode) //couldnt find explorable node and is in a
                                            //safe place with every node being on our team
                {
                    explorableNode = dangerNode; //will explore towards the enemy
                }
            }
            else
            {
                safeState = false;
            }

            dangerNode = SetDangerNode(dangerNode, cityNode, farmNode, playerNode, explorableNode);
            if (safeState)
            {
                //Attack nearby monuments
                //Attack nearby units
                //Explore / Conquest
                await thisEnemy.StarEnemyMove(ExploreOrAttack(dangerNode, cityNode, farmNode, playerNode, explorableNode));
            }
            else
            {
                //Move towards nearby ally (check if node is avaliable first)
                //Move towards nearby highest influence node (check if node is avaliable first)
                if (safestNode == defaultNode)
                {
                    safestNode = explorableNode;
                }
                //Debug.Log(safestNode);
                await MakePathAndGoToSafety(path, safestNode);
            }
            thisEnemy.enemyNode.team = 1;

        }
        else
        {
            Debug.Log("Error. No neighbours for this enemy. " +
                "Check if setted in a Node");
        }
    }

    private Node ChooseRandomExplorableNode(Node _explorableNode)
    {
        if (explorableNodes.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, explorableNodes.Count);
            _explorableNode = explorableNodes[randomIndex];
        }

        return _explorableNode;
    }

    private async Task MakePathAndGoToSafety(Path _path, Node _safestNode)
    {
        _path = pathfinder.FindPath(thisEnemy.enemyNode, _safestNode, false, false);
        await thisEnemy.StarEnemyMove(_path);
    }

    private Path ExploreOrAttack(Node dangerNode, Node cityNode, Node farmNode, Node playerNode, Node explorableNode)
    {
        
        if (cityNode == defaultNode && farmNode == defaultNode && playerNode == defaultNode && explorableNode != defaultNode)
        {
            //Go to explorableNode
            Debug.Log(thisEnemy.enemyNode);
            path = pathfinder.FindPath(thisEnemy.enemyNode, explorableNode, false, false);
        }
        else
        {
            //Go to dangerNode
            path = pathfinder.FindPath(thisEnemy.enemyNode, dangerNode, false, false);

        }
        return path;
    }

    private Node SetDangerNode(Node dangerNode, Node cityNode, Node farmNode, Node playerNode, Node explorableNode)
    {
        //Declaration of the dangerest dangerNode
        if (cityNode != defaultNode)
        {
            dangerNode = cityNode;
        }
        /*else if (towerNode != null)
        {
            dangerNode = towerNode;
        }*/
        else if (farmNode != defaultNode)
        {
            dangerNode = farmNode;
        }
        else if (playerNode != defaultNode)
        {
            dangerNode = playerNode;
        }
        else
        {
            dangerNode = explorableNode;
        }

        return dangerNode;
    }
}
