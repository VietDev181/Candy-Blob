public class CardModel
{
    public int Type { get; private set; }

    public CardModel(int type) => Type = type;

    public void SetType(int type) => Type = type;
}
