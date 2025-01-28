using UnityEngine;
using Twitch_data;
using TMPro;

public class ExampleManager : MonoBehaviour
{
    public static ExampleManager instance;

    private void Awake()
    {
        instance = this;
    }

    [Header("Follower event example")]
    [SerializeField] AssignFollower assignFollower;

    [Header("Chat Messages event Example")]
    [SerializeField] TextMeshProUGUI chatText;
    [SerializeField] int chatMessageLimit = 5;

    [Header("Donation event Example")]
    [SerializeField] TextMeshProUGUI bitsText;


    private void Start()
    {
        chatText.text = "";
    }

    public void FollowExample(TwitchUtils.User user)
    {
        assignFollower.AssignFollowerData(user.UserName, user.profilePictureURL);
    }

    public void AddChatMessage(TwitchUtils.User user, string message)
    {
        // Si el texto del chat tiene mas mensajes que el limite, eliminamos el primero
        // If the chat text has more than the limit of messages, we remove the first one
        if (chatText.text.Split('\n').Length > chatMessageLimit)
        {
            // We delete the last message
            chatText.text = chatText.text.Substring(chatText.text.IndexOf('\n') + 1);
        }

        // Añadimos el nuevo mensaje
        // We add the new message
        chatText.text += user.UserName + " : " + message + "\n";
    }

    public void BitsDonationExample(int bitsUsed)
    {
        bitsText.text = "Bits received : " + bitsUsed;
    }

}
