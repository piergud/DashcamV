[assembly: Rage.Attributes.Plugin("Dashcam V", Description = "Realistic dashcam for emergency vehicles", Author = "PieRGud", DefaultVehicleType = "DashcamV.DashVehicle")]

namespace DashcamV
{
    using System.Windows.Forms;
    using Rage;
    using Rage.Native;

    public sealed class EntryPoint
    {
        static Camera cam = new Camera(false);
        static bool dashView = false;

        public static void Main()
        {
            DashVehicle activVeh = null;
            bool state = false;
            bool lastSirenState = false;

            Configs.RunConfigCheck();
            cam.Delete();

            while (true)
            {
                if (cam.IsValid() && Game.LocalPlayer.Character.LastVehicle.Exists())
                {
                    cam.Rotation = Game.LocalPlayer.Character.LastVehicle.Rotation;
                    cam.FOV = NativeFunction.CallByHash<float>(0x65019750A0324133);
                }

                if (Game.LocalPlayer.Character.LastVehicle.Exists())
                {
                    if (Game.LocalPlayer.Character.LastVehicle.IsDead)
                    {
                        if (cam.IsValid())
                        {
                            if (cam.Active)
                            {
                                dashView = false;
                                state = false;
                                cam.Delete();
                            }
                        }
                    }
                    if (state)
                    {
                        DashHelper.DrawDashcam((DashVehicle)Game.LocalPlayer.Character.LastVehicle, true);
                    }
                }

                if (Game.LocalPlayer.Character.CurrentVehicle == null && Game.LocalPlayer.Character.LastVehicle != null && !Game.LocalPlayer.Character.LastVehicle.IsDead
                    && Game.LocalPlayer.Character.LastVehicle.Exists()
                    && ((DashVehicle)Game.LocalPlayer.Character.LastVehicle).CheckIfEmergencyVehicle()
                    && (((Control.ModifierKeys & Keys.Shift) != 0 && Game.IsKeyDown(Configs.remoteViewKey)) || Game.IsControllerButtonDown(Configs.remoteViewBut)))
                {
                    if (!state)
                    {
                        lastSirenState = Game.LocalPlayer.LastVehicle.IsSirenSilent;
                        cam = new Camera(true);
                        Game.LocalPlayer.LastVehicle.IsSirenSilent = true;
                        ((DashVehicle)Game.LocalPlayer.Character.LastVehicle).SetDashview(cam);
                        cam.Rotation = Game.LocalPlayer.Character.LastVehicle.Rotation;
                        cam.FOV = NativeFunction.CallByHash<float>(0x65019750A0324133);
                        state = true;
                    }
                    else
                    {
                        cam.Delete();
                        Game.LocalPlayer.LastVehicle.IsSirenSilent = lastSirenState;
                        state = false;
                    }
                }

                if (dashView && Game.IsControlJustPressed(0, GameControl.Enter))
                {
                    if (cam.IsValid())
                    {
                        if (cam.Active)
                        {
                            dashView = false;
                            cam.Delete();
                        }
                    }
                }
                else if (dashView && !NativeFunction.CallByHash<bool>(0x70FDA869F3317EA9) && !cam.IsValid())
                {
                    cam = new Camera(true);
                    ((DashVehicle)Game.LocalPlayer.Character.LastVehicle).SetDashview(cam);
                    cam.Rotation = Game.LocalPlayer.Character.LastVehicle.Rotation;
                    cam.FOV = NativeFunction.CallByHash<float>(0x65019750A0324133);
                }
                else if (NativeFunction.CallByHash<bool>(0x39B5D1B10383F0C8) || NativeFunction.CallByHash<bool>(0x70FDA869F3317EA9))
                {
                    if (dashView)
                    {
                        if (cam.IsValid())
                        {
                            if (cam.Active)
                            {
                                cam.Delete();
                            }
                        }
                    }
                    if (DashHelper.dashAllViews && Game.LocalPlayer.Character.CurrentVehicle != null
                        && ((DashVehicle)Game.LocalPlayer.Character.CurrentVehicle).CheckIfEmergencyVehicle()
                        && (!(Game.LocalPlayer.Character.CurrentVehicle.Position.X > 366 && Game.LocalPlayer.Character.CurrentVehicle.Position.X < 426
                        && Game.LocalPlayer.Character.CurrentVehicle.Position.Y > -1000 && Game.LocalPlayer.Character.CurrentVehicle.Position.Y < -940
                        && Game.LocalPlayer.Character.CurrentVehicle.Position.Z > -129 && Game.LocalPlayer.Character.CurrentVehicle.Position.Z < -69)))
                    {
                        DashHelper.DrawDashcam((DashVehicle)Game.LocalPlayer.Character.CurrentVehicle, true);
                    }
                    else
                    {
                        DashHelper.DrawDashcam(null, false);
                    }
                }
                else if (Game.LocalPlayer.Character.CurrentVehicle != null
                    && ((DashVehicle)Game.LocalPlayer.Character.CurrentVehicle).CheckIfEmergencyVehicle())
                {
                    if (!(Game.LocalPlayer.Character.CurrentVehicle.Position.X > 366 && Game.LocalPlayer.Character.CurrentVehicle.Position.X < 426
                        && Game.LocalPlayer.Character.CurrentVehicle.Position.Y > -1000 && Game.LocalPlayer.Character.CurrentVehicle.Position.Y < -940
                        && Game.LocalPlayer.Character.CurrentVehicle.Position.Z > -129 && Game.LocalPlayer.Character.CurrentVehicle.Position.Z < -69))
                    {
                        if (Game.IsControlJustReleased(0, GameControl.NextCamera))
                        {
                            if (!dashView && NativeFunction.CallByHash<int>(0xA4FF579AC0E3AAAE) == 4)
                            {
                                dashView = true;
                                cam = new Camera(true);
                                ((DashVehicle)Game.LocalPlayer.Character.CurrentVehicle).SetDashview(cam);
                                cam.Rotation = Game.LocalPlayer.Character.CurrentVehicle.Rotation;
                                cam.FOV = NativeFunction.CallByHash<float>(0x65019750A0324133);
                                activVeh = (DashVehicle)Game.LocalPlayer.Character.CurrentVehicle;
                            }
                            else if (dashView)
                            {
                                dashView = false;
                                cam.Delete();
                                NativeFunction.CallByHash<uint>(0xAC253D7842768F48, 4);
                            }
                        }
                        if (state)
                        {
                            state = false;
                            if (cam.IsValid())
                            {
                                if (cam.Active)
                                {
                                    cam.Delete();
                                }
                            }
                        }
                        if (dashView)
                        {
                            DashHelper.DrawDashcam((DashVehicle)Game.LocalPlayer.Character.CurrentVehicle, true);
                        }
                    }
                    else
                    {
                        DashHelper.DrawDashcam(null, false);
                    }
                }
                else if (Game.LocalPlayer.Character.CurrentVehicle == null && !state)
                {
                    if (cam.IsValid())
                    {
                        if (cam.Active)
                        {
                            dashView = false;
                            cam.Delete();
                        }
                    }
                }

                if ((DashVehicle)Game.LocalPlayer.Character.CurrentVehicle != activVeh 
                    && (DashVehicle)Game.LocalPlayer.Character.CurrentVehicle != null)
                {
                    if (cam.IsValid())
                    {
                        if (cam.Active)
                        {
                            cam.Delete();
                            if (((DashVehicle)Game.LocalPlayer.Character.CurrentVehicle).CheckIfEmergencyVehicle())
                            {
                                cam = new Camera(true);
                                ((DashVehicle)Game.LocalPlayer.Character.CurrentVehicle).SetDashview(cam);
                                cam.Rotation = Game.LocalPlayer.Character.CurrentVehicle.Rotation;
                                cam.FOV = NativeFunction.CallByHash<float>(0x65019750A0324133);
                                activVeh = (DashVehicle)Game.LocalPlayer.Character.CurrentVehicle;
                            }
                        }
                    }
                }

                GameFiber.Yield();
            }
        }
    }
}
