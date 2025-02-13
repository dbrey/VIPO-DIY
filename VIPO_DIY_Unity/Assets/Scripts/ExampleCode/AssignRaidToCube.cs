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
        // Generate a random rotation axis on start
        randomRotationAxis = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    void Update()
    {
        // Rotate the cube around the random axis
        if (isRotating)
            transform.Rotate(randomRotationAxis * rotationSpeed * Time.deltaTime);
    }

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


            // CAREFUL !! If you activate this AND trigger the event, the raid will start!
            //TwitchManager.instance.getUDPSender().doAction("Start Raid", "", streamerName, 0);
        }
    }
}
