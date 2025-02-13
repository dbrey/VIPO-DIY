using StreamerBotUDP;
using System.Collections.Generic;
using Twitch_data;
using Unity.VisualScripting;
using UnityEngine;
using static Twitch_data.TwitchUtils;

public class TwitchManager : MonoBehaviour
{
    public static TwitchManager instance;

    UDPSend udpSender;
    public UDPSend getUDPSender() { return udpSender; }

    Dictionary<string, User> userList;

    public enum WhoRequested
    {
        FollowManager,
        RaidManager,
        SubscriptionManager,
        ChatManager,
        DonationManager,
        None
    }
    public WhoRequested whoRequested;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        userList = new Dictionary<string, User>();
        udpSender = transform.parent.GetComponent<UDPSend>();
        whoRequested = WhoRequested.None;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region User Management
    // Interrogation is to return a null value if the user is not found
    public void getUser(string username, ref User user)
    {
        if (userList.ContainsKey(username))
            user = userList[username];
    }

    public void updateUser(User user)
    {
        userList[user.UserName] = user;
    }

    public void addNewUser(StreamerBotEventData eventData)
    {
        User user = new User();
        user.active = true;
        user.UserName = eventData.UserName;
        user.profilePictureURL = eventData.UserProfileImage;
        user.subscription = new Subscription();

        // Independientemente del status del usuario, si esta suscrito, le asignamos el tier y el tiempo que lleva suscrito
        if (eventData.isSuscribed)
        {
            // Agarramos el tipo de suscripcion y el tiempo que lleva suscrito
            user.permissions = Permissions.Subscribers;
            user.subscription.SubscribedMonthCount = eventData.monthsSuscribed;
            user.subscription.selectTierINT(eventData.tier);
        }
        else
            user.subscription.Tier = SubscriptionTier.NotSet;

        // Miramos el status del usuario para asignarle los permisos correspondientes
        if (eventData.isMod)
            user.permissions = Permissions.Mods;
        else if (eventData.isVip)
            user.permissions = Permissions.VIPs;
        else if (eventData.followAgeDays > 0 && !eventData.isSuscribed) // Si el usuario no esta suscrito pero sigue al canal
            user.permissions = Permissions.Follower;
        else if (!eventData.isSuscribed) // Si el usuario no esta suscrito ni sigue al canal
            user.permissions = Permissions.Everyone;

        userList.Add(user.UserName, user);
    }
    #endregion

    void ShoutOutEvent(string streamerName)
    { 
        udpSender.doAction("ShoutOut", "",streamerName, 0);
    }


}
