namespace DashcamV
{
    using System;
    using Rage;
    using Rage.Native;

    internal class DashVehicle : Vehicle
    {
        public String assignedUnit = "";
        public String randomNum1 = "0" + new Random().Next(10001, 30000).ToString();
        public String randomNum2 = "1:" + new Random().Next(11, 50).ToString();

        public DashVehicle(Model model, Vector3 position) : base(model, position)
        {
        }

        public DashVehicle(Model model, Vector3 position, float heading) : base(model, position, heading)
        {
        }

        protected internal DashVehicle(PoolHandle handle) : base(handle)
        {
        }

        public string Unit
        {
            get; set;
        }

        public void SetDashview(Camera cam)
        {
            for (int i = 0; i < Configs.confVehs.Length; i++)
            {
                uint vehHash = this.Model.Hash;
                if (vehHash == DashHelper.StringToHash(Configs.confVehs[i].ToLower()))
                {
                    String[] offs = Configs.confOffset[i].Split(',');
                    NativeFunction.CallByName<uint>("ATTACH_CAM_TO_ENTITY", cam, this, float.Parse(offs[0]), float.Parse(offs[1]), float.Parse(offs[2]), 1);
                    break;
                }
                else
                {
                    if (i == Configs.confVehs.Length - 1)
                    {
                        NativeFunction.CallByName<uint>("ATTACH_CAM_TO_ENTITY", cam, this, 0f, 1.3f, 1f, 1);
                    }
                }
            }

            //String name = this.Model.Name.ToLower();
            /*if (name == "sheriff2" || name == "fbi2" || name == "lguard" || name == "pranger")
            {
                NativeFunction.CallByName<uint>("ATTACH_CAM_TO_ENTITY", cam, this, 0f, 1.3f, 1f, 1);
            }
            else if (name == "ambulance" || name == "policet")
            {
                NativeFunction.CallByName<uint>("ATTACH_CAM_TO_ENTITY", cam, this, 0f, 1.5f, 1.3f, 1);
            }
            else if (name == "policeold1")
            {
                NativeFunction.CallByName<uint>("ATTACH_CAM_TO_ENTITY", cam, this, 0f, 0.8f, 1f, 1);
            }
            else if (name == "firetruk")
            {
                NativeFunction.CallByName<uint>("ATTACH_CAM_TO_ENTITY", cam, this, 0f, 4f, 0.8f, 1);
            }
            else if (name == "riot")
            {
                NativeFunction.CallByName<uint>("ATTACH_CAM_TO_ENTITY", cam, this, 0f, 1.7f, 1.6f, 1);
            }
            else
            {
                NativeFunction.CallByName<uint>("ATTACH_CAM_TO_ENTITY", cam, this, 0f, 0.75f, 0.65f, 1);
            }*/
        }

        public String GetCurrentSpeed(int unit)
        {
            if (unit == 0)
            {
                int speed = Convert.ToInt32(this.Speed * 3.6);
                return speed.ToString() + "KMH";
            }
            else
            {
                int speed = Convert.ToInt32(this.Speed * 2.23694);
                return speed.ToString() + "MPH";
            }
        }

        public String GetStats() 
        {
            String stat = "";
            if (this.IsSirenSilent && this.IsSirenOn)
            {
                stat = "L";
            }
            else if (this.IsSirenOn)
            {
                stat = "LS";
            }
            if (Game.IsControlPressed(0, GameControl.VehicleBrake) && Game.LocalPlayer.Character.CurrentVehicle != null)
            {
                stat += "B";
            }
            return stat;
        }

        public bool CheckIfEmergencyVehicle()
        {
            if (this.HasSiren && this.Model.Name.ToLower() != "policeb")
            {
                return true;
            }
            return false;
        }

        public String GetUnit()
        {
            if (String.IsNullOrWhiteSpace(DashHelper.configUnit))
            {
                String name = this.Model.Name.ToLower();
                int livery = NativeFunction.CallByHash<int>(0x2BB9230590DA5E8A, this);

                if (name.Equals("police") || name.Equals("police2") || name.Equals("police3") || name.Equals("0x54830233"))
                {
                    switch (livery)
                    {
                        case 0:
                            return "UNIT 32";
                        case 1:
                            return "UNIT 76";
                        case 2:
                            return "UNIT 05";
                        case 3:
                            return "UNIT 84";
                        case 4:
                            return "UNIT 29";
                        case 5:
                            return "UNIT 43";
                        case 6:
                            return "UNIT 93";
                        case 7:
                            return "UNIT 58";
                    }
                }
                else if (name.Equals("sheriff"))
                {
                    switch (livery)
                    {
                        case 0:
                            return "UNIT 42";
                        case 1:
                            return "UNIT 17";
                        case 2:
                            return "UNIT 03";
                        case 3:
                            return "UNIT 58";
                    }
                }
                else if (name.Equals("sheriff2"))
                {
                    switch (livery)
                    {
                        case 0:
                            return "UNIT 32";
                        case 1:
                            return "UNIT 76";
                        case 2:
                            return "UNIT 05";
                        case 3:
                            return "UNIT 84";
                    }
                }
                else if (name.Equals("firetruk"))
                {
                    return "ENGINE 32";
                }
                else if (name.Equals("ambulance"))
                {
                    return "RESCUE 32";
                }
                else if (name.Equals("lguard"))
                {
                    return "RESCUE 455";
                }
                else if (assignedUnit.Equals(""))
                {
                    assignedUnit = "UNIT " + new Random().Next(100, 1000).ToString();
                }
                return assignedUnit;
            }
            else
            {
                return DashHelper.configUnit;
            }
        }
    }
}
