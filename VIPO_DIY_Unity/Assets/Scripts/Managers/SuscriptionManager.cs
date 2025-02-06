using UnityEngine;
using Twitch_data;
using System.Collections.Generic;
using static Twitch_data.TwitchUtils;

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

    #region Methods called by StreamerBotEvent Manager

    public void SuscriptionEvent(User user)
    {
        // We receive the user who already has the subscription, the name of the receiver, the months suscribed and the tier of the subscription

        Debug.Log(user.UserName + " decided to suscribe to me for " + user.subscription.SubscribedMonthCount + " months with tier " + user.subscription.Tier);
    }

    public void SuscriptionGiftEvent(User user, User userGifter)
    {
        // We receive the user who already has the subscription, the name of the receiver, the months suscribed and the tier of the subscription

        
        // We have to take into account if the gifter is anonymous. If it is, then we don't have a user to access
        //Debug.Log(subscription.Gifter + " decided to gift a suscription to " + receiverName + " for " + monthsSuscribed + " months with tier " + tier);
    }

    #endregion

    #region You don't need to touch this


    #endregion
}
