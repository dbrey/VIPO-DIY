using StreamerBotUDP;
using UnityEngine;
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
        
    }


    public void ReceiveRaidEvent(User raidUser, int NViewers)
    {
        ExampleManager.instance.RaidExample(raidUser, NViewers);
        Debug.Log("Raid received from " + raidUser.UserName + " with : " + NViewers);
    }

    public void SendRaidEvent()
    { 
        
    }
}
