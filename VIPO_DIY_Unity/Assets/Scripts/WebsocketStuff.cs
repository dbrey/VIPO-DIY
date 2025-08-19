using System;
using System.Collections;
using System.Collections.Generic;
using WebSocketSharp; // You'll need to import this library

//using WebSocketSharp;
using UnityEngine;
using StreamerBotUDP;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using UnityEditor.PackageManager;


public class WebsocketStuff : MonoBehaviour
{
    public string serverAddress = "ws://localhost:8080"; // The address of your WebSocket server
    public string authToken = "nottoken"; // Your authentication token

    private WebSocket ws;

    #region Threading Stuff
    private static readonly ConcurrentQueue<StreamerBotEventData>? _events = new(); // Cola de eventos

    #endregion

    #region Delegate Stuff
    public delegate void StreamerBotEvent(StreamerBotEventData eventData);
    private Dictionary<string, StreamerBotEvent> _eventHandlers = new(); // Diccionario para almacenar nombres de eventos y su acción asociada.

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
            _eventHandlers[eventType] += action;
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
            handler?.Invoke(eventData);
        else
            Debug.LogWarning($"StreamerBot sent event type \"{eventData.Event}\" but no matching action is registered for this event");
    }

    #endregion

    void ConnectWebSocket()
    {
        ws = new WebSocket(serverAddress);

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket Connected!");

            // In order to work, after connecting we need to authenticate with the server. Else, any other event we send from Streamerbot
            // won't be even received in Unity. To do that, we need to send a request (most likely with a token, but not entirely sure) for
            // Streamerbot to authenticate us. This probably can be done by sending a JSON object with the request type and the authentication token.



            // We prepare the authentication request
            var authRequest = new
            {
                request = "Authenticate",
                id = Guid.NewGuid().ToString(), // Do we even need an ID?
                authentication = "nottoken"
            };

            // Serialize the object to a JSON string
            string jsonRequest = JsonUtility.ToJson(authRequest);

            // Send the JSON string to the server
            ws.Send(jsonRequest);
            Debug.Log("Sent authentication request.");
        };

        // This works and can read events from streamerbot
        // There might be a chance that with websockets we are forced to do an authentication each time? Not really sure
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Received message: " + e.Data);

            
            // Check if the message is valid JSON
            if (IsValidJson(e.Data))
            {
                // If you're using Unity's JsonUtility (requires a data class):
                // MyDataClass receivedData = JsonUtility.FromJson<MyDataClass>(e.Data);
                StreamerBotEventData newEvent = JsonUtility.FromJson<StreamerBotEventData>(e.Data);

                // Add the new event to our events queue to be processed on the main thread.
                if (newEvent != null)
                {
                    _events?.Enqueue(newEvent);
                }
            }
            else
            {
                Debug.LogWarning("Received message is not valid JSON.");
            }
        };

        ws.OnError += (sender, e) =>
        {
            Debug.LogError("WebSocket Error: " + e.Message + " 😢");
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket Closed. Code: " + e.Code + ", Reason: " + e.Reason + " 🚪");
        };

        ws.Connect();
    }

    // Basic check for JSON validity (you might want a more robust one)
    private bool IsValidJson(string str)
    {
        if (string.IsNullOrWhiteSpace(str)) return false;
        str = str.Trim();
        return (str.StartsWith("{") && str.EndsWith("}")) || // For objects
               (str.StartsWith("[") && str.EndsWith("]"));   // For arrays
    }


    private void Update()
    {
        // Make sure the events queue is not null.
        if (_events == null) return;

        // Send any events that have been queued up to be processed.
        while (_events.TryDequeue(out StreamerBotEventData? newEvent))
        {
            if (newEvent != null)
            {
                ProcessEvents(newEvent);
            }
        }

    }

    /// <summary>
    /// Initialises the UDP receiver thread and delegate lists.
    /// </summary>
    private void Init()
    {
        _eventHandlers = new Dictionary<string, StreamerBotEvent>();
        InitialiseStreamerBotEvents();
    }

    /// <summary>
    /// Called at the end of Init(), this function is intended to house the registration
    /// of StreamerBot events and their associated action.
    /// </summary>
    protected virtual void InitialiseStreamerBotEvents()
    {
        // Example:
        //RegisterEvent("Test", StreamerBotTest);
    }

    #region Automatic stuff
    public void Reset()
    {
        // ????? Como resetear??
    }

    private void OnEnable()
    {
        ConnectWebSocket();
        Init();
    }
    private void OnDisable()
    {
        if (ws != null)
        {
            ws.Close();
        }
    }
    void OnApplicationQuit()
    {
        if (ws != null)
        {
            ws.Close();
        }
    }
    #endregion


}
