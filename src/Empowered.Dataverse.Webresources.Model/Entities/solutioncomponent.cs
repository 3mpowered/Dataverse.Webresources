#pragma warning disable CS1591
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Empowered.Dataverse.Webresources.Model
{
	
	
	/// <summary>
	/// Indicates the include behavior of the root component.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.11")]
	public enum solutioncomponent_rootcomponentbehavior
	{
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		IncludeSubcomponents = 0,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		Donotincludesubcomponents = 1,
		
		[System.Runtime.Serialization.EnumMemberAttribute()]
		IncludeAsShellOnly = 2,
	}
	
	/// <summary>
	/// A component of a CRM solution.
	/// </summary>
	[System.Runtime.Serialization.DataContractAttribute()]
	[Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("solutioncomponent")]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Dataverse Model Builder", "2.0.0.11")]
	public partial class SolutionComponent : Microsoft.Xrm.Sdk.Entity
	{
		
		/// <summary>
		/// Available fields, a the time of codegen, for the solutioncomponent entity
		/// </summary>
		public partial class Fields
		{
			public const string ComponentType = "componenttype";
			public const string CreatedBy = "createdby";
			public const string CreatedOn = "createdon";
			public const string CreatedOnBehalfBy = "createdonbehalfby";
			public const string IsMetadata = "ismetadata";
			public const string ModifiedBy = "modifiedby";
			public const string ModifiedOn = "modifiedon";
			public const string ModifiedOnBehalfBy = "modifiedonbehalfby";
			public const string ObjectId = "objectid";
			public const string RootComponentBehavior = "rootcomponentbehavior";
			public const string RootSolutionComponentId = "rootsolutioncomponentid";
			public const string SolutionComponentId = "solutioncomponentid";
			public const string Id = "solutioncomponentid";
			public const string SolutionId = "solutionid";
			public const string VersionNumber = "versionnumber";
			public const string Referencedsolutioncomponent_parent_solutioncomponent = "Referencedsolutioncomponent_parent_solutioncomponent";
			public const string lk_solutioncomponentbase_createdonbehalfby = "lk_solutioncomponentbase_createdonbehalfby";
			public const string lk_solutioncomponentbase_modifiedonbehalfby = "lk_solutioncomponentbase_modifiedonbehalfby";
			public const string solution_solutioncomponent = "solution_solutioncomponent";
			public const string Referencingsolutioncomponent_parent_solutioncomponent = "solutioncomponent_parent_solutioncomponent";
		}
		
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public SolutionComponent() : 
				base(EntityLogicalName)
		{
		}
		
		public const string EntityLogicalName = "solutioncomponent";
		
		public const string EntityLogicalCollectionName = "solutioncomponentss";
		
		public const string EntitySetName = "solutioncomponents";
		
		/// <summary>
		/// The object type code of the component.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("componenttype")]
		public virtual componenttype? ComponentType
		{
			get
			{
				return ((componenttype?)(EntityOptionSetEnum.GetEnum(this, "componenttype")));
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who created the solution
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdby")]
		public Microsoft.Xrm.Sdk.EntityReference CreatedBy
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("createdby");
			}
		}
		
		/// <summary>
		/// Date and time when the solution was created.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdon")]
		public System.Nullable<System.DateTime> CreatedOn
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("createdon");
			}
		}
		
		/// <summary>
		/// Unique identifier of the delegate user who created the solution.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		public Microsoft.Xrm.Sdk.EntityReference CreatedOnBehalfBy
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("createdonbehalfby");
			}
		}
		
		/// <summary>
		/// Indicates whether this component is metadata or data.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("ismetadata")]
		public System.Nullable<bool> IsMetadata
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<bool>>("ismetadata");
			}
		}
		
		/// <summary>
		/// Unique identifier of the user who last modified the solution.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedby")]
		public Microsoft.Xrm.Sdk.EntityReference ModifiedBy
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("modifiedby");
			}
		}
		
		/// <summary>
		/// Date and time when the solution was last modified.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedon")]
		public System.Nullable<System.DateTime> ModifiedOn
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.DateTime>>("modifiedon");
			}
		}
		
		/// <summary>
		/// Unique identifier of the delegate user who modified the solution.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		public Microsoft.Xrm.Sdk.EntityReference ModifiedOnBehalfBy
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("modifiedonbehalfby");
			}
		}
		
		/// <summary>
		/// Unique identifier of the object with which the component is associated.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("objectid")]
		public System.Nullable<System.Guid> ObjectId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("objectid");
			}
		}
		
		/// <summary>
		/// Indicates the include behavior of the root component.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("rootcomponentbehavior")]
		public virtual solutioncomponent_rootcomponentbehavior? RootComponentBehavior
		{
			get
			{
				return ((solutioncomponent_rootcomponentbehavior?)(EntityOptionSetEnum.GetEnum(this, "rootcomponentbehavior")));
			}
		}
		
		/// <summary>
		/// The parent ID of the subcomponent, which will be a root
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("rootsolutioncomponentid")]
		public System.Nullable<System.Guid> RootSolutionComponentId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("rootsolutioncomponentid");
			}
		}
		
		/// <summary>
		/// Unique identifier of the solution component.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("solutioncomponentid")]
		public System.Nullable<System.Guid> SolutionComponentId
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<System.Guid>>("solutioncomponentid");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("solutioncomponentid")]
		public override System.Guid Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				base.Id = value;
			}
		}
		
		/// <summary>
		/// Unique identifier of the solution.
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("solutionid")]
		public Microsoft.Xrm.Sdk.EntityReference SolutionId
		{
			get
			{
				return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("solutionid");
			}
		}
		
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("versionnumber")]
		public System.Nullable<long> VersionNumber
		{
			get
			{
				return this.GetAttributeValue<System.Nullable<long>>("versionnumber");
			}
		}
		
		/// <summary>
		/// 1:N solutioncomponent_parent_solutioncomponent
		/// </summary>
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("solutioncomponent_parent_solutioncomponent", Microsoft.Xrm.Sdk.EntityRole.Referenced)]
		public System.Collections.Generic.IEnumerable<Empowered.Dataverse.Webresources.Model.SolutionComponent> Referencedsolutioncomponent_parent_solutioncomponent
		{
			get
			{
				return this.GetRelatedEntities<Empowered.Dataverse.Webresources.Model.SolutionComponent>("solutioncomponent_parent_solutioncomponent", Microsoft.Xrm.Sdk.EntityRole.Referenced);
			}
			set
			{
				this.SetRelatedEntities<Empowered.Dataverse.Webresources.Model.SolutionComponent>("solutioncomponent_parent_solutioncomponent", Microsoft.Xrm.Sdk.EntityRole.Referenced, value);
			}
		}
		
		/// <summary>
		/// N:1 lk_solutioncomponentbase_createdonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("createdonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_solutioncomponentbase_createdonbehalfby")]
		public Empowered.Dataverse.Webresources.Model.SystemUser lk_solutioncomponentbase_createdonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Empowered.Dataverse.Webresources.Model.SystemUser>("lk_solutioncomponentbase_createdonbehalfby", null);
			}
		}
		
		/// <summary>
		/// N:1 lk_solutioncomponentbase_modifiedonbehalfby
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("modifiedonbehalfby")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("lk_solutioncomponentbase_modifiedonbehalfby")]
		public Empowered.Dataverse.Webresources.Model.SystemUser lk_solutioncomponentbase_modifiedonbehalfby
		{
			get
			{
				return this.GetRelatedEntity<Empowered.Dataverse.Webresources.Model.SystemUser>("lk_solutioncomponentbase_modifiedonbehalfby", null);
			}
		}
		
		/// <summary>
		/// N:1 solution_solutioncomponent
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("solutionid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("solution_solutioncomponent")]
		public Empowered.Dataverse.Webresources.Model.Solution solution_solutioncomponent
		{
			get
			{
				return this.GetRelatedEntity<Empowered.Dataverse.Webresources.Model.Solution>("solution_solutioncomponent", null);
			}
		}
		
		/// <summary>
		/// N:1 solutioncomponent_parent_solutioncomponent
		/// </summary>
		[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("rootsolutioncomponentid")]
		[Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute("solutioncomponent_parent_solutioncomponent", Microsoft.Xrm.Sdk.EntityRole.Referencing)]
		public Empowered.Dataverse.Webresources.Model.SolutionComponent Referencingsolutioncomponent_parent_solutioncomponent
		{
			get
			{
				return this.GetRelatedEntity<Empowered.Dataverse.Webresources.Model.SolutionComponent>("solutioncomponent_parent_solutioncomponent", Microsoft.Xrm.Sdk.EntityRole.Referencing);
			}
		}
	}
}
#pragma warning restore CS1591
