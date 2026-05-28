using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("과일 설정")]
    public GameObject[] fruitPrefabs;
    public GameObject[] goldFruitPrefabs;  // 황금 과일 Prefab 배열
    public Transform dropPoint;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;
    
    [Header("디버그")]
    public TextMeshProUGUI debugText;

    public GameObject currentFruit;
    private bool canDrop = true;
    private int score = 0;
    private bool isGameOver = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (isGameOver) return;

        if (currentFruit == null && canDrop)
        {
            SpawnFruit();
        }

        if (currentFruit != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(
                UnityEngine.InputSystem.Mouse.current.position.ReadValue());
            float clampedX = Mathf.Clamp(mousePos.x, -3.8f, 3.8f);
            currentFruit.transform.position = new Vector3(clampedX, dropPoint.position.y, 0);
        }

        if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame 
            && currentFruit != null 
            && !AbilityManager.Instance.IsBusy)
        {
            Drop();
        }
    }

    void SpawnFruit()
    {
        int randomLevel = Random.Range(0, 5);

        // 5% 확률로 황금 과일 등장
        bool spawnGolden = Random.value < 0.05f;
        GameObject prefab = spawnGolden
            ? goldFruitPrefabs[randomLevel]
            : fruitPrefabs[randomLevel];

        currentFruit = Instantiate(prefab, dropPoint.position, Quaternion.identity);
        currentFruit.GetComponent<Rigidbody2D>().simulated = false;

        if (shrinkNext)
        {
            currentFruit.transform.localScale *= 0.5f;
            currentFruit.GetComponent<CircleCollider2D>().radius *= 0.5f;
            shrinkNext = false;
        }
    }

    void Drop()
    {
        currentFruit.GetComponent<Rigidbody2D>().simulated = true;
        currentFruit = null;
        canDrop = false;
        Invoke("ResetDrop", 0.5f);
    }

    void ResetDrop()
    {
        canDrop = true;
    }

    public void Merge(Fruit a, Fruit b, bool anyGolden)
    {
        StartCoroutine(MergeRoutine(a, b, a.level, 
            (a.transform.position + b.transform.position) / 2, anyGolden));
    }

    private System.Collections.IEnumerator MergeRoutine(Fruit a, Fruit b, 
        int level, Vector3 mergePos, bool anyGolden)
    {
        yield return null;
        if (a == null || b == null) yield break;

        Destroy(a.gameObject);
        Destroy(b.gameObject);

        // 점수 추가
        score += (level + 1) * 10;
        scoreText.text = "Score: " + score;

        // 황금 과일 합체 시 능력 획득
        if (anyGolden)
        {
            AbilityManager.Instance.GainRandomAbility();
            ShowDebug("능력 획득!");
        }

        if (level >= 10) yield break;

        // 황금이 포함됐으면 다음도 황금, 아니면 일반
        bool bothGolden = a.isGolden && b.isGolden;
        GameObject nextPrefab = bothGolden
            ? goldFruitPrefabs[level + 1]
            : fruitPrefabs[level + 1];

        GameObject newFruit = Instantiate(nextPrefab, mergePos, Quaternion.identity);
        newFruit.GetComponent<Fruit>().isGolden = bothGolden;
    }

    private System.Collections.IEnumerator MergeRoutine(Fruit a, Fruit b, int level, Vector3 mergePos)
    {
        yield return null;
        if (a == null || b == null) yield break;

        Destroy(a.gameObject);
        Destroy(b.gameObject);

        // 점수 추가
        score += (level + 1) * 10;
        scoreText.text = "Score: " + score;

        if (level >= 10) yield break;

        GameObject newFruit = Instantiate(fruitPrefabs[level + 1], mergePos, Quaternion.identity);
        newFruit.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 0.1f, ForceMode2D.Impulse);
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        gameOverPanel.SetActive(true);
    }
    
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void GoToTitle()
    {
        SceneManager.LoadScene("StartScene");
    }
    
    // 지정 드롭용
    public void ForceNextFruit(int level)
    {
        if (currentFruit != null)
        {
            Destroy(currentFruit);
            currentFruit = null;
        }
        currentFruit = Instantiate(fruitPrefabs[level], dropPoint.position, Quaternion.identity);
        var rb = currentFruit.GetComponent<Rigidbody2D>();
        rb.simulated = false;
    
        // isMerging 초기화
        var fruit = currentFruit.GetComponent<Fruit>();
        fruit.isMerging = false;
    
        canDrop = true;
    }

// 크기 축소용
    public void ShrinkNextFruit()
    {
        shrinkNext = true;
    }
    
    public void ShowDebug(string msg)
    {
        if (debugText == null) return;
        debugText.text = msg;
        CancelInvoke("ClearDebug");
        Invoke("ClearDebug", 2f); // 2초 후 자동으로 사라짐
    }

    void ClearDebug()
    {
        if (debugText) debugText.text = "";
    }

// GameManager 클래스 변수 맨 위에 아래 줄도 추가
    private bool shrinkNext = false;
}