using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopHandler : MonoBehaviour
{
    public Text actualMoney;
    public int dragon1Cost = 5;
    public int dragon2Cost = 10;
    public int dragon3Cost = 15;
    public int barnCost    = 15;
    public int wallCost    = 40;

    public GameObject Dragon1; // Referencia al mesh del drag�n que aparecer�
    public GameObject Dragon2;
    public GameObject Dragon3;
    public GameObject Granja;
    public GameObject Muro;

    public Camera mainCam;
    public LayerMask interactMask;
    public GameObject invoked = null;
    public Node currentNode;

    public PlayerProfile playerProfile;
    public ChangeCharacterControl changeControl; 


    public CanvasGroup WarnPanel;
    
    private bool ShowWarn = true;

    void Start()
    {
        playerProfile.UpdateText();
    }

    private void Update()
    {
        if (invoked != null)
        {
            if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200f, interactMask))
            {
                currentNode = hit.transform.GetComponent<Node>();
                if (currentNode.available && currentNode.walkable && currentNode.team == 0)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3 newPosition = new Vector3(currentNode.transform.position.x, currentNode.transform.position.y + invoked.transform.position.y, currentNode.transform.position.z);
                        GameObject creation = Instantiate(invoked, newPosition, invoked.transform.rotation);
                        currentNode.onTopGmObj = creation;
                        if (invoked != Granja)
                        {
                            creation.GetComponent<PlayerMovement>().characterNode = currentNode;
                            creation.GetComponent<PlayerMovement>().moved = false;
                            changeControl.allUnits.Add(creation);
                            playerProfile.globalAI.updateInfluence(currentNode, creation.GetComponent<PlayerMovement>().influenceStrength, false);
                        }
                        playerProfile.UpdateText();
                        invoked = null;
                    }
                }
            }
        }
    }
    public void SpawnDragon1()
    {
        if (playerProfile.coins >= dragon1Cost)
        {
            invoked = Dragon1;
            playerProfile.coins = playerProfile.coins - dragon1Cost;
            playerProfile.fluxMoney -= 1;
        }
        else
        {
            Debug.Log("Not enough coins to buy Dragon Lvl 1");
            notEnoughtMoney();
        }
    }


    public void SpawnDragon2()
    {
        if (playerProfile.coins >= dragon2Cost)
        {
            invoked = Dragon2;
            playerProfile.coins = playerProfile.coins - dragon2Cost;
            playerProfile.fluxMoney -= 2;
        }
        else
        {
            Debug.Log("Not enough coins to buy Dragon Lvl 2");
            notEnoughtMoney();
        }
    }

    public void SpawnDragon3()
    {
        if (playerProfile.coins >= dragon3Cost)
        {
            invoked = Dragon3;
            playerProfile.coins = playerProfile.coins - dragon3Cost;
            playerProfile.fluxMoney -= 3;
        }
        else
        {
            Debug.Log("Not enough coins to buy Dragon Lvl 3");
            notEnoughtMoney();
        }
    }

    public void SpawnBarn()
    {
        if (playerProfile.coins >= barnCost)
        {
            invoked = Granja;
            playerProfile.coins = playerProfile.coins - barnCost;
            playerProfile.fluxMoney += 2;
        }
        else
        {
            Debug.Log("Not enough coins to buy Barn");
            notEnoughtMoney();
        }
    }
    public void SpawnWall()
        {
            if (playerProfile.coins >= wallCost)
            {
                //playerProfile.coins -= wallCost;
                Debug.Log("Spawn Wall for Player");
                InstantiateMuro();
        }
            else
            {
                Debug.Log("Not enough coins to buy Wall");
                notEnoughtMoney();
            }
        }

    void InstantiateMuro()
    {
        Instantiate(Muro, new Vector3(8, 1, 12), Quaternion.identity);
    }

    void notEnoughtMoney(){
        if (ShowWarn){
            StartCoroutine(alfaChange());
        }
        
    }

    private IEnumerator alfaChange(){
        Debug.Log("corutina");
        ShowWarn = false;
        WarnPanel.alpha = 0f;
        float tiempoInicio = Time.time;
        while (WarnPanel.alpha < 1f)
        {
            WarnPanel.alpha += Time.deltaTime / 0.3f;
            yield return null;
        }
        
        yield return new WaitForSeconds(0.5f);

        tiempoInicio = Time.time;
        while (WarnPanel.alpha > 0f)
        {
            WarnPanel.alpha -= Time.deltaTime / 0.3f;
            yield return null;
        }
        ShowWarn = true;
    }
}
