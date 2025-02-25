using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StreamerBotUDP;
using Twitch_data;
using static Twitch_data.TwitchUtils;


public class StreamerBotEventManager : StreamerBotUDPReceiver
{
    // Singleton instance so every script can access the StreamerBotEventManager.
    public static StreamerBotEventManager instance;

    public UDPSend udpSend;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// Registers event names and corresponding actions to listen for from StreamerBot.
    /// The name of the event must exactly match the "Event" variable in the UDP Payload.
    /// </summary>
    protected override void InitialiseStreamerBotEvents()
    {
        RegisterEvent("TestConnection", TestConnection);
        RegisterEvent("Follow", FollowEvent);
        RegisterEvent("Bits", BitsEvent);
        RegisterEvent("ChatMessage", ChatMessageEvent);
        RegisterEvent("Suscription", SuscriptionEvent);
        RegisterEvent("SuscriptionGift", SuscriptionGiftEvent);
        RegisterEvent("ChannelReward", ChannelRewardEvent);
        RegisterEvent("ReceiveRaid", ReceiveRaidEvent);
        RegisterEvent("SendRaid", SendRaid);
        RegisterEvent("AdsRunning", AdsRunningEvent);
        RegisterEvent("AdsIncoming", AdsIncomingEvent);

        RegisterEvent("GetUser", GetUser);

    }

    #region Functions called by the registered events.

    private void TestConnection(StreamerBotEventData eventData)
    {
        // Aparentemente StreamerBot le llega el mensaje para ejecutar TestConnection pero no es capaz de enviar un mensaje de vuelta(?)
        Debug.Log("Hola buenas tardes");
    }

    private void FollowEvent(StreamerBotEventData eventData)
    {
        User user = new User();
        TwitchManager.instance.getUser(eventData.UserName, ref user);

        // The user already exists in the list
        if (user.active)
        {
            // Con esto evitamos que los suscriptores pierdan sus permisos al seguir el canal
            if(user.permissions < Permissions.Follower)
            {
                user.permissions = Permissions.Follower;
            }
            TwitchManager.instance.updateUser(user);
        }
        else
        {
            // We add the user to the list and refresh the user
            TwitchManager.instance.addNewUser(eventData);
            TwitchManager.instance.getUser(eventData.UserName, ref user);
        }

        FollowManager.instance.FollowEvent(user);
    }

    private void BitsEvent(StreamerBotEventData eventData)
    {
        User user = new User();
        TwitchManager.instance.getUser(eventData.UserName, ref user);

        // The user does not exists in the list
        if (!user.active)
        {
            // We add the user to the list and refresh the user
            TwitchManager.instance.addNewUser(eventData);
            TwitchManager.instance.getUser(eventData.UserName, ref user);
        }

        DonationManager.instance.ReceiveBitsEvent(user,eventData.Amount);

        Debug.Log(eventData.UserName + " sent " + eventData.Amount + " bits! Thank so much");
    }
    
    private void ChatMessageEvent(StreamerBotEventData eventData)
    {
        User user = new User();
        TwitchManager.instance.getUser(eventData.UserName, ref user);

        // If the user does not exists in the list
        if (!user.active)
        {
            // We add the user to the list and refresh the user
            TwitchManager.instance.addNewUser(eventData);
            TwitchManager.instance.getUser(eventData.UserName, ref user);
        }

        ChatManager.instance.ReceiveChatMessage(user, eventData.Message);
    }

    private void AdsRunningEvent(StreamerBotEventData eventData)
    {
        Debug.Log("Never gonna give you up, never gonna let you down... ");
    }

    private void AdsIncomingEvent(StreamerBotEventData eventData)
    {
        Debug.Log("Ads Incoming");
    }

    private void SuscriptionEvent(StreamerBotEventData eventData)
    {
        User user = new User();
        TwitchManager.instance.getUser(eventData.UserName, ref user);

        if (!user.active)
        {
            // Metemos al usuario a la lista y refrescamos el usuario
            // We add the user to the list and refresh the user
            TwitchManager.instance.addNewUser(eventData);
            TwitchManager.instance.getUser(eventData.UserName, ref user);
        }
        else
        {
            // Agarramos el tipo de suscripcion y el tiempo que lleva suscrito
            // We get the type of subscription and the time that has been subscribed
            user.permissions = Permissions.Subscribers;
            user.subscription.SubscribedMonthCount = eventData.monthsSuscribed;
            user.subscription.selectTierINT(eventData.tier);
        }

        SuscriptionManager.instance.SuscriptionEvent(user);
    }

