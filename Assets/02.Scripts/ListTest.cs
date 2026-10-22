using System.Collections.Generic;
using UnityEngine;

public class ListTest : MonoBehaviour
{
    private void Start()
    {
        // data structure
        // definition: a specialized format for organizing, processing, storing, and retrieving data in a computer efficiently

        // CRUDS: Create, Read, Update, Delete, Search
        // Create: Create a new instance of a data structure.
        // Read: Access an element in the data structure by its index.
        // Update: Modify an element in the data structure by its index.
        // Delete: Remove an element from the data structure by its index or value.
        // Search: Find an element in the data structure by its value.

        int[] ages = { 22, 21, 20, 35 };

        ages[0] = 17;
        ages[1] = 40;
        ages[2] = 27;
        ages[3] = 32;

        Debug.Log(ages[0]);

        var agesList = new List<int> { 22, 21, 20, 35 };

        agesList[0] = 17;
        agesList[1] = 40;
        agesList[2] = 27;
        agesList[3] = 32;

        agesList.Add(24);
        agesList.Insert(1, 12);
        agesList.Remove(40);
        agesList.RemoveAt(3);
        // agesList.Find();

        foreach (int age in agesList)
        {
            Debug.Log(age);
        }
    }
}
