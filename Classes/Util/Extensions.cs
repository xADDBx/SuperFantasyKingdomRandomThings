using System.Collections.Generic;

namespace RandomThings {
    public static class Extensions {
        public enum SortMode {
            byNameAsc = 0,
            byNameDesc = 1,
            byResCountAsc = 2,
            byResCountDesc = 3
        }
        public static SolidResourceHolder getInventory(this InventoryGrid inv) {
            return (inv.GetSourceOfElement(0) as SolidResourceHolder.SlotData)?.ResourceHolder;
        }
        public static Dictionary<string, int> getAmountResourceDictionary(this SolidResourceHolder container) {
            Dictionary<string, int> ret = new();
            foreach (var slot in container._slotDataIndex) {
                if (slot.ResourceInstance != null) {
                    if (!ret.ContainsKey(slot.ResourceInstance.ResourceTypeIdentifier)) {
                        ret[slot.ResourceInstance.ResourceTypeIdentifier] = slot.StackSize;
                    } else {
                        ret[slot.ResourceInstance.ResourceTypeIdentifier] += slot.StackSize;
                    }
                }
            }
            return ret;
        }


        public static void sort(this SolidResourceHolder container, SortMode sort = SortMode.byNameAsc) {
            int backIndex = container.NumOfSlots - 1;
            for (int frontIndex = 0; frontIndex < container.NumOfOccupiedSlots; frontIndex++) {
                var slot = container._slotDataIndex[frontIndex];
                if (slot.StackSize == 0) {
                    while (backIndex >= 0) {
                        var slotBack = container._slotDataIndex[backIndex];
                        if (slotBack.StackSize > 0) {
                            slot.SwapSlotData(slotBack);
                            break;
                        }
                        backIndex -= 1;
                    }
                }
            }

            bool isStringComparison = true;
            bool isAsc = true;
            if (sort == SortMode.byNameDesc || sort == SortMode.byResCountDesc) {
                isAsc = false;
            }
            Dictionary<string, int> ResourceCount = null;
            if (sort == SortMode.byResCountAsc || sort == SortMode.byResCountDesc) {
                isStringComparison = false;
            }

            for (int start = 0; start < container.NumOfOccupiedSlots - 1; start++) {
                for (int runner = 0; runner < container.NumOfOccupiedSlots - (start + 1); runner++) {
                    var slot1 = container._slotDataIndex[runner];
                    var slot2 = container._slotDataIndex[runner + 1];
                    int result;
                    if (isStringComparison) {
                        result = slot1.getName().CompareTo(slot2.getName());
                    } else {
                        if (ResourceCount == null) {
                            ResourceCount = container.getAmountResourceDictionary();
                        }
                        result = ResourceCount[slot1.ResourceInstance.ResourceTypeIdentifier] - ResourceCount[slot2.ResourceInstance.ResourceTypeIdentifier];
                    }
                    if ((isAsc && result > 0) || (!isAsc && result < 0)) {
                        slot1.SwapSlotData(slot2);
                    } else if (result == 0) {
                        var tmp = slot2.StackSize;
                        slot1.AddAmountToStackFrom(ref tmp, slot2.ResourceInstance.ResourceTypeIdentifier, slot2, slot2.ResourceInstance);
                        if (tmp == 0) {
                            // Adding some kind of elegant solution is beyond me right now.
                            container.sort();
                            return;
                        }
                    }
                }
            }
        }

        public static string getName(this SolidResourceHolder.SlotData slot) {
            return slot.ResourceInstance?.Name ?? null;
        }
    }
}
