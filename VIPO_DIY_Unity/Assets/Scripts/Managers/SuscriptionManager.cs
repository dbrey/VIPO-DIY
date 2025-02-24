using UnityEngine;
using Twitch_data;
using System.Collections.Generic;
using static Twitch_data.TwitchUtils;

#region Como usar
///
/// Subscription Manager tiene 2 funciones principales: SuscriptionEvent y SuscriptionGiftEvent
/// Ambos ya recibe el usuario con la suscripcion, el nombre del receptor, los meses suscritos y el tier de la suscripcion
/// 
/// En el caso de SuscriptionGiftEvent, tambien recibe el usuario que ha regalado la suscripcion.
/// 
/// ADVERTENCIA 1: Es posible que SuscriptionGiftEvent reciba un usuario anonimo, por lo que no tendremos acceso a el
/// ADVERTENCIA 2: Si el gifter no es anonimo pero no estaba previamente en la lista de usuarios, solo tendremos acceso a su nombre
/// 
#endregion

#region How to use
/// 
/// Subscription Manager has 2 main functions: SuscriptionEvent and SuscriptionGiftEvent
/// Both of them receive the user with the subscription, the name of the receiver, the months suscribed and the tier of the subscription
/// 
/// In the case of SuscriptionGiftEvent, it also receives the user who has gifted the subscription.
/// 
/// WARNING 1: It's possible that SuscriptionGiftEvent receives an anonymous user, so we won't have access to it
/// WARNING 2: If the gifter is not anonymous but it wasn't previously in the user list, we will only have access to its name
///  
#endregion

public class SuscriptionManager : MonoBehaviour
{
    public static SuscriptionManager instance;

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

    #region Methods called by StreamerBotEvent Manager

    public void SuscriptionEvent(User user)
    {
        // Recibimos al usuario que ya tiene la suscripcion, el nombre del receptor, los meses suscritos y el tier de la suscripcion
        // We receive the user who already has the subscription, the name of the receiver, the months suscribed and the tier of the subscription

        // En este caso, solo mostramos el nombre del usuario y su foto de perfil
        // In this case, we only show the name of the user and its profile picture
        ExampleManager.instance.SubscriptionExample(user);
    }

    public void SuscriptionGiftEvent(User user, User userGifter)
    {
        // Recibimos al usuario que ya tiene la suscripcion, el nombre del receptor, los meses suscritos y el tier de la suscripcion
        // We receive the user who already has the subscription, the name of the receiver, the months suscribed and the tier of the subscription

        // Hay que tener en cuenta si el gifter es anonimo. Si lo es, entonces no tenemos un usuario al que acceder
        // We have to take into account if the gifter is anonymous. If it is, then we don't have a user to access
        if (userGifter.active)
        {
            // Si el usuario es activo, entonces podemos acceder a su informacion ya que el gifter no es anonimo
            // If the user is active, then we can access its information as the gifter is not anonymous

            ExampleManager.instance.SubscriptionGiftExample(user, userGifter.UserName);
        }
        else
        {
            // Si el usuario no es activo, entonces el gifter es anonimo
            // If the user is not active, then the gifter is anonymous
            ExampleManager.instance.SubscriptionGiftExample(user, "");
        }
    }

    #endregion

    #region You don't need to touch this


    #endregion
}
