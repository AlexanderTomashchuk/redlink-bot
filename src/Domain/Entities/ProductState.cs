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
    Finished,
    Aborted = 100
}