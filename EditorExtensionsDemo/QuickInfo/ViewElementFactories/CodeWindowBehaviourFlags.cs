﻿namespace EditorExtensionsDemo
{
    public enum CodeWindowBehaviourFlags : uint
    {
        CWB_DISABLEDROPDOWNBAR = 1U,
        CWB_DISABLESPLITTER = 2U,
        CWB_DISABLEDIFF = 4U,
        CWB_DISABLESCROLLBARS = 8U,
        CWB_DISABLESTATUSBAR = 16U,
        CWB_DISABLEALL = 31U,
        CWB_DEFAULT = 0U,
    }
}
