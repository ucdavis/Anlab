﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AnlabMvc.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class QueryResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal QueryResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AnlabMvc.Resources.QueryResource", typeof(QueryResource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT        dbo.USERFLDS.LOCCODE AS ClientId, dbo.USERFLDS.CPYEMAIL AS CopyEmail, dbo.USERFLDS.CPYPHONE AS CopyPhoneNumber, dbo.USERFLDS.SUBEMAIL AS EMail, 
        ///                         dbo.USERFLDS.SUBPHONE AS PhoneNumber, dbo.LOCLIST.REPADD1 AS Name
        ///FROM            dbo.USERFLDS INNER JOIN
        ///                         dbo.LOCLIST ON dbo.USERFLDS.LOCCODE = dbo.LOCLIST.LOCCODE
        ///WHERE        dbo.USERFLDS.LOCCODE = @clientId.
        /// </summary>
        internal static string AnlabClientIdLookup {
            get {
                return ResourceManager.GetString("AnlabClientIdLookup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT dbo.USERFLDS.LOCCODE AS ClientId, dbo.USERFLDS.CPYEMAIL AS CopyEmail, dbo.LOCLIST.[INVADD4] as DefaultAccount
        ///FROM dbo.USERFLDS INNER JOIN
        ///dbo.LOCLIST ON dbo.USERFLDS.LOCCODE = dbo.LOCLIST.LOCCODE
        ///WHERE dbo.USERFLDS.SUBEMAIL = @userEmail.
        /// </summary>
        internal static string AnlabDefaultsForUser {
            get {
                return ResourceManager.GetString("AnlabDefaultsForUser", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DECLARE @setup int;
        ///SELECT @setup = APRICE from dbo.ANL_LIST where ACODE = &apos;SETUP&apos;
        ///SELECT[ANL_LIST].[ACODE] as Id,[APRICE] as InternalCost,[ANAME] as &apos;Name&apos;,[WORKUNIT] as Multiplier, @setup as SetupCost , CAST(SUBSTRING(CASNUMB, 0, 4) as INT) as SOP, 
        ///CASE WHEN [NONREP] = &apos;0&apos; THEN 0 ELSE 1 END as NONREP , 
        ///CASE WHEN [NONINV] = &apos;0&apos; THEN 0 ELSE 1 END as NONINV
        ///FROM [ANL_LIST] INNER JOIN [ANALYTES] ON [ANL_LIST].[ACODE] = [ANALYTES].[ACODE]    
        ///where ANL_LIST.ACODE in @codes.
        /// </summary>
        internal static string AnlabItemPrices {
            get {
                return ResourceManager.GetString("AnlabItemPrices", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DECLARE @setup int;
        ///SELECT @setup = APRICE from dbo.ANL_LIST where ACODE = &apos;SETUP&apos;
        ///SELECT[ANL_LIST].[ACODE] as Id,[APRICE] as InternalCost,[ANAME] as &apos;Name&apos;,[WORKUNIT] as Multiplier, @setup as SetupCost , CAST(SUBSTRING(CASNUMB, 0, 4) as INT) as SOP, 
        ///CASE WHEN [NONREP] = &apos;0&apos; THEN 0 ELSE 1 END as NONREP , 
        ///CASE WHEN [NONINV] = &apos;0&apos; THEN 0 ELSE 1 END as NONINV
        ///FROM [ANL_LIST] INNER JOIN [ANALYTES] ON [ANL_LIST].[ACODE] = [ANALYTES].[ACODE]  where [ANL_LIST].[ACODE]  = @code .
        /// </summary>
        internal static string AnlabPriceForCode {
            get {
                return ResourceManager.GetString("AnlabPriceForCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT COUNT(LOCCODE) AS Quantity, LOCCODE as ClientId FROM dbo.SAMPLE
        ///GROUP BY LOGBATCH, LOCCODE 
        ///HAVING (LOGBATCH = @RequestNum).
        /// </summary>
        internal static string AnlabQuantityClientId {
            get {
                return ResourceManager.GetString("AnlabQuantityClientId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT DISTINCT([RUSHYNP]) as RushMultiplier FROM [Labworks5].[dbo].[SUSERFLDS] where RUSHYNP != &apos;N&apos; and WORK_REQ = @RequestNum.
        /// </summary>
        internal static string AnlabRushMultiplierForOrder {
            get {
                return ResourceManager.GetString("AnlabRushMultiplierForOrder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT DISTINCT [ACODE] as Id
        ///FROM            [SAMPLE] INNER JOIN
        ///                         [ANALYSIS] ON [SAMPLE].[SAMPNO] = [ANALYSIS].[SAMPNO]
        ///WHERE        [ACODE] != &apos;()&apos; and [SAMPLE].[LOGBATCH] = @RequestNum.
        /// </summary>
        internal static string AnlabTestsRunForOrder {
            get {
                return ResourceManager.GetString("AnlabTestsRunForOrder", resourceCulture);
            }
        }
    }
}
