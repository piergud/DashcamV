namespace DashcamV
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Rage;
    using Rage.Attributes;
    using Rage.Native;

    public static class Commands
    {
        [ConsoleCommand(Description="Set Dashcam V to use the metric measurement system.")]
        public static void UseMetric()
        {
            DashHelper.units = 0;
            Game.Console.Print("[DASHCAM V] Now using metric system.");
        }

        [ConsoleCommand(Description = "Set Dashcam V to use the imperial measurement system.")]
        public static void UseImperial()
        {
            DashHelper.units = 1;
            Game.Console.Print("[DASHCAM V] Now using imperial system.");
        }

        [ConsoleCommand(Description = "Enable or disable Dashcam V's black and white filter.")]
        public static void UseFilter(bool enable)
        {
            if (enable)
            {
                DashHelper.filter = true;
                Game.Console.Print("[DASHCAM V] Black and white filter enabled.");
            }
            else
            {
                DashHelper.filter = false;
                NativeFunction.CallByHash<uint>(0x0F07E7745A236711);
                Game.Console.Print("[DASHCAM V] Black and white filter disabled.");
            }
        }

        [ConsoleCommand(Description = "Set the unit name to use on the dashcam.")]
        public static void SetUnitName(String name)
        {
            DashHelper.configUnit = name.Replace("%20", " ");
            Game.Console.Print("[DASHCAM V] Set unit name to \"" + name.Replace("%20", " ") + "\".");
        }

        [ConsoleCommand(Description = "Resets the unit names to the default vehicle-unique names.")]
        public static void ResetUnitNames()
        {
            DashHelper.configUnit = "";
            Game.Console.Print("[DASHCAM V] Reset unit names to vehicle-unique names.");
        }

        [ConsoleCommand(Description = "Enable or disable dashcam text on all vehicle view modes.")]
        public static void EnableDashcamOnAllViews(bool enable)
        {
            if (enable)
            {
                DashHelper.dashAllViews = true;
                Game.Console.Print("[DASHCAM V] Dashcam text on all views enabled.");
            }
            else
            {
                DashHelper.dashAllViews = false;
                Game.Console.Print("[DASHCAM V] Dashcam text on all views disabled.");
            }
        }

        [ConsoleCommand(Description = "Set which layout Dashcam V should use (0 - regular, 1 - IVDashCam layout).")]
        public static void SetLayout(int layout) // 0 - regular layout, 1 - sniper296 layout
        {
            if (DashHelper.eventOn && DashHelper.isDashOn)
            {
                if (DashHelper.layout == 1)
                {
                    Game.FrameRender -= DashHelper.OnFrameRenderLayout2;
                }
                else
                {
                    Game.FrameRender -= DashHelper.OnFrameRenderLayout1;
                }
                DashHelper.eventOn = false;
            }

            if (layout == 0)
            {
                DashHelper.layout = 0;
                Game.Console.Print("[DASHCAM V] Set to use regular layout.");
            }
            else if (layout == 1)
            {
                DashHelper.layout = 1;
                Game.Console.Print("[DASHCAM V] Set to use Sniper296's IVDashCam layout.");
            }
            else
            {
                Game.Console.Print("[DASHCAM V] Invalid input.");
                Game.Console.Print("[DASHCAM V] 0 for regular layout, 1 for IVDashCam layout.");
            }

            if (DashHelper.layout == 1 && !DashHelper.eventOn && DashHelper.isDashOn)
            {
                DashHelper.InitializeDashcam(1);
                DashHelper.eventOn = true;
            }
            else if (!DashHelper.eventOn && DashHelper.isDashOn)
            {
                DashHelper.InitializeDashcam(0);
                DashHelper.eventOn = true;
            }
        }
    }
}
