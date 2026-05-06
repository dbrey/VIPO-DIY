using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StreamerBotUDP;
using Twitch_data;
using static Twitch_data.TwitchUtils;

#region Como usar
/// 
/// Todos los eventos que queramos recibir de Streamerbot, debemos registrarlos en el metodo InitialiseStreamerBotEvents.
/// Para registrar un evento, debemos llamar a RegisterEvent con el nombre del evento y la funcion que queremos que se ejecute cuando se reciba el evento.
/// La funcion asignada, debe tener un parametro de tipo StreamerBotEventData. Este parametro contiene toda la informacion que Streamerbot nos envia.
/// 
/// Hay un ejemplo de como registrar un evento, llamado TestConnection
/// 
#endregion

#region How to use
/// 
/// All the events that we want to receive from Streamerbot, we must register them in the method InitialiseStreamerBotEvents.
/// To register an event, we must call RegisterEvent with the name of the event and the function that we want to execute when the event is received.
/// The asigned function, must have a parameter of type StreamerBotEventData. This parameter contains all the information that Streamerbot sends us.
/// 
/// There is an example of how to register an event, called TestConnection
///
#endregion

public class StreamerBotEventManager : StreamerBotUDPReceiver
{
    public static StreamerBotEventManager instance;

    private void Awake()
    {
        // Si no hay ninguna instancia, establecemos esta como la instancia
        // If there's no instance, we set this as the instance
        if (instance == null)
        {
            instance = this;
        }
        // Si ya hay una instancia, destruimos esta
        // If there's already an instance, we destroy this one
        else
        {
            Destroy(gameObject);
        }
    }

    /// Registramos los nombres de los eventos y sus acciones correspondientes para escuchar desde StreamerBot
    /// El nombre del evento debe coincidir exactamente con la variable "Event" en el UDP Payload en Streamerbot
    /// Registers event names and corresponding actions to listen for from StreamerBot.
    /// The name of the event must exactly match the "Event" variable in the UDP Payload in Streamerbot.
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

