using UnityEngine;

public class Water
{
}

public class Coke
{
}

public class Cup<T>
{
    public T Drink;

    public Cup(T drink)
    {
        Drink = drink;
    }
}

public class GenericExample : MonoBehaviour
{
    private void Start()
    {
        var cup1 = new Cup<int>(17);
        var cup2 = new Cup<float>(0.2f);
        var cup3 = new Cup<string>("coke");
        var cup4 = new Cup<bool>(true);
        var cup5 = new Cup<Water>(new Water());
        var cup6 = new Cup<Coke>(new Coke());
    }
}
