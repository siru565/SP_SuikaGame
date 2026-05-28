using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;

    [Header("능력 사용 횟수")]
    public int bombCount;
    public int specifyCount;
    public int downgradeCount;
    public int earthquakeCount;

    [Header("쿨다운 (초)")]
    public float bombCooldown = 5f;
    public float specifyCooldown = 8f;
    public float downgradeCooldown = 6f;
    public float earthquakeCooldown = 10f;

    float _bombTimer;
    float _specifyTimer;
    float _downgradeTimer;
    float _earthquakeTimer;

    [Header("UI 버튼")]
    public Button bombButton;
    public Button specifyButton;
    public Button downgradeButton;
    public Button earthquakeButton;

    [Header("UI 텍스트 (남은 횟수)")]
    public TextMeshProUGUI bombCountText;
    public TextMeshProUGUI specifyCountText;
    public TextMeshProUGUI downgradeCountText;
    public TextMeshProUGUI earthquakeCountText;

    [Header("과일 선택 패널")]
    public GameObject fruitSelectPanel;

    [Header("능력 패널")]
    public GameObject abilityPanel;

    [Header("폭탄 범위")]
    public float bombRadius = 1.0f;

    bool _isBombMode;
    bool _isDowngradeMode;
    bool _isEarthquaking;
    GameObject _bombRangeIndicator;

    public bool IsBusy => _isBombMode || _isDowngradeMode || _isEarthquaking ||
                          (fruitSelectPanel != null && fruitSelectPanel.activeSelf) ||
                          (abilityPanel != null && abilityPanel.activeSelf);

    void Awake()
    {
        Instance = this;

        _bombRangeIndicator = new GameObject("BombRange");
        var sr = _bombRangeIndicator.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite();
        sr.color = new Color(1f, 0.2f, 0.2f, 0.3f);
        _bombRangeIndicator.transform.localScale = Vector3.one * bombRadius * 2f;
        _bombRangeIndicator.SetActive(false);
    }

    Sprite CreateCircleSprite()
    {
        int size = 128;
        var tex = new Texture2D(size, size);
        var center = new Vector2(size / 2f, size / 2f);
        float r = size / 2f;
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                tex.SetPixel(x, y, Vector2.Distance(new Vector2(x, y), center) <= r
                    ? Color.white : Color.clear);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    void Update()
    {
        if (_bombTimer > 0) _bombTimer -= Time.deltaTime;
        if (_specifyTimer > 0) _specifyTimer -= Time.deltaTime;
        if (_downgradeTimer > 0) _downgradeTimer -= Time.deltaTime;
        if (_earthquakeTimer > 0) _earthquakeTimer -= Time.deltaTime;

        UpdateButtons();

        if (UnityEngine.InputSystem.Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (fruitSelectPanel != null && fruitSelectPanel.activeSelf) return;
            if (abilityPanel != null) abilityPanel.SetActive(!abilityPanel.activeSelf);
        }

        if (_isBombMode)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(
                UnityEngine.InputSystem.Mouse.current.position.ReadValue());
            worldPos.z = 0;
            _bombRangeIndicator.transform.position = worldPos;
            _bombRangeIndicator.transform.localScale = Vector3.one * bombRadius * 2f;

            if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
                PlaceBomb(worldPos);
        }

        if (_isDowngradeMode)
        {
            if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
                SelectDowngradeTarget();
        }
    }

    void UpdateButtons()
    {
        if (bombButton) bombButton.interactable = _bombTimer <= 0 && bombCount > 0;
        if (specifyButton) specifyButton.interactable = _specifyTimer <= 0 && specifyCount > 0;
        if (downgradeButton) downgradeButton.interactable = _downgradeTimer <= 0 && downgradeCount > 0;
        if (earthquakeButton) earthquakeButton.interactable = _earthquakeTimer <= 0 && earthquakeCount > 0;

        if (bombCountText) bombCountText.text = bombCount.ToString();
        if (specifyCountText) specifyCountText.text = specifyCount.ToString();
        if (downgradeCountText) downgradeCountText.text = downgradeCount.ToString();
        if (earthquakeCountText) earthquakeCountText.text = earthquakeCount.ToString();
    }

    // ── 1. 폭탄 ──────────────────────────────────────────
    public void OnBombButton()
    {
        if (_bombTimer > 0 || bombCount <= 0) return;
        _isBombMode = true;
        _bombRangeIndicator.SetActive(true);
        GameManager.Instance.ShowDebug("터뜨릴 위치를 클릭하세요!");
    }

    void PlaceBomb(Vector3 worldPos)
    {
        _isBombMode = false;
        _bombRangeIndicator.SetActive(false);
        bombCount--;
        _bombTimer = bombCooldown;

        var hits = Physics2D.OverlapCircleAll(worldPos, bombRadius);
        int removed = 0;
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Fruit _))
            {
                Destroy(hit.gameObject);
                removed++;
            }
        }
        GameManager.Instance.ShowDebug($"폭탄! 과일 {removed}개 제거");
    }

    // ── 2. 픽 ────────────────────────────────────────────
    public void OnSpecifyButton()
    {
        if (_specifyTimer > 0 || specifyCount <= 0) return;
        if (fruitSelectPanel) fruitSelectPanel.SetActive(true);
    }

    public void OnFruitSelected(int level)
    {
        if (fruitSelectPanel) fruitSelectPanel.SetActive(false);
        specifyCount--;
        _specifyTimer = specifyCooldown;
        GameManager.Instance.ForceNextFruit(level);
        GameManager.Instance.ShowDebug($"레벨 {level} 과일 지정!");
    }

    // ── 3. 레벨 다운 ─────────────────────────────────────
    public void OnDowngradeButton()
    {
        if (_downgradeTimer > 0 || downgradeCount <= 0) return;
        _isDowngradeMode = true;
        GameManager.Instance.ShowDebug("다운그레이드할 과일을 클릭하세요!");
    }

    void SelectDowngradeTarget()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
            UnityEngine.InputSystem.Mouse.current.position.ReadValue());
        worldPos.z = 0;

        var hit = Physics2D.OverlapPoint(worldPos);
        if (hit != null && hit.TryGetComponent(out Fruit fruit))
        {
            if (fruit.level > 0)
            {
                _isDowngradeMode = false;
                downgradeCount--;
                _downgradeTimer = downgradeCooldown;

                Vector3 pos = fruit.transform.position;
                bool isGolden = fruit.isGolden;
                int newLevel = fruit.level - 1;

                Destroy(fruit.gameObject);

                var prefab = isGolden
                    ? GameManager.Instance.goldFruitPrefabs[newLevel]
                    : GameManager.Instance.fruitPrefabs[newLevel];

                var newFruit = Instantiate(prefab, pos, Quaternion.identity);
                newFruit.GetComponent<Fruit>().isGolden = isGolden;
                GameManager.Instance.ShowDebug($"레벨 {newLevel + 1} -> {newLevel} 다운!");
            }
            else
            {
                GameManager.Instance.ShowDebug("최소 레벨 과일은 다운 불가!");
            }
        }
    }

    // ── 4. 지진 ──────────────────────────────────────────
    public void OnEarthquakeButton()
    {
        if (_earthquakeTimer > 0 || earthquakeCount <= 0) return;
        earthquakeCount--;
        _earthquakeTimer = earthquakeCooldown;
        StartCoroutine(EarthquakeRoutine());
    }

    System.Collections.IEnumerator EarthquakeRoutine()
    {
        _isEarthquaking = true;
        GameManager.Instance.ShowDebug("지진 발생!");

        var allFruits = FindObjectsByType<Fruit>();
        foreach (var fruit in allFruits)
        {
            if (fruit.gameObject == GameManager.Instance.currentFruit) continue;
            var rb = fruit.GetComponent<Rigidbody2D>();
            if (rb == null) continue;
            rb.AddForce(new Vector2(Random.Range(-8f, 8f), Random.Range(3f, 8f)), ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(0.5f);

        allFruits = FindObjectsByType<Fruit>();
        foreach (var fruit in allFruits)
        {wwwwwwwwwwwwwww    
            if (fruit.gameObject == GameManager.Instance.currentFruit) continue;
            var rb = fruit.GetComponent<Rigidbody2D>();
            if (rb == null) continue;
            rb.AddForce(new Vector2(Random.Range(-5f, 5f), 2f), ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(1.5f);
        _isEarthquaking = false;
    }

    // ── 능력 획득 ─────────────────────────────────────────
    public void GainRandomAbility()
    {
        int rand = Random.Range(0, 100);

        if (rand < 10)
        {
            bombCount++;
            GameManager.Instance.ShowDebug("폭탄 능력 획득!");
        }
        else if (rand < 20)
        {
            specifyCount++;
            GameManager.Instance.ShowDebug("픽 능력 획득!");
        }
        else if (rand < 50)
        {
            earthquakeCount++;
            GameManager.Instance.ShowDebug("지진 능력 획득!");
        }
        else
        {
            downgradeCount++;
            GameManager.Instance.ShowDebug("레벨다운 능력 획득!");
        }
    }
}