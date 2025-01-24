using UnityEngine;
using Twitch_data;

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

    public void SuscriptionEvent(TwitchUtils.User user, TwitchUtils.Subscription subscription)
    {
        // We receive the name, profile picture, months suscribed and tier of the subscriber

        assignSuscriptionToUser(user, subscription);

        Debug.Log(user.UserName + " decided to suscribe to me for " + subscription.SubscribedMonthCount + " months with tier " + subscription.Tier);
    }

    public void SuscriptionGiftEvent(TwitchUtils.User user, TwitchUtils.Subscription subscription)
    {
        // We receive the name, profile picture, months suscribed and tier of the subscriber
        assignSuscriptionToUser(user, subscription);

        // We have to take into account if the gifter is anonymous. If it is, then we don't have a user to access
        //Debug.Log(subscription.Gifter + " decided to gift a suscription to " + receiverName + " for " + monthsSuscribed + " months with tier " + tier);
    }

    #endregion

    #region You don't need to touch this

    // Aqui asignamos la suscripcion al usuario para proximas referencias 
    // Here we assign the suscription to the user for future references
    void assignSuscriptionToUser(TwitchUtils.User user, TwitchUtils.Subscription subscription)
    { 
        user.subscription = subscription;
    }

    #endregion
}
