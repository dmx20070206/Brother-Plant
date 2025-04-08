using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class EnemyStatusSystem : MonoBehaviour
{
    public enum StatusType { Normal, Slow, Frozen, Burn, Poison }

    [System.Serializable]
    public class StatusConfig
    {
        public StatusType type;
        public Color effectColor;
        public GameObject effectPrefab;
        public float duration;
        public Transform spawnPos;
    }

    [Header("状态配置")]
    [SerializeField] private StatusConfig[] statusConfigs;

    public List<ActiveStatus> _activeStatuses = new List<ActiveStatus>();

    private Dictionary<StatusType, StatusConfig> _configMap = new Dictionary<StatusType, StatusConfig>();
    public Color _originalColor;

    public SpriteRenderer _spriteRenderer;
    public Enemy _enemy;

    public Dictionary<StatusType, bool> _have_status = new Dictionary<StatusType, bool>();

    public class ActiveStatus
    {
        public StatusType type;
        public float endTime;
        public Coroutine coroutine;
        public GameObject effect;
    }

    #region Life Cycle
    private void Awake()
    {
        InitializeConfigMap();
        CacheOriginalColor();
    }

    private void Update()
    {
        UpdateVisualEffects();
    }

    private void OnDisable()
    {
        CleanExpiredStatuses();
    }
    #endregion

    #region Init
    private void InitializeConfigMap()
    {
        foreach (var config in statusConfigs)
        {
            _configMap[config.type] = config;
        }

        _have_status.Add(StatusType.Normal, false);
        _have_status.Add(StatusType.Slow, false);
        _have_status.Add(StatusType.Burn, false);
        _have_status.Add(StatusType.Frozen, false);
        _have_status.Add(StatusType.Poison, false);
    }

    private void CacheOriginalColor()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
        _enemy = GetComponent<Enemy>();
    }
    #endregion

    #region API
    public void ApplyStatus(StatusType type, float duration)
    {
        if (!_configMap.ContainsKey(type)) return;

        HandleSpecialRules(type);
        AddNewStatus(type, duration);
    }
    #endregion

    #region Special Rules
    private void HandleSpecialRules(StatusType newType)
    {
        if (newType == StatusType.Burn)
        {
            CancelStatus(StatusType.Frozen);
            CancelStatus(StatusType.Slow);
        }
        else if (newType == StatusType.Frozen)
        {
            CancelStatus(StatusType.Slow);
            CancelStatus(StatusType.Burn);
        }
        else if (newType == StatusType.Slow)
        {
            CancelStatus(StatusType.Burn);
        }
    }
    #endregion

    #region Add and Start
    private void AddNewStatus(StatusType type, float duration)
    {
        var existing = _activeStatuses.Find(s => s.type == type);
        if (existing != null)
        {
            existing.endTime = Time.time + duration;
        }
        else
        {
            var newStatus = new ActiveStatus
            {
                type = type,
                endTime = Time.time + duration
            };
            StartStatusCoroutines(ref newStatus);
            _activeStatuses.Add(newStatus);
            _have_status[newStatus.type] = true;
        }

        SortByPriority();
    }

    private void StartStatusCoroutines(ref ActiveStatus status)
    {
        switch (status.type)
        {
            case StatusType.Burn:
                status.coroutine = StartCoroutine(BurnDamageRoutine(status));
                break;
            case StatusType.Poison:
                status.coroutine = StartCoroutine(PoisonDamageRoutine(status));
                break;
            case StatusType.Slow:
                status.coroutine = StartCoroutine(SlowDamageRoutine(status));
                break;
            case StatusType.Frozen:
                status.coroutine = StartCoroutine(FrozenDamageRoutine(status));
                break;
        }
    }
    #endregion

    #region IEnumerator
    private IEnumerator BurnDamageRoutine(ActiveStatus status)
    {
        while (Time.time < status.endTime)
        {
            if (_configMap[StatusType.Burn].effectPrefab)
            {
                status.effect = Instantiate(_configMap[StatusType.Burn].effectPrefab,
                           _configMap[StatusType.Burn].spawnPos.position,
                           Quaternion.identity);
                status.effect.transform.SetParent(transform);
            }
            yield return new WaitForSeconds(0.3f);

            if (status.effect != null)
                Destroy(status.effect);

            yield return new WaitForSeconds(0.7f);
        }
    }
 
    private IEnumerator PoisonDamageRoutine(ActiveStatus status)
    {
        while (Time.time < status.endTime)
        {
            if (_configMap[StatusType.Poison].effectPrefab)
            {
                status.effect = Instantiate(_configMap[StatusType.Poison].effectPrefab,
                           _configMap[StatusType.Poison].spawnPos.position,
                           Quaternion.identity);
                status.effect.transform.SetParent(transform);
            }
            yield return new WaitForSeconds(0.3f);

            if (status.effect != null)
                Destroy(status.effect);

            yield return new WaitForSeconds(0.7f);
        }
    }

    private IEnumerator SlowDamageRoutine(ActiveStatus status)
    {
        while (Time.time < status.endTime)
        {
            if (_configMap[StatusType.Slow].effectPrefab)
            {
                status.effect = Instantiate(_configMap[StatusType.Slow].effectPrefab,
                           _configMap[StatusType.Slow].spawnPos.position,
                           Quaternion.identity);
                status.effect.transform.SetParent(transform);
            }
            yield return new WaitForSeconds(0.3f);

            if (status.effect != null)
                Destroy(status.effect);

            yield return new WaitForSeconds(0.7f);
        }
    }

    private IEnumerator FrozenDamageRoutine(ActiveStatus status)
    {
        while (Time.time < status.endTime)
        {
            if (_configMap[StatusType.Frozen].effectPrefab)
            {
                if (status.effect == null)
                    status.effect = Instantiate(_configMap[StatusType.Frozen].effectPrefab,
                               _configMap[StatusType.Frozen].spawnPos.position,
                               Quaternion.identity);
                status.effect.transform.SetParent(transform);
            }
            yield return new WaitForSeconds(1f);
        }
    }
    #endregion

    #region Priority
    private void SortByPriority()
    {
        _activeStatuses.Sort((a, b) =>
            GetStatusPriority(b.type).CompareTo(GetStatusPriority(a.type)));
    }

    private int GetStatusPriority(StatusType type)
    {
        return type switch
        {
            StatusType.Poison => 4,
            StatusType.Frozen => 3,
            StatusType.Burn => 3,
            StatusType.Slow => 2,
            _ => 0
        };
    }
    #endregion

    #region Color Effect
    private void UpdateVisualEffects()
    {
        ApplyHighestPriorityEffect();
        CleanExpiredStatuses();
    }
    private void ApplyHighestPriorityEffect()
    {
        if (_activeStatuses.Count > 0)
            _spriteRenderer.color = _configMap[_activeStatuses[0].type].effectColor;
    }

    private void ResetVisuals()
    {
        _spriteRenderer.color = _originalColor;
    }

    private void CleanExpiredStatuses()
    {
        for (int i = _activeStatuses.Count - 1; i >= 0; i--)
        {
            var status = _activeStatuses[i];
            if (Time.time > status.endTime)
            {
                if (status.coroutine != null)
                    StopCoroutine(status.coroutine);
                if (status.effect != null)
                    Destroy(status.effect);
                _have_status[status.type] = false;
                _activeStatuses.RemoveAt(i);
                ResetVisuals();
            }
        }
    }

    private void CancelStatus(StatusType type)
    {
        var status = _activeStatuses.Find(s => s.type == type);
        if (status != null)
        {
            StopCoroutine(status.coroutine);
            if (status.effect != null)
                Destroy(status.effect);
            _have_status[status.type] = false;
            _activeStatuses.Remove(status);
        }
    }

    #endregion
}