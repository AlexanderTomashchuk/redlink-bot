using System;
using System.Collections.Generic;
using Application.Common;
using Application.Workflows.CreateProduct;
using Application.Workflows.Profile;

namespace Application.Workflows;

public class WorkflowType : WorkflowTypeEnumeration
{
    public static readonly WorkflowType Start = new(1, "Start", typeof(StartWorkflow));

    public static readonly WorkflowType CreateProduct = new(2, "CreateProduct", typeof(CreateProductWorkflow));
    
    public static readonly WorkflowType FindProduct = new(3, "FindProduct", typeof(FindProductWorkflow));

    public static readonly WorkflowType EditProfile = new(3, "EditProfile", typeof(EditProfileWorkflow));

    public static readonly WorkflowType DemandCountry = new(98, "DemandCountry", typeof(DemandCountryWorkflow));

    public static readonly WorkflowType ChatMemberUpdated =
        new(99, "ChatMemberUpdated", typeof(ChatMemberUpdatedWorkflow));

    public static readonly WorkflowType Unknown = new(100, "Unknown", typeof(UnknownUpdateWorkflow));

    private WorkflowType(int id, string name, Type workflowType)
        : base(id, name, workflowType)
    {
    }
}

public abstract class WorkflowTypeEnumeration : Enumeration
{
    public Type WorkflowType { get; }

    protected WorkflowTypeEnumeration(int id, string name, Type workflowType) : base(id, name) =>
        (WorkflowType) = (workflowType);

    public static IEnumerable<WorkflowType> GetAll() => GetAll<WorkflowType>();
}