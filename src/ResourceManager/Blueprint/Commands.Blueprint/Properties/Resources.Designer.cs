﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.Blueprint.Properties {
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.Azure.Commands.Blueprint.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to An assignment with name &apos;{0}&apos; in Subscription &apos;{1}&apos; already exists. Please use Set-AzBlueprintAssignment to update an existing assignment..
        /// </summary>
        internal static string AssignmentExists {
            get {
                return ResourceManager.GetString("AssignmentExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Blueprint assignment &apos;{0}&apos; could not be found within Subscription &apos;{1}&apos;..
        /// </summary>
        internal static string BlueprintAssignmentNotFoun {
            get {
                return ResourceManager.GetString("BlueprintAssignmentNotFoun", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Creating blueprint assignment &apos;{0}&apos;.
        /// </summary>
        internal static string CreateAssignmentShouldProcessString {
            get {
                return ResourceManager.GetString("CreateAssignmentShouldProcessString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deleting blueprint assignment &apos;{0}&apos; in Subscription &apos;{1}&apos;.
        /// </summary>
        internal static string DeleteAssignmentShouldProcessString {
            get {
                return ResourceManager.GetString("DeleteAssignmentShouldProcessString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to We can&apos;t find any Management Groups that you have access to..
        /// </summary>
        internal static string ManagementGroupNotFound {
            get {
                return ResourceManager.GetString("ManagementGroupNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find any Blueprints assignments within &apos;{0}&apos;. Please check the Subscription Id and try again..
        /// </summary>
        internal static string NoBlueprintAssignmentsNotFound {
            get {
                return ResourceManager.GetString("NoBlueprintAssignmentsNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find any Blueprints..
        /// </summary>
        internal static string NoBlueprintsFound {
            get {
                return ResourceManager.GetString("NoBlueprintsFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Published Blueprint &apos;{0}&apos; could not be found in management group &apos;{1}&apos;..
        /// </summary>
        internal static string PublishedBlueprintNotFound {
            get {
                return ResourceManager.GetString("PublishedBlueprintNotFound", resourceCulture);
            }
        }
    }
}
