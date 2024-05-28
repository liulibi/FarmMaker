using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTest : MonoBehaviour
{
    //Player动画参数编号
    [Header("移动参数")]
    public  float xInput;
    public  float yInput;
    public  bool isWalking;
    public  bool isRunning;
    public  bool isIdle;
    public  bool isCarrying;

    [Header("使用工具参数")]
    public  ToolEffect toolEffect;
    public  bool isUsingToolRight;
    public  bool isUsingToolLeft;
    public  bool isUsingToolUp;
    public  bool isUsingToolDown;

    [Header("工具举起动作参数")]
    public  bool isLiftingToolRight;
    public  bool isLiftingToolLeft;
    public  bool isLiftingToolUp;
    public  bool isLiftingToolDown;

    [Header("挥舞工具参数")]
    public  bool isSwingingToolRight;
    public  bool isSwingingToolLeft;
    public  bool isSwingingToolUp;
    public  bool isSwingingToolDown;

    [Header("拾取物品参数")]
    public  bool isPickingRight;
    public  bool isPickingLeft;
    public  bool isPickingUp;
    public  bool isPickingDown;

    [Header("玩家参数")]
    public  bool idleUp;
    public  bool idleDown;
    public  bool idleLeft;
    public  bool idleRight;

    private void Update()
    {
        EventHander.CallMovementEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying,
            toolEffect,
            isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
            isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
            isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
            isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
            idleRight, idleLeft, idleUp, idleDown);
    }
}
