using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyMovement : MonoBehaviour
{
    #region
    public bool Moving { get; private set; } = false;
    public PlayerMoveData enemyMoveData;
    public Node enemyNode;
    public EnemyInfoGetter enemyDecisionMaking;

    [SerializeField] LayerMask GroundLayerMask;

    public float influenceStrength;
    public int team = 1; //enemy team
    public int level = 0;
    public bool movido = false;
    public ChangeCharacterControl changeCharacterControl;
    //public int enemyID = 0;
    #endregion

    private void Start()
    {
        changeCharacterControl = GameObject.Find("GameplayControl").GetComponent<ChangeCharacterControl>();
        FindMyStartNode();
    }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            FindMyStartNode();
            enemyNode.influence = -5;
            enemyDecisionMaking.EnemyPlay();
        }
    }*/

    void FindMyStartNode()
    {
        if (enemyNode != null)
        {
            FinalizePosition(enemyNode);
            return;
        }
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 100f, GroundLayerMask))
        {
            FinalizePosition(hit.transform.GetComponent<Node>());
            return;
        }
        Debug.Log("Unable to find a start position for enemy: ");
    }

    async Task MoveAlongPath(Path path)
    {
        const float MIN_DISTANCE = 0.05f;
        //const float TERRAIN_PENALTY = 0.5f;

        int currentStep = 0;
        int pathLength = path.waypoints.Length - 1;
        Node currentNode = path.waypoints[0];
        float animationTime = 0f;

        enemyDecisionMaking.globalAI.updateInfluence(currentNode, -influenceStrength, true);

        while (currentStep <= pathLength)
        {
            await Task.Yield();

            //Move towards next step
            Vector3 nextNodePosition = path.waypoints[currentStep].transform.position;

            float movementTime = animationTime / (enemyMoveData.MoveSpeed);
            MoveAndRotate(currentNode.transform.position, nextNodePosition, movementTime);
            animationTime += Time.deltaTime;

            if (Vector3.Distance(transform.position, nextNodePosition) > MIN_DISTANCE)
            {
                continue;
            }

            //Min distance reached, look to next step
            currentNode = path.waypoints[currentStep];
            currentStep++;
            animationTime = 0f;
        }
        FinalizePosition(path.waypoints[pathLength]);
        enemyDecisionMaking.globalAI.updateInfluence(path.waypoints[pathLength], influenceStrength, false);
        await Task.Yield();
    }
    public async Task StarEnemyMove(Path _path)
    {
        Moving = true;
        enemyNode.available = true;
        enemyNode.onTopGmObj = null;
        enemyNode.enemyCharacter = null;
        await MoveAlongPath(_path);
    }
    void FinalizePosition(Node node)
    {
        transform.position = node.transform.position;
        enemyNode = node;
        Moving = false;
        node.available = true;
        enemyDecisionMaking.globalAI.moneyFlux += 1;
        Killing(node);
        node.team = 1;
        node.ChangeToTeamColor();
        node.enemyCharacter = this;
        node.onTopGmObj = this.gameObject;
        movido = true;
    }
    void MoveAndRotate(Vector3 origin, Vector3 destination, float duration)
    {
        transform.position = Vector3.Lerp(origin, destination, duration);
        transform.rotation = Quaternion.LookRotation(origin.DirectionTo(destination).Flat(), Vector3.up);
    }

    public void Killing(Node node)
    {
        if (node.onTopGmObj != null && node.team == 0)
        {
            if (node.occupyingCharacter != null)
            {
                changeCharacterControl.allUnits.Remove(node.onTopGmObj);
                enemyDecisionMaking.globalAI.updateInfluence(node, -node.onTopGmObj.GetComponent<PlayerMovement>().influenceStrength, false);
                Destroy(node.onTopGmObj);
                changeCharacterControl.playerProfile.fluxMoney -= 1;
            }
            else if(node.onTopGmObj.CompareTag("Castillo"))
            {
                SceneManager.LoadScene(3);
            }
            else
            {
                Destroy(node.onTopGmObj);
                changeCharacterControl.playerProfile.fluxMoney -= 3;
            }
        }

        if (node.onTopGmObj != null && node.team == 1) //destruir equipo enemigo
        {
            if (node.enemyCharacter != null)
            {
                changeCharacterControl.allUnits.Remove(node.onTopGmObj);
                changeCharacterControl.globalAI.updateInfluence(node, -node.onTopGmObj.GetComponent<EnemyMovement>().influenceStrength, false);

                Destroy(node.onTopGmObj);
                enemyDecisionMaking.globalAI.moneyFlux -= 1;
            }
            else if (node.onTopGmObj.CompareTag("Castillo"))
            {
                SceneManager.LoadScene(2);
            }
            else
            {
                Destroy(node.onTopGmObj);
                enemyDecisionMaking.globalAI.moneyFlux -= 3;
            }
        }
    }


}
