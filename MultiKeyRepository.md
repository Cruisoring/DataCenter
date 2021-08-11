A common scenario is comparing values of same or different types when the they could stand for same thing. For example, when Enum values would be converted to numbers by default of System.Text.Json or Newton JSON, and DateTime objects can be serialised to strings or long values.

Consequently, a reasonable deep comparor of a complex C# class instance with the objects deserialised from JSON shall be handled with pseudo codes below:

```
... if objectA and objectB are not null:
Type typeA = objectA.GetType();
Type typeB = objectB.GetType();

switch(typeA) {
    case StringType:
        switch(typeB): {
            case StringType: return compareStrings(objectA, objectB);
            case IntType:
            case LongType: return compareStringWithNumber(objectA, objectB);
            case DateTime: return compareStringWithDateTime(objectA, objectB); ...
        }
    case EnumTypeA: switch(typeB) {}...
    case EnumTypeB: ...
    case DateTimeType: ...
}
```

There could be lots of codes duplications and it is hard to support new types without changing the above SWITCH or IF statements.

The [MRepository](https://github.com/Cruisoring/DataCenter/blob/master/DataCenter.MultiKeys/MRepository.cs) or Multi-Key Repository enables a more elastic approach to handle it.

```
public static IDictionary<(Type, Type), Func<object, object, bool>> PredefinedComparers =
    new Dictionary<(Type, Type), Func<object, object, bool>>()
    {
        {(typeof(int), typeof(long)), (a, b) => Convert.ToInt64(a).Equals(Convert.ToInt64(b))},
        {(typeof(DateTime), typeof(string)), (a, b) => a.Equals(DateTime.Parse(b.ToString()))},
    };

private static Func<object, object, bool> ComparerFactory(Type typeA, Type typeB)
{
    if (typeA == typeB) 
        return (a, b) => a == b;
    //Build more complex comparer here, or use the default logic below to compare their string forms
    return (a, b) => a.ToString().Equals(b.ToString(), StringComparison.CurrentCulture);
}

public static MRepository<Type, Type, Func<object, object, bool>> TypeComparers =
    new MRepository<Type, Type, Func<object, object, bool>>(ComparerFactory, PredefinedComparers);

public static bool CompareWithNull<T>(T obj)
{
    return  (obj is null) || object.Equals(obj, default(T));
}

public static bool Compare(object a, object b)
{
    if (a is null && b is null) return true;
    if (a is null || b is null) return CompareWithNull(a ?? b);
    (Type typeA, Type typeB) = (a.GetType(), b.GetType());
    Func<object, object, bool> comparer = TypeComparers.Contains((typeB, typeA)) ? TypeComparers.Get((typeB, typeA)) : TypeComparers.Get(typeA, typeB);
    return comparer(a, b);
}

Assert.IsTrue(Compare(123, "123"));
Assert.IsTrue(Compare(123L, 123)
                && TypeComparers.Contains((typeof(int), typeof(long)))
                && !TypeComparers.Contains((typeof(long), typeof(int))));

```

There are two places to inject your business logic:
- Insert the logic to compare concerned logic into the **PredefinedComparers**
- Create comparers at run-time in the factory method **ComparerFactory(Type typeA, Type typeB)**

With the flattened structure to organise the business logic with the Repository, there are some benefits:
- Easy to extend functionalities.
- New business logic could be created with the factory at run time, that is ideal to handle similar but different types like different Enums.
- Least codes can be tested easily.