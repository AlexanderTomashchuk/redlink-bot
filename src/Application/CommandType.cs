using Ardalis.SmartEnum;

namespace Application;

public class CommandType : SmartEnum<CommandType>
{
    public const string StartCmdName = "/start";

    public static readonly CommandType Start
        = new(1, StartCmdName, "Getting started with the bot", false);

    public const string SellCmdName = "/sell";

    public static readonly CommandType Sell
        = new(2, SellCmdName, "Create a new product");

    public const string FindCmdName = "/find";

    public static readonly CommandType Find
        = new(3, FindCmdName, "Find a product to buy");

    public const string ProfileCmdName = "/profile";

    public static readonly CommandType Profile
        = new(4, ProfileCmdName, "Change your profile settings");

    private CommandType(short id, string name, string description, bool isVisibleInCommandsMenu = true)
        : base(name, id)
        => (Description, IsVisibleInCommandsMenu) = (description, isVisibleInCommandsMenu);

    public string Description { get; }

    public bool IsVisibleInCommandsMenu { get; }
}