using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatModifierType
{
    Flat = 100,
    PercentageAdditive = 200,
    PercentageMultiply = 300,
}
public class StatModifier
{
    public StatModifierType Type;
    public float Value = 0f;
    public int Order;
    public object Source;

    public StatModifier(float value, StatModifierType type, int order, object source)
    {
        Value = value;
        Type = type;
        Order = order;
        Source = source;
    }

    public StatModifier(float value, StatModifierType type) : this(value, type, (int)type, null) { }

    public StatModifier(float value, StatModifierType type, int order) : this(value, type, order, null) { }

    public StatModifier(float value, StatModifierType type, object source) : this(value, type, (int)type, source) { }

}
