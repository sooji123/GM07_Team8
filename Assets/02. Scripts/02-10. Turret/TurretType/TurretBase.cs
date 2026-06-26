using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class TurretBase : MonoBehaviour, IPointerClickHandler
{
    [Header("Turret Data")]
    [SerializeField]
    protected TurretData _turretData;
    [SerializeField]
    protected LayerMask _enemyLayerMask;

    [Header("업그레이드 스텟")]
    [SerializeField]
    protected float _addDamage;
    [SerializeField]
    protected float _addAttackCool;

    [Header("스텟")]
    [SerializeField]
    protected float _damage;
    [SerializeField]
    protected float _attckRange;
    [SerializeField]
    protected float _attackCool;
    [SerializeField]
    protected EElement _element;

    private float _bonusDamage = 0f;
    private float _bonusAttackCool = 0f;

    protected int _currentLevel = 1;
    protected TurretLevelStat _currentStat;

    public TurretLevelStat CurrentStat 
    {
        get { return _currentStat; }
    }

    //protected float _damage;
    //protected float _attckRange = 3f;
    //protected EElement _element;
    //protected float _attackCool;
    protected SpriteRenderer _spriteRenderer;
    private int _totalCost = 0;
    private float _lastAttackTime;

    public string TurretName => _turretData.turretName;
    //public float Damage => _damage;

    public float Damage => _damage+_bonusDamage;
    public float AttackRange => _attckRange;
    public int Cost => _turretData.cost;
    public EElement Element => _element;
    //public float AttackCool => _attackCool;
    public float AttackCool => Mathf.Max(0.05f, _attackCool - _bonusAttackCool);
    public int CurrentLevel => _currentLevel;

    public int TotalCost => _totalCost;

    public TowerBuilder Builder { get; private set; }

    protected void Awake()
    {
        //UpdateStat(1);

        _totalCost = _turretData.cost;
        _element = _turretData.elementType;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        GetElement(_element);
        _lastAttackTime = -_attackCool;
        
    }
    protected virtual void Update()
    {
        if (Time.time >= _lastAttackTime + AttackCool)
        {
            GameObject target = FindTarget();
            if (target != null)
            {
                FlipToTarget(target);
                Attack(target);

                _lastAttackTime = Time.time;
            }
        }

        GetElement(_element); //테스트용
    }

    protected abstract GameObject FindTarget();

    protected abstract void Attack(GameObject target);

    protected void FlipToTarget(GameObject target)
    {
        if (_spriteRenderer != null)
        {
            if (target.transform.position.x < transform.position.x)
            {
                _spriteRenderer.flipX = true;
            }
            else
            {
                _spriteRenderer.flipX = false;
            }
        }
    }

    public virtual void Upgrade()
    {
        if (_currentLevel >= 3)
        {
            return;
        }

        _totalCost += CurrentStat.upgradeCost;
        _currentLevel++;
        UpdateStat(_currentLevel);

        //업그레이드 이펙트, 사운드
    }

    public virtual void UpgradeDamage()
    {
        _bonusDamage += _addDamage;
    }

    public virtual void UpgradeSpeed()
    {
        _bonusAttackCool += _addAttackCool;
    }

    private void UpdateStat(int level)
    {
        if(_turretData == null)
        {
            return;
        }

        _currentStat = _turretData.GetStat(level);

        if (_currentStat != null) 
        {
            _damage = _currentStat.damage;
            _attackCool = _currentStat.attackCool;
            _attckRange = _currentStat.attckRange;
        }
    }

    public virtual void GetElement(EElement element)
    {
        _element = element;
        _spriteRenderer.color = ElementColor.GetElementColor(element);
    }

    public void SetupBuilder(TowerBuilder builder)
    {
        Builder = builder;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnClick();
        }
    }

    public void OnClick()
    {
        if (UI_Manager.Instance != null)
        {
            UI_Manager.Instance.OpenTurretWindow(this, transform.position);
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (_turretData != null)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, _attckRange);
#endif
        }
    }

    [ContextMenu("Debug/테스트: 공격력 증가")]
    public void TestAddDamage()
    {
        _bonusDamage += _addDamage;
        Debug.Log($"[테스트] 공격력이 {_addDamage}만큼 증가했습니다. (총 보너스: {_bonusDamage})");
    }

    [ContextMenu("Debug/테스트: 공격속도 증가 (쿨타임 감소)")]
    public void TestAddAttackCool()
    {
        _bonusAttackCool += _addAttackCool;
        Debug.Log($"[테스트] 공격 쿨타임이 {_addAttackCool}초만큼 감소했습니다. (총 보너스 감소량: {_bonusAttackCool})");
    }

    [ContextMenu("Debug/테스트: 보너스 스텟 초기화")]
    public void TestResetBonus()
    {
        _bonusDamage = 0f;
        _bonusAttackCool = 0f;
        Debug.Log("[테스트] 보너스 스텟이 초기화되었습니다.");
    }
}
