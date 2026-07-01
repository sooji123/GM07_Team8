using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField]
    private bool _isDontDestroyOnLoad;

    [SerializeField]
    private bool _lazyInitialization;

    private static bool _applicationIsQuitting = false;

    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                return null;
            }

            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }


    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }
    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _applicationIsQuitting = true;
        }
    }

    public static bool HasInstance => _instance != null;

    protected virtual void Awake()
    {
        if (!_lazyInitialization)
        {
            if (_instance == null || _instance == this)
            {
                _instance = this as T;
                if (_isDontDestroyOnLoad)
                {
                    DontDestroyOnLoad(transform.root.gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
