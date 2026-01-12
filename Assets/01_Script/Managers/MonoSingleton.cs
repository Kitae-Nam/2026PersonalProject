using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static object lockobject = new object();
    private static T instance;
    private static bool isQuitting = false;

    public static T Instance
    {
        get
        {
            lock(lockobject)
            {
                if (isQuitting)
                {
                    return null;
                }
                
                if(instance == null)
                {
                    instance = GameObject.Instantiate(Resources.Load<T>("MonoSingleton" + typeof(T).Name));
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
    }
    private void OnDisable()
    {
        isQuitting = false;
        instance = null;
    }
}
