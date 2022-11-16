using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BranchScript : MonoBehaviour
{
    public BranchManager BranchManager;
    public List<Spot> AvailableSpots = new List<Spot>();
    public List<Item> sameTypeOfItems = new List<Item>();

    [SerializeField]
    bool isSelected = false;
    public static int ID;
    public int branchID;
    private void Start()
    {
        BranchManager = GameObject.FindObjectOfType<BranchManager>();
        Debug.Log("Name: " + gameObject.name + " ID value = " + ID);
        branchID = ID;
        ID++;

        if(GameEvents.instance == null)
        {
            Debug.Log("GameEvents.instance is null");
        }
        else 
            GameEvents.instance.onBranchClick += BranchClicked;
    }

    private void OnDisable()
    {
        GameEvents.instance.onBranchClick -= BranchClicked;
    }

    public void OnBranchClicked()
    {
        GameEvents.instance.OnBranchClicked(branchID);
    }

    void BranchClicked(int ID)
    {
        if (ID != branchID)
        {
            OnDeSelect();
            return;
        }

        if (!isSelected)
        {
            OnSelect();
        }
        else
        {
            OnDeSelect();
            Invoke(nameof(DelayedBranchReset), 0.01f);
        }
    }

    bool noItemsInSelectedBranch => BranchManager.SelectedBranchItems.Items.Count == 0;
    bool selectedBirdsNotFromThisBranch => BranchManager.SelectedBranchItems.branchID == branchID;
    bool spotsAvailableOnThisBranch => BranchManager.SelectedBranchItems.Items.Count <= GetAvailableSpotsOnBranch();
    bool branchIsEmpty => GetAvailableSpotsOnBranch() == GameConstants.MaxBranchItemCount;
    bool FirstItemIsSameAsSelectedItems => BranchManager.SelectedBranchItems.Items[0].ItemID == GetFirstItemID();
    void TryToGetBirdsFromSelectedBranch()
    {
        Debug.Log("BranchManager.SelectedBranchItems.Items.Count: " + BranchManager.SelectedBranchItems.Items.Count);
        //if selected Branch Item Count > 0
        if (noItemsInSelectedBranch)
        {
            Debug.Log("1 - noItemsInSelectedBranch", this);
            return;
        }

        Debug.Log("BranchManager.SelectedBranchItems.branchID == branchID");
        Debug.Log(BranchManager.SelectedBranchItems.branchID + " " + branchID);
        // check if selected birds are not from this branch. 
        if (selectedBirdsNotFromThisBranch)
        {
            Debug.Log("2 - selectedBirdsNotFromThisBranch", this);
            return;
        }

        //Then check if we can allow birds on this branch, 
        //if we have spots available
        Debug.Log("3 - Start spotsAvailableOnThisBranch", this);
        Debug.Log(BranchManager.SelectedBranchItems.Items.Count + " <= " + GetAvailableSpotsOnBranch());

        if (spotsAvailableOnThisBranch)
        {
            Debug.Log("3 - spotsAvailableOnThisBranch", this);
            Debug.Log(gameObject.name + " has " + GetAvailableSpotsOnBranch() + " spots available", this);
        }
        else return;

        Debug.Log("4 - Start GetAvailableSpotsOnBranch() == GameConstants.MaxBranchItemCount");
        Debug.Log(GetAvailableSpotsOnBranch() + " == " + GameConstants.MaxBranchItemCount);
        //if branch has no items
        if (branchIsEmpty)
        {
            Debug.Log("4 - branchIsEmpty", this);
            SwapItems();
        }
        else if (FirstItemIsSameAsSelectedItems)
        {
            Debug.Log("5 - FirstItemIsSameAsSelectedItems", this);
            SwapItems();
        }
        //If yes, then fly them here
    }

    void SwapItems()
    {
        Debug.Log("6 - SwapItems");


        while(BranchManager.SelectedBranchItems.Items.Count > 0)
        {
            //1 - get ver first empty spot
            Spot firstEmptySpot = getFirstEmptySpot();

            //2- then remove first item from branch manager and from it's current spot
            Item item = BranchManager.SelectedBranchItems.Items[BranchManager.SelectedBranchItems.Items.Count - 1];
            BranchManager.SelectedBranchItems.Items.Remove(item);
            //2.1 and from it's current spot

            //3. ReParent that
            firstEmptySpot.item = item;
            item.itemState = Item.ItemState.CanNotBeSelected;
            item.StartSwap();

            item.GetComponentInParent<Spot>().item = null;
            item.transform.parent = firstEmptySpot.transform;
            item.transform.DOLocalMove(Vector3.zero, GameConstants.MoveTime).OnComplete( 
                () => 
                {
                    item.itemState = Item.ItemState.CanBeSelected;
                    CheckIfAllBirdsAreSame();
                    item.EndSwap();
                }
                );
            item.GetDeSelected();

            OnDeSelect(); // shyd!
        }
    }

    void CheckIfAllBirdsAreSame()
    {
        Debug.Log("CheckIfAllBirdsAreSame", this);
    }


    Spot getFirstEmptySpot()
    {
        for(int i= AvailableSpots.Count-1; i>= 0; i--)
        {
            if (AvailableSpots[i].item == null)
                return AvailableSpots[i];
        }
        return null;
    }


    void OnSelect()
    {
        if(BranchManager.SelectedBranchItems.branchID != branchID)
            TryToGetBirdsFromSelectedBranch();

        if(ItemsInCanNotBeSelectedState() > 0)
        {
            return;
        }


        isSelected = true;
        foreach (Spot x in AvailableSpots)
        {
            SelectSpot(x);
        }

        UpdateGetBirdsInBranch();
        Debug.Log("BranchManager Selected Branch ID : " + branchID);
        GameEvents.instance.OnBranchItemsSelected(branchID, sameTypeOfItems);

        foreach (Item x in sameTypeOfItems)
        {
            if(x != null)
                SelectItem(x);
        }
    }
    void OnDeSelect()
    {
        isSelected = false;
        foreach (Spot x in AvailableSpots)
        {
            DeSelectSpot(x);
        }

        //// if Branch Manager has same ID as this branch, then reset BranchMangaer Selected Items
        //if(BranchManager.GetSelectedBranchID() == branchID)
        //{
        //    BranchManager.ResetSelectedItems();
        //}
    }

    void DelayedBranchReset()
    {
        BranchManager.ResetBranch();
    }


    void UpdateGetBirdsInBranch()
    {
        //        List<Item> sameTypeOfBirds = new List<Item>();
        sameTypeOfItems.Clear();
        bool firstBirdSelected = false;

        foreach (Spot x in AvailableSpots)
        {

            if (x.GetItem() == null)
            {
//                Debug.Log("Item at spot "+ x.SpotID + " is Null");
                continue;
            }

            if (x.GetItem().itemState == Item.ItemState.CanNotBeSelected)
            {
                sameTypeOfItems.Clear();
                return;
            }


            if (x.GetItem().ItemID != GameConstants.NullItem && !firstBirdSelected)
            {
                sameTypeOfItems.Add(x.GetItem());
                firstBirdSelected = true;
            }
            else if(firstBirdSelected)
            {
                if (AreSameTypesOfItems(x, sameTypeOfItems[0]))
                {
                    sameTypeOfItems.Add(x.GetItem());
                }
                else
                {
                    return;// sameTypeOfBirds;
                }
            }
        }
    }

    int GetFirstItemID()
    {
        int firstItemID = GameConstants.NullItem;
        foreach (Spot x in AvailableSpots)
        {
            if (x.GetItem() == null)
            {
                continue;
            }

            if (x.GetItem().ItemID != GameConstants.NullItem)
            {
                return x.GetItem().ItemID;
            }
        }
        return firstItemID;
    }

    bool AreSameTypesOfItems(Spot x, Item item)
    {
        if (x.GetItem().ItemID == item.ItemID)
            return true;
        else return false;
    }

    int ItemsThatCanNotBeSelected = 0;
    int ItemsInCanNotBeSelectedState()
    {
        ItemsThatCanNotBeSelected = 0;
        foreach (Spot x in AvailableSpots)
        {
            if (x.GetItem()?.itemState == Item.ItemState.CanNotBeSelected)
                ItemsThatCanNotBeSelected++;
        }
        return ItemsThatCanNotBeSelected;
    }



    void SelectSpot(Spot s)
    {
        s.GetSelected();
    }
    void DeSelectSpot(Spot s)
    {
        s.GetDeSelected();
    }

    void SelectItem(Item s)
    {
        s.GetSelected();
    }
    void DeSelectItem(Item s)
    {
        s.GetDeSelected();
    }


    int availableSpotCount = 0;
    int GetAvailableSpotsOnBranch()
    {
        availableSpotCount = 0;
        foreach (Spot x in AvailableSpots)
        {
            if (x.GetItem() == null)
            {
                availableSpotCount++;
            }
            else return availableSpotCount;
        }
        return availableSpotCount;
    }

    private void OnValidate()
    {
        AvailableSpots.Clear();
        foreach (Spot x in GetComponentsInChildren<Spot>())
        {
            AvailableSpots.Add(x);
        }

        if(GameObject.FindObjectOfType<BranchManager>())
            BranchManager = GameObject.FindObjectOfType<BranchManager>();
        else
        {
            Debug.LogError("BranchManager NOT found. Kindly add BranchManager in the Scene!");
        }
    }

}


