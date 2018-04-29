using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public MatchConfig matchConfig;

    private void Awake()
    {
        if (instance != null)
            Debug.LogError("More than one game manager!");
        else
            instance = this;
    }
    #region Player/AI Registering
    private const string AI_ID_PREFIX = "Ai ";
    private const string PLAYER_ID_PREFIX = "Player ";
    private static Dictionary<string, AI> ai_players = new Dictionary<string, AI>();
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public static void RegisterPlayer(string netId, Player player)
    {
        string playerId = PLAYER_ID_PREFIX + netId;
        players.Add(playerId, player);
        player.transform.name = playerId;
    }

    public static void RegisterAi(string netId, AI aiPlayer)
    {
        string aiId = AI_ID_PREFIX + netId;
        ai_players.Add(aiId, aiPlayer);
        aiPlayer.transform.name = aiId;
    }

    public static void DeRegistPlayer(string playId)
    {
        players.Remove(playId);
    }

    public static void DeRegisterAi(string aiId)
    {
        ai_players.Remove(aiId);
    }

    public static Player GetPlayer(string playerId)
    {
        return players[playerId];
    }

    public static AI getAI(string aiId)
    {
        return ai_players[aiId];
    }

    public static Player[] getPlayerList()
    {
        Player[] players_arr = new Player[players.Count];
        players.Values.CopyTo(players_arr, 0);
        return players_arr;
    }

    public static AI[] getAIList()
    {
        AI[] ai_arr = new AI[ai_players.Count];
        ai_players.Values.CopyTo(ai_arr, 0);
        return ai_arr;
    }
    
    /*
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200, 200, 200, 500));
        GUILayout.BeginVertical();
        foreach(string id in players.Keys)
        {
            GUILayout.Label(id + " - " + players[id].transform.name);
        }
        foreach(string id in ai_players.Keys)
        {
            GUILayout.Label(id + " - " + ai_players[id].transform.name);
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    */
    #endregion
}
