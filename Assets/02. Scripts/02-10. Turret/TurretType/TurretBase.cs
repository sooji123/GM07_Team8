using UnityEngine;
using UnityEngine.UI;

public abstract class TurretBase : MonoBehaviour
{
    [Header("Turret Data")]
    [SerializeField]
    protected TurretData _turretData;
    [SerializeField]
    protected LayerMask _enemyLayerMask;

    protected int _currentLevel = 1;
    protected TurretLevelStat _currentStat;

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
    private Button _button;

    public string TurretName => _turretData.turretName;
    public float Damage => _damage;
    public float AttackRange => _attckRange;
    public int Cost => _turretData.cost;
    public EElement Element => _element;
    public float AttackCool => _attackCool;
    public int CurrentLevel => _currentLevel;

    public int TotalCost => _totalCost;
    private int _totalCost = 0;

    protected void Awake()
    {
        UpdateStat(1);

        _totalCost = _turretData.cost;
        _element = _turretData.elementType;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _lastAttackTime = -_attackCool;
        _button = GetComponentInChildren<Button>();
        if (_button != null)
        { 
            _button.onClick.AddListener(OnClick);
        }
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

        _totalCost += CurrentStat.upgradeCost;
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
}
