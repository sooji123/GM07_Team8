public enum EBGMType
{
    None,
    Title,
    Game,
}

public enum ESFXType
{
    None,
    ButtonClick,
    Build,
    UIOpen,
    UIClose,
    BatHit,
    SunFlowerHit,
    WizardHit,
    Upgrade,
    Upgrade_fail,
    Buff,
    Spike,
    MagicCircle,
    GameOver,

    Explosion,
    Explosion_Water,
    Explosion_Fire,
    Explosion_Grass,
    Explosion_Electric,

    EnemyHit_Barrier,
    EnemyHit_Star,
    EnemyHit_Shield,
    EnemyHit_Normal,
    Enemy_BossHeal,

    // 퍼즐 사운드
    Puzzle_Swap,
    Puzzle_OrbObtain,
    Puzzle_OnClick,
    Puzzle_OnMouse,
    Puzzle_Match,
    Puzzle_MatchFail,
    Puzzle_MatchSP,

    // 에너지 차지 사운드
    Energy_1Charge, // 1
    Energy_2Charge, // 2
    Energy_3Charge, // 3
    
    // 에너지 사용(스킬) 사운드
    SkillStun, // 1
    SkillBuff, // 2
    SkillMeteor, // 3


}