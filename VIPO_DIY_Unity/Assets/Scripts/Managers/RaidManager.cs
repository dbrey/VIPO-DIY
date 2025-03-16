using StreamerBotUDP;
using UnityEngine;
using UnityEngine.Networking;
using static Twitch_data.TwitchUtils;

#region Como usar
///
/// RaidManager tiene 2 funciones principales: ReceiveRaidEvent y StartRaidEvent
/// ReceiveRaidEvent recibe el usuario que realiza el raid y el numero de espectadores que ha traido
/// 
/// Antes de StartRaidEvent, se debe mandar un request para recibir una lista de streamers activos a los que hacer raid mediante el metodo publico SendRaidEventRequest
/// Luego, StartRaidEvent recibe una lista de streamers activos y lo guardamos en un array de strings. Selecciona el streamer que quieras.
/// Puedes inmediatamente hacer el raid con el nombre del streamer seleccionado mediante llamando a TwitchManager para que mande un request "Start Raid"
/// Tambien se da herramientas para recibir informacion de un streamer mediante el request "Request User Info"
/// Se da un ejemplo funcional de como puedes usar estas funciones
/// 
/// ADVERTENCIA: Para mandar requests al TwitchManager, debes haber iniciado el servidor UDP en StreamerBot
/// 
#endregion

#region How to use
/// 
/// RaidManager has 2 main functions: ReceiveRaidEvent and StartRaidEvent
/// ReceiveRaidEvent receives the user that makes the raid and the number of viewers it has brought
/// 
/// Before StartRaidEvent, you must send a request to receive a list of active streamers using the public method SendRaidEventRequest
/// Then, StartRaidEvent receives a list of active streamers and we save it in an array of strings. Select the streamer you want.
/// You can immediately start the raid with the name of the selected streamer by calling TwitchManager to send a "Start Raid" request
/// You also have tools to receive information about a streamer by sending a "Request User Info" request
/// You have a functional example of how you can use these functions
///
/// WARNING: In order to send requests to the TwitchManager, you must have started the UDP server in StreamerBot
///  
#endregion

public class RaidManager : MonoBehaviour
{
    public static RaidManager instance;

    private void Awake()
    {
        // Si no hay ninguna instancia, establecemos esta como la instancia
        // If there's no instance, we set this as the instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // Si ya hay una instancia, destruimos esta
        // If there's already an instance, we destroy this one
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReceiveRaidEvent(User raidUser, int NViewers)
    {
        // Puedes borrar esta lineas y es completamente seguro! Simplemente desconecta la accion del evento
        // You can delete this lines and it's completely safe! It simply disconnects the action from the event
        if (ExampleManager.instance != null)
            ExampleManager.instance.RaidExample(raidUser, NViewers);

        Debug.Log("Raid received from " + raidUser.UserName + " with : " + NViewers);
    }

    public void StartRaidEvent(string streamerList)
    {
        #region Documentation in Spanish
        ///
        /// Recibes una lista de streamers ACTIVOS a los que hacer raid en una lista. Debes seleccionar uno de ellos
        /// Despues de seleccionar un nombre de un streamer de la lista de streamers, puedes iniciar la raid inmediatamente solicitandolo
        ///
        //  TwitchManager.instance.getUDPSender().doAction("Start Raid", "", nombre del streamer, 0);
        ///
        /// En caso de que quieras conseguir informacion sobre el streamer puedes solicitarlo usando el siguiente codigo y asegurarte de que fue el RaidManager quien solicito esa accion
        ///
        //  TwitchManager.instance.getUDPSender().doAction("Request User Info", "StreamerBot", streamerName, 0);
        //  TwitchManager.instance.whoRequested = TwitchManager.WhoRequested.RaidManager;
        ///
        #endregion

        #region Documentation in English
        ///
        /// You receive a list of ACTIVE streamers to raid in a list. You must select one of them
        /// After selecting one name of a streamer from the streamers list, you can raid inmediatly that streamer by simply requesting it
        ///
        //  TwitchManager.instance.getUDPSender().doAction("Start Raid", "", name of the streamer, 0); 
        /// 
        /// In case you want to retrieve information about the streamer you can request it by using the following code and make sure that it was the RaidManager who requested that action
        //  TwitchManager.instance.getUDPSender().doAction("Request User Info", "StreamerBot", streamerName, 0);
        //  TwitchManager.instance.whoRequested = TwitchManager.WhoRequested.RaidManager;
        ///
        #endregion

        // Separamos el string en un array de strings separados por comas
        // We split the string into an array of strings separated by commas
        string[] streamers = streamerList.Split(',');

        // En este ejemplo seleccionamos un streamer aleatorio, recogemos informacion sobre el streamer y luego iniciamos la raid
        // In this example we select a random streamer, retrieve information about the streamer and then later start the raid

        // Puedes borrar esta lineas y es completamente seguro! Simplemente desconecta la accion del evento
        // You can delete this lines and it's completely safe! It simply disconnects the action from the event
        if (ExampleManager.instance != null)
            ExampleManager.instance.selectRandomActiveStreamer(streamers);

    }

    public void SendRaidEventRequest()
    {
        TwitchManager.instance.getUDPSender().doAction("Request Send Raid","", "", 0);
    }
}
