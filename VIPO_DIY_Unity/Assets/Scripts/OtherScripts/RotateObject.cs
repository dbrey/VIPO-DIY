using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 10f;
    
    // We can choose which axis we want to rotate the object.
    // We make it public in case there are other scripts that want to modify it.
    public bool x;
    public bool y;
    public bool z;

    void Update()
    {
        // We rotate the object in the chosen axis.
        if (x)
        {
            // X axis
            transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        }
        if (y)
        {
            // Y axis
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
        if (z)
        {
            // Z axis
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }

    }
}
