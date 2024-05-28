using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemNudge : MonoBehaviour
{
    private WaitForSeconds pause;
    private bool isAnimation = false;


    private void Awake()
    {
        pause = new WaitForSeconds(0.04f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAnimation == false)
        {
            if (gameObject.transform.position.x < collision.gameObject.transform.position.x)
            {
                StartCoroutine(RotateAntiClock());
            }
            else
            {
                StartCoroutine(RotateClock());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isAnimation == false)
        {
            if (gameObject.transform.position.x > collision.gameObject.transform.position.x)
            {
                StartCoroutine(RotateAntiClock());
            }
            else
            {
                StartCoroutine(RotateClock());
            }

            //play sound
            if (collision.gameObject.tag == "Player")
            {
                AudioManager.Instance.PlaySound(SoundName.effectRustle);
            }
        }
    }

    //逆时针旋转
    private IEnumerator RotateAntiClock()
    {
        isAnimation = true;

        for (int i = 0; i < 4; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);//逆时针旋转2度*4

            yield return pause;//有短暂停顿
        }

        for(int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);//顺时针旋转2度*5

            yield return pause;
        }

        gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);//逆时针旋转2度

        yield return pause;//停顿

        isAnimation = false;
    }

    //顺时针旋转
    private IEnumerator RotateClock()
    {
        isAnimation = true;

        for (int i = 0; i < 4; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);//顺时针旋转2度*4

            yield return pause;//有短暂停顿
        }

        for (int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);//逆时针旋转2度*5

            yield return pause;
        }

        gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);//顺时针旋转2度

        yield return pause;//停顿

        isAnimation = false;
    }


}
