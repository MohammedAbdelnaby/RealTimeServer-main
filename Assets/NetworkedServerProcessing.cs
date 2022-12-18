using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class NetworkedServerProcessing
{
    #region Send and Receive Data Functions
    static public void ReceivedMessageFromClient(string msg, int id)
    {
        Debug.Log("msg received = " + msg + ".  connection id = " + id);

        string[] csv = msg.Split(',');

        switch (csv[0])
        {
            case "Host":
                networkedServer.HostID = id;
                SendMessageToAllPlayers("Player");
                Debug.Log(id);
                break;
            case "Ball":
                SendMessageToAllPlayers(msg);
                SaveBallon(msg);
                break;
            case "Destroy":
                SendMessageToAll(msg);
                DestroyBallon(int.Parse(csv[1]));
                break;
            default:
                break;
        }
    }
    static public void SendMessageToClient(string msg, int clientConnectionID)
    {
        networkedServer.SendMessageToClient(msg, clientConnectionID);
    }

    static public void SendMessageToAllPlayers(string msg)
    {
        for (int i = 0; i < networkedServer.AllPlayerIds.Count; i++)
        {
            if (networkedServer.AllPlayerIds[i] != networkedServer.HostID)
                SendMessageToClient(msg, networkedServer.AllPlayerIds[i]);
        }
    }

    static void DestroyBallon(int i)
    {
        networkedServer.BallonHistory[i] = Vector2.zero;
    }

    static public void SaveBallon(string msg)
    {
        string[] csv = msg.Split(',');
        networkedServer.BallonHistory.Add(StringToVector2(csv[1], csv[2]));
    }

    static public Vector2 StringToVector2(string X, string Y)
    {
        Vector2 temp;
        temp = new Vector2(float.Parse(X), float.Parse(Y));
        return temp;
    }

    static public void SendMessageToAll(string msg)
    {
        for (int i = 0; i < networkedServer.AllPlayerIds.Count; i++)
        {
            SendMessageToClient(msg, networkedServer.AllPlayerIds[i]);
        }
    }

    static public void SendGameState(int id)
    {
        networkedServer.SendMessageToClient("History," + ListToString(), id);
    }

    static public string ListToString()
    {
        string temp = "";
        foreach (Vector2 pos in networkedServer.BallonHistory)
        {
            temp += pos.x + ";" + pos.y + ":";
        }
        return temp;
    }

    #endregion

    #region Connection Events

    static public void ConnectionEvent(int clientConnectionID)
    {
        Debug.Log("New Connection, ID == " + clientConnectionID);
        networkedServer.AllPlayerIds.Add(clientConnectionID);
        if (networkedServer.HostID != 0)
        {
            CantBeHost(clientConnectionID);
            SendGameState(clientConnectionID);
        }
    }
    static public void DisconnectionEvent(int clientConnectionID)
    {
        Debug.Log("New Disconnection, ID == " + clientConnectionID);
    }

    static public void CantBeHost(int id)
    {
        SendMessageToClient("Player", id);
    }

    #endregion

    #region Setup
    static NetworkedServer networkedServer;
    static GameLogic gameLogic;

    static public void SetNetworkedServer(NetworkedServer NetworkedServer)
    {
        networkedServer = NetworkedServer;
    }
    static public NetworkedServer GetNetworkedServer()
    {
        return networkedServer;
    }
    static public void SetGameLogic(GameLogic GameLogic)
    {
        gameLogic = GameLogic;
    }

    #endregion
}

#region Protocol Signifiers
static public class ClientToServerSignifiers
{
    public const int asd = 1;
}

static public class ServerToClientSignifiers
{
    public const int asd = 1;
}

#endregion

