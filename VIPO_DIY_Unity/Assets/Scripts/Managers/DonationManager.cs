using Twitch_data;
using UnityEngine;

#region Como usar
/// 
/// Por el momento, DonationManager solo tiene la funcion de recibir una cantidad de bits y el usuario que los mando.
/// ReceiveDonationEvent no tiene ninguna funcionalidad por el momento
/// 
#endregion

#region How to use
///
/// At the moment, DonationManager only has the function of receiving an amount of bits and the user that sent them.
/// ReceiveDonationEvent has no functionality at the moment
///
#endregion

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

    // Sin funcionalidad por el momento
    // No functionality yet
    public void ReceiveDonationEvent(TwitchUtils.User user, float amount)
    {
        // Existen varias plataformas conectadas a Streamerbot como Patreon, Ko-fi, Shopify... Se puede crear un evento por cada tipo pero
        // lo suyo seria hacer un evento generico que reciba una cantidad de dinero (y quizas el nombre de la plataforma)

        // There are several platforms connected to Streamerbot like Patreon, Ko-fi, Shopify... You can create an event for each type but
        // it would be better to create a generic event that receives an amount of money (and maybe the name of the platform)

        Debug.Log("Received " + amount + " dollars");
    }
}
