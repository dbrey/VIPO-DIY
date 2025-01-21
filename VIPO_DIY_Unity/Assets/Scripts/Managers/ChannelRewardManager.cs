using System.Collections.Generic;
using UnityEngine;

public class ChannelRewardManager : MonoBehaviour
{
    public static ChannelRewardManager instance;

    struct ManagedReward
    {
        public string name;
        public int cost;
        public float coolDown;
        public bool enabled;
        public int permissions; // 0 = everyone, 1 = subscriber, 2 = VIP, 3 = moderator, 4 = broadcaster

        public void ExecuteCommand(string username, string userProfilePicture, List<string> rewardArguments)
        {
            // We will override this method later
        }
    }
    
    // Later we need to be able to change the cost, cooldown and permissions of the rewards in the Unity Editor
    [SerializeField] List<string> rewardsToAdd = new List<string>();
    Dictionary<string, ManagedReward> managedRewards = new Dictionary<string, ManagedReward>();

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

    // Update is called once per frame
    void Update()
    {
        // We need to check if the cooldown is over
        foreach (KeyValuePair<string, ManagedReward> entry in managedRewards)
        {
            if (managedRewards[entry.Key].coolDown > 0)
            {
                // If the cooldown is not over, we decrease the timer
                ManagedReward auxReward= managedRewards[entry.Key];
                auxReward.coolDown -= Time.deltaTime;

                // If after decreasing the timer, the timer is less than or equal to 0, we enable the reward
                if (auxReward.coolDown <= 0)
                    auxReward.enabled = true;

                managedRewards[entry.Key] = auxReward;
            }
        }
    }

    #region Methods called by StreamerBotEvent Manager
    
    public void RewardEvent(string rewardName, string username, string userProfilePicture, List<string> rewardArguments)
    {
        // We receive the name, profile picture, months suscribed and tier of the subscriber
        Debug.Log(username + " decided to redeem the reward " + rewardName);
    }

    #endregion

    #region Reward Management

    void AddReward(string rewardName)
    {
        // If the command doesn't exist, we add it
        if (!managedRewards.ContainsKey(rewardName))
        {
            // We set the managed command values to default
            ManagedReward newReward = new ManagedReward();
            newReward.name = rewardName;
            newReward.enabled = true;
            newReward.coolDown = 0; // No cooldown
            newReward.permissions = 0; // Everybody can use the command
            managedRewards.Add(rewardName, newReward);
        }
    }

    void updateCostReward(string rewardToUpdate ,int newCost)
    {
        if (managedRewards.ContainsKey(rewardToUpdate))
        {
            ManagedReward auxReward = managedRewards[rewardToUpdate];
            auxReward.cost = newCost;
            managedRewards[rewardToUpdate] = auxReward;

            // Here we have to tell StreamerBot to update the reward cost in Twitch as well
            // For now, this only changes the cost of one specific reward
            // If we want to specify which reward we need to define some "Selection" type in the doAction method
            StreamerBotEventManager.instance.udpSend.doAction("UpdateChannelReward", "", auxReward.cost);
        }
        else
        { 
            Debug.Log("The reward " + rewardToUpdate + " does not exist");
        }
    }

    #endregion

    #region Program here the methods for the rewards

    #endregion

}
