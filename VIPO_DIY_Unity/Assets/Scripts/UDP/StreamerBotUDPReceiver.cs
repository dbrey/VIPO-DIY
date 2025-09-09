using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Collections.Concurrent;
#nullable enable

#region Documentacion en Español
///
/// No tocar esto. Es el script que se encarga de recibir los eventos de StreamerBot
///
#endregion

#region Documentation in English
///
/// Do not touch this. This is the script that is in charge of receiving the events from StreamerBot
/// 
#endregion

namespace StreamerBotUDP
{

    public class StreamerBotUDPReceiver : MonoBehaviour
    {

        [Header("Connection")]
        [Tooltip("The port that StreamerBot is sending the event over. This is set in the Action dialogue box for each action.")]
        [SerializeField] private int _port = 5069;

        #region Threading Stuff

        private Thread? _receiveThread;
        private UdpClient? _client;
        private CancellationTokenSource? _cancellationTokenSource;
        private static readonly ConcurrentQueue<StreamerBotEventData>? _events = new();

        #endregion

        #region Delegate Stuff
        public delegate void StreamerBotEvent(StreamerBotEventData eventData);
        private Dictionary<string, StreamerBotEvent> _eventHandlers = new();

        /// <summary>
        /// Registers a new StreamerBotEvent.
        /// </summary>
        /// <param name="eventType">The name of the event. Must exactly match the Event value passed in from StreamerBot.</param>
        /// <param name="action">The function to be called when this event is received.</param>
        protected void RegisterEvent(string eventType, StreamerBotEvent action)
        {

            // If we haven't already registered this event type, set it to this action.
            if (!_eventHandlers.ContainsKey(eventType))
            {
                _eventHandlers[eventType] = action;
                // If we have registered it, add the action to the event type.
            }
            else
            {
                _eventHandlers[eventType] += action;
            }

        }

        /// <summary>
        /// Checks to see if we have a registered action for the given StreamerBotEventData and runs that action
        /// if we do.
        /// </summary>
        /// <param name="eventData">The StreamerBotEventData received from StreamerBot.</param>
        private void ProcessEvents(StreamerBotEventData eventData)
        {

            if (eventData == null || eventData.Event == null)
            {
                Debug.LogWarning("Attempted to process a null event.");
                return;
            }

            // If we have a registered action for this event, run that function. Else log a warning.
            if (_eventHandlers.TryGetValue(eventData.Event, out StreamerBotEvent? handler))
            {
                handler?.Invoke(eventData);
            }
            else
            {
                Debug.LogWarning($"StreamerBot sent event type \"{eventData.Event}\" but no matching action is registered for this event");
            }
        }

        #endregion

        /// Inicializamos el hilo receptor UDP y las listas de delegados.
        /// Initialises the UDP receiver thread and delegate lists.
        private void Init()
        {

            Debug.Log($"Attempting to initialise StreamerBot UDP Receiver: 127.0.0.1:{_port}");

            // Nos aseguramos de que no hemos empezado ya el hilo.
            // We make sure we haven't already started the thread.
            if (_receiveThread == null)
            {
                // Preparamos el hilo y lo iniciamos
                // Setup the thread and start it running.
                _cancellationTokenSource = new();
                CancellationToken token = _cancellationTokenSource.Token;
                _receiveThread = new Thread(() => ReceiveData(token));
                _receiveThread.IsBackground = true;
                _receiveThread.Start();
            }
            else
            {
                Debug.LogWarning("Attempted to start StreamerBot UDP Receiver thread but thread was already running.");
            }

            _eventHandlers = new Dictionary<string, StreamerBotEvent>();
            InitialiseStreamerBotEvents();

        }


        /// Comprobamos si tenemos un hilo o cliente en marcha y los abortamos/cerramos.
        /// Checks to see if we have a thread or client running and aborts/closes them.
        private void CloseConnection()
        {
            // Nos aseguramos de que el receptor no sea nulo
            // Make sure the receiver thread is not null.
            if (_receiveThread != null)
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }

            // Establecemos el hilo a nulo para que pueda ser reiniciado si es necesario.
            // Set receiver thread to null so it can be reinitialised if needed.
            _receiveThread = null;
            _client?.Close();

        }

