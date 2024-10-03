using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCharacterControl : MonoBehaviour
{
    public List<GameObject> allUnits = new List<GameObject>();
    public CameraControl cameraChange;
    public GlobalAI globalAI;
    public PlayerProfile playerProfile;
    public bool enemyTurn = false;

    public int actualDragon = 0;
    public GameObject FocusDragon;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cameraChange.worldView = false;
            ChangeUnit();
            
        }

    }

    public void TakeTurns()
    {
        if (enemyTurn)
        {
            actualDragon = 0;
            globalAI.Play();
            enemyTurn = false;
        }
        else
        {
            playerProfile.coins += playerProfile.fluxMoney;
            playerProfile.UpdateText();
            foreach (GameObject unit in allUnits)
            {
                unit.GetComponent<PlayerMovement>().moved = false;
            }
            foreach (GameObject unit in allUnits)
            {
                ChangeUnit();
                Debug.Log("dragon actual " + actualDragon);
                
            }
        }
    }

    public void ChangeUnit()
    {

        if (allUnits.Count > 0)
        {
            if (actualDragon < allUnits.Count)
            {
                cameraChange.player = allUnits[actualDragon];
                FocusDragon = allUnits[actualDragon];
                actualDragon += 1;
            }
        }
        if (actualDragon >= allUnits.Count)
        {
            actualDragon = 0;
        }
    }

    public void PassTurn()
    {
        enemyTurn = true;
        TakeTurns();
    }
}
