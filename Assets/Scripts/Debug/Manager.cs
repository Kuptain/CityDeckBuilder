using UnityEngine;

public class Manager: MonoBehaviour 
{
    public bool debugMode;

    public void SendLog(object msg)
    {
        if (debugMode)
        {
            Debug.Log(GetType()+ ": " + msg);
        }
    }
    public void SendError(object msg)
    {
        if (debugMode)
        {
            Debug.LogError(GetType() + ": " + msg);
        }
    }

    public void SendWarning(object msg)
    {
        if (debugMode)
        {
            Debug.LogWarning(GetType() + ": " + msg);
        }
    }
}
