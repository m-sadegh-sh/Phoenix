﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
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
    public class SplashResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public SplashResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Phoenix.Resources.SplashResources", typeof(SplashResources).Assembly);
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
        ///   Looks up a localized string similar to امکان آماده‌سازی پایگاه ‌داده وجود ندارد..
        /// </summary>
        public static string DatabaseInitFailed {
            get {
                return ResourceManager.GetString("DatabaseInitFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to تست برقراری ارتباط با پایگاه داده{0}.
        /// </summary>
        public static string DatabaseTest {
            get {
                return ResourceManager.GetString("DatabaseTest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to امکان برقراری ارتباط با پایگاه داده وجود ندارد..
        /// </summary>
        public static string DatabaseTestFailed {
            get {
                return ResourceManager.GetString("DatabaseTestFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to لطفا صبر کنید{0}.
        /// </summary>
        public static string PleaseWait {
            get {
                return ResourceManager.GetString("PleaseWait", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to تست/ثبت رکوردهای مورد نیاز.
        /// </summary>
        public static string TestCheckIndeedRecords {
            get {
                return ResourceManager.GetString("TestCheckIndeedRecords", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to امکان ایجاد رکوردهای مورد نیاز وجود ندارد..
        /// </summary>
        public static string TestCheckIndeedRecordsFailed {
            get {
                return ResourceManager.GetString("TestCheckIndeedRecordsFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to با عرض پوزش، ققنوس امکان ادامه اجرا را ندارد..
        /// </summary>
        public static string UnhandledExceptionOcurred {
            get {
                return ResourceManager.GetString("UnhandledExceptionOcurred", resourceCulture);
            }
        }
    }
}