using UnityEngine;
using System.Collections.Generic;

public class StateSync : MonoBehaviour
{
    [Header("Sync Settings")]
    [SerializeField] private float networkDelay = 0.1f;
    [SerializeField] private int bufferSize = 300; 

    private Queue<PlayerAction> actionQueue = new Queue<PlayerAction>();

    public void RecordAction(PlayerAction action)
    {
        // Add to queue with timestamp
        actionQueue.Enqueue(action);

        // Maintain buffer size
        if (actionQueue.Count > bufferSize)
        {
            actionQueue.Dequeue();
        }
    }

    public PlayerAction GetDelayedAction()
    {
        float targetTime = Time.time - networkDelay;

        // Find the action closest to the delayed time
        PlayerAction closestAction = null;
        float closestDiff = float.MaxValue;

        foreach (PlayerAction action in actionQueue)
        {
            float diff = Mathf.Abs(action.timestamp - targetTime);
            if (diff < closestDiff)
            {
                closestDiff = diff;
                closestAction = action;
            }
        }

        return closestAction;
    }

    public List<PlayerAction> GetActionsSince(float time)
    {
        List<PlayerAction> actions = new List<PlayerAction>();

        foreach (PlayerAction action in actionQueue)
        {
            if (action.timestamp >= time)
            {
                actions.Add(action);
            }
        }

        return actions;
    }

    public void ClearActions()
    {
        actionQueue.Clear();
    }

    public int GetQueueSize()
    {
        return actionQueue.Count;
    }
}
public enum ActionType
{
    Position,
    Jump
}

[System.Serializable]
public class PlayerAction
{
    public float timestamp;
    public ActionType actionType;
    public Vector3 position;
    public float velocity;
    public int collectibleId;
}