﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MAPeD {
	
	
	[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("ICSharpCode.SettingsEditor.SettingsCodeGeneratorTool", "4.4.2.9749")]
	internal sealed partial class MAPeD_CFG : global::System.Configuration.ApplicationSettingsBase {
		
		private static MAPeD_CFG defaultInstance = ((MAPeD_CFG)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new MAPeD_CFG())));
		
		public static MAPeD_CFG Default {
			get {
				return defaultInstance;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("True")]
		public bool auto_show_description {
			get {
				return ((bool)(this["auto_show_description"]));
			}
			set {
				this["auto_show_description"] = value;
			}
		}
	}
}
