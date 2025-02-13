using StreamerBotUDP;
using UnityEngine;
using UnityEngine.Networking;
using static Twitch_data.TwitchUtils;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SendRaidEventRequest();
        }
    }


    public void ReceiveRaidEvent(User raidUser, int NViewers)
    {
        ExampleManager.instance.RaidExample(raidUser, NViewers);
        Debug.Log("Raid received from " + raidUser.UserName + " with : " + NViewers);
    }

    public void StartRaidEvent(string streamerList)
    {
        // Separamos el string en un array de strings separados por comas
        // We split the string into an array of strings separated by commas
        string[] streamers = streamerList.Split(',');

        // Recibes una lista de streamers ACTIVOS a los que hacer raid en una lista. Debes seleccionar uno de ellos
        // You receive a list of ACTIVE streamers to raid in a list. You must select one of them


        // After selecting one name of a streamer from the streamers list, you can raid inmediatly that streamer by simply requesting it
        // TwitchManager.instance.getUDPSender().doAction("Start Raid", "", name of the streamer, 0);


        // In case you want to retrieve information about the streamer you can request it by using the following code and make sure that it was the RaidManager who requested that action
        // TwitchManager.instance.getUDPSender().doAction("Request User Info", "StreamerBot", streamerName, 0);
        // TwitchManager.instance.whoRequested = TwitchManager.WhoRequested.RaidManager;


        // En este ejemplo seleccionamos un streamer aleatorio, retrieve information about the streamer and then later start the raid
        // In this example we select a random streamer
        ExampleManager.instance.selectRandomActiveStreamer(streamers);

    }

    public void SendRaidEventRequest()
    {
        TwitchManager.instance.getUDPSender().doAction("Request Send Raid","", "", 0);
    }


}
