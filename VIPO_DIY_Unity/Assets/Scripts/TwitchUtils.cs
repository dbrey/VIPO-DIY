using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Twitch_data
{
    public class TwitchUtils : MonoBehaviour
    {
        public enum Permissions
        {
            /// The higher the number, the more permissions the user has
            /// Maybe the suscribers have another tier of permissions depending on the tier of the suscription


            Broadcaster = 5,

            // Have to be assigned by the broadcaster in Twitch. Should have access to Moderation tools
            Mods = 4,

            // Have to be assigned by the broadcaster in Twitch
            VIPs = 3,

            // This can only be accessed if a viewer has a subscription
            Subscribers = 2,
            
            // This can only be accessed if a viewer is following the channel
            Follower = 1,

            Everyone = 0
        }
        public enum SubscriptionTier
        {
            /// I suppose this is the order of the tiers simply because in order to be tier 3 
            /// you have to pay like 25$ meanwhile in order to be tier1 you just need 5$

            // The user have to pay 25$ to be tier 3, so it's the highest tier
            Tier3 = 4,

            // The user have to pay 10$ to be tier 2
            Tier2 = 3,

            // The user have to pay 5$ to be tier 1
            Tier1 = 2,

            // The user have to have Amazon Prime to be Prime (Amazon Prime gives the user a free subscription to a channel)
            Prime = 1,

            // The user is not subscribed or there is an error and the tier is not set
            NotSet = 0

        }

        public struct User
        {
            /// The user's username, used to log in to Twitch
            public string UserName;

            public string profilePictureURL;

            public Permissions permissions; // We know what kind of user it is by the permissions

            /// Whether or not the user is lurking in chat
            //public bool IsLurking;

            /// Details of the user's subscription. Will be null if the user isn't subscribed. May also be null even if the user *is* subscribed.
            public Subscription subscription;

            public void newUser(string userName, string profileURL, Permissions permissions, Subscription sub)
            {
                UserName = userName;
                profilePictureURL = profileURL;
                this.permissions = permissions;
                subscription = sub;
                // Suscription is null by default
            }
        }

        public class Subscription
        {
            
            /// The total number of months the user has been subscribed to the channel
            public int SubscribedMonthCount;

            /// <summary>
            /// The number of concurrent months in the user has been subscribed in their current streak
            /// </summary>
            /// <remarks>
            /// This is only set if the user subscribed/re-subscribed since the overlay was opened
            /// </remarks>
            //public int StreakMonths { get; internal set; }

            
            /// The tier the user subscribed at.
            /// This should always be set if the user is subscribed and the data is available
            public SubscriptionTier Tier;

            /// Whether the subscription is a gift sub
            /// This should always be set if the user is subscribed
            public bool IsGift;

            /// A user with details of the gifter
            /// This will be null if this is not a gift subscription or the gift was anonymous
            public User Gifter;


            public void selectTier(string tier)
            {
                switch (tier)
                {
                    case "prime":
                        Tier = SubscriptionTier.Prime;
                        break;
                    case "tier 1":
                        Tier = SubscriptionTier.Tier1;
                        break;
                    case "tier 2":
                        Tier = SubscriptionTier.Tier2;
                        break;
                    case "tier 3":
                        Tier = SubscriptionTier.Tier3;
                        break;
                }
            }

            public void newSubscription(int subscribedMonthCount, string tier, bool isGift, User gifter)
            {
                SubscribedMonthCount = subscribedMonthCount;
                selectTier(tier);
                IsGift = isGift;
                Gifter = gifter;
            }
        }
       
    }
}

