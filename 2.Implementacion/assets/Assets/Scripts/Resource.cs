using System;
using UnityEngine;

public class Resource
{
    public ResourceType ResourceType { get; private set; }
    public int Amount { get; private set; }
    public int MaxAmount { get; private set; }

    public Resource(ResourceType resourceType, int amount, int maxAmount)
    {
        ResourceType = resourceType;
        Amount = amount;
        MaxAmount = maxAmount;
    }

    public void Add(int amount)
    {
        Amount = Mathf.Min(Amount + amount, MaxAmount);
    }

    public void Subtract(int amount)
    {
        if (Amount - amount < 0)
            throw new Exception("Not enough resources");

        Amount -= amount;
    }

    public void IncreaseMaxAmount(int amount)
    {
        if (ResourceType != ResourceType.Population)
            throw new Exception("Max amount can only be increased for population");

        MaxAmount += amount;
    }

    public void DecreaseMaxAmount(int amount)
    {
        if (ResourceType != ResourceType.Population)
            throw new Exception("Max amount can only be decreased for population");

        if (MaxAmount - amount < 0)
            throw new Exception("Max amount can't be negative");

        MaxAmount -= amount;
    }
}