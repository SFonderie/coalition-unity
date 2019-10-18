using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class CharStats
{

}

[Serializable]
public class CharacterStatistic
{
    public float BaseValue;

    public float Value {
        get
        {
            if(isDirty)
            {
                _value = GetFinalValue();
                isDirty = false;
            }

            return _value;
        }
    }

    private readonly List<StatMod> modifiers;

    public readonly ReadOnlyCollection<StatMod> StatModifiers;

    private bool isDirty = true;
    private float _value;

    public CharacterStatistic()
    {
        modifiers = new List<StatMod>();
        StatModifiers = modifiers.AsReadOnly();
    }

    public CharacterStatistic(float baseValue) : this()
    {
        BaseValue = baseValue;
    }

    public void AddModifier(StatMod mod)
    {
        isDirty = true;
        modifiers.Add(mod);
        modifiers.Sort(CompareModOrder);
    }

    private int CompareModOrder(StatMod a, StatMod b)
    {
        if (a.Order < b.Order) return -1;
        else if (a.Order > b.Order) return 1;
        return 0;
    }

    public bool RemoveModifier(StatMod mod)
    {
        if(modifiers.Remove(mod))
        {
            isDirty = true;
            return true;
        }

        return false;
    }

    public bool removeAllModifiersFromSource(object source)
    {
        bool didRemove = false;

        for(int i = modifiers.Count - 1; i >= 0; i--)
        {
            if(modifiers[i].Source == source)
            {
                isDirty = true;
                didRemove = true;
                modifiers.RemoveAt(i);
            }
        }

        return didRemove;
    }

    public float GetFinalValue()
    {
        float final = BaseValue;
        float sumPercentAdd = 0;

        for(int i = 0; i < modifiers.Count; i++)
        {
            StatMod mod = modifiers[i];

            if(mod.Type == StatModType.flat)
            {
                final += mod.Mod;
            }
            else if (mod.Type == StatModType.percent_add)
            {
                sumPercentAdd += mod.Mod;

                if(i + 1 >= modifiers.Count || modifiers[i + 1].Type != StatModType.percent_add)
                {
                    final *= 1 + sumPercentAdd;
                    sumPercentAdd = 0;
                }
            }
            else if(mod.Type == StatModType.percent_multiply)
            {
                final *= 1 + mod.Mod;
            }
        }

        return (float)Math.Round(final, 4);
    }
}


public enum StatModType
{
    flat = 100,
    percent_add = 200,
    percent_multiply = 300
}


public class StatMod
{
    public readonly float Mod;
    public readonly StatModType Type;
    public readonly int Order;
    public readonly object Source;

    public StatMod(float mod, StatModType type, int order, object source)
    {
        Mod = mod;
        Type = type;
        Order = order;
        Source = source;
    }

    public StatMod(float mod, StatModType type) : this(mod, type, (int)type, null) { }

    public StatMod(float mod, StatModType type, int order) : this(mod, type, order, null) { }

    public StatMod(float mod, StatModType type, object source) : this(mod, type, (int)type, source) { }
}