namespace GameReadyGoap;

internal sealed class DictionaryComparer<TKey, TValue> : IEqualityComparer<Dictionary<TKey, TValue>> where TKey : notnull {
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
        foreach ((TKey Key, TValue ValueA) in DictA) {
            if (!DictB.TryGetValue(Key, out TValue? ValueB) || !Equals(ValueA, ValueB)) {
                return false;
            }
        }
        return true;
    }
    public int GetHashCode(Dictionary<TKey, TValue> Dict) {
        unchecked {
            int Hash = 0;
            foreach ((TKey Key, TValue Value) in Dict) {
                Hash += HashCode.Combine(Key, Value);
            }
            return Hash;
        }
    }
}