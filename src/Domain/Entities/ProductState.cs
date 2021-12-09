namespace Domain.Entities;

public enum ProductState
{
    Initial = 0,
    NameRequested = 1,
    NameProvided = 2,
    PhotoRequested = 3,
    PhotoProvided = 4,
    ConditionRequested = 5,
    ConditionProvided = 6,
    PriceRequested = 7,
    PriceProvided = 8,
    Finished = 9
}