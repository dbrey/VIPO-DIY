using System.Collections.Generic;
using UnityEngine;
using static Twitch_data.TwitchUtils;

#region Como usar
///
/// ChannnelRewardManager llama a RewardEvent cuando se recibe cualquier recompensa de Twitch. Si la recompensa existe, está activa y el usuario tiene los permisos necesarios, se ejecuta la recompensa.
/// Para añadir una recompensa, sigue los siguientes pasos:
/// 1º : Añade el nombre de la recompensa a la lista rewardsToAdd en el editor de Unity
/// 2º : Crea una clase que herede de ManagedReward y sobreescribe el método ExecuteReward con la funcionalidad deseada. Se muestran ejemplos de cómo hacerlo con y sin argumentos
/// 3º : Añade la recompensa y su clase correspondiente en el método AddReward. Se muestran ejemplos de cómo hacerlo
/// 
/// ADVERTENCIA 1: No se pueden añadir recompensas con el mismo nombre
/// ADVERTENCIA 2: Las recompensas con un cooldown mayor que 0 no pueden ser ejecutadas hasta que el cooldown haya terminado
/// ADVERTENCIA 3: Las recompensas solo pueden ser ejecutadas si el usuario tiene los mismos o mayores permisos que la recompensa
/// ADVERTENCIA 4: Las recompensas pueden ser desactivadas y activadas en cualquier momento cambiando el valor de enabled
/// ADVERTENCIA 5: Las recompensas deben añadirse manualmente en StreamerBot para poder modificar algunos de sus valores
/// 
#endregion

#region How to use
/// 
/// ChannelRewardManager calls RewardEvent when any reward from Twitch is received. If the reward exists, is enabled and the user has the necessary permissions, the reward is executed.
/// To add a reward, follow these steps:
/// 1º : Add the name of the reward to the rewardsToAdd list in the Unity editor
/// 2º : Create a class that inherits from ManagedReward and override the ExecuteReward method with the desired functionality. Examples are shown of how to do this with and without arguments
/// 3º : Add the reward and its corresponding class in the AddReward method. Examples are shown of how to do this
/// 
/// WARNING 1: Rewards with the same name cannot be added
/// WARNING 2: Rewards with a cooldown greater than 0 cannot be executed until the cooldown has ended
/// WARNING 3: Rewards can only be executed if the user has the same or greater permissions than the reward
/// WARNING 4: Rewards can be disabled and enabled at any time by changing the value of enabled
/// WARNING 5: Rewards must be added manually in StreamerBot in order to modify some of their values
#endregion

public class ChannelRewardManager : MonoBehaviour
{
    public static ChannelRewardManager instance;

    public class ManagedReward
    {
        public string name;
        public int cost;
        public float coolDown;
        public bool enabled;
        public Permissions permissions; // 0 = everyone, 1 = subscriber, 2 = VIP, 3 = moderator, 4 = broadcaster

        public ManagedReward()
        {
            name = "";
            cost = 0;
            coolDown = 0;
            enabled = true;
            permissions = Permissions.Everyone;
        }

        public ManagedReward(string name, int cost, float coolDown, bool enabled, Permissions permissions)
        {
            this.name = name;
            this.cost = cost;
            this.coolDown = coolDown;
            this.enabled = enabled;
            this.permissions = permissions;
        }

        public virtual void ExecuteReward(User user, List<string> rewardArguments)
        {
            // We will override this method later
        }
    }
    
