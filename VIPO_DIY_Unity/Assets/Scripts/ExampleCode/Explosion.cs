using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float areaExplosion;
    [SerializeField] float explosionForce;

    private void OnDestroy()
    {
        // We look for all the colliders in the area of the explosion
        // Buscamos todos los colliders en el area de la explosion
        Collider[] colliders = Physics.OverlapSphere(transform.position, areaExplosion);

        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();

            // If it has a rigidbody, we apply an explosion force to it taking into account the location of both the explosion and the collider
            // Si tiene un rigidbody, le aplicamos una fuerza de explosion teniendo en cuenta la ubicacion tanto de la explosion como del collider
            if (rb != null)
            {
                Vector3 direction = collider.transform.position - transform.position;
                rb.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // We destroy the explosion object when it collides with something to trigger the explosion
        // Destruimos el objeto de la explosion cuando colisiona con algo para desencadenar la explosion
        Destroy(gameObject);
    }

}
