using UnityEngine;

public class Launch : MonoBehaviour
{
    [SerializeField] float launchForce = 1;
    
    // We launch the object towards the objective position with a certain launch force
    // Lanzamos el objeto hacia la posicion objetivo con una cierta fuerza de lanzamiento
    public void LaunchObject(Vector3 objectivePosition)
    { 
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 direction = objectivePosition - transform.position;
            rb.AddForce(direction.normalized * launchForce);
        }
    }
}
