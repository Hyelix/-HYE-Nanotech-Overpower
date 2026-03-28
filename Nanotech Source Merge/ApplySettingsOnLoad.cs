using System;
using Verse;

namespace Nanotech
{
    [StaticConstructorOnStartup]
    public static class ApplySettingsOnLoad
    {
        static ApplySettingsOnLoad()
        {
            LongEventHandler.ExecuteWhenFinished(ApplySettingsToDefs.Apply);
        }
    }
}
