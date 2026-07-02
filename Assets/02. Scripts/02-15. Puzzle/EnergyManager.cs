using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnergyManager : Singleton<EnergyManager>
{
    [Header("에너지 세팅")] // 칸
    public int currentEnergyLv = 0;
    public int maxEnergyLv = 3;

    [Header("에너지 게이지")]
    public int currentCost = 0;
    public int lvUpCost = 0;

    [Header("에너지별 게이지 세팅")] // 게이지
    [SerializeField] private int lv1Cost = 30;
    [SerializeField] private int lv2Cost = 60;
    [SerializeField] private int lv3Cost = 100;

    [Header("스킬별 수치 세팅")]
    [SerializeField] private float lv1Skill = 1.5f; // 몹 스턴 시간
    [SerializeField] private float lv2Skill = 0.5f; // 터렛 공속 증가치
    [SerializeField] private float lv2SkillDuration = 15.0f; // 터렛 공속 증가 지속 시간
    [SerializeField] private float lv3Skill = 50.0f; // 적 체력 비례 데미지

    [Header("매치별 게이지 수급 세팅")]
    [SerializeField] public int match3Energy = 1; // 기본 매치 시 게이지 수급량
    [SerializeField] public int match4Energy = 4; // 4개 매치 시 게이지 수급량
    [SerializeField] public int matchTLEnergy = 10; // TL 매치 시 게이지 수급량
    [SerializeField] public int match5Energy = 15; // 5개 매치 시 게이지 수급량


    public event Action<int, int, int> OnEnergyChanged; // 퍼즐을 맞췄을 때 호출
    public event Action OnEnergyNotEnough;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 씬 로드될 때마다 실행
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            currentCost = 0;
            currentEnergyLv = 0;
            lvUpCost = 0;
        }
    }
    public void AddEnergy(int amount)
    {
        if (currentEnergyLv >= maxEnergyLv)
        {
            Debug.Log($"이미 에너지 max");
            return;
            
            // 오버차지 메서드 (만약 들어간다면)
        }

        currentCost += amount;
        UpdateLv();

        while (currentCost >= lvUpCost && currentEnergyLv < maxEnergyLv)
        {
            currentCost -= lvUpCost;

            currentEnergyLv++;

            Debug.Log($"에너지 레벨업 [ Lv.{currentEnergyLv} ]");

            if (SoundManager.Instance != null)
            {
                switch (currentEnergyLv)
                {
                    case 1:
                        SoundManager.Instance.PlayeSFX(ESFXType.Energy_1Charge);
                        break;
                    case 2:
                        SoundManager.Instance.PlayeSFX(ESFXType.Energy_2Charge);
                        break;
                    case 3:
                        SoundManager.Instance.PlayeSFX(ESFXType.Energy_3Charge);
                        break;
                }
            }

            UpdateLv();
        }

        if (currentEnergyLv >= maxEnergyLv)
        {
            currentEnergyLv = maxEnergyLv;
            currentCost = lvUpCost;
        }

        Debug.Log($"현재 게이지 [ {currentCost} / {lvUpCost} ]");

        OnEnergyChanged?.Invoke(currentEnergyLv, currentCost, lvUpCost);
        // UI 신호 쏘기 -> 변수 앞에서부터 현재 단수, 현재 게이지량, 현재 단수 최대 게이지량 
    }

    private void UpdateLv()
    {
        switch (currentEnergyLv)
        {
            case 0
                :
                lvUpCost = lv1Cost;
                break;
            case 1
                :
                lvUpCost = lv2Cost;
                break;
            case 2
                :
                lvUpCost = lv3Cost;
                break;
            case 3
                : // 만렙이라 갱신 없음
                break;
        }
    }

    public void UseEnergy(int level)
    {
        if (currentEnergyLv >= level)
        {
            currentEnergyLv -= level; // 사용한 단수만큼 차감
            currentCost = 0; // 에너지로 전환되지 않은 게이지는 삭제
            UpdateLv();

            OnEnergyChanged?.Invoke(currentEnergyLv, currentCost, lvUpCost);

            UseSkill(level);
            Debug.Log($"Lv.{level}단 스킬 발동 -> 남은 에너지 {currentEnergyLv}");
        }
        else
        {
            OnEnergyNotEnough?.Invoke();
            Debug.Log($"에너지 부족 -> 필요량: {level}, 현재: {currentEnergyLv})");
        }
    }

    private void UseSkill(int level)
    {
        if (level == 1) // 몹 스턴
        {
            EnemyBase[] enemyInScreen = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);

            foreach (EnemyBase enemy in enemyInScreen)
            {
                enemy.ApplyStun(lv1Skill);
            }

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayeSFX(ESFXType.SkillStun);
            }

            Debug.Log($"Lv{level} 스킬 발동 : 화면 내 적군 {enemyInScreen.Length}마리, {lv1Skill}초 스턴");
        }

        if (level == 2) // 터렛 공속 증가
        {
            TurretBase[] installedTurret = FindObjectsByType<TurretBase>(FindObjectsSortMode.None);

            foreach (TurretBase turret in installedTurret)
            {
                turret.AddSkillBuff(lv2Skill, lv2SkillDuration);
            }

            float originalSize = Camera.main.orthographicSize;

            Camera.main.transform.DOComplete();
            Camera.main.DOComplete();

            Camera.main.transform.DOShakePosition(0.2f, 0.3f, 20, 90, false, true);

            DOTween.To(() => Camera.main.orthographicSize,
               x => Camera.main.orthographicSize = x,
               originalSize - 0.3f, 0.05f)
           .SetEase(Ease.OutQuad)
           .OnComplete(() =>
           {
               DOTween.To(() => Camera.main.orthographicSize,
                          x => Camera.main.orthographicSize = x,
                          originalSize, 0.15f).SetEase(Ease.InOutQuad);
           });

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayeSFX(ESFXType.SkillBuff);
            }

            Debug.Log($"Lv{level} 스킬 발동 : 설치된 터렛 {installedTurret.Length}개, {lv2SkillDuration}초간 공격 쿨타임 {lv2Skill} 감소");
        }

        if (level == 3) // 적 전체 최대 체력 비례 데미지
        {
            EnemyBase[] enemyInScreen = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);

            foreach (EnemyBase enemy in enemyInScreen)
            {
                enemy.TakePercentageDamage(lv3Skill);
            }

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayeSFX(ESFXType.SkillMeteor);
            }

            Debug.Log($"Lv{level} 스킬 발동 : 화면 내 적군 {enemyInScreen.Length}마리, 최대 체력의 {lv3Skill}% 데미지");
        }
    }
}