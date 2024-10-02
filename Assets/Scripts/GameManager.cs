using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// This class is responsible for managing
/// game state and logic.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Define the different states of the game
    public enum GameState
    {
        Gameplay,
        Inventory,
        Stats,
        Options
        //GameOver,
    }

    public GameState currentState;//stores the current state of the game
    public GameState previousState;//stores the previous state of the game before pause

    //[Header("Damage Text Settings")]
    //public Canvas damageTextCanvas; //used to draw the floating text
    //public float textFontSize = 20; //adjustable font size
    //public TMP_FontAsset textFont;
    //public Camera referenceCamera; //allows the ability to know where on the canvas the text should appear

    [Header("Screens")]
    public GameObject inventoryScreen;//game object to store the pause menu
    public GameObject statsScreen;
    public GameObject optionsScreen;

    public GameObject playerObject; //reference player game object

    void Awake()
    {
        //check to see if there is another singleton of this kind
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("EXTRA" + this + " DELETED");
            Destroy(gameObject);
        }

        DisableScreens();
    }

    private void Update()
    {
        //Defining the behavior for each game state
        switch (currentState)
        {
            case GameState.Gameplay:
            case GameState.Inventory:
            case GameState.Options:
                CheckForPauseAndResume();
                break;
            case GameState.Stats:
                break;
            default:
                Debug.LogWarning("STATE DOES NOT EXIST");
                break;
        }
    }

    //allows control over all state changes
    public void ChangeState(GameState newState)
    {
        previousState = currentState;
        currentState = newState;
    }

    public void OpenInventory()
    {
        //checks if the game is not already paused
        if (currentState != GameState.Inventory)
        {
            ChangeState(GameState.Inventory);
            Time.timeScale = 0f;//stop the game
            inventoryScreen.SetActive(true);//activates pause screen
        }
    }

    public void ResumeGame()
    {
        if (currentState != GameState.Gameplay)
        {
            ChangeState(GameState.Gameplay);
            Time.timeScale = 1f;
            DisableScreens();
        }
    }

    //checks for input to pause or resume game
    void CheckForPauseAndResume()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (currentState != GameState.Gameplay)
            {
                ResumeGame();
            }
            else
            {
                OpenInventory();
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (currentState != GameState.Gameplay)
            {
                ResumeGame();
            }
            else
            {
                OpenOptions();
            }
        }
    }

    // References for different tabs in inventory. 
    // Control with onClick events.

    public void OpenStats()
    {
        DisableScreens();
        ChangeState(GameState.Stats);
        statsScreen.SetActive(true);
    }

    public void OpenOptions()
    {
        DisableScreens();
        ChangeState(GameState.Options);
        optionsScreen.SetActive(true);
    }

    //reference to deactivate all game screens
    void DisableScreens()
    {
        inventoryScreen.SetActive(false);
        statsScreen.SetActive(false);
        optionsScreen.SetActive(false);
    }
}
