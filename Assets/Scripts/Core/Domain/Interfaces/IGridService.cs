using System.Collections.Generic;

/// <summary>
/// Contract cho grid logic — zero UnityEngine dependency.
/// </summary>
public interface IGridService
{
    void RegisterSlots(GridSlotModel[,] slots);
    GridSlotModel GetSlotAt(GridPos pos);
    bool IsGridFull();
    List<GridSlotModel> FindConnectedSlots(GridSlotModel startSlot, int type);
    GridSlotModel FindCenterSlot(List<GridSlotModel> slots);
    void ClearAllSlots();
}
