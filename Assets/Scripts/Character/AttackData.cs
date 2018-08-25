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
    public AttackPropertiesStructure(float damage, float hitStun, float blockStun, float pushBack, BlockType blockType)
    {
        this.damage = damage;
        this.hitStun = hitStun;
        this.blockStun = blockStun;
        this.pushBack = pushBack;
        this.blockType = blockType;
    }
}

public enum BlockType
{
    Standing,
    Crouching,
    Either
}

public class AttackData {

    private static AttackData instance;
    public static AttackData Instance
    {
        get
        {
            if (instance == null)
                instance = new AttackData();
            return instance;
        }
    }
    private Dictionary<string, AttackPropertiesStructure> attackProperties;
    public Dictionary<string, AttackPropertiesStructure> AttackProperties
    {
        get
        {
            return attackProperties;
        }
    }
    public AttackData()
    { 
        attackProperties = new Dictionary<string, AttackPropertiesStructure>();
        attackProperties.Add("StandingLeftPunch", new AttackPropertiesStructure(10, 26, 25, 3, BlockType.Standing));
        attackProperties.Add("StandingRightPunch", new AttackPropertiesStructure(15, 30, 23, 2, BlockType.Standing));
        attackProperties.Add("StandingLeftKick", new AttackPropertiesStructure(14, 26, 18, 1, BlockType.Crouching));
        attackProperties.Add("StandingRightKick", new AttackPropertiesStructure(16, 35, 20, 1, BlockType.Standing));
        attackProperties.Add("CrouchingLeftPunch", new AttackPropertiesStructure(6, 18, 17, 1, BlockType.Either));
        attackProperties.Add("CrouchingRightPunch", new AttackPropertiesStructure(18, 42, 23, 2, BlockType.Standing));
        attackProperties.Add("CrouchingLeftKick", new AttackPropertiesStructure(14, 55, 52, 0, BlockType.Crouching));
        attackProperties.Add("CrouchingRightKick", new AttackPropertiesStructure(13, 26, 26, 3, BlockType.Standing));
        attackProperties.Add("DashingLeftPunch", new AttackPropertiesStructure(10, 21, 25, 1, BlockType.Standing));
        attackProperties.Add("DashingRightPunch", new AttackPropertiesStructure(15, 27, 16, 3, BlockType.Standing));
        attackProperties.Add("DashingLeftKick", new AttackPropertiesStructure(8, 14, 22, 0, BlockType.Crouching));
        attackProperties.Add("DashingRightKick", new AttackPropertiesStructure(20, 44, 22, 1, BlockType.Standing));
        attackProperties.Add("JumpingLeftPunch", new AttackPropertiesStructure(22, 33, 20, 3, BlockType.Standing));
        attackProperties.Add("JumpingRightPunch", new AttackPropertiesStructure(12, 22, 23, 5, BlockType.Standing));
        attackProperties.Add("JumpingLeftKick", new AttackPropertiesStructure(25, 50, 20, 3, BlockType.Standing));
        attackProperties.Add("JumpingRightKick", new AttackPropertiesStructure(15, 40, 19, 3, BlockType.Standing));
        attackProperties.Add("BackdashingLeftPunch", new AttackPropertiesStructure(18, 45, 26, 1, BlockType.Standing));
        attackProperties.Add("BackdashingLeftKick", new AttackPropertiesStructure(20, 39, 22, 2, BlockType.Standing));
        attackProperties.Add("RunningRightPunch", new AttackPropertiesStructure(18, 50, 38, 1, BlockType.Standing));
        attackProperties.Add("RunningLeftKick", new AttackPropertiesStructure(20, 50, 40, 2, BlockType.Standing));
        attackProperties.Add("RunningRightKick", new AttackPropertiesStructure(30, 51, 52, 0, BlockType.Standing));
    }
	
}
