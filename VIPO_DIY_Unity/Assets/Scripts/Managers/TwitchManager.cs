using StreamerBotUDP;
using System.Collections.Generic;
using Twitch_data;
using Unity.VisualScripting;
using UnityEngine;
using static Twitch_data.TwitchUtils;

#region Como usar
///
/// Twitch Manager tiene la funcion de guardar la informacion de los usuarios que interactuan con el canal con los eventos de StreamerBot
/// Esta informacion se guarda mientras la aplicacion este activa y desechamos toda la información al cerrar la aplicacion
/// Ademas, Twitch Manager provee acceso a la lista de usuarios que han interactuado con el canal
/// Si el usuario quiere guardar esta informacion de forma permanente, debera hacerlo manualmente por su cuenta
/// 
/// Twitch Manager tambien tiene la funcion de mandar request a Streamerbot para acciones secundarias como shoutout, encuestas, predicciones...
/// Twitch Manager es el que se encarga de dar al resto de managers la capacidad de mandar requests a StreamerBot
/// Cada manager tiene la responsabilidad de establecer quien ha mandado el request correspondiente para realizar la accion adecuada
/// 
#endregion

#region How to use
/// 
/// Twitch Maanger has the function of saving the information of the users that interact with the channel with the StreamerBot events
/// This information is saved while the application is active and we discard all the information when we close the application
/// Also, Twitch Manager provides access to the list of users that have interacted with the channel
/// If the user wants to save this information permanently, they will have to do it manually on their own
/// 
/// Twitch Manager also has the function of sending requests to Streamerbot for secondary actions such as shoutout, polls, predictions...
/// Twitch Manager is the one in charge of giving the rest of the managers the ability to send requests to StreamerBot
/// Every manager has the responsibility of establishing who has sent the corresponding request to perform the appropriate action
///  
#endregion

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
    
    void Start()
    {
        userList = new Dictionary<string, User>();
        udpSender = transform.parent.GetComponent<UDPSend>();
        whoRequested = WhoRequested.None;
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

    // This method should only be used when the only information we have is the username
    public void addDefaultUser(string username)
    {
        // Apart from the userName, all the other information is set to default
        User user = new User();
        user.active = true;
        user.UserName = username;
        user.profilePictureURL = "";
        user.subscription = new Subscription();
        user.subscription.Tier = SubscriptionTier.NotSet;
        user.permissions = Permissions.Everyone;

        userList.Add(user.UserName, user);
    }
    #endregion

    // Mandamos un request a StreamerBot para realizar la accion "ShoutOut"
    // We request StreamerBot to start the action "ShoutOut"
    void ShoutOutEvent(string streamerName)
    { 
        udpSender.doAction("ShoutOut", "",streamerName, 0);
    }


}
