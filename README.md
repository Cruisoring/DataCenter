# EasyWorks.DataCenter

## Introduction

To put it simple, the Repository in this library is a advanced version of [Lazy<T>](https://docs.microsoft.com/en-us/dotnet/api/system.lazy-1?view=net-5.0) backed with [Dictionary<TKey,TValue>](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2?view=net-5.0) and [ValueTuple](https://docs.microsoft.com/en-us/dotnet/api/system.valuetuple?view=net-5.0) with [Function Programming](https://en.wikipedia.org/wiki/Functional_programming) paradigm to:
- Avoid writing boiler plate codes.
- Enable efficient caching of **ANY types of** values associated with **ANY types of** keys.
- Mandatory Factory method to create values for keys not presented in the Repository.
- Extended versions like [Repository<TKey, TValue1, TValue2, TValue3>](https://github.com/Cruisoring/DataCenter/blob/master/DataCenter.Common/Repository.cs) can keep multiple values associated with the same key.
- Multi-keys version like [MRepository<TKey1, TKey2, TKey3, TValue>](https://github.com/Cruisoring/DataCenter/blob/master/DataCenter.MultiKeys/MRepository.cs) can flat combinations of multiple input parameters to simplify complex switchings.


## Getting Started with Examples

### Simplest Lazy Loader

For expensive operations like reflection of types to get their PropertyInfo, it is preferred to operate once and keep the result in a dictoary for later use as codes below:

```
private static IDictionary<Type, PropertyInfo[]> cache = new Dictionary<Type, PropertyInfo[]>;

public static PropertyInfo[] GetProperties(Type type)
{
    if (!cache.ContainsKey(type))
    {
        cache.Add(type, type.GetProperties());
    }
    return cache[type];
}

var propertyInfos = GetProperties(typeof(MyClass));
```

It can be replaced with:
```
public static readonly Repository<System.Type, PropertyInfo[]> TypeProperties =
            new Repository<System.Type, PropertyInfo[]>(t => t.GetProperties());

var propertyInfos = TypeProperties.Get(typeof(MyClass));
```

### Cache of multiple values

With ValueTuples, both Keys and Values of the Repository can be of multiple values that can make Lazy loading more efficient.

Stick with the use of PropertyInfo by assuming it would be ued to extract value of the class instances, following code snippet shows how to cache multiple values and turn the Repository into a higher-order-functions.

```
public static readonly Repository<System.Type, (PropertyInfo[], Dictionary<string, Func<object, object>>)> TypeProperties =
    new Repository<System.Type, (PropertyInfo[], Dictionary<string, Func<object, object>>)>(ParseType);

internal static (PropertyInfo[], Dictionary<string, Func<object, object>>) ParseType(System.Type type)
{
    PropertyInfo[] propertyInfos = type.GetProperties();
    Dictionary<string, Func<object, object>> extractors = propertyInfos.ToDictionary(
        pi => pi.Name,
        pi => (Func<object, object>)(obj => pi.GetValue(obj))
    );
    return (propertyInfos, extractors);
}

public static bool TryGetPropertyValue(object source, string propertyName, out object value)
{
    if (source is null || propertyName is null)
    {
        value = source;
        return false;
    }

    (PropertyInfo[] _, Dictionary<string, Func<object, object>> extractors) = TypeProperties.Get(source.GetType());

    if (extractors.ContainsKey(propertyName))
    {
        value = extractors[propertyName](source);
        return true;
    }

    string caseIgnoredName =
        extractors.Keys.SingleOrDefault(k => k.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));

    if (caseIgnoredName is null)
    {
        value = source;
        Console.WriteLine($"Failed to locate property of {propertyName} from object {source}");
        return false;
    }

    value = extractors[caseIgnoredName](source);
    return true;
}
```

Given you want to access the property of a class instance by its name, following codes would be enough:
```
bool result = TryGetPropertyValue(myClassInstance, "propertyName", out object value);

result = TryGetPropertyValue(oneDay, "Year", out object year)
```

Here is the benefits:
- The *myClassInstance* can be of any type, the Repository would create an entry against its type once only.
- Each PropertyInfo would be converted to an anonymous function that shall be faster than calling *PropertyInfo.GetValue()* directly.
- The higher order function **ParseType(System.Type type)** can be tested easily.


### Flattened Dictionary for complex choices

See [MRepository example](MultiKeyRepository.md).

There are two places to inject your business logic:
- Insert the logic to compare concerned logic into the **PredefinedComparers**
- Create comparers at run-time in the factory method **ComparerFactory(Type typeA, Type typeB)**

With the flattened structure to organise the business logic with the Repository, there are some benefits:
- Easy to extend functionalities.
- New business logic could be created with the factory at run time, that is ideal to handle similar but different types like different Enums.
- Least codes can be tested easily.


## More thoughts

The idea of this project came when I tried to apply Functional Programming paradigm into my coding practice: if you treat functions as values of a dictionary referred by corresponding keys, a lot of technical challenges could be handled gracefully.

This idea has been applied successfully with other languages (JAVA, Python and TypeScript) to make my life much easier. Before exploring some other topics with .NET core, refactoring this project might save me some time in long run and hope this document can also help others to save unnecessary codes. 

Enjoy coding.

