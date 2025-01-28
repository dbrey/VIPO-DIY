using Twitch_data;
using UnityEngine;

public class DonationManager : MonoBehaviour
{
    public static DonationManager instance;

    void Awake()
    {
        instance = this;
    }

    /// Recibimos una cantidad de bits y escribimos la cantidad que recibimos. Tenemos acceso al usuario que envió los bits.
    /// We receive an amount of bits and we write the amount that we received. We have access to the user that sent the bits.
    public void ReceiveBitsEvent(TwitchUtils.User user, int bits)
    { 
        ExampleManager.instance.BitsDonationExample(bits);
        Debug.Log("Received " + bits + " bits");
    }

    public void ReceiveDonationEvent(TwitchUtils.User user, float amount)
    {
        // Existen varias plataformas conectadas a Streamerbot como Patreon, Ko-fi, Shopify... Se puede crear un evento por cada tipo pero
        // lo suyo seria hacer un evento generico que reciba una cantidad de dinero (y quizas el nombre de la plataforma)

        Debug.Log("Received " + amount + " dollars");
    }
}
