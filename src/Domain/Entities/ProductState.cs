namespace Domain.Entities;

public enum ProductState
{
    Initial,
    NameRequested,
    NameProvided,
    PhotoRequested,
    PhotoProvided,
    ConditionRequested,
    ConditionProvided,
    PriceRequested,
    PriceProvided,
    ProductShowed
}