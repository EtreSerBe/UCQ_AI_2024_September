using UnityEngine;

public class BossEnemy : BaseEnemy
{

    public int NumberOfBasicAttacksBeforeExit = 3;

    public float BasicAttackRange = 5.0f;



    // Cuánto tiempo debe durar haciendo el ataque de área en el Subestado de Ataque de área de MeleeState.
    public float AreaAttackTime = 2.0f;

    public float AreaAttackBuildupTime = 1.0f;

    public float AreaAttackCooldownTime = 1.0f;

    public float AreaAttackRange = 10.0f;

    public float DashAttackTime = 2.0f;

    public float DashBuildupTime = 0.3f;

    public float DashCooldownTime = 1.0f;

    public bool EnableDebug = true;

    public override void Awake()
    {
        base.Awake();
    }

}
