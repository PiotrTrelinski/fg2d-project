using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttackProperties
{
    public float damage;
    public float hitStun;
    public float blockStun;
    public float pushBack;
    public BlockType blockType;
    public float crossFade;
    public AttackProperties(float damage, float hitStun, float blockStun, float pushBack, BlockType blockType, float crossFade)
    {
        this.damage = damage;
        this.hitStun = hitStun;
        this.blockStun = blockStun;
        this.pushBack = pushBack;
        this.blockType = blockType;
        this.crossFade = crossFade;
    }
}

public enum BlockType
{
    Standing,
    Crouching,
    Either
}

public class AttackData {
    public static Dictionary<string, AttackProperties> AttackProperties = new Dictionary<string, AttackProperties>
    {
        {"StandingLeftPunch", new AttackProperties(10, 26, 25, 3, BlockType.Standing, 0)},
        {"StandingRightPunch", new AttackProperties(15, 33, 23, 2, BlockType.Standing, 0)},
        {"StandingLeftKick", new AttackProperties(14, 26, 18, 1, BlockType.Crouching, 0)},
        {"StandingRightKick", new AttackProperties(16, 35, 20, 1, BlockType.Standing, 0)},
        {"CrouchingLeftPunch", new AttackProperties(6, 18, 17, 1, BlockType.Either, 0)},
        {"CrouchingRightPunch", new AttackProperties(18, 42, 23, 2, BlockType.Standing, 0.3f)},
        {"CrouchingLeftKick", new AttackProperties(14, 55, 52, 0, BlockType.Crouching, 0)},
        {"CrouchingRightKick", new AttackProperties(13, 26, 26, 3, BlockType.Standing, 0.3f)},
        {"DashingLeftPunch", new AttackProperties(10, 21, 25, 1, BlockType.Standing, 0.01f)},
        {"DashingRightPunch", new AttackProperties(15, 27, 16, 3, BlockType.Standing, 0.01f)},
        {"DashingLeftKick", new AttackProperties(8, 14, 22, 0, BlockType.Crouching, 0.01f)},
        {"DashingRightKick", new AttackProperties(20, 44, 22, 1, BlockType.Standing, 0.3f)},
        {"JumpingLeftPunch", new AttackProperties(22, 33, 20, 3, BlockType.Standing, 0)},
        {"JumpingRightPunch", new AttackProperties(12, 22, 23, 5, BlockType.Standing, 0)},
        {"JumpingLeftKick", new AttackProperties(25, 50, 20, 3, BlockType.Standing, 0)},
        {"JumpingRightKick", new AttackProperties(15, 40, 19, 3, BlockType.Standing, 0)},
        {"BackdashingLeftPunch", new AttackProperties(18, 45, 26, 1, BlockType.Standing, 0.01f)},
        {"BackdashingLeftKick", new AttackProperties(20, 39, 22, 2, BlockType.Standing, 0.01f)},
        {"RunningRightPunch", new AttackProperties(18, 50, 38, 1, BlockType.Standing, 0.1f)},
        {"RunningLeftKick", new AttackProperties(20, 50, 40, 2, BlockType.Standing, 0.1f)},
        {"RunningRightKick", new AttackProperties(30, 51, 52, 0, BlockType.Standing, 0.1f)}
    };
}
