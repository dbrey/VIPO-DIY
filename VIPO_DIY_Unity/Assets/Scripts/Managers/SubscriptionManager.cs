using UnityEngine;
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

public class SubscriptionManager : MonoBehaviour
{
    public static SubscriptionManager instance;

    private User saveUserToGift;

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

    public void SubscriptionEvent(User user)
    {
        // Recibimos al usuario que ya tiene la suscripcion, el nombre del receptor, los meses suscritos y el tier de la suscripcion
        // We receive the user who already has the subscription, the name of the receiver, the months suscribed and the tier of the subscription

        // En este caso, solo mostramos el nombre del usuario y su foto de perfil (Puedes borrar la linea y es completamente seguro)
        // In this case, we only show the name of the user and its profile picture (You can delete the line and it's completely safe)
        if (ExampleManager.instance != null)
            ExampleManager.instance.SubscriptionExample(user);
        else
            Debug.LogWarning("There's no ExampleManager. Either add an ExampleManager or get rid of this part of the code in SubscriptionEvent of SubscriptionManager");

    }

    public void SubscriptionGiftEvent(User user, User userGifter)
    {
        // Recibimos al usuario que ya tiene la suscripcion, el nombre del receptor, los meses suscritos y el tier de la suscripcion
        // We receive the user who already has the subscription, the name of the receiver, the months suscribed and the tier of the subscription

        // Hay que tener en cuenta si el gifter es anonimo. Si lo es, entonces no tenemos un usuario al que acceder
        // We have to take into account if the gifter is anonymous. If it is, then we don't have a user to access
        if (userGifter.active)
        {
            // Si el usuario es activo, entonces podemos acceder a su informacion ya que el gifter no es anonimo (Puedes borrar la linea y es completamente seguro)
            // If the user is active, then we can access its information as the gifter is not anonymous (You can delete the line and it's completely safe)

            string gifterUsername = userGifter.UserName;
            if (TwitchManager.instance == null)
            {
                Debug.LogWarning("There's no TwitchManager, add one in the scene");
            }

            if (TwitchManager.instance.getUser(gifterUsername, ref userGifter))
            {
                // El usuario ya estaba en la lista de usuarios, por lo que podemos acceder a su informacion
                // The user was already in the list of users, so we can access its information

                if (ExampleManager.instance != null)
                    ExampleManager.instance.SubscriptionGiftExample(user, userGifter.UserName);
                else
                {
                    Debug.LogWarning("There's no ExampleManager. Either add an ExampleManager or get rid of this part of the code in SubscriptionGiftEvent (known user) of SubscriptionManager");
                }
            }
            else
            {
                // Si no tenemos informacion del usuario, guardamos el usuario en una variable temporal y solicitamos la informacion del gifter
                // If we don't have information of the user, we save the user in a temporal variable and request the information of the gifter
                saveUserToGift = user;
                requestGifterUser(gifterUsername);
            }
        }
        else
        {
            // Si el usuario no es activo, entonces el gifter es anonimo (Puedes borrar las lineas y es completamente seguro)
            // If the user is not active, then the gifter is anonymous (You can delete the lines and it's completely safe)
            if (ExampleManager.instance != null)
                ExampleManager.instance.SubscriptionGiftExample(user, "");
            else
                Debug.LogWarning("There's no ExampleManager. Either add an ExampleManager or get rid of this part of the code in SubscriptionGiftEvent (user anonymous) of SubscriptionManager");

        }
    }

    #endregion

    #region Auxiliar Methods

    private void requestGifterUser(string username)
    {
        // Mandamos un request para conseguir informacion del streamer
        TwitchManager.instance.getUDPSender().doAction("GetUserInfo", "Request User Info", username, 0);
        TwitchManager.instance.whoRequested = TwitchManager.WhoRequested.SubscriptionManager;
    }

    public void SubscriptionGiftEventWithGifterInfo(User gifterWithInfo)
    {
        // Guardamos la informacion del gifter en la lista de usuarios por si vuelve a donar suscripcion
        // We save the information of the gifter in the list of users in case it donates a subscription again
        TwitchManager.instance.updateUser(gifterWithInfo);

        // Puedes borrar esta lineas y es completamente seguro! Simplemente desconecta la accion del evento
        // You can delete this lines and it's completely safe! It simply disconnects the action from the event
        if (ExampleManager.instance != null)
            ExampleManager.instance.SubscriptionGiftExample(saveUserToGift, gifterWithInfo.UserName);
        

        // Despues de realizar el evento que queriamos, reseteamos la variable temporal para evitar posibles errores
        // After doing the event we wanted, we reset the temporal variable to avoid possible errors
        saveUserToGift = new User();
    }

    #endregion
}
