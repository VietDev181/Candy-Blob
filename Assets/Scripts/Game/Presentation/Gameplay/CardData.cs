using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class CardData : MonoBehaviour
{
    [Header("Card Info")]
    public int type;
    public Vector2Int gridPos;
    public bool isEmpty = false;

    [Header("Render")]
    public SpriteRenderer spriteRenderer;

    [HideInInspector] public Vector3 homePosition; // vị trí spawn gốc để snap về khi thả sai

    public Vector3 baseScale = new Vector3(0.95f, 0.95f, 1f);
    private Color normalColor = Color.white;
    private Color selectedColor = new Color(1f, 0.9f, 0.4f);

    private Tween colorTween, fadeTween, scaleTween;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (GetComponent<BoxCollider2D>() == null)
            gameObject.AddComponent<BoxCollider2D>();

        transform.localScale = Vector3.zero; // CardSpawner handle animation bật lên
    }

    public void ResetState()
    {
        DOTween.Kill(transform);
        DOTween.Kill(spriteRenderer);

        isEmpty = false;
        spriteRenderer.enabled = true;
        GetComponent<Collider2D>().enabled = true;
        spriteRenderer.color = Color.white;
        transform.localScale = baseScale;
    }

    public void SetSelected(bool selected)
    {
        if (spriteRenderer == null) return;

        DOTween.Kill(transform);
        DOTween.Kill(spriteRenderer);

        colorTween = spriteRenderer
            .DOColor(selected ? selectedColor : normalColor, 0.2f)
            .SetEase(Ease.OutQuad);

        if (selected)
        {
            scaleTween = transform
                .DOScale(1.1f, 0.2f).SetEase(Ease.OutBack)
                .OnComplete(() => transform.DOScale(baseScale, 0.25f).SetEase(Ease.OutQuad));

            fadeTween = spriteRenderer
                .DOFade(0.85f, 0.1f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
        else
        {
            scaleTween = transform.DOScale(baseScale, 0.25f).SetEase(Ease.OutBack);
            fadeTween = spriteRenderer.DOFade(1f, 0.1f);
        }
    }

    public void SetVisible(bool visible)
    {
        DOTween.Kill(transform);
        DOTween.Kill(spriteRenderer);

        isEmpty = !visible;

        if (!visible)
        {
            Sequence vanish = DOTween.Sequence();
            vanish.Append(spriteRenderer.DOFade(0f, 0.25f));
            vanish.Join(transform.DOScale(0f, 0.3f).SetEase(Ease.InBack));
            vanish.OnComplete(() =>
            {
                spriteRenderer.enabled = false;
                GetComponent<Collider2D>().enabled = false;
            });
        }
        else
        {
            spriteRenderer.enabled = true;
            GetComponent<Collider2D>().enabled = true;
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
            transform.localScale = Vector3.zero;

            Sequence appear = DOTween.Sequence();
            appear.Append(spriteRenderer.DOFade(1f, 0.25f));
            appear.Join(transform.DOScale(baseScale, 0.35f).SetEase(Ease.OutBack));
        }
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
        DOTween.Kill(spriteRenderer);
    }
}
