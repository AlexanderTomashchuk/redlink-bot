namespace Domain.Entities;

public enum ProductState
{
    Initial,
    EmptyProductCreated,
    NameRequested,
    NameProvided,
    CategoryRequested,
    CategoryProvided,
    PhotoRequested,
    PhotoProvided,
    ConditionRequested,
    ConditionProvided,
    PriceRequested,
    PriceProvided,
    CurrencyRequested,
    CurrencyProvided,
    ReadyForPublishing,
    Published = 99,
    Aborted = 100
}