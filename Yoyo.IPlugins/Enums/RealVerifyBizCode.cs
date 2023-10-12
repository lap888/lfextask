using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Enums
{
    /// <summary>
    /// 认证场景码和用户发起认证的端有关：
    /// 当用户在 iOS 或安卓平台发起认证时，认证场景码是 FACE_SDK。
    /// 当用户在小程序中或 H5 页面中发起认证时，认证场景码是 FACE
    /// </summary>
    public enum RealVerifyBizCode
    {
        FACE_SDK,
        FACE
    }
}
