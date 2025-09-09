using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssignRaiderTexture : MonoBehaviour
{
    [SerializeField] Image[] raiderImages;
    [SerializeField] TextMeshProUGUI raiderText;

    public void assignRaider(Texture2D raiderProfile, string raiderName)
    { 
        // Recibimos la textura del raider, y por cada lado del cubo, le asignamos el sprite al componente de imagen
        // Tambien asignamos el nombre del raider al componente de texto

        // We receive the texture of the raider, and for each side of the cube, we assign the sprite to the image component.
        // We also assign the raider's name to the text component

        foreach (Image image in raiderImages)
        {
            image.sprite = Sprite.Create(raiderProfile, new Rect(0, 0, raiderProfile.width, raiderProfile.height), new Vector2(0.5f, 0.5f));
        }

        raiderText.text = raiderName;
    }
}
