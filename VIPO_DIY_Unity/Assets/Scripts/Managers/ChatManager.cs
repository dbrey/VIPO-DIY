using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChatManager : MonoBehaviour
{
    public static ChatManager instance;

    struct ManagedCommand
    { 
        public string name; // The name of the command
        public bool enabled; // Is the command enabled?
        public float timer; // Cooldown timer
        public int permissions; // 0 = everyone, 1 = subscriber, 2 = VIP, 3 = moderator, 4 = broadcaster
        // We add an empty method to the struct so we can override it later
        public void ExecuteCommand(string username, string userProfilePicture, List<string> commandArguments)
        {
            // We will override this method later
        }
        
    }

    // The user will write down what kind of commands they want to add
    // Later we need to tell the user what kind of permissions and cooldowns they want to add
    [SerializeField] List<string> commandsToAdd = new List<string>();
     Dictionary<string, ManagedCommand> commands;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        commands = new Dictionary<string, ManagedCommand>();

    }

    public void AddCommand(string commandName)
    { 
        // If the command doesn't exist, we add it
        if(!commands.ContainsKey(commandName))
        {
            // We set the managed command values to default
            ManagedCommand newCommand = new ManagedCommand();
            newCommand.name = commandName;
            newCommand.enabled = true;
            newCommand.timer = 0; // No cooldown
            newCommand.permissions = 0; // Everybody can use the command
            commands.Add(commandName, newCommand);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // We need to check if the cooldown is over
        foreach (KeyValuePair<string, ManagedCommand> entry in commands)
        {
            if (commands[entry.Key].timer > 0)
            {
                // If the cooldown is not over, we decrease the timer
                ManagedCommand command = commands[entry.Key];
                command.timer -= Time.deltaTime;
                
                // If after decreasing the timer, the timer is less than or equal to 0, we enable the command
                if (command.timer <= 0)
                    command.enabled = true;

                commands[entry.Key] = command;
            }
        }
    }

    #region Methods called by StreamerBotEvent Manager
    
    public void CallCommand(string username, string userProfilePicture, List<string> commandArguments)
    {
        // We check the first element of the command, which is the command itself
        commands[commandArguments[0]].ExecuteCommand(username, userProfilePicture,commandArguments);
    }

    public void ReceiveChatMessage(string username, string userProfilePicture, string message)
    {
        // We check if the message is a command
        if (message[0] == '!')
        {
            // We split the message by spaces
            string[] messageParts = message.Split(' ');
            // We remove the '!' from the command
            messageParts[0] = messageParts[0].Substring(1);
            // We call the command
            CallCommand(username, userProfilePicture, new List<string>(messageParts));
        }
        else
        {
            // Do whatever you want with the message here
            // You also have access to the user's name and the profile picture of that user
            Debug.Log(username + " sent a message: " + message);
        }
        
    }

    #endregion

    #region Program the effects of your commands here!

    // Here's a simple example of a command that will print a message to the console
    void printMessage(List<string> commandArguments)
    {
        Debug.Log(commandArguments[0]);
    }

    #endregion
}
