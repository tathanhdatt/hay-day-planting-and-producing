using System;

public interface ICurrency
{
    event Action<CurrencyType> OnAmountChanged;
    int GetAmount(CurrencyType currencyType);
    void SetAmount(CurrencyType currencyType, int amount);
    void AddCurrency(CurrencyType currencyType, int amount = 0);
    void AddAmount(CurrencyType currencyType, int amount);
    void SubtractAmount(CurrencyType currencyType, int amount);
    bool IsEnough(CurrencyType currencyType, int amount);
}