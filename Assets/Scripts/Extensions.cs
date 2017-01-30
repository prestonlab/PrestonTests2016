// Put your favorite extension methods in here! And other helpful functions.
using System.Collections.Generic;
using System.Collections;
using System;

public static class E{
    // Yields all (remaining) objects from a given IEnumerator
    public static IEnumerable<object> YieldFrom(IEnumerator iter){
        while(iter.MoveNext())
            yield return iter.Current;
    }
}
