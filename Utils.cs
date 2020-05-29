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


    /// <summary>
    /// Best guesses when trawling through code.
    /// </summary>
    public enum ComponentFlags
    {
        HumanInBed =    0x000_0002,     // Combined with 0x40 for normal bed
        Automatic =     0x000_0008,     // In code as this OR base.anyInteractions()
        CanDisable =    0x000_0020,
        InBed =         0x000_0040,     // In sick bay code
        FastRate =      0x000_0800,
        SlowRate =      0x000_0400,
        NeedsDay =      0x000_8000,
        AltProduce =    0x002_0000,     // in GuiInfoPanel
        Consumes =      0x004_0000,     // Or Produces, or getNeededResources?
        HasAnchor =     0x040_0000,
        DisasterWatch = 0x800_0000
    }
}
