using System.Collections.Generic;
using UnityEngine;

public class DictionaryTest : MonoBehaviour
{
    private void Start()
    {
        // dictionary: a non-linear data structure used to store data in key-value pairs, where each unique key maps to a specific value
        var students = new Dictionary<string, int> { { "오나현", 22 }, { "박철규", 21 }, { "최재원", 20 } };

        // accessing values
        Debug.Log(students["오나현"]);

        // adding a new key-value pair
        students.Add("김지훈", 23);

        students.Remove("최재원");

        bool hasKey = students.ContainsKey("송재욱");

        foreach (KeyValuePair<string, int> item in students)
        {
            Debug.Log($"{item.Key}의 나이: {item.Value}");
        }
    }
}
