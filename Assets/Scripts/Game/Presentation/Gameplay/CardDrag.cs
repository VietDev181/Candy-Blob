using UnityEngine;
using DG.Tweening;

/// <summary>
/// Gắn lên card trong tay người chơi.
/// Xử lý kéo (drag) từ khu vực spawn vào slot trên lưới.
/// </summary>
[RequireComponent(typeof(CardData))]
public class CardDrag : MonoBehaviour
{
    [HideInInspector] public int spawnerIndex;
    [HideInInspector] public CardSpawner spawner;

    private CardData _card;
    private GridController _gridController;

    private Vector3 _dragOffset;
    private bool _isDragging;
    private bool _placed;          // đã thả xuống slot thành công → không drag nữa
    private int _originalSortOrder;

    // ─────────────────────────────────────────────────────────────

    private void Awake() => _card = GetComponent<CardData>();
    private void Start() => _gridController = FindObjectOfType<GridController>();

    // ── Mouse events ──────────────────────────────────────────────

    private void OnMouseDown()
    {
        if (_placed || _card == null) return;

        _isDragging = true;

        // Đưa card lên layer trên cùng khi kéo
        _originalSortOrder = _card.spriteRenderer.sortingOrder;
        _card.spriteRenderer.sortingOrder = 20;

        // Phóng to nhẹ khi nhấc lên
        DOTween.Kill(transform);
        transform.DOScale(_card.baseScale * 1.15f, 0.12f).SetEase(Ease.OutBack);

        // Lưu offset để card không giật về tay khi kéo
        _dragOffset = transform.position - WorldMouse();
    }

    private void OnMouseDrag()
    {
        if (!_isDragging) return;
        transform.position = WorldMouse() + _dragOffset;
    }

    private void OnMouseUp()
    {
        if (!_isDragging) return;
        _isDragging = false;

        _card.spriteRenderer.sortingOrder = _originalSortOrder;
        transform.DOScale(_card.baseScale, 0.15f);

        // Raycast tìm GridSlot tại vị trí thả
        Vector2 point = WorldMouse();
        Collider2D[] hits = Physics2D.OverlapPointAll(point);

        GridSlot targetSlot = null;
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;   // bỏ qua chính mình
            var slot = hit.GetComponent<GridSlot>();
            if (slot != null && !slot.HasCard())
            {
                targetSlot = slot;
                break;
            }
        }

        if (targetSlot != null && _gridController != null)
        {
            _placed = true;
            _gridController.OnCardDroppedOnSlot(targetSlot, _card, spawnerIndex, spawner);
        }
        else
        {
            // Không có slot hợp lệ → trả về vị trí ban đầu
            transform.DOMove(_card.homePosition, 0.35f).SetEase(Ease.OutBack);
        }
    }

    // ── Helper ────────────────────────────────────────────────────

    private Vector3 WorldMouse()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0f;
        return pos;
    }
}
