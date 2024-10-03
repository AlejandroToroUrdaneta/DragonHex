using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Players availables
public class PlayerAvailable
{
    public string Name;

    public string getName(){
        return this.Name;
    }

    public void setName(string name){
        this.Name = name;
    }
}

public class SceneController : MonoBehaviour
{
    public Slider ProgressBar;
    public int maxMoves = 3; // Max moves per turn
    private int currentMoves; // Remaining moves in current turn
    private PlayerAvailable activePlayer; // Tracks the active player
    public Text playerText;

    // Start is called before the first frame update
    void Start()
    {
        this.currentMoves = this.maxMoves;
        this.activePlayer = new PlayerAvailable();
        this.activePlayer.setName("Player1");  // Starts with the human player
        playerText.text = this.activePlayer.getName();
    }

    // Update is called once per frame
    void Update()
    {
        var Bar = this.ProgressBar.GetComponent<TimeBar> ();
        Bar.DecreaseTurnTimer(1);
        // Controls if time is over or the moves are finished
        if (Bar.IsEmpty() || this.currentMoves == 0)
        {
            SwitchTurn();
        }

        // Controls if it's computer's turn to allow it to move
        if (this.activePlayer.getName() == "COM")
        {
            // Aggiungi qui la logica per le mosse automatiche del computer
            // Ad esempio, chiamare una funzione del tuo script di intelligenza artificiale del computer

            var COMCoroutine = COMTurn();
            StartCoroutine(COMCoroutine);
        }
    }

    // Coroutine for COM's turn that allows the COM to process the moves without blocking the entire game
    private IEnumerator COMTurn()
    {
        this.currentMoves = 0;

        // Aggiungi qui la logica per le mosse del computer
        while(this.currentMoves < this.maxMoves){
            this.PerformAction();

            yield return new WaitForSeconds(1f); // COM waits 1 sec to process the move
        }

        this.SwitchTurn(); // Switches automatically the turn after COM's turn
    }

    // Function to switch the active player
    private void SwitchPlayer()
    {
        if (this.activePlayer.getName() == "COM")
        {
            this.activePlayer.setName("Player1");
            playerText.text = this.activePlayer.getName();
        }
        else
        {
            this.activePlayer.setName("COM");
            playerText.text = this.activePlayer.getName();
        }

        // Puoi inserire qui la logica specifica per il cambio giocatore, se necessario
        this.PerformAction();
    }

    // Function called to switch a turn
    public void SwitchTurn()
    {
        var Bar = this.ProgressBar.GetComponent<TimeBar> ();
        // Resets the available moves number and the progress bar
        this.currentMoves = this.maxMoves;
        Bar.setBarMaxValue();

        // Cambia il giocatore attivo (puoi implementare la logica qui)
        this.SwitchPlayer();

    }

    // Function called when an action is performed during a turn
    public void PerformAction()
    {
        // Reduces the available moves number
        currentMoves++;

        // Puoi inserire qui la logica dell'azione del giocatore
    }
}
