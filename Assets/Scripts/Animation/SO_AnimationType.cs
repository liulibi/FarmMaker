using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="so_AnimationType",menuName ="Scriptable Objects/Animation Type")]
public class SO_AnimationType : ScriptableObject
{
    public AnimationClip animationClip;//动画片段
    public AnimationName animationName;
    public CharacterPartAnimator characterPart;//动画部位
    public PartVariantColour partVariantColour;//颜色
    public PartVariantType partVariantType;//动画片段的替换部分carry片段

}
