namespace DashcamV
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Drawing;
    using Rage;

    internal static class Configs
    {
        private static KeyToggle _remKeys;
        private static ButtonToggle _remBtns;

        public static void RunConfigCheck()
        {
            InitializationFile iniFile = new InitializationFile(@"Plugins\DashcamV.ini");
            if (!iniFile.Exists())
            {
                iniFile.Create();
                iniFile.Write("SETTINGS", "EnableFilter", false);
                iniFile.Write("SETTINGS", "EnableRemoteView", true);
                iniFile.Write("SETTINGS", "EnableDashcamOnAllViews", false);
                iniFile.Write("SETTINGS", "LayoutStyle", 0);
                iniFile.Write("SETTINGS", "MeasurementSystem", 1);
                iniFile.Write("SETTINGS", "DateFormat", 0);
                iniFile.Write("SETTINGS", "UnitName", "");
                iniFile.Write("CONTROLS", "RemoteViewToggleKey", Keys.E);
                iniFile.Write("CONTROLS", "RemoteViewToggleGamepad", ControllerButtons.LeftThumb);
            }

            Filter = iniFile.ReadBoolean("SETTINGS", "EnableFilter");
            DashTextOnPOVOnly = iniFile.ReadBoolean("SETTINGS", "EnableDashcamOnAllViews");
            Layout = iniFile.ReadEnum<LayoutType>("SETTINGS", "LayoutStyle", 0);
            System = iniFile.ReadEnum<MeasurementType>("SETTINGS", "MeasurementSystem", 0);
            DateFormat = iniFile.ReadString("SETTINGS", "DateFormat");
            Unit = iniFile.ReadString("SETTINGS", "UnitName");

            _remKeys.Key = iniFile.ReadEnum<Keys>("CONTROLS", "RemoteViewToggleKey", Keys.E);
            _remBtns.Button = iniFile.ReadEnum<ControllerButtons>("CONTROLS", "RemoteViewToggleGamepad", ControllerButtons.LeftThumb);

            if (!iniFile.ReadBoolean("SETTINGS", "EnableRemoteView", true))
            {
                _remKeys.Key = Keys.None;
                _remBtns.Button = ControllerButtons.None;
            }

            var sections = iniFile.GetSectionNames();
            foreach (var section in sections)
            {
                if (section.Substring(0, 4).ToLower() == "veh:")
                {
                    var vehName = section.Substring(4).ToLower();

                    var depName = iniFile.ReadString(section, "Department");
                    var depShort = iniFile.ReadString(section, "Acronym");

                    var arrColour = iniFile.ReadString(section, "Colour").Split(',');
                    var colour = Color.FromArgb(255, int.Parse(arrColour[0]), int.Parse(arrColour[1]), int.Parse(arrColour[2]));

                    var arrOffset = iniFile.ReadString(section, "Offset").Split(',');
                }
            }
            confVehs = temp.ToArray();

            List<String> temp_confDepName= new List<String>();
            List<String> temp_confDepAbrv = new List<String>();
            List<String> temp_confDepNameR = new List<String>();
            List<String> temp_confDepNameG = new List<String>();
            List<String> temp_confDepNameB = new List<String>();
            List<String> temp_confOffset = new List<String>();
            for (int i = 0; i < confVehs.Length; i++)
            {
                temp_confDepName.Add(iniFile.ReadString("Veh:" + confVehs[i], "Department", "Generic Department"));
                temp_confDepAbrv.Add(iniFile.ReadString("Veh:" + confVehs[i], "Acronym", "GD"));
                String[] rgb = iniFile.ReadString("Veh:" + confVehs[i], "Colour", "255,255,255").Split(',');
                temp_confDepNameR.Add(rgb[0]);
                temp_confDepNameG.Add(rgb[1]);
                temp_confDepNameB.Add(rgb[2]);
                temp_confOffset.Add(iniFile.ReadString("Veh:" + confVehs[i], "Offset", "0,0.75,0.65"));
            }
            confDepName = temp_confDepName.ToArray();
            confDepAbrv = temp_confDepAbrv.ToArray();
            confDepNameR = temp_confDepNameR.ToArray();
            confDepNameG = temp_confDepNameG.ToArray();
            confDepNameB = temp_confDepNameB.ToArray();
            confOffset = temp_confOffset.ToArray();
        }

        public struct KeyToggle
        {
            public Keys Key;
            public Keys Modifier;
        }

        public struct ButtonToggle
        {
            public ControllerButtons Button;
            public ControllerButtons Modifier;
        }

        public static KeyToggle RemoteKeys { get { return _remKeys; } }

        public static ButtonToggle RemoteButtons { get { return _remBtns; } }

        public static string Unit { get; set; }

        public static MeasurementType System { get; set; }

        public static LayoutType Layout {  get; set; }

        public static bool Filter { get; set; }

        public static bool DashTextOnPOVOnly { get; set; }

        public static string DateFormat { get; set; }

        public static List<VehicleInfo> Vehicles { get; private set; }
    }

    internal struct VehicleInfo
    {
        public readonly string ModelName;
        public readonly uint ModelHash;
        public readonly Vector3 CamOffset;
        public readonly Department Dep;

        public VehicleInfo(string modelName, string depName, string depShortName, Color colour, Vector3 offset)
        {
            this.ModelName = modelName;
            this.ModelHash = Game.GetHashKey(modelName);
            this.CamOffset = offset;
            this.Dep = new Department(depName, depShortName, colour);
        }
    }

    internal struct Department
    {
        public readonly string Name, ShortName;
        public readonly Color Colour;

        public Department(string name, string shortName, Color colour)
        {
            this.Name = name;
            this.ShortName = shortName;
            this.Colour = colour;
        }
    }

    internal enum MeasurementType
    {
        Imperial,
        Metric
    }

    internal enum LayoutType
    {
        Lore,
        IVDashCam,
        Realistic
    }
}
