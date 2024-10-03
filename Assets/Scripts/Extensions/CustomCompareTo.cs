using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICustomComparable<T>
{
    int CompareByAttributeIndex(T other);
    int CompareByAttributeInfluence(T other);
}

