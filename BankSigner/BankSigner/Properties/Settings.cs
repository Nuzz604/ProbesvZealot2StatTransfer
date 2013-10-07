namespace BankSigner.Properties
{
    using System;
    using System.CodeDom.Compiler;
    using System.Configuration;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    [CompilerGenerated, GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed class Settings : ApplicationSettingsBase
    {
        private static Settings defaultInstance = ((Settings) SettingsBase.Synchronized(new Settings()));

        [DefaultSettingValue("1-S2-1-1234567"), UserScopedSetting, DebuggerNonUserCode]
        public string AAText
        {
            get
            {
                return (string) this["AAText"];
            }
            set
            {
                this["AAText"] = value;
            }
        }

        [DefaultSettingValue("No bank opened yet."), UserScopedSetting, DebuggerNonUserCode]
        public string BDText
        {
            get
            {
                return (string) this["BDText"];
            }
            set
            {
                this["BDText"] = value;
            }
        }

        [UserScopedSetting, DefaultSettingValue("Bank"), DebuggerNonUserCode]
        public string BNText
        {
            get
            {
                return (string) this["BNText"];
            }
            set
            {
                this["BNText"] = value;
            }
        }

        public static Settings Default
        {
            get
            {
                return defaultInstance;
            }
        }

        [DebuggerNonUserCode, DefaultSettingValue(""), UserScopedSetting]
        public string FilePath
        {
            get
            {
                return (string) this["FilePath"];
            }
            set
            {
                this["FilePath"] = value;
            }
        }

        [DefaultSettingValue("1-S2-1-1234567"), UserScopedSetting, DebuggerNonUserCode]
        public string UAText
        {
            get
            {
                return (string) this["UAText"];
            }
            set
            {
                this["UAText"] = value;
            }
        }

        [DebuggerNonUserCode, DefaultSettingValue("0, 0"), UserScopedSetting]
        public Point WindowPos
        {
            get
            {
                return (Point) this["WindowPos"];
            }
            set
            {
                this["WindowPos"] = value;
            }
        }

        [DefaultSettingValue("501, 530"), DebuggerNonUserCode, UserScopedSetting]
        public Size WindowSize
        {
            get
            {
                return (Size) this["WindowSize"];
            }
            set
            {
                this["WindowSize"] = value;
            }
        }
    }
}

