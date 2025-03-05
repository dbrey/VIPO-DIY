using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Twitch_data
{
    public class TwitchUtils : MonoBehaviour
    {
        public enum Permissions
        {
            /// Cuanto mas alto el numero, mas permisos tiene el usuario
            /// Es posible que los suscriptores tengan otro nivel de permisos dependiendo del tier de la suscripcion

            /// The higher the number, the more permissions the user has
            /// Maybe the suscribers have another tier of permissions depending on the tier of the suscription


            Broadcaster = 5,

            // Deberia ser asignado por el broadcaster en Twitch. Deberia tener acceso a las herramientas de Moderacion
            // Have to be assigned by the broadcaster in Twitch. Should have access to Moderation tools
            Mods = 4,

            // Deberia ser asignado por el broadcaster en Twitch
            // Have to be assigned by the broadcaster in Twitch
            VIPs = 3,

            // Solo puede ser accedido si el viewer tiene una suscripcion
            // This can only be accessed if a viewer has a subscription
            Subscribers = 2,
            
            // Solo puede ser accedido si el viewer esta siguiendo el canal
            // This can only be accessed if a viewer is following the channel
            Follower = 1,

            Everyone = 0
        }
        public enum SubscriptionTier
        {
            // The user have to pay 25$ to be tier 3, so it's the highest tier
            Tier3 = 4,

            // The user have to pay 10$ to be tier 2
            Tier2 = 3,

            // The user have to pay 5$ to be tier 1
            Tier1 = 2,

            // El usuario tiene que tener Amazon Prime para ser Prime (Amazon Prime da una suscripcion gratuita a un canal)
            // The user have to have Amazon Prime to be Prime (Amazon Prime gives the user a free subscription to a channel)
            Prime = 1,

            // El usuario no esta suscrito o hay un error y el tier no esta asignado
            // The user is not subscribed or there is an error and the tier is not set
            NotSet = 0

        }

        public struct User
        {
            // Esto es una variable dada por defecto a todos los usuarios. Se vuelve true cuando añadimos al usuario
            // This is a flag given by default to all users. It turns true when we add the user
            public bool active;

            /// El nombre de usuario del usuario, usado para iniciar sesion en Twitch
            /// The user's username, used to log in to Twitch
            public string UserName;

            public string profilePictureURL;

            // Sabemos que tipo de usuario es por los permisos
            // We know what kind of user it is by the permissions
            public Permissions permissions; 

            /// Detaññes de la suscripcion del usuario. Sera null si el usuario no esta suscrito. Tambien puede ser null incluso si el usuario *esta* suscrito.
            /// Details of the user's subscription. Will be null if the user isn't subscribed. May also be null even if the user *is* subscribed.
            public Subscription subscription;

            public void newUser(string userName, string profileURL, Permissions permissions, Subscription sub)
            {
                active = true;
                UserName = userName;
                profilePictureURL = profileURL;
                this.permissions = permissions;
                subscription = sub;
            }

            public User(bool exists)
            { 
                active = exists;
                UserName = "";
                profilePictureURL = "";
                permissions = Permissions.Everyone;
                subscription = new Subscription();
            }
        }

        public class Subscription
        {
            /// El numero total de meses que el usuario ha estado suscrito al canal
            /// The total number of months the user has been subscribed to the channel
            public int SubscribedMonthCount;

            /// <summary>
            /// The number of concurrent months in the user has been subscribed in their current streak
            /// </summary>
            /// <remarks>
            /// This is only set if the user subscribed/re-subscribed since the overlay was opened
            /// </remarks>
            /// STILL IN DEVELOPMENT
            //public int StreakMonths { get; internal set; }

            
            /// El tier al que el usuario se ha suscrito
            /// Esto siempre deberia estar asignado si el usuario esta suscrito y los datos estan disponibles
            /// The tier the user subscribed at.
            /// This should always be set if the user is subscribed and the data is available
            public SubscriptionTier Tier;

            /// Si la suscripcion es un regalo
            /// Esto siempre deberia estar asignado si el usuario esta suscrito
            /// Whether the subscription is a gift sub
            /// This should always be set if the user is subscribed
            public bool IsGift;

            /// Un usuario con detalles del gifter
            /// Esto sera null si esto no es una suscripcion regalada o el regalo fue anonimo
            /// A user with details of the gifter
            /// This will be null if this is not a gift subscription or the gift was anonymous
            public User Gifter;


            public void selectTierINT(int tier)
            {
                switch (tier)
                {
                    case 1000:
                        Tier = SubscriptionTier.Tier1;
                        break;
                    case 2000:
                        Tier = SubscriptionTier.Tier2;
                        break;
                    case 3000:
                        Tier = SubscriptionTier.Tier3;
                        break;
                    default:
                        Tier = SubscriptionTier.Prime;
                        break;
                }
            }

            public void selectTierSTRING(string tier)
            {
                switch (tier)
                {
                    case "tier1":
                        Tier = SubscriptionTier.Tier1;
                        break;
                    case "tier2":
                        Tier = SubscriptionTier.Tier2;
                        break;
                    case "tier3":
                        Tier = SubscriptionTier.Tier3;
                        break;
                    case "prime":
                        Tier = SubscriptionTier.Prime;
                        break;
                }
            }

            public void newSubscription(int subscribedMonthCount, string tier, bool isGift, User gifter)
            {
                SubscribedMonthCount = subscribedMonthCount;
                selectTierSTRING(tier);
                IsGift = isGift;
                Gifter = gifter;
            }
        }
       
    }
}

