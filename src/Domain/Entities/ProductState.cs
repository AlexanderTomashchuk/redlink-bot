namespace Domain.Entities;

public enum ProductState
{
    Initial,
    EmptyProductCreated,
    NameRequested,
    NameProvided,
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