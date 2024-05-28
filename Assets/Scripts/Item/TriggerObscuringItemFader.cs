using UnityEngine;

public class TriggerObscuringItemFader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)//淡出
    {
        //获取玩家碰撞到的碰撞体，并存入其所有的退色组件
        ObscurantItemFader[] obscurantItemFaders = collision.gameObject.GetComponentsInChildren<ObscurantItemFader>();

        if (obscurantItemFaders.Length > 0)//判断是否可以褪色，如果存在褪色组件那么就开始操作
        {
            for(int i = 0; i < obscurantItemFaders.Length; i++)
            {
                obscurantItemFaders[i].FadeOut();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)//淡入
    {
        //获取玩家碰撞到的碰撞体，并存入其所有的退色组件
        ObscurantItemFader[] obscurantItemFaders = collision.gameObject.GetComponentsInChildren<ObscurantItemFader>();

        if (obscurantItemFaders.Length > 0)
        {
            for(int i = 0; i < obscurantItemFaders.Length; i++)
            {
                obscurantItemFaders[i].FadeIn();
            }
        }
    }
}
