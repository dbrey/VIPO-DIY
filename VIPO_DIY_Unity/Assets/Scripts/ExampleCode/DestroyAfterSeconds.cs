using UnityEngine;

#region Como usar
// Este script destruye el objeto al que se le adjunta despues de un tiempo determinado
// En el editor de Unity, se puede modificar "seconds" para cambiar el tiempo que tarda en destruirse
// Cada ciclo de Update, comprobamos si "timer" es mayor que 0, si es asi, restamos el tiempo que ha pasado desde el ultimo ciclo
// Si "timer" es menor o igual a 0, destruimos el objeto
#endregion

#region How to use
// This script destroys the object it is attached to after a certain amount of time
// In the Unity editor, you can modify "seconds" to change the time it takes to destroy
// Each Update cycle, we check if "timer" is greater than 0, if so, we subtract the time that has passed since the last cycle
// If "timer" is less than or equal to 0, we destroy the object
#endregion

public class DestroyAfterSeconds : MonoBehaviour
{
    
    [SerializeField] float seconds = 1f;
    float timer;

    void Start()
    {
        timer = seconds;
    }

    
    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