    private void SuscriptionGiftEvent(StreamerBotEventData eventData)
    {
        //Debug.Log("Thanks for the sub gift " + eventData.UserName+ " for " + eventData.Message);
        User user = new User();
        TwitchManager.instance.getUser(eventData.UserName, ref user);

        User gifter = new User();

        // Si el usuario no es anonimo, podemos intentar obtener al gifter
        // If the user is not anonymous, we can try to get the gifter
        if (!eventData.isAnonymous)
        {
            TwitchManager.instance.getUser(eventData.UserName2, ref gifter);
        }
       

        if (!user.active)
        {
            // Metemos al usuario a la lista y refrescamos el usuario
            // We add the user to the list and refresh the user
            TwitchManager.instance.addNewUser(eventData);
            TwitchManager.instance.getUser(eventData.UserName, ref user);
        }
        else
        {
            // Agarramos el tipo de suscripcion y el tiempo que lleva suscrito
            // We get the type of subscription and the time that has been subscribed
            user.permissions = Permissions.Subscribers;
            user.subscription.SubscribedMonthCount = eventData.monthsSuscribed;
            user.subscription.selectTierINT(eventData.tier);
        }

        // Si el gifter no esta activo y no es anonimo
        // If the gifter is not active and is not anonymous
        if(!gifter.active && !eventData.isAnonymous)
        {
            // Metemos al usuario a la lista y refrescamos el usuario
            // We add the user to the list and refresh the user
            TwitchManager.instance.addDefaultUser(eventData.UserName2);
            TwitchManager.instance.getUser(eventData.UserName2, ref gifter);
        }

        SuscriptionManager.instance.SuscriptionGiftEvent(user, gifter);
    }

    private void ChannelRewardEvent(StreamerBotEventData eventData)
    {

        User user = new User();
        TwitchManager.instance.getUser(eventData.UserName, ref user);

        if (!user.active)
        {
            // We add the user to the list and refresh the user
            TwitchManager.instance.addNewUser(eventData);
            TwitchManager.instance.getUser(eventData.UserName, ref user);
        }
        
        // Be aware that the last argument must me the list of arguments of the reward (That right we do not have it yet)
        ChannelRewardManager.instance.RewardEvent(eventData.Message, user, new List<string>());

    }

    private void ReceiveRaidEvent(StreamerBotEventData eventData)
    {
        // We check if the user is already in the list
        User user = new User();
        TwitchManager.instance.getUser(eventData.UserName, ref user);

        if (!user.active)
        {
            // We add the user to the list and refresh the user
            TwitchManager.instance.addNewUser(eventData);
            TwitchManager.instance.getUser(eventData.UserName, ref user);
        }

        RaidManager.instance.ReceiveRaidEvent(user, eventData.Amount);

    }

    private void SendRaid(StreamerBotEventData eventData)
    {
        if(eventData.Message == "%streamDisplayNames%")
        {
            Debug.LogWarning("There are no active streamers to raid");
        }
        else
            RaidManager.instance.StartRaidEvent(eventData.Message);
    }
    #endregion

    #region Auxiliar functions
    
    // Si queremos acceder a la informacion de un usuario, dependiendo del manager que solicito la informacion se haran cosas diferentes
    // If we want to access to a user information, depending on the manager that requested the information it will do different things
    private void GetUser(StreamerBotEventData eventData)
    {
        User user = new User();

        user.UserName = eventData.UserName;
        user.profilePictureURL = eventData.UserProfileImage;
        user.active = true;

        if (eventData.isMod)
        {
            user.permissions = Permissions.Mods;
        }
        else if (eventData.isVip)
        {
            user.permissions = Permissions.VIPs;
        }
        else if (eventData.isSuscribed)
        {
            user.permissions = Permissions.Subscribers;
        }
        else
            user.permissions = Permissions.Everyone;
        
        switch (TwitchManager.instance.whoRequested)
        { 
            case TwitchManager.WhoRequested.FollowManager:
                break;
            case TwitchManager.WhoRequested.RaidManager:
                // This is just an example of how to show the streamer to raid
                ExampleManager.instance.showStreamerToRaid(user);
                break;
            default:
                Debug.Log("Who requested is not defined");
                break;
        }

    }
    #endregion
}