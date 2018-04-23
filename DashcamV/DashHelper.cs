namespace DashcamV
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Rage;
    using Rage.Native;
    using System.Drawing;

    internal static class DashHelper
    {
        public static String configUnit = "";
        public static int layout = 0; // 0 - regular layout (default), 1 - sniper296 layout
        public static int dateForm = 0; // 0 - MM/dd/yy (default), 1 - dd/MM/yy, 2 - yy/MM/dd
        public static int units = 0; // 0 - metric, 1 - imperial (default)
        public static bool filter = false;
        public static bool dashAllViews = false;
        static String date = "00/00/00", unit = "UNIT 876", department = "null", acronym = "null";
        static int colR = 0, colG = 0, colB = 0;
        public static bool isDashOn = false;
        static DashVehicle veh = null;
        public static bool eventOn = false;

        public static void DrawDashcam(DashVehicle vehicle, bool draw)
        {
            if (draw)
            {
                veh = vehicle;
                GetDate(dateForm);
                unit = vehicle.GetUnit();
                for (int i = 0; i < Configs.confVehs.Length; i++)
                {
                    uint vehHash = veh.Model.Hash;
                    if (vehHash == StringToHash(Configs.confVehs[i].ToLower()))
                    {
                        department = Configs.confDepName[i];
                        acronym = Configs.confDepAbrv[i];
                        colR = Int32.Parse(Configs.confDepNameR[i]);
                        colG = Int32.Parse(Configs.confDepNameG[i]);
                        colB = Int32.Parse(Configs.confDepNameB[i]);
                        break;
                    }
                    else
                    {
                        if (i == Configs.confVehs.Length - 1)
                        {
                            department = "Generic Department";
                            acronym = "GD";
                            colR = 255;
                            colG = 255;
                            colB = 255;
                        }
                    }
                }

                if (filter)
                {
                    NativeFunction.CallByHash<uint>(0x2C933ABF17A1DF41, "phone_cam2");
                }
                if (layout == 1 && !eventOn)
                {
                    InitializeDashcam(1);
                    eventOn = true;
                }
                else if (!eventOn)
                {
                    InitializeDashcam(0);
                    eventOn = true;
                }
                isDashOn = true;
            }
            else
            {
                if (isDashOn)
                {
                    if (filter)
                    {
                        NativeFunction.CallByHash<uint>(0x0F07E7745A236711);
                    }
                    if (eventOn)
                    {
                        if (layout == 1)
                        {
                            Game.FrameRender -= OnFrameRenderLayout2;
                        }
                        else
                        {
                            Game.FrameRender -= OnFrameRenderLayout1;
                        }
                        eventOn = false;
                    }
                    isDashOn = false;
                }
            }
        }

        public static void InitializeDashcam(int layout)
        {
            if (layout == 1)
            {
                Game.FrameRender += OnFrameRenderLayout2;
            }
            else
            {
                Game.FrameRender += OnFrameRenderLayout1;
            }
        }

        public static uint StringToHash(String s)
        {
            uint hash = 0;
            for (int i = 0; i < s.Length; i++)
            {
                hash += s[i];
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }
            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);
            return hash;
        }

        public static void GetDate(int form) // 0 - MM/dd/yy (default), 1 - dd/MM/yy, 2 - yy/MM/dd
        {
            if (form == 1)
            {
                date = DateTime.Now.ToString("dd/MM/yy");
            }
            else if (form == 2)
            {
                date = DateTime.Now.ToString("yy/MM/dd");
            }
            else
            {
                date = DateTime.Now.ToString("MM/dd/yy");
            }
        }

        public static void OnFrameRenderLayout1(object sender, GraphicsEventArgs e)
        {
            // realistic dashcam layout
            String hours = NativeFunction.CallByName<int>("GET_CLOCK_HOURS").ToString();
            String mins = NativeFunction.CallByName<int>("GET_CLOCK_MINUTES").ToString();
            if (hours.Length == 1)
            {
                hours = "0" + hours;
            }
            if (mins.Length == 1)
            {
                mins = "0" + mins;
            }
            DrawText(veh.randomNum1 + " " + veh.randomNum2, 0.08f, 0.05f, 0.9f, false, false, 0f, true, Color.FromArgb(255, 255, 255, 255));
            DrawText(date.Substring(0, 5) + " " + veh.GetStats(), 0.08f, 0.1f, 0.9f, false, false, 0f, true, Color.FromArgb(255, 255, 255, 255));
            DrawText(unit, 0.743f, 0.05f, 0.9f, false, false, 0f, true, Color.FromArgb(255, 255, 255, 255));
            DrawText(hours + ":" + mins + ":00", 0.805f, 0.1f, 0.9f, false, true, 0.916f, true, Color.FromArgb(255, 255, 255, 255));
            DrawText("M1 M2", 0.8348f, 0.15f, 0.9f, false, false, 0f, true, Color.FromArgb(255, 255, 255, 255));
            DrawText("This vehicle camera is licensed to the", 0.5f, 0.786f, 0.25f, true, false, 0f, true, Color.FromArgb(255, 255, 255, 255));
            if (filter)
            {
                float gsCol = (0.21f * colR) + (0.72f * colG)  + (0.07f * colB);
                DrawText(department, 0.5f, 0.80f, 0.7f, true, false, 0f, true, Color.FromArgb(255, (int)gsCol, (int)gsCol, (int)gsCol));
            }
            else
            {
                DrawText(department, 0.5f, 0.80f, 0.7f, true, false, 0f, true, Color.FromArgb(255, colR, colG, colB));
            }
            DrawText("Any unauthorized use is subject to heavy penalty under 13 S.A. Pen. Code 502(a).", 0.5f, 0.846f, 0.25f, true, false, 0f, true, Color.FromArgb(255, 255, 255, 255));
        }

        public static void OnFrameRenderLayout2(object sender, GraphicsEventArgs e)
        {
            // classic Sniper296's IVDashCam layout
            float rectWidth = 0.13125f;
            float rectPosX = 0.12642f;
            float rectPixWidth = Game.Resolution.Width * rectWidth;
            float rectPosX2 = rectPosX - ((rectPixWidth / 2) / Game.Resolution.Width);
            SizeF size = new SizeF();
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(new Bitmap(1, 1)))
            {
                size = graphics.MeasureString(unit, new Font("Arial", 36, FontStyle.Bold, GraphicsUnit.Pixel));
            }

            if (size.Width > rectPixWidth)
            {
                rectWidth += (size.Width / Game.Resolution.Width) - rectWidth + 0.01f;
                rectPixWidth = Game.Resolution.Width * rectWidth;
                rectPosX = rectPosX2 + ((rectPixWidth / 2) / Game.Resolution.Width);
            }

            String hours = NativeFunction.CallByName<int>("GET_CLOCK_HOURS").ToString();
            String mins = NativeFunction.CallByName<int>("GET_CLOCK_MINUTES").ToString();
            if (hours.Length == 1)
            {
                hours = "0" + hours;
            }
            if (mins.Length == 1)
            {
                mins = "0" + mins;
            }

            // upper left panel
            DrawRect(rectPosX, 0.12111f, rectWidth, 0.12111f, 10, 10, 10, 128);
            DrawText(date, 0.06111f, 0.06f, 0.65f, false, false, 0f, false, Color.FromArgb(255, 255, 255, 255));
            DrawText(unit, 0.06111f, 0.097f, 0.65f, false, false, 0f, false, Color.FromArgb(255, 255, 255, 255));
            DrawText("000%", 0.06111f, 0.134f, 0.65f, false, false, 0f, false, Color.FromArgb(255, 255, 255, 255));
            // middle panel
            DrawRect(0.5f, 0.08425f, 0.10156f, 0.04814f, 10, 10, 10, 128);
            DrawText("MANUAL", 0.459f, 0.06f, 0.65f, false, false, 0f, false, Color.FromArgb(255, 255, 255, 255));
            // upper right panel
            DrawRect(0.87395f, 0.120f, 0.13125f, 0.125f, 10, 10, 10, 128);
            DrawText(hours + ":" + mins + ":00", 0.857f, 0.06f, 0.65f, false, true, 0.939f, false, Color.FromArgb(255, 255, 255, 255));
            DrawText("0" + veh.randomNum1 + veh.GetStats(), 0.845f, 0.097f, 0.65f, false, true, 0.939f, false, Color.FromArgb(255, 255, 255, 255));
            DrawText(acronym, 0.857f, 0.134f, 0.65f, false, true, 0.939f, false, Color.FromArgb(255, 255, 255, 255));
            // speed panel
            DrawRect(0.86354f, 0.81296f, 0.15208f, 0.04722f, 10, 10, 10, 255);
            DrawText("o:", 0.788f, 0.788f, 0.65f, false, false, 0f, false, Color.FromArgb(255, 255, 255, 255));
            DrawText(veh.GetCurrentSpeed(units), 0f, 0.788f, 0.65f, false, true, 0.94f, false, Color.FromArgb(255, 255, 255, 255));
        }

        private static void DrawText(String text, float x, float y, float scale, bool centre, bool rightJust, float wrapEnd, bool shadow, Color fcolor)
        {
            NativeFunction.CallByName<uint>("SET_TEXT_FONT", 0);
            NativeFunction.CallByName<uint>("SET_TEXT_SCALE", scale, scale);
            NativeFunction.CallByName<uint>("SET_TEXT_COLOUR", (int)fcolor.R, (int)fcolor.G, (int)fcolor.B, (int)fcolor.A);
            NativeFunction.CallByHash<uint>(0x038C1F517D7FDCF8, false);
            NativeFunction.CallByHash<uint>(0xC02F4DBFB51D988B, centre);
            NativeFunction.CallByHash<uint>(0x6B3C4650BC8BEE47, rightJust);
            if (!rightJust)
            {
                NativeFunction.CallByName<uint>("SET_TEXT_WRAP", 0.0f, 1.0f);
            }
            else
            {
                NativeFunction.CallByName<uint>("SET_TEXT_WRAP", 0.0f, wrapEnd);
            }
            if (!shadow)
            {
                NativeFunction.CallByName<uint>("SET_TEXT_DROPSHADOW", 0, 0, 0, 0, 0);
            }
            else
            {
                NativeFunction.CallByName<uint>("SET_TEXT_DROPSHADOW", 100, 0, 0, 0, 0);
            }
            NativeFunction.CallByName<uint>("SET_TEXT_EDGE", 1, 0, 0, 0, 205);
            NativeFunction.CallByName<uint>("_SET_TEXT_ENTRY", "STRING");
            NativeFunction.CallByName<uint>("_ADD_TEXT_COMPONENT_STRING", text);
            NativeFunction.CallByName<uint>("_DRAW_TEXT", x, y);
        }

        private static void DrawRect(float x1, float y1, float x2, float y2, int r, int g, int b, int a)
        {
            NativeFunction.CallByName<uint>("DRAW_RECT", x1, y1, x2, y2, r, g, b, a);
        }
    }
}