        /// Cerramos la conexion actual (si hay una) e inicializamos una nueva.
        /// Closes the current connection (if there is one) and initialises a new one.
        public void Reset()
        {
            CloseConnection();
            Init();
        }

        /// Llamamos a Init al final, esta funcion esta pensada para guardar los registros
        /// de los eventos de StreamerBot y sus acciones asociadas.

        /// Called at the end of Init(), this function is intended to house the registration
        /// of StreamerBot events and their associated action.
        protected virtual void InitialiseStreamerBotEvents()
        {

            // Example:
            //RegisterEvent("Test", StreamerBotTest);

        }

        /// Chequea connstantemente la informacion del puerto UDP. Diseñado para correr en un hilo separado.
        /// NO LLAMAR DESDE EL HILO PRINCIPAL!

        /// Checks continously for information from UDP port. Designed to run on a separate thread.
        /// DO NOT CALL FROM MAIN THREAD!
        private void ReceiveData(CancellationToken token)
        {

            Debug.Log($"StreamerBot UDP Receiver thread started for 127.0.0.1:{_port}");

            using (_client = new UdpClient(_port))
            {
                // Iniciamos el bucle del receptor UDP.
                // Begin UDP Receiver loop.
                while (!token.IsCancellationRequested)
                {
                    // Intentamos recibir datos JSON y empaquetarlos en una clase StreamerBotEventData. Si tiene exito,
                    // enviamos los datos resultantes a TryEvent para que se usen.

                    // Try to receive JSON data and packaged into a StreamerBotEventData class. If successful,
                    // send the resulting data to TryEvent to be used.
                    try
                    {
                        Debug.Log("Waiting for UDP data...");

                        // Conseguimos la informacion JSON del mensaje UDP.
                        // Get the JSON information from the UDP message.
                        IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                        byte[] data = _client.Receive(ref anyIP);
                        string receivedData = Encoding.UTF8.GetString(data);

                        // Serializamos los datos JSON en una clase StreamerBotEventData.
                        // Serialize the JSON data into a StreamerBotEventData class.
                        StreamerBotEventData newEvent = JsonUtility.FromJson<StreamerBotEventData>(receivedData);

                        // Añadimos el nuevo evento a nuestra cola de eventos para ser procesados en el hilo principal.
                        // Add the new event to our events queue to be processed on the main thread.
                        if (newEvent != null)
                        {
                            _events?.Enqueue(newEvent);
                        }

                    }
                    catch (SocketException ex) when (ex.SocketErrorCode == SocketError.Interrupted)
                    {
                        Debug.LogWarning("StreamerBot UDP Receiver thread was interrupted.");
                    }
                    catch (Exception err)
                    {
                        Debug.LogWarning(err.ToString());
                    }
                }
            }
            Debug.Log("StreamerBot UDP Receiver thread has stopped.");
        }

        private void Update()
        {
            // Nos aseguramos que la cola de eventos no sea nula.
            // Make sure the events queue is not null.
            if (_events == null) return;

            // Enviamos cualquier evento que haya sido puesto en cola para ser procesado.
            // Send any events that have been queued up to be processed.
            while (_events.TryDequeue(out StreamerBotEventData? newEvent))
            {
                if (newEvent != null)
                {
                    ProcessEvents(newEvent);
                }
            }

        }

        #region Spanish Documentation
        // Estas funciones se ejecutan automaticamente cuando el GameObject padre es activado/desactivado o si
        // la aplicacion se cierra. Llamar a Init() y CloseConnection() desde aqui asegura que si tu
        // objeto StreamerBotManager es desactivado/activado, tiene el mismo comportamiento que resetear
        // la conexion UDP/hilo receptor.
        #endregion
        #region English Documentation
        // These functions run automatically when the parent GameObject is enabled/disabled or the
        // application quits. Calling Init() and CloseConnection() from here ensures that if your
        // StreamerBotManager object is disabled/activated, it has the same behaviour as resetting
        // the UDP connection/receiver thread.
        #endregion

