// Put your favorite extension methods in here!
using System.Collections.Generic;
using System.Collections;
using System;

public static class E{
    public static IEnumerable<object> YieldFrom(IEnumerator iter){
        while(iter.MoveNext())
            yield return iter.Current;
    }
}
