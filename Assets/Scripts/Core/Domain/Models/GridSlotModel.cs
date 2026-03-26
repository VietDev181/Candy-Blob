public class GridSlotModel
{
    public GridPos Position { get; }
    public int CardType { get; private set; }
    public bool HasCard { get; private set; }

    public GridSlotModel(GridPos position) => Position = position;

    public void PlaceCard(int type) { CardType = type; HasCard = true; }

    public void ClearCard() { HasCard = false; CardType = 0; }
}
