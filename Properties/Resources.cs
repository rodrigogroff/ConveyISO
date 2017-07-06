// Decompiled with JetBrains decompiler
// Type: ConveyISO.Properties.Resources
// Assembly: ConveyISO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A1A29DB8-D4AD-4F47-B8FA-3FADEED7E861
// Assembly location: C:\Users\rodrigo.groff\Desktop\ciso\ConveyISO.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace ConveyISO.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
  [CompilerGenerated]
  [DebuggerNonUserCode]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (ConveyISO.Properties.Resources.resourceMan == null)
          ConveyISO.Properties.Resources.resourceMan = new ResourceManager("ConveyISO.Properties.Resources", typeof (ConveyISO.Properties.Resources).Assembly);
        return ConveyISO.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get
      {
        return ConveyISO.Properties.Resources.resourceCulture;
      }
      set
      {
        ConveyISO.Properties.Resources.resourceCulture = value;
      }
    }

    internal Resources()
    {
    }
  }
}