        RegisterEvent("GetUser", GetUser);

    }

    #region Functions called by the registered events.

    private void TestConnection(StreamerBotEventData eventData)
    {
        Debug.Log("Event Received");
    }

    private void FollowEvent(StreamerBotEventData eventData)
    {
        User user = new User();

        if (TwitchManager.instance != null)
        {
            TwitchManager.instance.getUser(eventData.UserName, ref user);

            // El usuario ya existe en la lista
            // The user already exists in the list
            if (user.active)
            {
                // Con esto evitamos que los suscriptores pierdan sus permisos al seguir el canal
                // With this we avoid that subscribers lose their permissions when following the channel
                if (user.permissions < Permissions.Follower)
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
        else
        {
            Debug.LogError("There's no TwitchManager!");
        }
    }

    private void BitsEvent(StreamerBotEventData eventData)
    {
        User user = new User();

        if (TwitchManager.instance != null)
        {
            TwitchManager.instance.getUser(eventData.UserName, ref user);

            // El usuario no existe en la lista
            // The user does not exists in the list
            if (!user.active)
            {
                // Metemos al usuario a la lista y actualizamos el usuario
                // We add the user to the list and refresh the user
                TwitchManager.instance.addNewUser(eventData);
                TwitchManager.instance.getUser(eventData.UserName, ref user);
            }

            DonationManager.instance.ReceiveBitsEvent(user, eventData.Amount);

            Debug.Log(eventData.UserName + " sent " + eventData.Amount + " bits! Thank so much");
        }
        else
        { 
            Debug.LogError("There's no TwitchManager!");
        }
        
    }
    
    private void ChatMessageEvent(StreamerBotEventData eventData)
    {
        User user = new User();

        if (TwitchManager.instance != null)
        {
            TwitchManager.instance.getUser(eventData.UserName, ref user);

            // El usuario no existe en la lista
            // If the user does not exists in the list
            if (!user.active)
            {
                // Metemos al usuario a la lista y actualizamos el usuario
                // We add the user to the list and refresh the user
                TwitchManager.instance.addNewUser(eventData);
                TwitchManager.instance.getUser(eventData.UserName, ref user);
            }

            ChatManager.instance.ReceiveChatMessage(user, eventData.Message);
            Debug.Log("Usuario: " + eventData.UserName + "\n" + "Mensaje: " + eventData.Message);
        }
        else
        {
            Debug.LogError("There's no TwitchManager!");
        }
    }

    private void SuscriptionEvent(StreamerBotEventData eventData)
    {
        User user = new User();

        if (TwitchManager.instance != null)
        {
            TwitchManager.instance.getUser(eventData.UserName, ref user);

            // El usuario no existe en la lista
            // The user does not exists in the list
            if (!user.active)
            {
                // Metemos al usuario a la lista y actualizamos el usuario
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
                TwitchManager.instance.updateUser(user);
            }

            SubscriptionManager.instance.SubscriptionEvent(user);
        }
        else
        { 
            Debug.LogError("There's no TwitchManager!");
        }
    }

    private void SuscriptionGiftEvent(StreamerBotEventData eventData)
    {
        User user = new User();
        if (TwitchManager.instance != null)
        {
            TwitchManager.instance.getUser(eventData.UserName, ref user);

            User gifter = new User();

            // Si el usuario no es anonimo, podemos intentar obtener al gifter
            // If the user is not anonymous, we can try to get the gifter
            if (!eventData.isAnonymous)
            {
                TwitchManager.instance.getUser(eventData.UserName2, ref gifter);
            }

            // El usuario no existe en la lista
            // The user does not exists in the list
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
                TwitchManager.instance.updateUser(user);
            }

            // Si el gifter no esta activo y no es anonimo
            // If the gifter is not active and is not anonymous
            if (!gifter.active && !eventData.isAnonymous)
            {
                // Metemos al usuario a la lista y refrescamos el usuario
                // We add the user to the list and refresh the user
                TwitchManager.instance.createDefaultUser(eventData.UserName2, ref gifter);
            }

            SubscriptionManager.instance.SubscriptionGiftEvent(user, gifter);
        }
        else
        {
            Debug.LogError("There's no TwitchManager!");
        }

        
    }

    private void ChannelRewardEvent(StreamerBotEventData eventData)
    {

        User user = new User();

        if (TwitchManager.instance != null)
        {
            TwitchManager.instance.getUser(eventData.UserName, ref user);

            // El usuario no existe en la lista
            // The user does not exists in the list
            if (!user.active)
            {
                // Metemos al usuario a la lista y actualizamos el usuario
                // We add the user to the list and refresh the user
                TwitchManager.instance.addNewUser(eventData);
                TwitchManager.instance.getUser(eventData.UserName, ref user);
            }

            // Tener en cuenta que el ultimo argumento debe ser la lista de argumentos de la recompensa (Todavia no esta disponible)
            // Be aware that the last argument must me the list of arguments of the reward (Still not available)
            ChannelRewardManager.instance.RewardEvent(eventData.Message, user, new List<string>());
        }
        else
        {
            Debug.LogError("There's no TwitchManager!");
        }
    }

    private void ReceiveRaidEvent(StreamerBotEventData eventData)
    {
        // Comprobamos si el usuario ya esta en la lista
        // We check if the user is already in the list
        User user = new User();

        if (TwitchManager.instance != null)
        {
            TwitchManager.instance.getUser(eventData.UserName, ref user);

            // El usuario no existe en la lista
            // The user does not exists in the list
            if (!user.active)
            {
                // Metemos al usuario a la lista y actualizamos el usuario
                // We add the user to the list and refresh the user
                TwitchManager.instance.addNewUser(eventData);
                TwitchManager.instance.getUser(eventData.UserName, ref user);
            }

            RaidManager.instance.ReceiveRaidEvent(user, eventData.Amount);
        }
        else
        {
            Debug.LogError("There's no TwitchManager!");
        }
    }

    private void SendRaid(StreamerBotEventData eventData)
    {
        // En caso de no recibir streamers activos o no haya mensaje, mostraremos un aviso
        // In case we receive no active streamers or there is no message, we will show a warning
        if(eventData.Message == "%streamDisplayNames%" || eventData.Message == "")
        {
            Debug.LogWarning("There are no active streamers to raid. If there is, check the SendRaid UDP Broadcast or StreamerBot Group (in Settings)");
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

        if (TwitchManager.instance != null)
        {
            user.UserName = eventData.UserName;
            user.profilePictureURL = eventData.UserProfileImage;

            // Asignar suscripcion usuario
            // Asignar dias Seguidos de Follow

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

            // Dependiendo de quien solicito la informacion, se haran cosas diferentes
            // Depending on who requested the information, it will do different things
            switch (TwitchManager.instance.whoRequested)
            {
                case TwitchManager.WhoRequested.FollowManager:
                    break;
                case TwitchManager.WhoRequested.SubscriptionManager:
                    // Llamamos al evento de suscripcion de gifter pero con la informacion del gifter
                    // We call the subscription event of gifter but with the information of the gifter
                    SubscriptionManager.instance.SubscriptionGiftEventWithGifterInfo(user);
                    break;
                case TwitchManager.WhoRequested.RaidManager:
                    // Este es solo un ejemplo que muestra el streamer al que hacer raid (Eliminar estas lineas es seguro)
                    // This is just an example that show the streamer to raid (Deleting these lines is safe)
                    if (ExampleManager.instance != null)
                        ExampleManager.instance.showStreamerToRaid(user);
                    break;
                default:
                    Debug.Log("Who requested is not defined");
                    break;
            }
        }
        else
        {
            Debug.LogError("There's no TwitchManager!");
        }
    }
    #endregion
}