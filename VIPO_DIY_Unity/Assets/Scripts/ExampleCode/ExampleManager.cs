using UnityEngine;
using Twitch_data;

public class ExampleManager : MonoBehaviour
{
    public static ExampleManager instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] AssignFollower assignFollower;

    public void FollowExample(TwitchUtils.User user)
    {
        assignFollower.AssignFollowerData(user.UserName, user.profilePictureURL);
    }

}
