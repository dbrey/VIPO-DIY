using UnityEngine;

public class SuscriptionManager : MonoBehaviour
{
    public static SuscriptionManager instance;



    private void Awake()
    {
        // If there's no instance, we set this as the instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // If there's already an instance, we destroy this one
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Methods called by StreamerBotEvent Manager

    public void SuscriptionEvent(string subscriberName, string subscriberProfilePicture, int monthsSuscribed, string tier)
    { 
        // We receive the name, profile picture, months suscribed and tier of the subscriber

        Debug.Log(subscriberName + " decided to suscribe to me for " + monthsSuscribed + " months with tier " + tier);
    }

    public void SuscriptionGiftEvent(string gifterName, string gifterProfilePicture, string receiverName, string receiverProfilePicture, int monthsSuscribed, string tier)
    {
        // We receive the name, profile picture, months suscribed and tier of the subscriber

        Debug.Log(gifterName + " decided to gift a suscription to " + receiverName + " for " + monthsSuscribed + " months with tier " + tier);
    }

    #endregion
}
