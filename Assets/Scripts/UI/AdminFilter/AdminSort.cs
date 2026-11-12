using System.Linq;
using UnityEngine;

public class AdminSort : MonoBehaviour
{
    [SerializeField] private Transform cardContainer;

    public void DropDown(int index)
    {
        Debug.Log($"Dropdown changed: {index}");
        switch (index)
        {
            case 1:
                SortCardsByName();
                break;
            case 2:
                SortCardsByBestTime(true);
                break;
            case 3:
                SortCardsByError();
                break;
        }
    }

    //Functions in the drop down on admin page.
    public void SortCardsByBestTime(bool lowestTimeFirst = true)
    {
        //Get all cards from child prefabs
        EmployeeCard[] cards = cardContainer.GetComponentsInChildren<EmployeeCard>();

        //Sort based on best time value
        var sortedCards = lowestTimeFirst ? cards.OrderBy(c => c.BestTime).ToList() : cards.OrderByDescending(c => c.BestTime).ToList();

        //Update hierachy Order

        for (int i = 0; i < sortedCards.Count; i++)
        {
            //SetSibling visually repositions the card in the UI layout Group
            sortedCards[i].transform.SetSiblingIndex(i);
        }

        Debug.Log("Sorted employees according to best time");

    }

    public void SortCardsByName()
    {
        EmployeeCard[] cards = cardContainer.GetComponentsInChildren<EmployeeCard>();

        var sortedCards = cards.OrderBy(c => c.EmployeeName).ToList();

        for (int i = 0; i < sortedCards.Count; i++)
        {
            sortedCards[i].transform.SetSiblingIndex(i);
        }

        Debug.Log($"Sorted cards by name. ");
    }

    public void SortCardsByError()
    {
        EmployeeCard[] cards = cardContainer.GetComponentsInChildren<EmployeeCard>();

        var sortedCards = cards.OrderBy(c => c.Errors).ToList();

        for (int i = 0; i < sortedCards.Count; i++)
        {
            sortedCards[i].transform.SetSiblingIndex(i);
        }

        Debug.Log($"Sorted cards by error. ");
    }
}
