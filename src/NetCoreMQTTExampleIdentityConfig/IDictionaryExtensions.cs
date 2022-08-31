namespace NetCoreMQTTExampleIdentityConfig;

using System.Diagnostics.CodeAnalysis;

public static class IDictionaryExtensions
{
    public static bool TryGetProperty<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, [NotNullWhen(true)]out TValue? value)
    {
        if (dictionary.ContainsKey(key))
        {
            value = dictionary[key]!;
            return true;
        }

        value = default;
        return false;
    }
}
