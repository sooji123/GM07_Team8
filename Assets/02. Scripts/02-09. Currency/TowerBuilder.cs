using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class TowerBuilder : MonoBehaviour
{
    public GameObject CurrentTurret { get; private set; }
    [SerializeField] protected int orbCost = 10; // <- 속성부여 필요 갯수 데이터가 없어서 임시로 여기에 지정

    private void Start()
    {
        if (CurrentTurret != null)
        {
            TurretBase thisTurret = CurrentTurret.GetComponent<TurretBase>();
        }
    }

    [SerializeField] protected int sellRatio = 80;

    // 타워 있는지 확인용
    public bool HasTower => CurrentTurret != null;

    // 타워 생성을 시도하는 메서드
    public void BuildTower(TurretData turretData)
    {
        // 이미 타워가 있다면 생성 취소
        if (HasTower == true)
        {
            Debug.Log("이미 타워가 있음");
            return;
        }

        // 골드 소모 해보고(안되면 리턴)
        if (CurrencyManager.Instance.UseGold(turretData.cost) == true)
        {
            // 현재 타일에 타워 생성
            CurrentTurret = Instantiate(turretData.turretPrefab, transform.position, Quaternion.identity);
            Debug.Log($"{turretData.turretName} 생성!");
        }
        else
        {
            // 골드 부족 처리 (UI 팝업) => CurrencyManager에서 이벤트를 만들어서 UI에 쏘는걸로 생각 중
            Debug.Log("골드 부족");
        }
    }

    // 타워 업그레이드
    public void UpgradeTower(TurretData turretData)
    {
        // 타워가 없다면 업그레이드 취소
        if (HasTower == false)
        {
            Debug.Log("타워가 없음");
            return;
        }

        // 골드 소모 해보고(안되면 리턴)
        if (CurrencyManager.Instance.UseGold(turretData.upgradeCost) == true)
        {
            // 현재 타워 업그레이드
            TurretBase thisTurret = CurrentTurret.GetComponent<TurretBase>();
            thisTurret.Upgrade();
            // turretData.isUpgrade = true; -> isUpgrade는 터렛 데이터에서 관리 부탁드립니다
            Debug.Log("업그레이드 성공");
        }
    }

    // 타워 속성 부여
    public void ElementTower(TurretData turretData, EElement element) 
    {
        // 타워가 없다면 속성 부여 취소
        if (HasTower == false)
        {
            Debug.Log("타워가 없음");
            return;
        }

        // 속성 자원 소모 해보고(안되면 리턴)
        if (CurrencyManager.Instance.UseElementOrbs(element, orbCost))
        {
            // 현재 타워 속성 부여
            TurretBase thisTurret = CurrentTurret.GetComponent<TurretBase>();
            thisTurret.GetElement(element);
            Debug.Log("속성 부여 성공");
        }
    }

    // 타워 판매
    public void SellTower(TurretData turretData)
    {
        if (HasTower == false)
        {
            return;
        }

        TurretBase thisTurret = CurrentTurret.GetComponent<TurretBase>();

        // 돈 환급
        int refundGold = (turretData.cost * sellRatio) / 100;
        int refundOrb = 0;


        if (turretData.isUpgrade == true)
        {
            refundGold += (turretData.upgradeCost * sellRatio) / 100;
        }

        EElement currentElement = thisTurret.Element;

            if (currentElement != EElement.None)
        {
            refundOrb = (orbCost * sellRatio) / 100;
            CurrencyManager.Instance.AddElementOrbs(currentElement, refundOrb);
        }

        CurrencyManager.Instance.AddGold(refundGold);

        Destroy(CurrentTurret);
        CurrentTurret = null;
        Debug.Log("타워 철거");
    }
}