        #region Automatic Initialisation/Connection Closing

        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            CloseConnection();
        }

        private void OnApplicationQuit()
        {
            CloseConnection();
        }

        #endregion

    }


    #region Spanish Documentation
    /// Contiene los datos pasados desde StreamerBot. Los datos pueden incluir cualquiera o todos los campos
    /// de esta clase. Por ejemplo, enviar un evento de Bit Cheer incluiria el Evento, Usuario, y
    /// Cantidad (y posiblemente Mensaje), mientras que enviar un evento de pausa publicitaria solo necesitaria un Evento
    #endregion
    #region English Documentation
    /// Contains the data passed in from StreamerBot. The data can include any or all of the fields
    /// in this class. For example, sending a Bit Cheer event would include the Event, User, and
    /// Amount (and possibly Message), whereas sending an ad-break event would only need an Event
    #endregion

    [System.Serializable]
    public class StreamerBotEventData
    {
        /// El tipo de evento. Puede ser cualquier cosa que desees pero la cadena pasada desde StreamerBot
        /// debe coincidir exactamente con lo que estas haciendo en Unity.

        /// The type of event. Can be anything you wish but the string passed from StreamerBot
        /// must match exactly with whatever you are doing in Unity.
        public string Event;

        #region MAIN USER DATA

        /// El usuario debe estar asociado con el evento. Por ejemplo, si el evento fue una suscripcion,
        /// este seria el nombre del suscriptor

        /// The username associated with the event. For example, if the event was a subscription,
        /// this would be the username of the subscriber.
        public string UserName;

        /// La imagen de perfil del usuario asociado con el evento. Por ejemplo, si el evento fue un follow,
        /// este seria la imagen de perfil del usuario que siguio.

        /// The profile image of the user associated with the event. For example, if the event was a follow,
        /// this would be the profile image of the user who followed.
        public string UserProfileImage;

        /// Si el usuario es un VIP
        /// If the user is a VIP
        public bool isVip;

        /// Si el usuario es un moderador
        /// If the user is a moderator
        public bool isMod;

        /// Si el usuario es suscriptor
        /// If the user is a subscriber
        public bool isSuscribed;

        /// El numero de dias que el usuario ha estado siguiendo el canal
        /// The number of days the user has been following the channel
        public int followAgeDays;

        /// Cuantos meses el usuario ha estado suscrito
        /// How many months the user has been suscribed
        public int monthsSuscribed;

        /// El nivel actual de la suscripcion
        /// The current tier of the subscription
        public int tier;

        #endregion

        /// Esta informacion puede tener valor si el evento es un regalo de suscripcion.
        /// This information may have value if the event is a subscription gift.
        #region Subscription Gift Data

        /// El usuario asociado al evento. Por ejemplo, si el evento fue un regalo de suscripcion,
        /// este seria el nombre del suscriptor

        /// The username associated with the event. For example, if the event was a subscription,
        /// this would be the username of the subscriber.
        public string UserName2;
        public bool isAnonymous;

        #endregion

        /// Un mensaje asociado al evento. Por ejemplo, si quisieras mostrar un mensage desde este evento,
        /// esta cadena contendria el mensaje.

        /// A message associated with the event. For example, if you wanted to show a message from this event,
        /// this string would contain the message.
        public string Message;

        /// Una cantidad numerica asociada con este evento. Por ejemplo, el numero de bits enviados o suscripciones regalados.
        /// A numerical amount associated with this event. For example, the number of bits cheered or subscriptions gifted.
        public int Amount;

        /// El constructor establece los valores por defecto para cada campo, que es lo que valdra si
        /// la carga de UDP desde StreamerBot no contiene un campo en particular.

        /// The constructor establishes the default values for each field, which is what the value will be if
        /// the UDP payload from StreamerBot does not contain a particular field.
        public StreamerBotEventData()
        {
            Event = string.Empty;
            
            UserName = string.Empty;
            UserProfileImage = string.Empty;
            isVip = false;
            isMod = false;
            isSuscribed = false;
            followAgeDays = 0;
            monthsSuscribed = 0;
            tier = 0;

            UserName2 = string.Empty;
            isAnonymous = false;
            Message = string.Empty;
            Amount = 0;
        }
    }
}