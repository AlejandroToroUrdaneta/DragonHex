using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAI : MonoBehaviour
{
    [SerializeField] public List<EnemyInfoGetter> troops = new List<EnemyInfoGetter>();
    public int money;
    public int moneyFlux;
    public MinHeapOfInfluence<Node> minHeapConqueredNodes;
    public MaxHeapOfInfluence<Node> maxHeapConqueredNodes;

    [SerializeField] GameObject farm;
    [SerializeField] GameObject dragonLvl1;
    [SerializeField] GameObject dragonLvl2;
    [SerializeField] GameObject dragonLvl3;

    public ChangeCharacterControl changeControl;

    void Start()
    {
        money = 18;
        moneyFlux = 18;
    }

    public void Play()
    {
        money += moneyFlux;

        /*if (troops.Count > 0)
        {
            foreach (EnemyInfoGetter troop in troops)
            {
                
                troop.EnemyPlay();
            }
        }*/

        if (money > 5)
        {
            if (moneyFlux > 10)
            {
                Debug.Log("jaja compro dragon");
                CreateDragon();
            }
            else
            {
                Debug.Log("jaja compro granja");
                CreateFarm();
            }
        }

        PlayTroopsSequentially();

        changeControl.enemyTurn = false;
        changeControl.TakeTurns();
    }

    public async void PlayTroopsSequentially()
    {
        if (troops.Count > 0)
        {
            int numberOfTroops = troops.Count;
            for (int i = 0; i < numberOfTroops; i++)
            {
                await troops[i].EnemyPlay();
            }
        }
    }

    public void CreateDragon()
    {
        if (money >= 15)
        {
            money -= 15;
            moneyFlux -= 3;
            Node nodeToCreate = minHeapConqueredNodes.RemoveFirst();
            maxHeapConqueredNodes.Remove(nodeToCreate.HeapIndexMaxInfluence);
            EnemyInfoGetter dragon = Instantiate(dragonLvl3, nodeToCreate.transform.position, dragonLvl3.transform.rotation).GetComponent<EnemyInfoGetter>();
            nodeToCreate.onTopGmObj = dragon.gameObject;
            dragon.thisEnemy.enemyNode = nodeToCreate;
            updateInfluence(nodeToCreate, dragon.thisEnemy.influenceStrength, false);
            troops.Add(dragon);
        }
        if (money >= 10)
        {
            money -= 10;
            moneyFlux -= 2;
            Node nodeToCreate = minHeapConqueredNodes.RemoveFirst();
            maxHeapConqueredNodes.Remove(nodeToCreate.HeapIndexMaxInfluence);
            EnemyInfoGetter dragon = Instantiate(dragonLvl2, nodeToCreate.transform.position, dragonLvl2.transform.rotation).GetComponent<EnemyInfoGetter>();
            nodeToCreate.onTopGmObj = dragon.gameObject;
            dragon.thisEnemy.enemyNode = nodeToCreate;
            updateInfluence(nodeToCreate, dragon.thisEnemy.influenceStrength, false);
            troops.Add(dragon);
        }
        if (money >= 5)
        {
            money -= 5;
            moneyFlux -= 1;
            Node nodeToCreate = minHeapConqueredNodes.RemoveFirst();
            maxHeapConqueredNodes.Remove(nodeToCreate.HeapIndexMaxInfluence);
            EnemyInfoGetter dragon = Instantiate(dragonLvl1, nodeToCreate.transform.position, dragonLvl1.transform.rotation).GetComponent<EnemyInfoGetter>();
            nodeToCreate.onTopGmObj = dragon.gameObject;
            dragon.thisEnemy.enemyNode = nodeToCreate;
            updateInfluence(nodeToCreate, dragon.thisEnemy.influenceStrength, false);
            troops.Add(dragon);
        }
    }

    public void CreateFarm()
    {
        money -= 15;
        moneyFlux += 2;
        Node nodeToCreate = maxHeapConqueredNodes.RemoveFirst();
        minHeapConqueredNodes.Remove(nodeToCreate.HeapIndexInfluence);
        GameObject _farm = Instantiate(farm, nodeToCreate.transform.position, farm.transform.rotation);
        nodeToCreate.onTopGmObj = _farm;
    }

    public void updateInfluence(Node node, float influenceToModify, bool exit)
    {
        List<Node> neighbours = node.GetNeighbors(3, false);
        foreach (Node n in neighbours)
        {
            if (n.team == 1 && n.onTopGmObj is null)
            {
                minHeapConqueredNodes.Remove(n.HeapIndexInfluence);
                maxHeapConqueredNodes.Remove(n.HeapIndexMaxInfluence);
            }
        }

        PropagateInfluence(node, neighbours, influenceToModify);

        foreach (Node n in neighbours)
        {
            if (n.team == 1 && n.onTopGmObj is null)
            {
                minHeapConqueredNodes.Add(n);
                maxHeapConqueredNodes.Add(n);
            }
        }

        if (exit)
        {
            minHeapConqueredNodes.Add(node);
            maxHeapConqueredNodes.Add(node);
            moneyFlux += 1;
        }

    }

    public void PropagateInfluence(Node thisNode, List<Node> neighbours, float influence)
    {
        thisNode.influence += influence;
        foreach (Node neighbour in neighbours)
        {
            float distance = Vector3.Distance(thisNode.transform.position, neighbour.transform.position);
            neighbour.influence += influence / Mathf.Pow(1 + distance, 0.5f);
        }

    }
}
