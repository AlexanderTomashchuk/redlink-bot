using System;
using Application.Workflows.ChatMemberUpdated;
using Application.Workflows.CreateProduct;
using Application.Workflows.FindProduct;
using Application.Workflows.Profile;
using Application.Workflows.Start;
using Application.Workflows.UnknownUpdate;
using Ardalis.SmartEnum;

namespace Application.Workflows;

public sealed class WorkflowType : SmartEnum<WorkflowType>
{
    public static readonly WorkflowType Start = new(1, nameof(Start), typeof(StartWorkflow));

    public static readonly WorkflowType CreateProduct = new(2, nameof(CreateProduct), typeof(CreateProductWorkflow));

    public static readonly WorkflowType FindProduct = new(3, nameof(FindProduct), typeof(FindProductWorkflow));

    public static readonly WorkflowType EditProfile = new(4, nameof(EditProfile), typeof(EditProfileWorkflow));

    public static readonly WorkflowType DemandCountry = new(98, nameof(DemandCountry), typeof(DemandCountryWorkflow));

    public static readonly WorkflowType ChatMemberUpdated =
        new(99, nameof(ChatMemberUpdated), typeof(ChatMemberUpdatedWorkflow));

    public static readonly WorkflowType Unknown = new(100, nameof(Unknown), typeof(UnknownUpdateWorkflow));

    private WorkflowType(short id, string name, Type typeOfWorkflow)
        : base(name, id)
    {
        TypeOfWorkflow = typeOfWorkflow;
    }

    public Type TypeOfWorkflow { get; }
}