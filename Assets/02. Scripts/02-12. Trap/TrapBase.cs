using UnityEngine;

public abstract class TrapBase : MonoBehaviour
{
    [Header("Trap Data")]
    [SerializeField]
    protected TrapData _trapData;

    protected float _activeCool;

    private float _lastAttackTime;

    public string TrampName => _trapData.trapName;
    public float ActiveCool => _activeCool;
    public int Cost => _trapData.cost;

    public TowerBuilder Builder { get; private set; }

    protected virtual void Awake()
    {
        if (_trapData != null)
        {
            _activeCool = _trapData.activeCool;
        }

        _lastAttackTime = -_activeCool;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckTrap(collision);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        CheckTrap(collision);
    }

    protected abstract void ActiveTrap(GameObject target);

    private void CheckTrap(Collider2D collision)
    {
        if (collision.CompareTag(nameof(ETags.Enemy)))
        {
            if (Time.time >= _lastAttackTime + _activeCool)
            {
                ActiveTrap(collision.gameObject);
                _lastAttackTime = Time.time;
            }
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
            UI_Manager.Instance.OpenTrapWindow(this, transform.position);
        }
    }
}
