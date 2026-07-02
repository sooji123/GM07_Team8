using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class TurretBase : MonoBehaviour
{
    #region 
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

    protected float _bonusDamage = 0f;
    protected float _bonusAttackCool = 0f;

    protected float _buffDamage = 0f;
    protected float _buffAttackCool = 0f;
    protected float _buffAttackRange = 0f;
    private int _upgradeLevel = 1;

    [Header("이미지")]
    [SerializeField]
    private GameObject _damageUpgradeImg;
    [SerializeField]
    private GameObject _coolUpgradeImg;
    [SerializeField]
    private GameObject _buffImg;
    [SerializeField]
    private GameObject _elementImg;
    [Header("텍스트")]
    [SerializeField]
    private TextMeshProUGUI _levelText;

    protected SpriteRenderer _spriteRenderer;
    private int _totalCost = 0;
    private float _lastAttackTime;
    private int _damageUpgradeCount;
    private int _speedUpgradeCount;
    private TextMeshProUGUI _damageUpgradeText;
    private TextMeshProUGUI _coolUpgradeText;
    private Coroutine _skillBuffCoroutine;

    public ETurretType TurretType => _turretData.turretType;

    public float Damage => _damage + _bonusDamage + _buffDamage;
    public float AttackRange => _attckRange + _buffAttackRange;
    public EElement Element => _element;
    public float AttackCool => Mathf.Max(0.5f, _attackCool - _bonusAttackCool - _buffAttackCool);
    public int DamageUpgradeCount => _damageUpgradeCount;
    public int SpeedUpgradeCount => _speedUpgradeCount;
    public int TotalCost => _totalCost;
    public int CurrentLevel => _upgradeLevel;
    public TowerBuilder Builder { get; private set; }

    #endregion

    protected void Awake()
    {
        _totalCost = _turretData.cost;
        _element = _turretData.elementType;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _lastAttackTime = -_attackCool;
        _damageUpgradeCount = 0;
        _speedUpgradeCount = 0;
        if (_damageUpgradeImg != null && _coolUpgradeImg != null && _buffImg != null && _elementImg != null)
        {
            _damageUpgradeImg.SetActive(false);
            _coolUpgradeImg.SetActive(false);
            _buffImg.SetActive(false);
            _elementImg.SetActive(false);
        }
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
    }

    private void OnEnable()
    {
        UpgradeManager.OnTurretTypeUpgraded += HandleUpgrade;
        UpgradeManager.OnUpgradesReset += HandleUpgradeReset;

        if (UpgradeManager.Instance != null)
        {
            _upgradeLevel = UpgradeManager.Instance.GetUpgradeLevel(TurretType);
            if (_levelText != null)
            {
                _levelText.text = _upgradeLevel.ToString();
            }
        }
    }
    private void OnDisable()
    {
        UpgradeManager.OnTurretTypeUpgraded -= HandleUpgrade;
        UpgradeManager.OnUpgradesReset -= HandleUpgradeReset;
    }

    private void HandleUpgrade(ETurretType upgradedTurret, int level)
    {
        if (TurretType == upgradedTurret && _levelText != null)
        {
            _upgradeLevel = level;

            _levelText.transform.DOKill();
            _levelText.transform.localScale = Vector3.one;

            _levelText.transform.DOScale(1.5f, 0.1f).SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _levelText.text = _upgradeLevel.ToString();

                    _levelText.transform.DOScale(1.0f, 0.3f).SetEase(Ease.OutBack);
                });
        }
        //업그레이드 이펙트나 연출이 필요하면 여기에 추가
    }

    private void HandleUpgradeReset()
    {
        _upgradeLevel = 1;
        if (_levelText != null)
        {
            _levelText.text = _upgradeLevel.ToString();
        }
    }

    protected virtual GameObject FindTarget() => null;

    protected virtual void Attack(GameObject target) { }

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

    public virtual void UpgradeDamage()
    {
        _bonusDamage += _addDamage;
        _damageUpgradeCount++;

        if (_damageUpgradeImg != null)
        {
            _damageUpgradeImg.SetActive(true);
            _damageUpgradeText = _damageUpgradeImg.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            _damageUpgradeText.text = $"{_damageUpgradeCount}";
        }
    }

    public virtual void UpgradeSpeed()
    {
        _bonusAttackCool += _addAttackCool;
        _speedUpgradeCount++;

        if (_coolUpgradeImg != null)
        {
            _coolUpgradeImg.SetActive(true);
            _coolUpgradeText = _coolUpgradeImg.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            _coolUpgradeText.text = $"{_speedUpgradeCount}";
        }
    }

    public void AddBuff(float damageBuff, float speedBuff, float rangeBuff)
    {
        if (_buffImg != null)
        {
            _buffImg.SetActive(true);
        }
        _buffDamage += damageBuff;
        _buffAttackCool += speedBuff;
        _buffAttackRange += rangeBuff;
    }
    public void RemoveBuff(float damageBuff, float speedBuff, float rangeBuff)
    {
        if (_buffImg != null)
        {
            _buffImg.SetActive(false);
        }
        _buffDamage = Mathf.Max(0f, _buffDamage - damageBuff);
        _buffAttackCool = Mathf.Max(0f, _buffAttackCool - speedBuff);
        _buffAttackRange = Mathf.Max(0f, _buffAttackRange - rangeBuff);
    }
    public void AddSkillBuff(float amount, float duration)
    {
        if (_skillBuffCoroutine != null)
        {
            StopCoroutine(_skillBuffCoroutine);
        }
        _skillBuffCoroutine = StartCoroutine(SkillBuff(amount, duration));
    }
    IEnumerator SkillBuff(float amount, float duration)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayeSFX(ESFXType.SkillBuff);
        }

        _buffAttackCool += amount;

        //버프효과가 필요할듯

        yield return new WaitForSeconds(duration);

        _buffAttackCool = Mathf.Max(0f, _buffAttackCool - amount);

        _skillBuffCoroutine = null;
    }

    public virtual void GetElement(EElement element, Sprite elemetnSprite)
    {
        _element = element;
        _spriteRenderer.color = ElementColor.GetElementColor(element);
        if (_elementImg != null)
        {
            _elementImg.SetActive(true);
            _elementImg.GetComponent<Image>().sprite = elemetnSprite;
        }
    }

    public void SetupBuilder(TowerBuilder builder)
    {
        Builder = builder;
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
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, AttackRange);
#endif
        }
    }
}
