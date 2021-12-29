using UnityEngine;

public static class AnimPool
{
    // Move
    public static readonly int RunAnim = Animator.StringToHash("Run");
    public static readonly int Walk    = Animator.StringToHash("Walk");
    public static readonly int Idle    = Animator.StringToHash("idle");
    public static readonly int Blend   = Animator.StringToHash("Blend");

    // monster
    public static readonly int attack = Animator.StringToHash("Atack");
    public static readonly int Dodge  = Animator.StringToHash("Dodge");

    // npc
    public static readonly int InCombat       = Animator.StringToHash("InCombat");
    public static readonly int DialogStr      = Animator.StringToHash("Dialog");
    public static readonly int Worker         = Animator.StringToHash("Worker");

    public static readonly int InputY         = Animator.StringToHash("InputY");
    public static readonly int InputX         = Animator.StringToHash("InputX");
    public static readonly int jump           = Animator.StringToHash("Jump");
    public static readonly int velocityStr    = Animator.StringToHash("velocity");
    public static readonly int distFromGround = Animator.StringToHash("DistFromGround");
    public static readonly int Die            = Animator.StringToHash("Die");
        
    public static readonly int Dismount       = Animator.StringToHash("Dismount");
    public static readonly int GetHit         = Animator.StringToHash("GetHit");
    public static readonly int GetHit1        = Animator.StringToHash("GetHit");

    // state names
    public const string Locomotion = nameof(Locomotion);

    public static readonly int Falling       = Animator.StringToHash("Falling");
    public static readonly int IsGrounded    = Animator.StringToHash("IsGrounded");
    public static readonly int Roll          = Animator.StringToHash("Roll");
    public static readonly int OpenDoor = Animator.StringToHash("OpenDoor");

    public static readonly int TintColor = Animator.StringToHash("_TintColor");
}