    [SerializeField] List<string> rewardsToAdd = new List<string>();
    Dictionary<string, ManagedReward> managedRewards = new Dictionary<string, ManagedReward>();

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
        managedRewards = new Dictionary<string, ManagedReward>();
        foreach (string reward in rewardsToAdd)
        {
            AddReward(reward);
        }

    }

    void Update()
    {
        #region Documentation in Spanish
        /// 
        /// Comprobamos constantemente si el cooldown de las recompensas ha terminado
        /// Disminuimos el tiempo restante al cooldown
        /// Si el cooldown ha terminado, activamos la recompensa
        /// Hacemos esto a todas las recompensas existentes con un cooldown mayor que 0
        ///
        #endregion
        #region Documentation in English
        /// 
        /// We check constantly if the cooldown of the rewards is over
        /// We decrease the remaining time to the cooldown
        /// If the cooldown is over, we enable the reward
        /// We do this to all the existing rewards with a cooldown greater than 0
        ///
        #endregion

        foreach (KeyValuePair<string, ManagedReward> entry in managedRewards)
        {
            if (managedRewards[entry.Key].coolDown > 0)
            {
                ManagedReward auxReward= managedRewards[entry.Key];
                auxReward.coolDown -= Time.deltaTime;

                if (auxReward.coolDown <= 0)
                    auxReward.enabled = true;

                managedRewards[entry.Key] = auxReward;
            }
        }
    }

    #region Methods called by StreamerBotEvent Manager
    
    public void RewardEvent(string rewardName, User userToAssign, List<string> rewardArguments)
    {
        // Si la recompensa existe, está activa y el usuario tiene los permisos necesarios, ejecutamos el efecto de la recompensa
        // If the reward exists, is enabled and the user has the necessary permissions, we execute the reward effect
        if (managedRewards.ContainsKey(rewardName) && managedRewards[rewardName].enabled && 
            userToAssign.permissions >= managedRewards[rewardName].permissions)
        {
            managedRewards[rewardName].ExecuteReward(userToAssign, rewardArguments);
            Debug.Log(userToAssign.UserName + " decided to redeem the reward " + rewardName);
        }
        
    }

    #endregion

    #region Reward Management

    void AddReward(string rewardName)
    {
        // Si la recompensa no existe, la añadimos
        // If the reward doesn't exist, we add it
        if (!managedRewards.ContainsKey(rewardName))
        {
            switch (rewardName)
            {
                case "ExampleReward":
                    managedRewards.Add(rewardName, new ExampleReward(rewardName, 1, 0, true, Permissions.Everyone));
                    break;

                default:
                    Debug.Log("Reward not found, check the reward name and its class");
                    break;

                    /* Example of how to add a reward
                    case "name of your reward":
                       managedRewards.Add(rewardName, new YourRewardClass (rewardName, isEnabled?, CoolDownTimer, Permissions));
                       break;
                    */
            }
        }
    }

    void updateCostReward(string rewardToUpdate ,int newCost)
    {
        if (managedRewards.ContainsKey(rewardToUpdate))
        {
            ManagedReward auxReward = managedRewards[rewardToUpdate];
            auxReward.cost = newCost;
            managedRewards[rewardToUpdate] = auxReward;

            // Here we have to tell StreamerBot to update the reward cost in Twitch as well
            // For now, this only changes the cost of one specific reward
            // If we want to specify which reward we need to define some "Selection" type in the doAction method
            StreamerBotEventManager.instance.udpSend.doAction("UpdateChannelReward","", "", auxReward.cost);
        }
        else
        { 
            Debug.Log("The reward " + rewardToUpdate + " does not exist");
        }
    }


    #endregion

    #region Program here the methods for the rewards
    class ExampleReward : ManagedReward
    {
        // Constructor para asignar los valores de la recompensa
        // Constructor to assign the values of the reward
        public ExampleReward(string name, int cost, float coolDown, bool enabled, Permissions permissions) : base(name, cost, coolDown, enabled, permissions)
        {}

        // Este metodo se llama cuando se ejecuta la recompensa en el metodo RewardEvent
        // This void is called when the reward is executed in the void RewardEvent
        public override void ExecuteReward(User user, List<string> rewardArguments)
        {
            // Aqui puedes programar los efectos de tu comando
            // Here you can program the effects of your command

            Debug.Log(user.UserName + " redeemed the reward EXAMPLE REWARD with cost :" + cost);
            ExampleManager.instance.RewardExample();
        }
    }

    class exampleArgumentsReward : ManagedReward
    {
        // Constructor para asignar los valores de la recompensa
        // Constructor to assign the values of the reward
        public exampleArgumentsReward(string name, int cost, float coolDown, bool enabled, Permissions permissions) : base(name, cost, coolDown, enabled, permissions)
        {
        }

        // Este metodo se llama cuando se ejecuta la recompensa en el metodo RewardEvent
        // This void is called when the reward is executed in the void RewardEvent
        public override void ExecuteReward(User user, List<string> rewardArguments)
        {
            // Aqui puedes programar los efectos de tu comando
            // Here you can program the effects of your command

            string arguments = "";
            foreach (string argument in rewardArguments)
            {
                arguments += argument + " ";
            }

            Debug.Log("Reward arguments: " + arguments);
        }
    }
    #endregion

}
