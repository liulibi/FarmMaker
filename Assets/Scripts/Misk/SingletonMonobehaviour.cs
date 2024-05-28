using UnityEngine;
//确保其单一性
public abstract class SingletonMonobehaviour<T> : MonoBehaviour where T:MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            return instance;
        }
    }
    protected virtual void Awake()
    {
        if (instance == null)//如果实例不存在那么将启动实列让其等于这个对象
        {
            instance = this as T;
        }
        else//如果实例已经存在那么销毁
        {
            Destroy(gameObject);
        }
    }
}
