using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public AudioSource efxSource;                  
    public AudioSource musicSource;                 
    public static SoundManager instance = null;               
    public float lowPitchRange = .95f;              
    public float highPitchRange = 1.05f;            


    void Awake()
    {
        //Comprueba si ya esta instanciado, si no, lo instancia, si ya esta lo destruye y lo crea de nuevo. Hacemos que no se destruya al pasar de nivel
        if (instance == null)
            
            instance = this;
        
        else if (instance != this)
           
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySingle(AudioClip clip)
    {
        //pasamos el audio a la variable clip
        efxSource.clip = clip;

        //Reproducimos el clip
        efxSource.Play();
    }


    //RandomizeSfx elige aleatoriamente entre varios audios y lo reproduce
    public void RandomizeSfx(params AudioClip[] clips)
    {
        //Genera un numero random entre 0 y el tamaño del array
        int randomIndex = Random.Range(0, clips.Length);

        //Elige un tono aleatorio entre el minimo y el maximo asignado
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        //Le da el tono al audio seleccionado
        efxSource.pitch = randomPitch;

        //Cogemos el clip
        efxSource.clip = clips[randomIndex];

        //Reproducimos el audio.
        efxSource.Play();
    }
}