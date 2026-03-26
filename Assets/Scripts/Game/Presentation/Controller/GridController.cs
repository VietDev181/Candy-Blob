using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [Header("Thiết lập lưới")]
    public Transform gridRoot;
    public GameObject slotPrefab;
    public int rows = 5;
    public int cols = 5;
    public float spacing = 1f;

    [Header("VFX")]
    public GameObject vfxPlace;
    public GameObject vfxMerge;
    public GameObject vfxSpawn;

    private IGridService _gridService;
    private IAudioService _audio;
    private GameController _gameController;
    private CardSpawner _spawner;

    // Mapping: GridPos → view/model
    private readonly Dictionary<GridPos, GridSlot> _views = new();
    private readonly Dictionary<GridPos, GridSlotModel> _models = new();

    public void Initialize(IGridService gridService, IAudioService audio, GameController gameController)
    {
        _gridService = gridService;
        _audio = audio;
        _gameController = gameController;
    }

    private void Start()
    {
        _spawner = FindObjectOfType<CardSpawner>();
        if (_spawner == null) { Debug.LogError("❌ Không tìm thấy CardSpawner!"); return; }
        BuildGrid();
    }

    private void BuildGrid()
    {
        var modelGrid = new GridSlotModel[cols, rows];
        float offsetX = (cols - 1) * spacing / 2f;
        float offsetY = (rows - 1) * spacing / 2f;

        for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
            {
                var pos = new GridPos(x, y); // ← domain value object

                // Visual
                var slotObj = Instantiate(slotPrefab, gridRoot);
                slotObj.transform.localPosition = new Vector3(x * spacing - offsetX, -y * spacing + offsetY, 0);

                var slot = slotObj.GetComponent<GridSlot>() ?? slotObj.AddComponent<GridSlot>();
                if (slotObj.GetComponent<Collider2D>() == null) slotObj.AddComponent<BoxCollider2D>();
                slot.gridPos = pos;
                slot.manager = this;

                // Domain model
                var model = new GridSlotModel(pos);
                modelGrid[x, y] = model;
                _views[pos] = slot;
                _models[pos] = model;
            }

        _gridService.RegisterSlots(modelGrid);
        Debug.Log($"✅ Đã tạo {rows * cols} slot.");
    }

    private GridSlot View(GridPos pos) => _views.TryGetValue(pos, out var v) ? v : null;
    private GridSlotModel Model(GridPos pos) => _models.TryGetValue(pos, out var m) ? m : null;

    public void OnCardDroppedOnSlot(GridSlot slot, CardData card, int spawnerIndex, CardSpawner spawner)
    {
        var model = Model(slot.gridPos);
        if (model == null || model.HasCard) return;

        // Vô hiệu drag trong lúc animate
        var drag = card.GetComponent<CardDrag>();
        if (drag != null) drag.enabled = false;

        // Reparent về gridRoot để không bị ảnh hưởng bởi spawnPoint
        card.transform.SetParent(gridRoot);

        // Bay đến vị trí slot
        card.transform.DOMove(slot.transform.position, 0.35f).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                slot.SetCard(card);
                model.PlaceCard(card.type);

                if (vfxPlace != null)
                {
                    _audio.PlayPlaceSFX();
                    Instantiate(vfxPlace, slot.transform.position, Quaternion.identity, gridRoot);
                }

                DOVirtual.DelayedCall(0.05f, () => CheckMerge(slot.gridPos));
            });

        card.transform.DOScale(0.95f, 0.35f).SetEase(Ease.OutBack);

        // Xoá khỏi tay ngay lập tức, spawn card mới sau chút
        spawner.ClearCardAt(spawnerIndex);
        DOVirtual.DelayedCall(0.5f, () =>
        {
            if (spawner != null && spawner.IsHandEmpty())
            {
                spawner.DealHand(); // 🔥 spawn lại toàn bộ 3 lá
            }
        });
    }

    private void CheckMerge(GridPos pos)
    {
        var model = Model(pos);
        if (model == null || !model.HasCard) return;

        var connected = _gridService.FindConnectedSlots(model, model.CardType);
        if (connected.Count >= 3)
            AnimateMerge(connected);
        else if (_gridService.IsGridFull())
        {
            Debug.Log("💀 Game Over!");
            _gameController.GameOver();
        }
    }

    private void AnimateMerge(List<GridSlotModel> modelsToMerge)
    {
        var centerModel = _gridService.FindCenterSlot(modelsToMerge);
        int type = centerModel.CardType;
        int maxType = _spawner.cardSprites.Length - 1;

        _audio.PlayMergeSFX();
        Camera.main.transform.DOShakePosition(0.2f, 0.2f);
        _gameController.AddScore(modelsToMerge.Count * (type + 1) * 2);

        foreach (var m in modelsToMerge)
        {
            var slotView = View(m.Position);
            var card = slotView?.GetCurrentCard();
            m.ClearCard();
            slotView?.ClearCard();

            if (card == null) continue;

            if (vfxMerge != null)
                Instantiate(vfxMerge, slotView.transform.position, Quaternion.identity, gridRoot);

            card.transform.DOScale(0, 0.25f)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(card.gameObject));
        }

        // 🔥 Nếu là max → chỉ biến mất
        if (type >= maxType)
        {
            Debug.Log("💥 Max level → biến mất");
            return;
        }

        // 🔥 Spawn card mới
        DOVirtual.DelayedCall(0.3f, () =>
        {
            SpawnMergedCard(centerModel, type + 1);

            // 🔥 Unlock CHỈ khi cần
            if (type == _spawner.GetMaxUnlockedType() - 1)
            {
                _spawner.UnlockNextType();
            }
        });
    }

    private void SpawnMergedCard(GridSlotModel targetModel, int newType)
    {
        var targetView = View(targetModel.Position);
        var newCardObj = Instantiate(_spawner.cardPrefab, targetView.transform.position, Quaternion.identity, gridRoot);
        if (vfxSpawn != null) Instantiate(vfxSpawn, targetView.transform.position, Quaternion.identity, gridRoot);

        var newCard = newCardObj.GetComponent<CardData>();
        newCard.type = newType;
        newCard.spriteRenderer.sprite = _spawner.cardSprites[Mathf.Min(newType, _spawner.cardSprites.Length - 1)];
        newCard.transform.localScale = Vector3.zero;
        newCard.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);

        targetView.SetCard(newCard);
        targetModel.PlaceCard(newType);
        DOVirtual.DelayedCall(0.45f, () => CheckMerge(targetModel.Position));
    }

    public void ClearAllSlots()
    {
        foreach (var (_, slotView) in _views)
        {
            var card = slotView.GetCurrentCard();
            if (card != null) { Destroy(card.gameObject); slotView.ClearCard(); }
        }
        _gridService.ClearAllSlots();
    }
}
