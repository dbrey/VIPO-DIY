using UnityEngine;
using Twitch_data;
using TMPro;
using static Twitch_data.TwitchUtils;
using System.Collections;
using UnityEngine.Networking;

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

    [Header("Raid event Example")]
    [SerializeField] Transform raidSpawner;
    [SerializeField] GameObject raidCube;


    private void Start()
    {
        chatText.text = "";
    }

    public void FollowExample(User user)
    {
        assignFollower.AssignFollowerData(user.UserName, user.profilePictureURL);
    }

    public void AddChatMessage(User user, string message)
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

    public void RaidExample(User user, int nViewers)
    {
        var textureRequest = UnityWebRequestTexture.GetTexture(user.profilePictureURL);
        var asyncOp = textureRequest.SendWebRequest();
        asyncOp.completed += (op) => {
            var texture = DownloadHandlerTexture.GetContent(textureRequest);

            StartCoroutine(spawnRaidCube(texture, user.UserName, nViewers));
        };
    }

    IEnumerator spawnRaidCube(Texture2D userProfile, string userName, int nViewers)
    {
        for (int i = 0; i < nViewers; i++)
        {
            Vector3 spawnPosition = raidSpawner.position + new Vector3(
            Random.Range(-raidSpawner.localScale.x/2, raidSpawner.localScale.x/2
            ), 0,
            Random.Range(-raidSpawner.localScale.z / 2, raidSpawner.localScale.z / 2));

            GameObject raidCubeAux = Instantiate(raidCube, spawnPosition, Quaternion.identity);
            raidCubeAux.GetComponent<AssignRaiderTexture>().assignRaider(userProfile, userName);

            yield return new WaitForSeconds(0.15f);
        }
    }

}
