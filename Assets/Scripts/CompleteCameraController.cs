using UnityEngine;
using System.Collections;

public class CompleteCameraController : MonoBehaviour
{

    public GameObject player;       //Variable de referencia al jugador


    private Vector3 offset;         //Dsitancia offset entre jugador y camara

    // Inicializacion
    void Start()
    {
        //Calcula y guarda el valor de offset por la distacia entre el jugador y la camara
        offset = transform.position - player.transform.position;
    }

    // LateUpdate actualiza cada frame
    void LateUpdate()
    {
        // Iguala la posicion de la camara con la del jugador
        transform.position = player.transform.position + offset;
    }
}