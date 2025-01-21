using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Twitch_data
{
    public class TwitchUtils : MonoBehaviour
    {
        public enum Permissions
        {
            
            /// Only the broadcaster is permitted to use the command. This is implied by all other permissions so never needs to be passed in with them
            Broadcaster = 0,

            /// Channel Moderators are permitted to use the command
            Mods = 1,

            /// Channel VIPs are permitted to use the command
            VIPs = 2,

            /// Channel Subscribers are permitted to use the command
            Subscribers = 4,

            /// Anyone viewing the stream is permitted to use the command. This obviously overrides all other permissions
            Everyone = 8
        }

        public class Subscription
        {
            /// <summary>
            /// The total number of months the user has been subscribed to the channel
            /// </summary>
            /// <remarks>
            /// Set if the user subscribed/re-subscribed or chatted since the overlay was opened
            /// </remarks>
            public int SubscribedMonthCount { get; internal set; }

            /// <summary>
            /// The number of concurrent months in the user has been subscribed in their current streak
            /// </summary>
            /// <remarks>
            /// This is only set if the user subscribed/re-subscribed since the overlay was opened
            /// </remarks>
            public int StreakMonths { get; internal set; }

            /// <summary>
            /// The tier the user subscribed at.
            /// </summary>
            /// <remarks>
            /// This should always be set if the user is subscribed and the data is available
            /// </remarks>
            public SubscriptionTier Tier { get; internal set; }

            /// <summary>
            /// The name of the subscription plan (usually something like "Channel Subscription (channelname)")
            /// </summary>
            /// <remarks>
            /// This should always be set if the user is subscribed
            /// </remarks>
            public string PlanName { get; internal set; }

            /// <summary>
            /// Whether the subscription is a gift sub
            /// </summary>
            /// <remarks>
            /// This should always be set if the user is subscribed
            /// </remarks>
            public bool IsGift { get; internal set; }

            /// <summary>
            /// A user with details of the gifter
            /// </summary>
            /// <remarks>
            /// This will be null if this is not a gift subscription or the gift was anonymous
            /// </remarks>
            public User Gifter { get; internal set; }

            static internal SubscriptionTier TierForString(string tierString)
            {
                if (tierString == "1000")
                {
                    return SubscriptionTier.Tier1;
                }
                else if (tierString == "2000")
                {
                    return SubscriptionTier.Tier2;
                }
                else if (tierString == "3000")
                {
                    return SubscriptionTier.Tier3;
                }
                else if (tierString == "Prime")
                {
                    return SubscriptionTier.Prime;
                }
                return SubscriptionTier.NotSet;
            }

            static internal string StringForTier(SubscriptionTier tier)
            {
                switch (tier)
                {
                    case SubscriptionTier.Prime:
                        return "Prime";
                    case SubscriptionTier.Tier1:
                        return "1000";
                    case SubscriptionTier.Tier2:
                        return "2000";
                    case SubscriptionTier.Tier3:
                        return "3000";
                    default:
                        return null;
                }
            }
        }


        /// A list of subscription tiers
        public enum SubscriptionTier
        {

            /// If the tier is not available, or if Twitch returns an unknown tier string the tier will be NotSet
            NotSet,
            Prime,
            Tier1,
            Tier2,
            Tier3
        }

        public class User
        {

            /// The ID of the user, usually a series of numbers
            public string UserId { get; }


            /// The user's username, used to log in to Twitch
            public string UserName { get; }


            /// The user's display name, shown in Twitch's UI
            public string DisplayName { get; }

            internal User(string userID, string userName, string displayName)
            {
                this.UserId = userID;
                this.UserName = userName;
                this.DisplayName = displayName;
            }

            /// Whether or not the user is the broadcaster
            public bool IsBroadcaster { get; internal set; } = false;

            /// Whether or not the user is a moderator
            public bool IsModerator { get; internal set; } = false;

            /// Whether or not the user is a VIP
            public bool IsVIP { get; internal set; } = false;

            /// Whether or not the user is Subscribed
            public bool IsSubscriber { get; internal set; } = false;

            /// Whether or not the user is lurking in chat
            public bool IsLurking { get; internal set; } = false;

            /// Details of the user's subscription. Will be null if the user isn't subscribed. May also be null even if the user *is* subscribed.
            public Subscription? Subscription { get; internal set; } = null;

            public Color? ChatColor { get; internal set; } = null;


            #region Permissions
             internal bool IsPermitted(Permissions permissions)
             {
                 if ((permissions & Permissions.Everyone) == Permissions.Everyone)
                     return true;
                 
                 if (this.IsBroadcaster)
                     return true;
                 
                 if ((permissions & Permissions.Mods) == Permissions.Mods && this.IsModerator)
                    return true;

                 if ((permissions & Permissions.VIPs) == Permissions.VIPs && this.IsVIP)
                     return true;
                 
                 if ((permissions & Permissions.Subscribers) == Permissions.Subscribers && this.IsSubscriber)
                     return true;
                 
                 return false;
             }
             #endregion
        }
    }
}

