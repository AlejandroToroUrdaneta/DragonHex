using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public GameObject sceneController;

    public void onButtonClick(){        //Event processing when the player clicks the button
        var controller = this.sceneController.GetComponent<SceneController> ();
        controller.SwitchTurn();        //Switches the turn
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
