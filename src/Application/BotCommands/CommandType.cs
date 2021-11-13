using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Application.BotCommands
{
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

    public abstract class CommandTypeEnumeration
    {
        public int Id { get; }

        public string Name { get; }

        public string Description { get; }

        public Type CommandHandlerType { get; }

        public bool IsVisibleInCommandsMenu { get; }

        protected CommandTypeEnumeration(int id, string name, string description, Type commandHandlerType,
            bool isVisibleInCommandsMenu = true)
            => (Id, Name, Description, CommandHandlerType, IsVisibleInCommandsMenu)
                = (id, name, description, commandHandlerType, isVisibleInCommandsMenu);

        public override string ToString() => Name;

        public static IEnumerable<CommandType> GetAll() =>
            typeof(CommandType).GetFields(BindingFlags.Public |
                                          BindingFlags.Static |
                                          BindingFlags.DeclaredOnly)
                .Select(f => f.GetValue(null))
                .Cast<CommandType>()
                .OrderBy(s => s.Id);
    }
}