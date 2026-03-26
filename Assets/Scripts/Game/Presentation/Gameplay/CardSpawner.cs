using UnityEngine;
using DG.Tweening;

public class CardSpawner : MonoBehaviour
{
    [Header("Thiết lập spawn")]
    public GameObject cardPrefab;
    public Sprite[] cardSprites;
    public Transform[] spawnPoints; // Cần đúng 3 phần tử trong Inspector

    private CardData[] _hand;

    private int _maxTypeUnlocked = 2;

    private void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("❌ CardSpawner: chưa gán spawnPoints! Cần 3 Transform.");
            return;
        }

        _hand = new CardData[spawnPoints.Length];
        DealHand();
    }

    // ─── Public API ───────────────────────────────────────────────

    /// <summary>Phát card cho tất cả slot trống trong tay.</summary>
    public void DealHand()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
            if (_hand[i] == null)
                SpawnCardAt(i);
    }

    /// <summary>Spawn một card mới tại vị trí index trong tay.</summary>
    public void SpawnCardAt(int index)
    {
        if (index < 0 || index >= spawnPoints.Length) return;

        if (_hand[index] != null)
        {
            Destroy(_hand[index].gameObject);
            _hand[index] = null;
        }

        var cardObj = Instantiate(cardPrefab, spawnPoints[index].position,
                                  Quaternion.identity, spawnPoints[index]);
        var card = cardObj.GetComponent<CardData>();

        int randType = Random.Range(0, _maxTypeUnlocked);
        card.type = randType;
        card.spriteRenderer.sprite = cardSprites[randType];
        card.homePosition = spawnPoints[index].position;

        // Gắn drag component
        var drag = cardObj.GetComponent<CardDrag>() ?? cardObj.AddComponent<CardDrag>();
        drag.spawnerIndex = index;
        drag.spawner = this;

        // Pop-in animation với stagger nhẹ
        cardObj.transform.localScale = Vector3.zero;
        cardObj.transform.DOScale(card.baseScale, 0.4f)
            .SetEase(Ease.OutBack)
            .SetDelay(index * 0.08f);

        _hand[index] = card;
    }

    public void UnlockNextType()
    {
        if (_maxTypeUnlocked >= cardSprites.Length) return;

        _maxTypeUnlocked++;
    }

    public int GetMaxUnlockedType()
    {
        return _maxTypeUnlocked;
    }

    public bool IsHandEmpty()
    {
        for (int i = 0; i < _hand.Length; i++)
        {
            if (_hand[i] != null)
                return false;
        }
        return true;
    }

    public void ClearCardAt(int index)
    {
        if (index >= 0 && index < _hand.Length)
        {
            _hand[index] = null;
        }
    }

    //public void ReplaceCard(int index) => SpawnCardAt(index);

    public CardData GetCardAt(int index) =>
        (index >= 0 && index < _hand.Length) ? _hand[index] : null;
}
