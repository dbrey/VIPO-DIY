using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEditor.VersionControl;
using UnityEngine;


#region Como usar
///
/// Para conseguir el ID de una accion, tienes que ir a Streamerbot, ir a la accion que quieras usar y hacer click izquierdo sobre ella.
/// Hay una opcion "Copy Action Id". Luego pegas en el campo ID en el editor de Unity.
/// ADVERTENCIA 1: La conexion a UDP no funciona si el servidor UDP no esta en ejecucion. Asegurate de que el servidor UDP este en ejecucion
/// ADVERTENCIA 2: Si algo no funciona, este codigo no te notifica que falla asi que asegurate de que todo este bien configurado
/// 
#endregion

#region How to use
/// 
///  To get the ID of an action, you must go to Streamerbot, go to the action you want to use and left click on it.
///  There is an option "Copy Action Id". Then you paste in the ID field in the Unity editor.
///  WARNING 1: The UDP connection does not work if the UDP server is not running. Make sure the UDP server is  running
///  WARNING 2: If something does not work, this code does not notify you that it fails so make sure everything is well configured
#endregion

public class UDPSend : MonoBehaviour
{
    // Streamerbot solo puede hacer DoActions, pero para eso necesita saber los ids especificos para poder usar la accion correcta
    // Streamerbot can only do DoActions, but for that it needs to know the specific ids to use the correct action
    [Serializable]
    public struct StreamerBotAction
    {
        public string name;
        public string id;
    }

    // Diccionario para almacenar las acciones
    // Dictionary to store the actions
    [Header("Put the action's id and names here (More info in this script)")]
    [SerializeField] StreamerBotAction[] actions;
    private Dictionary<string, StreamerBotAction> actionsDict;

    // El puerto del servidor UDP debe coincidir con el puerto que se haya puesto en Streamerbot
    // The UDP server port must match the port that has been set in Streamerbot
    [Header("UDP Server Port")]
    [SerializeField] int port = 4242;
    
    UdpClient udpClient;
    IPEndPoint serverEndpoint;
    string jsonString;

    void Start()
    {
        // Inicializamos todo
        // We initialize everything
        udpClient = new UdpClient();
        serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4242);
        udpClient.Client.Bind(serverEndpoint);
        actionsDict = new Dictionary<string, StreamerBotAction>();

        // Añadimos las acciones al diccionario para que sea mas facil acceder a ellos
        // We add the actions to the dictionary so it is easier to access them
        for (int i = 0; i < actions.Length; i++)
        {
            if (!actionsDict.ContainsKey(actions[i].name))
            {
                actionsDict.Add(actions[i].name, actions[i]);
            }
            else
            {
                Debug.Log("You may have a duplicated action with the same name. Make sure that all actions have different names");
            }
        }

        // Este funciona pero solo manda un mensaje
        //jsonString = @"{ ""request"": ""DoAction"", ""action"": { ""id"": ""e2458bd3-cd75-451a-a57a-64ebe184100a"" }, ""args"":{""key"": ""Este mensaje es el argumento""}  }";

        // Este funciona y manda lo que hay dentro del argumento
        //jsonString = @"{ ""request"": ""DoAction"", ""action"": { ""id"": ""5171e4e7-8268-4d0f-9a97-07aaa5c4d65f"" }, ""args"":{""argument"": ""Este mensaje es el argumento""}  }";

        // Este funciona y manda el argumento que se le pasa, en este caso cambia el coste de la recompensa a 148
        // Streamerbot no acepta argumentos sobre como cambiar el estado activo de una recompensa
        // Para modificar cualquier recompensa, esta debe haber sido creada desde Streamerbot
        //jsonString = @"{ ""request"": ""DoAction"", ""action"": { ""id"": ""5171e4e7-8268-4d0f-9a97-07aaa5c4d65f"" }, ""args"":{""argument"": ""148""}  }";

        //jsonString = @"{ ""request"": ""getActions"" }";

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            doAction("TestConnection", "There is a stable connection between Unity and StreamerBot UDP server", 10);
        }
    }

    public void doAction(string name, string textArguments, float numberArguments)
    {
        StreamerBotAction action = actionsDict[name];

        #region Documentation in Spanish
        // Este es el formato que se debe seguir para mandar un mensaje a Streamerbot
        /// "request": "DoAction" -> Indica que se va a hacer una accion" (Esta es la unica que se permite en UDP)
        /// "action" -> Aqui ponemos toda la informacion de la accion que necesitamos
        /// "id" -> El id de la accion que queremos hacer, la cual recogemos de actionsDict
        /// "args" -> Aqui ponemos los argumentos que necesita la accion
        /// "textArgument" -> Argumento de texto. En Streamerbot para usar este argumento se debe pasar como %textArgument%
        /// numberArgument -> Argumento numerico. En Streamerbot para usar este argumento se debe pasar como %numberArgument%
        #endregion
        #region Documentation in English
        // This is the format that must be followed to send a message to Streamerbot
        /// "request": "DoAction" -> Indicates that an action is going to be done" (This is the only one allowed in UDP)
        /// "action" -> Here we put all the information of the action that we need
        /// "id" -> The id of the action we want to do, which we collect from actionsDict
        /// "args" -> Here we put the arguments that the action needs
        /// "textArgument" -> Text argument. In Streamerbot to use this argument it must be passed as %textArgument%
        /// "numberArgument" -> Numeric argument. In Streamerbot to use this argument it must be passed as %numberArgument%
        #endregion

        jsonString = @"{ ""request"": ""DoAction"", ""action"": { ""id"": """ + action.id + @""" }, 
        ""args"":{""textArgument"": """ + textArguments + @""" , ""numberArgument"": """ + numberArguments + @"""} }";

        SendEvent();
    }

    void SendEvent()
    {
        #region Documentation in Spanish
        /// 
        /// Convertimos el string a bytes para poder enviarlo por UDP
        /// Luego nos conectamos al servidor mediante la direccion de broadcast y el puerto que hemos definido
        /// Enviamos el paquete con los datos
        /// 
        #endregion

        #region Documentation in English
        ///
        /// We convert the string to bytes to be able to send it by UDP
        /// Then we connect to the server using the broadcast address and the port we have defined
        /// We send the package with the data
        /// 
        #endregion

        byte[] dataByte = Encoding.UTF8.GetBytes(jsonString);
        serverEndpoint = new IPEndPoint(IPAddress.Broadcast, port);
        udpClient.Send(dataByte, dataByte.Length, serverEndpoint);
    }

    private void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
