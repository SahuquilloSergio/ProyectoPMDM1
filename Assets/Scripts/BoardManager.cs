using System.Collections.Generic; 
using Random = UnityEngine.Random; 
using System;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
     }

    public int colums = 10;                     //Numero de columnas
    public int rows = 10;                       //Numero de filas
    public Count wallCount = new Count(3, 6);   //Minimo y maximo del numero aleatorio de muros por nivel
    public Count foodCount = new Count(1, 4);   //Minimo y maximo del numero aleatorio de comida por nivel

    public GameObject exit;                     //Prefab para salir del nivel
    public GameObject[] floorTiles;             //Array con prefabs de suelo
    public GameObject[] wallTiles;              //Array con prefabs de muros
    public GameObject[] foodTiles;              //Array con prefabs de comida
    public GameObject[] enemyTiles;             //Array con prefabs de enemigos
    public GameObject[] outerWallTiles;         //Array con prefabs de muros externos
    public GameObject healthPack;

    private Transform boardHolder;              //Variables para guardar la referencia de la transformacion del board ojbect
    private List<Vector3> gridPositions = new List<Vector3>();  //Lista de posibles localizaciones para colocar Tiles

    void InitialiseList()
    {
        //Limpiamos la lista de gridPositions
        gridPositions.Clear();

        //Loop a traves del eje x
        for (int x = 1; x < colums - 1; x++)
        {
            //Por cada columna, loop en el eje y
            for (int y = 1; y < rows; y++)
            {
                //A cada posicion añade un vector3 a la lista con las coordenadas del punto
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetUp()
    {
        //Instancia board y lo prepara para su transformacion
        boardHolder = new GameObject("Board").transform;

        //Loop a través del eje x para colocar el suelo y los bordes
        for (int x = -1; x < colums + 1; x++)
        {
            //Loop a través del eje y para colocar el suelo y los bordes
            for (int y = -1; y < rows + 1; y++)
            {
                //Elige un Tile random del array y lo prepara para instanciar
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                //Comprueba si estamos en un borde, si lo estamos elige un tile de borde y lo prepara para instanciar
                if (x == -1 || x == colums || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                //Instanciamos el objecto utilizando el prefab elegido para toInstantiate al vector3 correspondiente
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }
    //RandoPosition devuelve una posicion aleatoria de nuestra lista gridPositions
    Vector3 RandomPosition()
    {
        //Declaracion de un integer randomIndex, le da el valor de un numero aleatorio entre 0 y el numero de items de la lista gridPositions
        int RandomIndex = Random.Range(0, gridPositions.Count);

        //Declara una variable del tipo vector3 llamada randomPosition, le da el valor de RandomIndex
        Vector3 randomPosition = gridPositions[RandomIndex];

        //Elimina la posicion anterior de la lista para que no pueda ser reutilizada
        gridPositions.RemoveAt(RandomIndex);

        //Devuelve la posicion aleatoria del vector3 seleccionado
        return randomPosition;
    }

    //LayoutObjectAtRandom recibe un array de game objects para elegir el numero de objetos a crear
    void LayoutObjetcAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        //Elige el numero aleatorio de objetos a instanciar con el limite de minimos y maximos
        int objectCount = Random.Range(minimum, maximum + 1);

        //Instancia objetos hasta que se alcanza el limite
        for (int i =0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }
    
    //Inicializa el nivel y llama a las funciones previas
    public void SetupScene (int level)
    {
        //Crea el sueloy los bordes
        BoardSetUp();
        //Resetea la lista de gridpositions
        InitialiseList();
        LayoutObjetcAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjetcAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int)Math.Log(level, 2f);
        LayoutObjetcAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(Random.Range(0, colums -1), Random.Range(0, rows - 1), 0f), Quaternion.identity);
        //Instantiate(healthPack, new Vector3(Random.Range(0, colums - 1), Random.Range(0, rows - 1), 0f), Quaternion.identity);
    }
}
