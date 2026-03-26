using System.Collections.Generic;

/// <summary>
/// Application Service — zero UnityEngine dependency.
/// Làm việc hoàn toàn với GridSlotModel và GridPos (pure C# value object).
/// </summary>
public class GridService : IGridService
{
    private readonly int rows;
    private readonly int cols;
    private GridSlotModel[,] slots;

    private static readonly GridPos[] Dirs =
    {
        new GridPos( 1,  0),
        new GridPos(-1,  0),
        new GridPos( 0,  1),
        new GridPos( 0, -1),
    };

    public GridService(int rows, int cols) { this.rows = rows; this.cols = cols; }

    public void RegisterSlots(GridSlotModel[,] slots) => this.slots = slots;

    public GridSlotModel GetSlotAt(GridPos pos)
    {
        if (pos.X < 0 || pos.Y < 0 || pos.X >= cols || pos.Y >= rows) return null;
        return slots[pos.X, pos.Y];
    }

    public bool IsGridFull()
    {
        foreach (var slot in slots)
            if (!slot.HasCard) return false;
        return true;
    }

    public List<GridSlotModel> FindConnectedSlots(GridSlotModel startSlot, int type)
    {
        var result  = new List<GridSlotModel>();
        var queue   = new Queue<GridSlotModel>();
        var visited = new HashSet<GridSlotModel>();

        queue.Enqueue(startSlot);
        visited.Add(startSlot);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            result.Add(current);

            foreach (var d in Dirs)
            {
                var neighbor = GetSlotAt(current.Position + d);
                if (neighbor != null &&
                    !visited.Contains(neighbor) &&
                    neighbor.HasCard &&
                    neighbor.CardType == type)
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return result;
    }

    public GridSlotModel FindCenterSlot(List<GridSlotModel> slotsToMerge)
    {
        float avgX = 0f, avgY = 0f;
        foreach (var s in slotsToMerge) { avgX += s.Position.X; avgY += s.Position.Y; }
        avgX /= slotsToMerge.Count;
        avgY /= slotsToMerge.Count;

        GridSlotModel center  = slotsToMerge[0];
        float         minDist = float.MaxValue;
        foreach (var s in slotsToMerge)
        {
            float dx = s.Position.X - avgX, dy = s.Position.Y - avgY;
            float d  = dx * dx + dy * dy;
            if (d < minDist) { minDist = d; center = s; }
        }
        return center;
    }

    public void ClearAllSlots()
    {
        if (slots == null) return;
        foreach (var slot in slots) slot?.ClearCard();
    }
}
