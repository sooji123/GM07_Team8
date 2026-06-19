using UnityEngine;

public abstract class TurretBase : MonoBehaviour
{
    [Header("Turret Data")]
    [SerializeField]
    protected TurretData _turretData;
    [SerializeField]
    protected LayerMask _enemyLayerMask;

    protected int _currentLevel = 1;
    protected TurretLevelStat _currentStat;

    // 밖에서 읽기 위해서 읽기 전용 프로퍼티 추가 - 장은수
    public TurretLevelStat CurrentStat 
    {
        get { return _currentStat; }
    }

    protected float _damage;
    protected float _attckRange = 3f;
    protected EElement _element;
    protected float _attackCool;
    protected SpriteRenderer _spriteRenderer;

    private float _lastAttackTime;

    public string TurretName => _turretData.turretName;
    public float Damage => _damage;
    public float AttackRange => _attckRange;
    public int Cost => _turretData.cost;
    public EElement Element => _element;
    public float AttackCool => _attackCool;
    public int CurrentLevel => _currentLevel;

    public int totalCost = 0; // 추가 - 장은수

    protected void Awake()
    {
        UpdateStat(1);

        totalCost = _turretData.cost; // 추가 - 장은수
        _element = _turretData.elementType;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _lastAttackTime = -_attackCool;
    }
    protected virtual void Update()
    {
        if (Time.time >= _lastAttackTime + _attackCool)
        {
            GameObject target = FindTarget();
            if (target != null)
            {
                FlipToTarget(target);
                Attack(target);

                _lastAttackTime = Time.time;
            }
        }
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

    [ContextMenu("Debug/Test Upgrade")]
    public virtual void Upgrade()
    {
        if (_currentLevel >= 3)
        {
            return;
        }

        totalCost += CurrentStat.upgradeCost; // 추가 - 장은수
        _currentLevel++;
        UpdateStat(_currentLevel);

        //업그레이드 이펙트, 사운드
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
}
