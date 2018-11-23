using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;      
using UnityEngine.UI;


public class Player : MovingObject
{
    public float restartLevelDelay = 1f;        //Delay que hay al iniciar el nivel
    public int pointsPerFood = 5;               //Puntos que obtienes al recoger comida
    public int pointsPerSoda = 10;              //Puntos que obtienes al recoger bebida
    public int pointsPerHealthPack = 25;        //Puntos que obtienes al recoger el Health Pack
    public int wallDamage = 1;                  //Daño por tic que haces a los muros internos
    public Text foodText;
    public AudioClip moveSound1;                //Audios
    public AudioClip moveSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip gameOverSound;


    private Animator animator;                  //Referencia al animator
    private int food;                           //Variable que guarda la comida
    private int step;                           //Variable para bajar la comida cada 2 pasos
    private Vector2 touchOrigin = -Vector2.one;


 
    protected override void Start()
    {
        animator = GetComponent<Animator>();

        //Le damos el valor de la comida al gameManager para que la guarde al cambiar de nivel
        food = GameManager.instance.playerFoodPoints;

        foodText.text = "Food: " + food;

        base.Start();
    }


    private void OnDisable()
    {
        //Recargamos el valor de la comida al cambiar de nivel
        GameManager.instance.playerFoodPoints = food;
    }


    private void Update()
    {
        //Si no es el truno del jugardor, sale de la funcion
        if (!GameManager.instance.playersTurn) return;

        int horizontal = 0;
        int vertical = 0;

        //#if UNITY_STANDALONE || UNITY_WEBPLAYER 

        //Detecta el input con el inputManager, lo castea a int y lo guarda en horizontal
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));

        //Detecta el input con el inputManager, lo castea a int y lo guarda en vertical
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        //Comprueba si nos movemos en horizontal, si lo hace pone vertical a 0, esto es para no movernos en diagonal
        if (horizontal != 0)
        {
            vertical = 0;
        }


        //CONTROLES ANDROID/IOS
        ////Comprueba si estamos en un dispositivo movil
        //#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

        ////Comprueba si se toca la pantalla
        //if (Input.touchCount > 0)
        //{
        //    //Guarda el primer toque
        //    Touch myTouch = Input.touches[0];

        //    //Check if the phase of that touch equals Began
        //    if (myTouch.phase == TouchPhase.Began)
        //    {
        //        //If so, set touchOrigin to the position of that touch
        //        touchOrigin = myTouch.position;
        //    }

        //    //If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
        //    else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
        //    {
        //        //Set touchEnd to equal the position of this touch
        //        Vector2 touchEnd = myTouch.position;

        //        //Calculate the difference between the beginning and end of the touch on the x axis.
        //        float x = touchEnd.x - touchOrigin.x;

        //        //Calculate the difference between the beginning and end of the touch on the y axis.
        //        float y = touchEnd.y - touchOrigin.y;

        //        //Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
        //        touchOrigin.x = -1;

        //        //Check if the difference along the x axis is greater than the difference along the y axis.
        //        if (Mathf.Abs(x) > Mathf.Abs(y))
        //            //If x is greater than zero, set horizontal to 1, otherwise set it to -1
        //            horizontal = x > 0 ? 1 : -1;
        //        else
        //            //If y is greater than zero, set horizontal to 1, otherwise set it to -1
        //            vertical = y > 0 ? 1 : -1;
        //    }
        //}

        //#endif 
        //Fin del codigo de movil

        //Comprueba que no hay un 0 en las variable de movimiento
        if (horizontal != 0 || vertical != 0)
        {
            //Llama al metodo de AttemptMove y especifica el objeto con el que se encuentra, en este caso un muro
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //Por cada 2 movimientos le restamos 1 punto de comida.
        step++;
        if (step == 2)
        {
            food--;
            step = 0;
        }

        foodText.text = "Food: " + food;
        
        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        if (Move (xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        if (Move(xDir, yDir, out hit))
        {
            //Añadimos sonido
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        //comprobamos si nos quedamos sin comida
        CheckIfGameOver();

        //Finaliza el turno del jugador
        GameManager.instance.playersTurn = false;
    }



    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;

        //Llamamos al metodo hitwall y le pasamos el objeto
        hitWall.DamageWall(wallDamage);

        //Animacion del jugador para golpear el muro
        animator.SetTrigger("playerChop");
    }


    //Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Comprueba el tag
        if (other.tag == "Exit")
        {
            //Llama al metodo restart y le aplica el delay
            Invoke("Restart", restartLevelDelay);

            //Desactiva el objecto jugador al salir del nivel
            enabled = false;
        }

        //Comprueba el tag
        else if (other.tag == "Food")
        {
            //Añadimos los puntos de comida al jugador
            food += pointsPerFood;
            foodText.text = " +" + pointsPerFood + " Food: " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);

            //Desactivamos el objeto al recogerlo
            other.gameObject.SetActive(false);
        }

        //Comprobamos el tag
        else if (other.tag == "Soda")
        {
            //Añadimos puntos de comida
            food += pointsPerSoda;
            foodText.text = " +" + pointsPerSoda + " Food: " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);

            //Desactivamos el objeto
            other.gameObject.SetActive(false);
        }
    }


    //Reinicia la escena cuando es llamado
    private void Restart()
    {
        //Recarga la escena
        SceneManager.LoadScene(0);
    }


    //Metodo que nos quita vida cada vez que nos golpean, le pasamos la cantidad
    public void LoseLife(int loss)
    {
        //Animacion del personaje al ser golpeado
        animator.SetTrigger("playerHit");

        //Restamos la comida
        food -= food;
       foodText.text = "-" + food + "Food: " + food;

        //Comprobamos si se acabo el juego
        CheckIfGameOver();
    }


    //Comprueba si se acaba el juego
    private void CheckIfGameOver()
    {
        //Comprueba si la comida es igual o inferior a 0
        if (food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            //Llama a GameOver
            GameManager.instance.GameOver();
        }
        
    }
}
