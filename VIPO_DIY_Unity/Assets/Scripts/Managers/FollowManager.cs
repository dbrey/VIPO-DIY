using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowManager : MonoBehaviour
{
    public static FollowManager instance;

    // We only want to store the latest n followers.
    [SerializeField] bool storeLatestFollowers = true;
    [SerializeField] int nLatestFollowers = 5;
    private List<string> latestFollowers = new List<string>();

    #region Get Variables
    
    // If someone wants to get the latest followers, they can call this function.
    public List<string> GetLatestFollowers()
    {
        return latestFollowers;
    }
    #endregion

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

    private void Start()
    {
        latestFollowers = new List<string>();
    }

    public void FollowEvent(string username, string profilePictureURL)
    {
        Debug.Log(username + " decided to follow me for some reason");

        // We store the latest follower
        if(storeLatestFollowers)
        {
            // If the list is full, we remove the oldest follower
            if(latestFollowers.Count >= nLatestFollowers)
            {
                latestFollowers.RemoveAt(0);
            }
        }
    }
}
