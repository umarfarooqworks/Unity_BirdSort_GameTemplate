using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SelectedBranchItems
{

    public SelectedBranchItems()
    {
        Reset();
    }

    public int branchID;
    public List<Item> Items = new List<Item>();

    public void Reset()
    {
        branchID = GameConstants.NullBranchSelected;

        while(Items.Count > 0)
        {
            Items.RemoveAt(0);
        }
    }
}

public class BranchManager : MonoBehaviour
{
    public SelectedBranchItems SelectedBranchItems = new SelectedBranchItems();

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.instance.onBranchItemsSelected += OnBranchItemsSelected;
    }
    private void OnDisable()
    {
        GameEvents.instance.onBranchItemsSelected -= OnBranchItemsSelected;
    }

    void OnBranchItemsSelected(int branchID, List<Item> items)
    {
        SelectedBranchItems = new SelectedBranchItems();
        SelectedBranchItems.branchID = branchID;
//        SelectedBranchItems.Items
//        SelectedBranchItems.Items.Clear();
        SelectedBranchItems.Items = items;
    }

    public int GetSelectedBranchID()
    {
        return SelectedBranchItems.branchID;
    }

    public void ResetSelectedItems()
    {
        SelectedBranchItems.Reset();
    }

    public SelectedBranchItems GetCurrentSelectedBranchItems()
    {
        return SelectedBranchItems;
    }

    public void ResetBranch()
    {
        SelectedBranchItems.branchID = 0;
        SelectedBranchItems.Items.Clear();
    }

}
