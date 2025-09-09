using UnityEngine;
using UnityEngine.UI;

public class AssignRaidToCube : MonoBehaviour
{
    [SerializeField] RawImage[] raiderImages;
    Sprite streamerImage;
    string streamerName;
    public void assignStreamerImage(Sprite newImage, string streamerToRaid) { streamerImage = newImage; streamerName = streamerToRaid; }

    [SerializeField] float rotationSpeed = 10f; // Adjust the speed of rotation
    bool isRotating = true;
    private Vector3 randomRotationAxis;

    void Start()
    {
        // Generamos un eje de rotacion aleatorio al iniciar
        // Generate a random rotation axis on start
        randomRotationAxis = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    void Update()
    {
        // Rotamos el cubo en un eje aletorio
        // Rotate the cube around the random axis
        if (isRotating)
            transform.Rotate(randomRotationAxis * rotationSpeed * Time.deltaTime);
    }

    // Comprobamos que el cubo este rotando, si lo esta haciendo, entonces detenemos la rotacion y "revelamos" la imagen del streamer en cada lado del cubo
    // We check if the cube is rotating, and if it is, then we stop the rotation and we "reveal" the streamer image on each side of the cube
    public void collisionWithPlayer()
    {
        if (isRotating)
        {
            isRotating = false;
            // Reset the rotation of the cube
            transform.rotation = Quaternion.identity;

            foreach (RawImage image in raiderImages)
            {
                image.texture = streamerImage.texture;
            }


            // CUIDADO !! Si activas esto Y se dispara el evento, se iniciara el raid!! Incluso aunque no estes en directo
            // CAREFUL !! If you activate this AND trigger the event, the raid will start! Even if you are not streaming live
            //TwitchManager.instance.getUDPSender().doAction("Start Raid", "", streamerName, 0);
        }
    }
}
