using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnimationProgramEnterControl : MonoBehaviour
{
    private Animator animator;

    //初始化
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    private void OnEnable()//game object可以使用时
    {
        EventHander.MovementEvent += SetAnimationParameters;//将运动参数传递给EventHander的运动委托函数

    }


    private void OnDisable()//game object 不可以使用时
    {
        EventHander.MovementEvent -= SetAnimationParameters;//将运动参数取消传递给EventHander的运动委托函数
    }


    private void SetAnimationParameters(float inputx, float inputy, bool isWalking, bool isRunning, bool isIdle, bool isCarrying,
    ToolEffect toolEffect,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolup, bool isLiftToolDown,
    bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
    bool idleRight, bool idleLeft, bool idleUp, bool idleDown)//设置animation参数
    {
        //运动参数
        animator.SetFloat(Settings.xInput, inputx);
        animator.SetFloat(Settings.yInput, inputy);
        animator.SetBool(Settings.isWalking, isWalking);
        animator.SetBool(Settings.isRunning, isRunning);

        animator.SetInteger(Settings.toolEffect, (int)toolEffect);

        //使用工具参数设置
        if (isUsingToolRight)
            animator.SetTrigger(Settings.isUsingToolRight);
        if (isUsingToolLeft)
            animator.SetTrigger(Settings.isUsingToolLeft);
        if (isUsingToolUp)
            animator.SetTrigger(Settings.isUsingToolUp);
        if (isUsingToolDown)
            animator.SetTrigger(Settings.isUsingToolDown);

        //工具举起参数
        if (isLiftingToolRight)
            animator.SetTrigger(Settings.isLiftingToolRight);
        if (isLiftingToolLeft)
            animator.SetTrigger(Settings.isLiftingToolLeft);
        if (isLiftingToolup)
            animator.SetTrigger(Settings.isLiftingToolUp);
        if (isLiftToolDown)
            animator.SetTrigger(Settings.isLiftingToolDown);

        //拾取参数
        if (isPickingRight)
            animator.SetTrigger(Settings.isPickingRight);
        if (isPickingLeft)
            animator.SetTrigger(Settings.isPickingLeft);
        if (isPickingUp)
            animator.SetTrigger(Settings.isPickingUp);
        if (isPickingDown)
            animator.SetTrigger(Settings.isPickingDown);

        //挥舞参数
        if (isSwingingToolRight)
            animator.SetTrigger(Settings.isSwingingToolRight);
        if (isSwingingToolLeft)
            animator.SetTrigger(Settings.isSwingingToolLeft);
        if (isSwingingToolUp)
            animator.SetTrigger(Settings.isSwingingToolUp);
        if (isSwingingToolDown)
            animator.SetTrigger(Settings.isSwingingToolDown);

        //Player状态参数
        if (idleRight)
            animator.SetTrigger(Settings.idleRight);
        if (idleLeft)
            animator.SetTrigger(Settings.idleLeft);
        if (idleUp)
            animator.SetTrigger(Settings.idleUp);
        if (idleDown)
            animator.SetTrigger(Settings.idleDown);


    }

    private void AnimationEventPlayFootStepSound()//移动事件脚步
    {
        AudioManager.Instance.PlaySound(SoundName.effectFootstepHardGround);
    }
}
