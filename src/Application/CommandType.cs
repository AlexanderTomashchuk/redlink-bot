using System.Collections.Generic;
using Application.Common;

namespace Application;

public class CommandType : CommandTypeEnumeration
{
    public static readonly CommandType Start
        = new(1, "/start", "Getting started with the bot", false);

    public static readonly CommandType Sell
        = new(2, "/sell", "Create a new product");

    public static readonly CommandType Find
        = new(3, "/find", "Find a product to buy");

    public static readonly CommandType Profile
        = new(4, "/profile", "Change your profile settings");

    private CommandType(int id, string name, string description, bool isVisibleInCommandsMenu = true)
        : base(id, name, description, isVisibleInCommandsMenu)
    {
    }
}

public abstract class CommandTypeEnumeration : Enumeration
{
    public string Description { get; }

    public bool IsVisibleInCommandsMenu { get; }

    protected CommandTypeEnumeration(int id, string name, string description,
        bool isVisibleInCommandsMenu = true) : base(id, name)
        => (Description, IsVisibleInCommandsMenu) =
            (description, isVisibleInCommandsMenu);

    public static IEnumerable<CommandType> GetAll() => GetAll<CommandType>();
}