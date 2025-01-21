using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StreamerBotUDP;

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
        RegisterEvent("Raid", RaidEvent);
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
        FollowManager.instance.FollowEvent(eventData.UserName, eventData.UserProfileImage);

    }

    private void BitsEvent(StreamerBotEventData eventData)
    {
        Debug.Log(eventData.UserName + " sent " + eventData.Amount + " bits! Thank so much");
    }
    
    private void ChatMessageEvent(StreamerBotEventData eventData)
    {
        // We send the message to the chat manager
        ChatManager.instance.ReceiveChatMessage(eventData.UserName, eventData.UserProfileImage, eventData.UserProfileImage);
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
        SuscriptionManager.instance.SuscriptionEvent(eventData.UserName, eventData.UserProfileImage, eventData.Amount, eventData.Message);
    }

    private void SuscriptionGiftEvent(StreamerBotEventData eventData)
    {
        Debug.Log("Thanks for the sub gift " + eventData.UserName+ " for " + eventData.Message);
        //SuscriptionManager.instance.SuscriptionGiftEvent(eventData.UserName, eventData.UserProfileImage, eventData.Message, eventData.Message, eventData.Amount, eventData.Message);
    }

    private void ChannelRewardEvent(StreamerBotEventData eventData)
    {

        Debug.Log("Channel Reward: " + eventData.Message);

        // Be aware that the last argument must me the list of arguments of the reward (That right we do not have it yet)
        ChannelRewardManager.instance.RewardEvent(eventData.Message, eventData.UserName, eventData.UserProfileImage, new List<string>());

    }

    private void RaidEvent(StreamerBotEventData eventData)
    {
        Debug.Log("Channel Reward: " + eventData.Message);
    }




    #endregion
}