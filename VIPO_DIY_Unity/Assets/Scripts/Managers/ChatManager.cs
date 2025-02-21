using Palmmedia.ReportGenerator.Core;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;
using Twitch_data;
using static Twitch_data.TwitchUtils;
using Unity.VisualScripting;

#region Como usar
///
/// Para recibir cualquier mensaje del chat, ReceiveChatMessage se encarga de filtrar si es un mensaje normal o un comando y recibe la informacion del usuario que lo ha enviado.
/// En el caso de ser un mensaje normal, se llama a printMessage cuya funcionalidad se puede cambiar a gusto del usuario. Ahora solo imprime el mensaje en consola.
/// 
/// En el caso de ser un comando, se llama a CallCommand que se encarga de llamar al comando correspondiente. Si el comando no existe o no esta activo, no se ejecuta.
/// Para añadir un comando nuevo hay que seguir los siguientes pasos:
/// 1º : Añadir el nombre del comando a la lista commandsToAdd en el editor de Unity
/// 2º : Crear una clase que herede de ManagedCommand y sobreescribir el metodo ExecuteCommand con la funcionalidad deseada. Se muestra ejemplos de como hacerlo con y sin argumentos
/// 3º : Añadir el comando y su clase correspondiente en el metodo AddCommand. Se muestra ejemplos de como hacerlos
/// 
/// ADVERTENCIA 1: No se pueden añadir comandos con el mismo nombre
/// ADVERTENCIA 2: Los comandos con cooldown mayor que 0 no se pueden ejecutar hasta que el cooldown haya terminado
/// ADVERTENCIA 3: Los comandos solo se pueden ejecutar si el usuario tiene el mismo o mayor permiso que el comando
/// ADVERTENCIA 4: Los comandos se pueden desactivar y activar en cualquier momento cambiando el valor de enabled
/// 
#endregion

#region How to use
/// 
/// ReceiveChatMessage is in charge of filtering if a message is a normal message or a command and receives the information of the user that has sent it.
/// In the case of a normal message, it calls printMessage whose functionality can be changed at the user's discretion. Now it only prints the message in the console.
/// 
/// In the case of a command, it calls CallCommand which is responsible for calling the corresponding command. If the command does not exist or is not active, it is not executed.
/// To add a new command you must follow the following steps:
/// 1º : Add the name of the command to the commandsToAdd list in the Unity editor
/// 2º : Create a class that inherits from ManagedCommand and override the ExecuteCommand method with the desired functionality. Examples are shown of how to do it with and without arguments
/// 3º : Add the command and its corresponding class in the AddCommand method. Examples are shown of how to do it
/// 
/// WARNING 1: Commands with the same name cannot be added
/// WARNING 2: Commands with a cooldown greater than 0 cannot be executed until the cooldown is over
/// WARNING 3: Commands can only be executed if the user has the same or greater permission than the command
/// WARNING 4: Commands can be deactivated and activated at any time by changing the value of enabled
///  
#endregion


public class ChatManager : MonoBehaviour
{
    public static ChatManager instance;

    class ManagedCommand
    { 
        public string name; // The name of the command
        public bool enabled; // Is the command enabled?
        public float timer; // Cooldown timer
        public Permissions permissions; // 0 = everyone, 1 = subscriber, 2 = VIP, 3 = moderator, 4 = broadcaster
        
        // We add an empty method to the struct so we can override it later
        public ManagedCommand()
        {
            name = "";
            enabled = true;
            timer = 0;
            permissions = 0;
        }

        public ManagedCommand (string name, bool enabled, float timer, Permissions permissions)
        {
            this.name = name;
            this.enabled = enabled;
            this.timer = timer;
            this.permissions = permissions;
        }
        
        public virtual void ExecuteCommand(User user, List<string> commandArguments)
        {
            // We will override this method later
        }
        
    }
    
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
        // We give the commands a default value
        commands = new Dictionary<string, ManagedCommand>();
        foreach(string command in commandsToAdd)
        {
            AddCommand(command);
        }

    }

    public void AddCommand(string commandName)
    { 
        // If the command doesn't exist, we add it
        if(!commands.ContainsKey(commandName))
        {
            switch (commandName)
            {
                case "exampleCommand":
                    commands.Add(commandName, new exampleCommand(commandName, true, 0, Permissions.Everyone));
                    break;

                case "exampleArgumentsCommand":
                    commands.Add(commandName, new exampleArgumentsCommand(commandName, true, 0, Permissions.Everyone));
                    break;
                default:
                    Debug.Log("Command not found, check the commands name and its class");
                    break;

                /* Example of how to add a command
                case "name of your command":
                   commands.Add(commandName, new YourCommandClass (commandName, isEnabled?, CoolDownTimer, Permissions));
                   break;
                */
            }
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
    

    private void CallCommand(User user, List<string> commandArguments)
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
            else
                // Si el usuario no tiene los permisos necesarios
                // If the user doesn't have the necessary permissions
                Debug.LogWarning("The user " + user.UserName + " doesn't have the permissions to execute the command " + commandArguments[0]);
        }
        else if (!commands.ContainsKey(commandArguments[0]))
        {
            // Si el comando no existe
            // If the command doesn't exist
            Debug.LogWarning("The command " + commandArguments[0] + " is not included");
        }
        else if (!commands[commandArguments[0]].enabled)
        {
            // Si el comando no esta activo pero tiene un cooldown
            // If the command is not enabled but has a cooldown
            if (commands[commandArguments[0]].timer > 0)
                Debug.LogWarning("The command " + commandArguments[0] + " is on cooldown");
            // Si el comando no esta activo y no tiene un cooldown
            // If the command is not enabled and doesn't have a cooldown
            else if (!commands[commandArguments[0]].enabled)
                Debug.LogWarning("The command " + commandArguments[0] + " is not enabled");
        }
        
    }


    public void ReceiveChatMessage(User user, string message)
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
    #region Program your messages and commands here

    void printMessage(User user, string message)
    {
        ExampleManager.instance.AddChatMessage(user, message);
    }

    class exampleCommand : ManagedCommand
    {
        // Constructor para asignar los valores del comando
        // Constructor to assign the values of the command
        public exampleCommand(string name, bool enabled, float timer, Permissions permissions) : base(name, enabled, timer, permissions)
        {
        }

        // This void is called when the command is executed in the void CallCommand
        public override void ExecuteCommand(User user, List<string> commandArguments)
        {
            // Aqui puedes programar los efectos de tu comando
            // Here you can program the effects of your command
            Debug.Log("This is just a test of the command");
        }
    }

    class exampleArgumentsCommand : ManagedCommand
    {
        // Constructor para asignar los valores del comando
        // Constructor to assign the values of the command
        public exampleArgumentsCommand(string name, bool enabled, float timer, Permissions permissions) : base(name, enabled, timer, permissions)
        {
        }

        // This void is called when the command is executed in the void CallCommand
        public override void ExecuteCommand(User user, List<string> commandArguments)
        {
            // Aqui puedes programar los efectos de tu comando
            // Here you can program the effects of your command

            string arguments = "";
            foreach (string argument in commandArguments)
            {
                arguments += argument + " ";
            }

            Debug.Log("Command arguments: " + arguments);
        }
    }

    #endregion
}
