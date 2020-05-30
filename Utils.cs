using Planetbase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tahvohck_Mods
{
    public class TahvUtil
    {
        /// <summary>
        /// Logfile gathered from Planetbase's utils since it's there.
        /// </summary>
        public readonly static string Logfile = Util.getFilesFolder() + "/Modhooker.log";

        /// <summary>
        /// Log to both the unity debug log and modhooker.log.
        /// </summary>
        /// <param name="message">Object to log to file. Will be converted to a string via ToString</param>
        /// <param name="context">Unity Context. Largely unneeded, but might be useful if you're using
        /// a debugger.</param>
        public static void Log(object message, UnityEngine.Object context = null)
        {
            string logMessage;
            MethodBase method = new StackFrame(1).GetMethod();
            logMessage = $"[{method.DeclaringType.FullName}] {message}";
            
            // Log to Logfile, if errors are encountered then modify the logMessage.
            try {
                File.AppendAllText(Logfile, logMessage + Environment.NewLine, Encoding.ASCII);
            } catch (Exception ex) {
                logMessage = $"{logMessage}" +
                    $"\n  Error logging this message to the modhooker log: {Logfile}" +
                    $"\n  {ex.Message}";
            }
            UnityEngine.Debug.Log(logMessage, context);
        }

        /// <summary>
        /// Only meant to be used internally to clear the log before use.
        /// </summary>
        internal static void ClearLog()
        {
            File.WriteAllText(Logfile, string.Empty);
        }
    }


    public interface ITahvUtilMod
    {
        void Setup(object caller, EventArgs evArgs);
    }


    /// <summary>
    /// Component flags based on the huge amount of ints in <see cref="Planetbase.ComponentType"/>
    /// </summary>
    public enum ComponentFlags
    {
        Extractor =         0x000_0001,
        Sleeper =           0x000_0002, // Combined with 0x40 for normal bed
        NeedsRepair =       0x000_0004,
        NeedsOperator =     0x000_0008, // In code as this OR base.anyInteractions()

        Relaxing =          0x000_0010,
        CanDisable =        0x000_0020,
        Healing =           0x000_0040, // In sick bay code
        NoPowerNeed =       0x000_0080,

        WallPart =          0x000_0100,
        SlowRate =          0x000_0400,
        FastRate =          0x000_0800,

        Hydrating =         0x000_1000,
        // There's seriously a gap here for no good reasons.
        IsTree =            0x000_8000,

        IsHanging =         0x001_0000,
        AltProduce =        0x002_0000, // in GuiInfoPanel
        AltConsume =        0x004_0000, // Also in GuiInfoPanel
        AwayFromCorridor =  0x008_0000,

        StoreProduct =      0x010_0000,
        QuadrantAutoRot =   0x020_0000, // What the fuck
        AutoAnchor =        0x040_0000,
        HasSparks =         0x080_0000,

        FaceCorridor =      0x100_0000,
        NeedsTracksuit =    0x200_0000,
        UncoveredHead =     0x400_0000,
        DisasterDetector =  0x800_0000
    }


    /// <summary>
    /// Based on the ints in <see cref="Planetbase.ModuleType"/>
    /// </summary>
    public enum ModuleFlags
    {
        LandingPad =        0x00_0001,
        Mine =              0x00_0002,
        Airlock =           0x00_0004,
        Storage =           0x00_0008,
        Dome =              0x00_0010,
        LightAtNight =      0x00_0020,
        NeedsWind =         0x00_0040,
        NeedsLight =        0x00_0080,
        NoFoundation =      0x00_0100,
        DeadEnd =           0x00_0400,
        Walkable =          0x00_0800,
        Blinkenlights =     0x00_1000,
        SnapComponent =     0x00_2000,
        Starport =          0x00_4000,
        Autorotate =        0x00_8000,
        Animated =          0x01_0000,
        CylinderBase =      0x02_0000,
        RemoteOperate =     0x04_0000,
        ScanAnimation =     0x08_0000,
        Prioritizable =     0x10_0000,
        AntiMeteor =        0x20_0000,
        LightningRod =      0x40_0000,
        DisasterDetector =  0x80_0000
    }
}
