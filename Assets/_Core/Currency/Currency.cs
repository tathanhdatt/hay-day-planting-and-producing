using System;
using System.Collections.Generic;

public class Currency : ICurrency
{
    private readonly Dictionary<CurrencyType, int> currencies = new Dictionary<CurrencyType, int>();

    public event Action<CurrencyType> OnAmountChanged;

    public int GetAmount(CurrencyType currencyType)
    {
        if (!this.currencies.TryGetValue(currencyType, out int amount))
        {
            throw new ArgumentException($"Currency {currencyType} does not exist!");
        }

        return amount;
    }

    public void SetAmount(CurrencyType currencyType, int amount)
    {
        if (!this.currencies.ContainsKey(currencyType))
        {
            throw new ArgumentException($"Currency {currencyType} does not exist!");
        }

        this.currencies[currencyType] = amount;
        OnAmountChanged?.Invoke(currencyType);
    }

    public void AddCurrency(CurrencyType currencyType, int amount = 0)
    {
        if (!this.currencies.TryAdd(currencyType, amount))
        {
            throw new ArgumentException($"Currency {currencyType} already exists!");
        }
    }

    public void AddAmount(CurrencyType currencyType, int amount)
    {
        if (!this.currencies.ContainsKey(currencyType))
        {
            throw new ArgumentException($"Currency {currencyType} does not exist!");
        }

        this.currencies[currencyType] += amount;
        OnAmountChanged?.Invoke(currencyType);
    }

    public void SubtractAmount(CurrencyType currencyType, int amount)
    {
        if (!this.currencies.ContainsKey(currencyType))
        {
            throw new ArgumentException($"Currency {currencyType} does not exist!");
        }

        this.currencies[currencyType] -= amount;
        OnAmountChanged?.Invoke(currencyType);
    }

    public bool IsEnough(CurrencyType currencyType, int amount)
    {
        if (!this.currencies.TryGetValue(currencyType, out int currentAmount))
        {
            throw new ArgumentException($"Currency {currencyType} does not exist!");
        }
        return currentAmount >= amount;
    }
}