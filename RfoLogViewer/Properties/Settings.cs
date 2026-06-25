using System.Configuration;

namespace RfoLogViewer.Properties
{
    internal sealed partial class Settings : ApplicationSettingsBase
    {
        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string LastLogin
        {
            get => (string)this[nameof(LastLogin)];
            set => this[nameof(LastLogin)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string LastDataSource
        {
            get => (string)this[nameof(LastDataSource)];
            set => this[nameof(LastDataSource)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string LastContextId
        {
            get => (string)this[nameof(LastContextId)];
            set => this[nameof(LastContextId)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string LastPassword
        {
            get => (string)this[nameof(LastPassword)];
            set => this[nameof(LastPassword)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool SavePassword
        {
            get => (bool)this[nameof(SavePassword)];
            set => this[nameof(SavePassword)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string LogTableColumnLayout
        {
            get => (string)this[nameof(LogTableColumnLayout)];
            set => this[nameof(LogTableColumnLayout)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string LogStructColumnLayout
        {
            get => (string)this[nameof(LogStructColumnLayout)];
            set => this[nameof(LogStructColumnLayout)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string LogStructColumnVisibility
        {
            get => (string)this[nameof(LogStructColumnVisibility)];
            set => this[nameof(LogStructColumnVisibility)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string LogTableColumnVisibility
        {
            get => (string)this[nameof(LogTableColumnVisibility)];
            set => this[nameof(LogTableColumnVisibility)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("1400")]
        public int MainFormWidth
        {
            get => (int)this[nameof(MainFormWidth)];
            set => this[nameof(MainFormWidth)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("900")]
        public int MainFormHeight
        {
            get => (int)this[nameof(MainFormHeight)];
            set => this[nameof(MainFormHeight)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("380")]
        public int SplitterDistance
        {
            get => (int)this[nameof(SplitterDistance)];
            set => this[nameof(SplitterDistance)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string ExcelLogTableColumnLayout
        {
            get => (string)this[nameof(ExcelLogTableColumnLayout)];
            set => this[nameof(ExcelLogTableColumnLayout)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string ExcelLogTableColumnVisibility
        {
            get => (string)this[nameof(ExcelLogTableColumnVisibility)];
            set => this[nameof(ExcelLogTableColumnVisibility)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("1400")]
        public int ExcelLogViewerFormWidth
        {
            get => (int)this[nameof(ExcelLogViewerFormWidth)];
            set => this[nameof(ExcelLogViewerFormWidth)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("900")]
        public int ExcelLogViewerFormHeight
        {
            get => (int)this[nameof(ExcelLogViewerFormHeight)];
            set => this[nameof(ExcelLogViewerFormHeight)] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("380")]
        public int ExcelLogViewerSplitterDistance
        {
            get => (int)this[nameof(ExcelLogViewerSplitterDistance)];
            set => this[nameof(ExcelLogViewerSplitterDistance)] = value;
        }
    }
}
