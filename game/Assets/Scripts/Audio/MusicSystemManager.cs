using UnityEngine;

public class MusicSystemManager : MonoBehaviour
{
    public static MusicSystemManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(this);
    }
}
