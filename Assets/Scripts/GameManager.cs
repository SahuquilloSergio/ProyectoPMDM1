using UnityEngine;
using System.Collections;


using System.Collections.Generic;       
using UnityEngine.UI;                   

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;                      //Tiempo de espera al iniciar el nivel
    public float turnDelay = 0.1f;                          //Delay entre turnos
    public int playerFoodPoints = 50;                       //Valor inicial de los puntos de comida
    public static GameManager instance = null;              //Instancia static del game manager para poder ser llamado en otras clases/metodos
    [HideInInspector] public bool playersTurn = true;       //Boolean para comprobar si es el turno del jugador, no sale en el editot


    private Text levelText;                                 //Texto del nivel
    private GameObject levelImage;                          //Imagen para tapar la generacion del nivel
    private BoardManager boardScript;                       //Referencia al boardmanager
    private int level = 1;                                  //Nivel inicial
    private List<Enemy> enemies;                            //Lista de los enemigos
    private bool enemiesMoving;                             //Boolean para comprobar si se mueven los enemigos
    private bool doingSetup = true;                         //Boolean que comprueba si se esta generando el nivel para evitar que se mueva



    //Awake siempre se llama antes de cualquier start
    void Awake()
    {
        //Comprueba si existe la instania
        if (instance == null)

            //si no, la crea
            instance = this;

        //Si la instancia existe pero no es this:
        else if (instance != this)

            //Lo destruye.
            Destroy(gameObject);

        //Hace que this no se destruya al recargar
        DontDestroyOnLoad(gameObject);

        //Assigna enemigos a la lista
        enemies = new List<Enemy>();

        //Referencia al boardManager
        boardScript = GetComponent<BoardManager>();

        //Incia el primer nivel
        InitGame();
    }

    //Este codigo se ejecuta cada vez que se recarga el nivel.
    void OnLevelWasLoaded(int index)
    {
        //Añade 1 al nivel
        level++;
        //Inicializa el nivel
        InitGame();
    }

    //Initializes the game for each level.
    void InitGame()
    {
        //While doingSetup is true the player can't move, prevent player from moving while title card is up.
        doingSetup = true;

        //Get a reference to our image LevelImage by finding it by name.
        levelImage = GameObject.Find("LevelImage");

        //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        //Set the text of levelText to the string "Day" and append the current level number.
        levelText.text = "Day " + level;

        //Set levelImage to active blocking player's view of the game board during setup.
        levelImage.SetActive(true);

        //Call the HideLevelImage function with a delay in seconds of levelStartDelay.
        Invoke("HideLevelImage", levelStartDelay);

        //Clear any Enemy objects in our List to prepare for next level.
        enemies.Clear();

        //Call the SetupScene function of the BoardManager script, pass it current level number.
        boardScript.SetupScene(level);

    }


    //Hides black image used between levels
    void HideLevelImage()
    {
        //Disable the levelImage gameObject.
        levelImage.SetActive(false);

        //Set doingSetup to false allowing player to move again.
        doingSetup = false;
    }

    //Update is called every frame.
    void Update()
    {
        //Check that playersTurn or enemiesMoving or doingSetup are not currently true.
        if (playersTurn || enemiesMoving || doingSetup)

            //If any of these are true, return and do not start MoveEnemies.
            return;

        //Start moving enemies.
        StartCoroutine(MoveEnemies());
    }

    //Call this to add the passed in Enemy to the List of Enemy objects.
    public void AddEnemyToList(Enemy script)
    {
        //Add Enemy to List enemies.
        enemies.Add(script);
    }


    public void GameOver()
    {
        levelText.text = "After " + level + " days, you died.";

        levelImage.SetActive(true);

        enabled = false;
    }

    //Coroutine to move enemies in sequence.
    IEnumerator MoveEnemies()
    {
        //While enemiesMoving is true player is unable to move.
        enemiesMoving = true;

        //Wait for turnDelay seconds, defaults to .1 (100 ms).
        yield return new WaitForSeconds(turnDelay);

        //If there are no enemies spawned (IE in first level):
        if (enemies.Count == 0)
        {
            //Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
            yield return new WaitForSeconds(turnDelay);
        }

        //Loop through List of Enemy objects.
        for (int i = 0; i < enemies.Count; i++)
        {
            //Call the MoveEnemy function of Enemy at index i in the enemies List.
            enemies[i].MoveEnemy();

            //Wait for Enemy's moveTime before moving next Enemy, 
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        //Once Enemies are done moving, set playersTurn to true so player can move.
        playersTurn = true;

        //Enemies are done moving, set enemiesMoving to false.
        enemiesMoving = false;
    }
}