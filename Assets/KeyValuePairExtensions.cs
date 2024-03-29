﻿using System.Collections.Generic;

internal static class KeyValuePairExtensions
{
    public static void Deconstruct<K, V>(this KeyValuePair<K, V> kvp, out K key, out V value)
    {
        key = kvp.Key;
        value = kvp.Value;
    }
}
