using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DeadLine : MonoBehaviour
{
    private float dangerTime = 0f;
    private bool isDanger = false;

    private HashSet<Collider2D> dangerFruits = new HashSet<Collider2D>();

    [Header("UI")]
    public TextMeshProUGUI dangerText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Fruit fruit)) return;
        dangerFruits.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Fruit fruit)) return;
        dangerFruits.Remove(collision);
    }

    private void Update()
    {
        int realDangerCount = 0;
        foreach (var col in dangerFruits)
        {
            if (col == null) continue;
            if (col.gameObject == GameManager.Instance.currentFruit) continue;

            // 과일이 아직 내려가는 중인지 확인 (속도가 아래 방향이면 무시)
            var rb = col.GetComponent<Rigidbody2D>();
            if (rb != null && rb.linearVelocity.y < -0.5f) continue;

            realDangerCount++;
        }

        isDanger = realDangerCount > 0;

        if (isDanger)
        {
            dangerTime += Time.deltaTime;
            float remaining = 3f - dangerTime;
            if (dangerText) dangerText.text = $"위험! {remaining:F1}초";

            if (dangerTime >= 3f)
            {
                if (dangerText) dangerText.text = "";
                GameManager.Instance.GameOver();
            }
        }
        else
        {
            dangerTime = 0f;
            if (dangerText) dangerText.text = "";
        }
    }
}