using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnumerableExtensions
{
    public static T GetRandom<T>(this IEnumerable<T> values) {
        return values.ElementAt(Random.Range(0, values.Count()));
    }

    public static T GetRandomOrDefault<T>(this IEnumerable<T> values) {
        if (values.Count() == 0) return default;
        return values.ElementAt(Random.Range(0, values.Count()));
    }
}