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
        RegisterEvent("AdsRunning", AdsRunningEvent);
        RegisterEvent("AdsIncoming", AdsIncomingEvent);

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
            // We add the user to the list and refresh the user
            TwitchManager.instance.addNewUser(eventData);
            TwitchManager.instance.getUser(eventData.UserName, ref user);
        }
        else
        {
            // Agarramos el tipo de suscripcion y el tiempo que lleva suscrito
            user.permissions = Permissions.Subscribers;
            user.subscription.SubscribedMonthCount = eventData.monthsSuscribed;
            user.subscription.selectTierINT(eventData.tier);
        }

        SuscriptionManager.instance.SuscriptionEvent(user);
    }

    private void SuscriptionGiftEvent(StreamerBotEventData eventData)
    {
        Debug.Log("Thanks for the sub gift " + eventData.UserName+ " for " + eventData.Message);
        //SuscriptionManager.instance.SuscriptionGiftEvent(eventData.UserName, eventData.UserProfileImage, eventData.Message, eventData.Message, eventData.Amount, eventData.Message);
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
        //ChannelRewardManager.instance.RewardEvent(eventData.Message, user, eventData.UserProfileImage, new List<string>());

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




    #endregion
}