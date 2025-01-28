using UnityEngine;
using Twitch_data;
using System.Collections.Generic;

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


    // Solo queremos almacenar los ultimos n seguidores.
    // We only want to store the latest n followers.
    [SerializeField] bool storeLatestSubscribers = true;
    [SerializeField] int nLatestSubscribers= 5;
    private List<string> latestSubscribers = new List<string>();

    private void Start()
    {
        latestSubscribers = new List<string>();
    }

    #region Methods called by StreamerBotEvent Manager

    public void SuscriptionEvent(TwitchUtils.User user, TwitchUtils.Subscription subscription)
    {
        // We receive the name, profile picture, months suscribed and tier of the subscriber

        assignSuscriptionToUser(user, subscription);

        if (storeLatestSubscribers)
        {
            // If the list is full, we remove the oldest follower
            if (latestSubscribers.Count >= nLatestSubscribers)
            {
                latestSubscribers.RemoveAt(0);
            }
        }

        Debug.Log(user.UserName + " decided to suscribe to me for " + subscription.SubscribedMonthCount + " months with tier " + subscription.Tier);
    }

    public void SuscriptionGiftEvent(TwitchUtils.User user, TwitchUtils.Subscription subscription)
    {
        // We receive the name, profile picture, months suscribed and tier of the subscriber
        assignSuscriptionToUser(user, subscription);

        // [OPTIONAL]
        // Guardamos el ultimo suscriptor para que el usuario pueda acceder a ello mas tarde
        // We store the latest subscriber so the user can access it later
        if (storeLatestSubscribers)
        {
            // Si la lista esta llena, eliminamos el suscriptor mas antiguo
            // If the list is full, we remove the oldest subscriber
            if (latestSubscribers.Count >= nLatestSubscribers)
            {
                latestSubscribers.RemoveAt(0);
            }
        }

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
