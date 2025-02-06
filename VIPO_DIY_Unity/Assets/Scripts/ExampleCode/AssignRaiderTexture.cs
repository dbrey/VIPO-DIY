using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssignRaiderTexture : MonoBehaviour
{
    [SerializeField] Image[] raiderImages;
    [SerializeField] TextMeshProUGUI raiderText;

    public void assignRaider(Texture2D raiderProfile, string raiderName)
    { 
        foreach (Image image in raiderImages)
        {
            image.sprite = Sprite.Create(raiderProfile, new Rect(0, 0, raiderProfile.width, raiderProfile.height), new Vector2(0.5f, 0.5f));
        }

        raiderText.text = raiderName;
    }
}
