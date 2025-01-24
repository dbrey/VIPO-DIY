using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AssignFollower : MonoBehaviour
{
    [SerializeField] Image followerImage;
    [SerializeField] TextMeshProUGUI textFollower;

    public void AssignFollowerData(string followerName, string followerSprite)
    {

        var textureRequest = UnityWebRequestTexture.GetTexture(followerSprite);
        var asyncOp = textureRequest.SendWebRequest();
        asyncOp.completed += (op) => {
            var texture = DownloadHandlerTexture.GetContent(textureRequest);

            textFollower.text = followerName;
            followerImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        };

    }
}
