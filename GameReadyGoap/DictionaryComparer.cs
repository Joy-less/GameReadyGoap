namespace GameReadyGoap;

internal class DictionaryComparer<TKey, TValue> : IEqualityComparer<Dictionary<TKey, TValue>> where TKey : notnull {
    public bool Equals(Dictionary<TKey, TValue>? DictA, Dictionary<TKey, TValue>? DictB) {
        if (DictA == DictB) {
            return true;
        }
        if (DictA is null || DictB is null) {
            return false;
        }
        if (DictA.Count != DictB.Count) {
            return false;
        }
        return DictA.All(Entry => DictB.TryGetValue(Entry.Key, out TValue? Value) && Equals(Entry.Value, Value));
    }
    public int GetHashCode(Dictionary<TKey, TValue> Dict) {
        unchecked {
            int Hash = 0;
            foreach ((TKey Key, TValue Value) in Dict) {
                Hash ^= Key.GetHashCode();
                Hash ^= Value?.GetHashCode() ?? 0;
            }
            return Hash;
        }
    }
}