using UnityEngine;

public class Fruit : MonoBehaviour
{
    public int level;
    public bool isMerging;
    public bool isGolden;  // 황금 과일 여부

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent(out Fruit other)) return;
        if (other.level != level) return;
        if (isMerging || other.isMerging) return;

        isMerging = true;
        other.isMerging = true;

        // 둘 중 하나라도 황금이면 능력 획득
        bool anyGolden = isGolden || other.isGolden;
        GameManager.Instance.Merge(this, other, anyGolden);
    }
}