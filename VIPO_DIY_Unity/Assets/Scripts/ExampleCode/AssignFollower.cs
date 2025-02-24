using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AssignFollower : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI text;

    public void AssignData(string Name, string sprite)
    {

        var textureRequest = UnityWebRequestTexture.GetTexture(sprite);
        var asyncOp = textureRequest.SendWebRequest();
        asyncOp.completed += (op) => {
            var texture = DownloadHandlerTexture.GetContent(textureRequest);

            text.text = Name;
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        };

    }
}
