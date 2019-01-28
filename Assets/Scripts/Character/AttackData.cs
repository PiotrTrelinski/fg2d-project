using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttackPropertiesStructure
{
    public float damage;
    public float hitStun;
    public float blockStun;
    public float pushBack;
    public BlockType blockType;
    public float crossFade;
    public AttackPropertiesStructure(float damage, float hitStun, float blockStun, float pushBack, BlockType blockType, float crossFade)
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
    public static Dictionary<string, AttackPropertiesStructure> AttackProperties = new Dictionary<string, AttackPropertiesStructure>
    {
        {"StandingLeftPunch", new AttackPropertiesStructure(10, 26, 25, 3, BlockType.Standing, 0)},
        {"StandingRightPunch", new AttackPropertiesStructure(15, 30, 23, 2, BlockType.Standing, 0)},
        {"StandingLeftKick", new AttackPropertiesStructure(14, 26, 18, 1, BlockType.Crouching, 0)},
        {"StandingRightKick", new AttackPropertiesStructure(16, 35, 20, 1, BlockType.Standing, 0)},
        {"CrouchingLeftPunch", new AttackPropertiesStructure(6, 18, 17, 1, BlockType.Either, 0)},
        {"CrouchingRightPunch", new AttackPropertiesStructure(18, 42, 23, 2, BlockType.Standing, 0.3f)},
        {"CrouchingLeftKick", new AttackPropertiesStructure(14, 55, 52, 0, BlockType.Crouching, 0)},
        {"CrouchingRightKick", new AttackPropertiesStructure(13, 26, 26, 3, BlockType.Standing, 0.3f)},
        {"DashingLeftPunch", new AttackPropertiesStructure(10, 21, 25, 1, BlockType.Standing, 0.01f)},
        {"DashingRightPunch", new AttackPropertiesStructure(15, 27, 16, 3, BlockType.Standing, 0.01f)},
        {"DashingLeftKick", new AttackPropertiesStructure(8, 14, 22, 0, BlockType.Crouching, 0.01f)},
        {"DashingRightKick", new AttackPropertiesStructure(20, 44, 22, 1, BlockType.Standing, 0.3f)},
        {"JumpingLeftPunch", new AttackPropertiesStructure(22, 33, 20, 3, BlockType.Standing, 0)},
        {"JumpingRightPunch", new AttackPropertiesStructure(12, 22, 23, 5, BlockType.Standing, 0)},
        {"JumpingLeftKick", new AttackPropertiesStructure(25, 50, 20, 3, BlockType.Standing, 0)},
        {"JumpingRightKick", new AttackPropertiesStructure(15, 40, 19, 3, BlockType.Standing, 0)},
        {"BackdashingLeftPunch", new AttackPropertiesStructure(18, 45, 26, 1, BlockType.Standing, 0.01f)},
        {"BackdashingLeftKick", new AttackPropertiesStructure(20, 39, 22, 2, BlockType.Standing, 0.01f)},
        {"RunningRightPunch", new AttackPropertiesStructure(18, 50, 38, 1, BlockType.Standing, 0.1f)},
        {"RunningLeftKick", new AttackPropertiesStructure(20, 50, 40, 2, BlockType.Standing, 0.1f)},
        {"RunningRightKick", new AttackPropertiesStructure(30, 51, 52, 0, BlockType.Standing, 0.1f)}
    };
}
