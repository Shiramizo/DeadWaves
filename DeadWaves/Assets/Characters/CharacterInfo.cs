using DeadWaves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultCharacterInfo", menuName = "Character/Information")]
public class CharacterInfo : ScriptableObject
{
    [Header("Maximum speed")]
    public float speed;
    [Header("How fast it reaches maximum speed (Use 0 to 1 on Y axis. X axis is time in seconds)")]
    public AnimationCurve accelerationCurve;
    [Header("How fast it turns 180, in seconds")]
    public float turnSpeed;
    [Header("How much the character can jump, in force (newtons I think)")]
    public float jumpForce;
    [Header("Used to define a minimum jump amount, in seconds")]
    public float minimumJumpTime;

    [Header("Write a brief description of what this character can do")]
    [TextArea(2, 20)] public string characterDescription;
}
