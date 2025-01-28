using Palmmedia.ReportGenerator.Core;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;
using Twitch_data;

public class ChatManager : MonoBehaviour
{
    public static ChatManager instance;

    class ManagedCommand
    { 
        public string name; // The name of the command
        public bool enabled; // Is the command enabled?
        public float timer; // Cooldown timer
        public TwitchUtils.Permissions permissions; // 0 = everyone, 1 = subscriber, 2 = VIP, 3 = moderator, 4 = broadcaster
        // We add an empty method to the struct so we can override it later
        public virtual void ExecuteCommand(TwitchUtils.User user, List<string> commandArguments)
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

    void Start()
    {
        // Le damos a los comandos un valor por defecto
        //We give the commands a default value
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

    void Update()
    {
        #region Documentation in Spanish
        /// 
        /// Comprobamos constantemente si el cooldown de los comandos ha terminado
        /// Disminuimos el tiempo restante al cooldown
        /// Si el cooldown ha terminado, activamos el comando
        /// Hacemos esto a todos los comandos existentes con un cooldown mayor que 0
        ///
        #endregion
        #region Documentation in English
        /// 
        /// We check constantly if the cooldown of the commands is over
        /// We decrease the remaining time to the cooldown
        /// If the cooldown is over, we enable the command
        /// We do this to all the existing commands with a cooldown greater than 0
        ///
        #endregion

        foreach (KeyValuePair<string, ManagedCommand> entry in commands)
        {
            if (commands[entry.Key].timer > 0)
            {
                ManagedCommand command = commands[entry.Key];
                command.timer -= Time.deltaTime;
                
                if (command.timer <= 0)
                    command.enabled = true;

                commands[entry.Key] = command;
            }
        }
    }

    #region Methods called by StreamerBotEvent Manager
    

    private void CallCommand(TwitchUtils.User user, List<string> commandArguments)
    {
        // We check the first element of the command, which is the command itself
        if (commands.ContainsKey(commandArguments[0]) && commands[commandArguments[0]].enabled)
        {
            // We check if the user has the same or above permissions than the command
            if (user.permissions >= commands[commandArguments[0]].permissions)
            {
                // We call the command
                commands[commandArguments[0]].ExecuteCommand(user, commandArguments);
            }
        }
        
    }


    public void ReceiveChatMessage(TwitchUtils.User user, string message)
    {
        // Comprobamos si el mensaje es un comando, mirando si su primer caracter es un !
        // We check if the message is a command, looking if its first character is a !

        #region Documentation in Spanish
        /// 
        /// Cuando recibimos un texto, primero comprobamos si es un comando mirando si su primer caracter es un !
        /// Si es un comando, lo separamos por espacios y quitamos el ! del comando
        /// Llamamos al metodo del metodo del comando y le pasamos el resto del mensaje en caso de ser necesario
        /// 
        /// Si es un mensaje normal, hacemos lo que queramos con el mensaje
        /// Tambien tenemos acceso al nombre del usuario y a la foto de perfil de ese usuario
        ///
        #endregion
        #region Documentation in English
        /// 
        /// When we receive a text, we first check if it is a command by looking if its first character is a !
        /// If it is a command, we split it by spaces and remove the ! from the command
        /// We call the command's method and pass the rest of the message if necessary
        /// 
        /// If it's a normal message, we do whatever we want with the message
        /// We also have access to the name of the user and the profile picture of that user
        ///
        #endregion

        if (message[0] == '!')
        {
            string[] messageParts = message.Split(' ');
            messageParts[0] = messageParts[0].Substring(1);
            CallCommand(user, new List<string>(messageParts));
        }
        else
        {
            // Aqui puedes programar los efectos de un mensaje normal
            // Here you can program the effects of a normal message
            printMessage(user, message);
        }
        
    }

    #endregion

    // Programa los efectos de tus comandos y mensajes normales aqui!
    // Program the effects of your commands and normal messages here!
    #region 

    void printMessage(TwitchUtils.User user, string message)
    {
        // Puedes borrar esta linea y es completamente seguro! Simplemente desconecta la accion del evento
        // You can delete this line and it's completely safe! It simply disconnects the action from the event
        ExampleManager.instance.AddChatMessage(user,message);
        
        Debug.Log(message);
    }

    #endregion
}
