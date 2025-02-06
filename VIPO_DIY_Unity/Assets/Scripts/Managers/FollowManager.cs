using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Twitch_data;

#region Como usar
///
/// Dentro del metodo FollowEvent, Streamerbot te da la informacion del usuario que te ha seguido
/// Con esa información, puedes hacer lo que quieras
/// Por ejemplo: Mostrar un mensaje con el nombre del usuario que te ha seguido y su foto de perfil
/// 
#endregion

#region How to use
/// 
/// In the FollowEvent method, Streamerbot gives you the information of the user that has followed you
/// With information, you can do everything you want
/// For example: Show a message with the name of the user that has followed you and their profile picture
///  
#endregion

public class FollowManager : MonoBehaviour
{
    public static FollowManager instance;

    private void Awake()
    {
        // Si no hay ninguna instancia, establecemos esta como la instancia
        // If there's no instance, we set this as the instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // Si ya hay una instancia, destruimos esta
        // If there's already an instance, we destroy this one
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
    }


    #region Documentation in Spanish
    // Este evento lo invoca Streamerbot cuando recibe una notificacion de que alguien te ha seguido
    /// Streamerbot te da la informacion del usuario que te ha seguido
    /// username -> Nombre del usuario en Twitch
    /// profilePictureURL -> URL de la foto de perfil del usuario
    /// CONSEJO : Si quieres acceder a la foto de perfil del usuario, puedes usar la URL para descargar la imagen y tenerlo como Texture2D
    ///
    #endregion
    #region Documentation in English
    // This event is invoked by Streamerbot when it receives a notification that someone has followed you
    /// Streamerbot gives you the information of the user that has followed you
    /// username -> Name of the user on Twitch
    /// profilePictureURL -> URL of the user's profile picture
    /// ADVISE : If you want to access to the profile picture of the user, you can use the URL to download the picture and have it as a Texture2D
    ///
    #endregion
    public void FollowEvent(TwitchUtils.User user)
    {
        // Puedes borrar esta linea y es completamente seguro! Simplemente desconecta la accion del evento
        // You can delete this line and it's completely safe! It simply disconnects the action from the event
        ExampleManager.instance.FollowExample(user);

    }

    // Programa los efectos de tus notificaciones de Seguidor aqui!
    // Program the effects of your Follow notifications here!
    #region 

    // Esto es un ejemplo de lo que puedes hacer cuando alguien te sigue
    // This is an example of what you can do when someone follows you
    void printFollowNotification(TwitchUtils.User user)
    {
        Debug.Log(user.UserName + " decided to follow me for some reason");
    }

    #endregion

}
