using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GridSlot : MonoBehaviour
{
    [HideInInspector] public GridController manager;
    [HideInInspector] public GridPos gridPos;

    private CardData currentCard;

    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;

        if (GetComponent<SpriteRenderer>() == null)
            gameObject.AddComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.1f);
    }

    public void SetCard(CardData card) => currentCard = card;
    public bool HasCard() => currentCard != null && !currentCard.isEmpty;
    public CardData GetCurrentCard() => currentCard;
    public void ClearCard() => currentCard = null;
}
