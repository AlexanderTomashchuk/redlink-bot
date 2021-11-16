using System;
using System.Collections.Generic;
using Application.Common;

namespace Application.BotCommands;

public class CommandType : CommandTypeEnumeration
{
    public static readonly CommandType Start
        = new(1, "/start", "Getting started with the bot", typeof(StartCommand)); //, false);

    public static readonly CommandType Sell
        = new(2, "/sell", "Create a new product", typeof(SellCommand));

    public static readonly CommandType Find
        = new(3, "/find", "Find a product to buy", typeof(FindCommand));

    public static readonly CommandType Profile
        = new(4, "/profile", "Change your profile settings", typeof(ProfileCommand));

    private CommandType(int id, string name, string description, Type commandHandlerType,
        bool isVisibleInCommandsMenu = true)
        : base(id, name, description, commandHandlerType, isVisibleInCommandsMenu)
    {
    }
}

public abstract class CommandTypeEnumeration : Enumeration
{
    public string Description { get; }

    public Type CommandHandlerType { get; }

    public bool IsVisibleInCommandsMenu { get; }

    protected CommandTypeEnumeration(int id, string name, string description, Type commandHandlerType,
        bool isVisibleInCommandsMenu = true) : base(id, name)
        => (Description, CommandHandlerType, IsVisibleInCommandsMenu) =
            (description, commandHandlerType, isVisibleInCommandsMenu);

    public static IEnumerable<CommandType> GetAll() => GetAll<CommandType>();
}