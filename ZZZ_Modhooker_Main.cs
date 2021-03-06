﻿using HarmonyLib;
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
            //OnPreResetEventHandler(EventArgs.Empty);
            RunEvent(PreResetEvent);

            // By setting these to null, the game will natively restore them during getInstance()
            NameGenerator.mInstance = null; // No issues found. Names generate fine.
            CharacterDefinitions.mInstance = null;  // No issues found. Characters seem fine.
            ExtraQualitySettings.mInstance = null;  // Possible use: More quality settings?

            // These all derive from TypeList
            AchievementList.mInstance = null;
            ComponentTypeList.mInstance = null;
            ConditionTypeList.mInstance = null;
            MilestoneList.mInstance = null;
            PlanetList.mInstance = null;
            ModuleTypeList.mInstance = null;



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
            //OnPostResetEventHandler(EventArgs.Empty);
            RunEvent(PostResetEvent);

            // Finally, call all mods that don't care about list reset.
            RunEvent(UtilsReadyEvent);
        }

        public void Update()
        {
            // Don't need to do anything here
            return;
        }

        public static event EventHandler PreResetEvent;
        public static event EventHandler PostResetEvent;
        public static event EventHandler UtilsReadyEvent;

        protected virtual void RunEvent(EventHandler handler)
        {
            foreach (Delegate del in handler?.GetInvocationList()) { Invoke(del, EventArgs.Empty); }
        }

        protected virtual void OnPreResetEventHandler(EventArgs e)
        {
            EventHandler handler = PreResetEvent;
            foreach (Delegate del in handler?.GetInvocationList()) { Invoke(del, e); }
        }

        protected virtual void OnPostResetEventHandler(EventArgs e)
        {
            EventHandler handler = PostResetEvent;
            foreach (Delegate del in handler?.GetInvocationList()) { Invoke(del, e); }
        }

        /// <summary>
        /// Handle invoking a delagte from generic EventHandlers.
        /// </summary>
        /// <param name="del">Delegate to invoke.</param>
        /// <param name="evArgs">EventArgs to pass.</param>
        /// <returns></returns>
        internal bool Invoke(Delegate del, EventArgs evArgs)
        {
            try {
                del.DynamicInvoke(new object[] { this, evArgs });
                return true;
            } catch (TargetInvocationException ex) {
                MethodInfo m = del.Method;
                TahvUtil.Log($"{ex.Message} [{m.DeclaringType.FullName}.{m.Name}()]" +
                    $"\n  Inner exception: {ex.InnerException.Message}");
                return false;
            } catch (MemberAccessException ex) {
                MethodInfo m = del.Method;
                TahvUtil.Log($"Issue while invoking [{m.DeclaringType.FullName}.{m.Name}()]: {ex.Message}");
                return false;
            } catch (ArgumentException ex) {
                TahvUtil.Log(ex.Message);
                return false;
            }

        }
    }
}
