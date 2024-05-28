using UnityEngine;

[ExecuteAlways]
/*不管是在编辑器状态还是在运行状态，该脚本始终在运行
当您希望脚本作为编辑器工具的一部分执行某些事情时，可以使用[ExecuteAlways]属性
A MonoBehaviour 要确保其中没有编写游戏逻辑，只是在编辑器模式下运行 ，否则可能会造成错误. 可以通过Application.IsPlaying来确定游戏是否在运行状态*/

public class GenerateGUID : MonoBehaviour
{
    [SerializeField]
    private string _gUID = "";

    public string GUID { get => _gUID; set => _gUID = value; }

    private void Awake()
    {
        //only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            //ensure the object has a guranteed unique id
            if (_gUID=="")
            {
                _gUID = System.Guid.NewGuid().ToString();
            }
        }
    }
}
