using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

    
public abstract class ValueAsset<T> : ScriptableObject
{
    [HideLabel]
    public T Value;
}
