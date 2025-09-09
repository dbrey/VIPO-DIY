using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 10f;
    
    // Podemos elegir que eje queremos rotar el objeto
    // We can choose which axis we want to rotate the object.
    public bool x;
    public bool y;
    public bool z;

    void Update()
    {
        if (x)
        {
            transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        }
        if (y)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
        if (z)
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
    }
}
