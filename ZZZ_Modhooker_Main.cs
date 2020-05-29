using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tahvohck_Mods
{
    public class ZZZ_Modhooker : IMod
    {
        public void Init()
        {
            FileLog.logPath = "./harmony.log";
            TahvUtil.ClearLog();
            string ver = FileVersionInfo
                .GetVersionInfo(Assembly.GetExecutingAssembly().Location)
                .FileVersion;
            TahvUtil.Log($"Initialized Modhooker v{ver}");

            // Call all code that needs to have the mInstance lists reset after it runs.
            OnPreResetEventHandler(EventArgs.Empty);

            // By setting these to null, the game will natively restore them during getInstance()
            PlanetList.mInstance = null;
            ModuleTypeList.mInstance = null;
            NameGenerator.mInstance = null; // No issues found. Names generate fine.
            CharacterDefinitions.mInstance = null;  // No issues found. Characters seem fine.
            ExtraQualitySettings.mInstance = null;  // Possible use: More quality settings?


            // Dangerous mInstances that fuck the game up.
            // ResourceList: Buggy in code. Does not check for null.
            // TODO: Harmony patch to un-fuck this?
            //ResourceList.mInstance = new ResourceList();
            // Fucks the planet selection screen for... some reason... It only gets referenced by
            // getInstance() for reads, but by .cctor(), CameraManager()
            // TODO: Find out why???
            //CameraManager.mInstance = null;
            // Issues with these enabled in the game. Will not enable until I have a reason.
            //MixerGroups.mInstance = null;     // Audio mixer. Nothing to Harmonize.
            //ProfileManager.mInstance = null;  // Seems to be some sort of debugger
            //SoundList.mInstance = null;       // No good reason to overload
            //GameManager.mInstance = null;     // A lot of shit in here. See what it does. Looks powerful.

            TahvUtil.Log("Done resetting lists.");
            OnPostResetEventHandler(EventArgs.Empty);
        }

        public void Update()
        {
            // Don't need to do anything here
            return;
        }

        public static event EventHandler PreResetEvent;
        public static event EventHandler PostResetEvent;

        protected virtual void OnPreResetEventHandler(EventArgs e)
        {
            EventHandler handler = PreResetEvent;
            if (!(handler is null)) {
                handler.Invoke(this, e);
            }
        }

        protected virtual void OnPostResetEventHandler(EventArgs e)
        {
            EventHandler handler = PostResetEvent;
            if (!(handler is null)) {
                handler.Invoke(this, e);
            }
        }
    }
}
