﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.235
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Phoenix.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class AccountResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public AccountResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Phoenix.Resources.AccountResources", typeof(AccountResources).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to تایید کلمه عبور جدید.
        /// </summary>
        public static string ConfirmNewPassword {
            get {
                return ResourceManager.GetString("ConfirmNewPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to کلمه عبور فعلی.
        /// </summary>
        public static string CurrentPassword {
            get {
                return ResourceManager.GetString("CurrentPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to کلمه‌عبور فعلی وارد شده اشتباه است..
        /// </summary>
        public static string CurrentPasswordWrong {
            get {
                return ResourceManager.GetString("CurrentPasswordWrong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to تغییر نام مستعار و/یا کلمه‌عبور.
        /// </summary>
        public static string Description {
            get {
                return ResourceManager.GetString("Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to نام.
        /// </summary>
        public static string Name {
            get {
                return ResourceManager.GetString("Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to تکرار کلمه‌عبور جدید با کلمه‌عبور جدید مطابقت ندارد..
        /// </summary>
        public static string NewConfirmPasswordDoestNotMatchWithNewPassword {
            get {
                return ResourceManager.GetString("NewConfirmPasswordDoestNotMatchWithNewPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to کلمه عبور جدید.
        /// </summary>
        public static string NewPassword {
            get {
                return ResourceManager.GetString("NewPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to کلمه‌عبور جدید معتبر نمی باشد. طول کلمه‌عبور حداقل 4 و حداکثر 128 کاراکتر می باشد..
        /// </summary>
        public static string NewPasswordInvalid {
            get {
                return ResourceManager.GetString("NewPasswordInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to پروفایل.
        /// </summary>
        public static string Title {
            get {
                return ResourceManager.GetString("Title", resourceCulture);
            }
        }
    }
}