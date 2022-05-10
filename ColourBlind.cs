using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IL2CPPAssetBundleAPI;
using MelonLoader;
using PlagueButtonAPI.Controls;
using UnityEngine;

namespace VRChatAccessibility
{
    internal class ColourBlind : BaseModule
    {
        private IL2CPPAssetBundle Bundle = new ();

        public override void OnApplicationStart()
        {
            if (!Bundle.LoadBundle(Assembly.GetExecutingAssembly(), "VRChatAccessibility.Resources.assistshader.asset")) // This If Also Checks If It Successfully Loaded As To Prevent Further Exceptions
            {
                MelonLogger.Error("Failed To Load Bundle! Error: " + Bundle.error);
            }
        }

        public override void OnQuickMenuInit()
        {
            var Comp = Camera.main.gameObject.AddComponent<ColourBlindComponent>();
            Comp.Material = Bundle.Load<GameObject>("LoadMe.prefab").GetComponent<MeshRenderer>().material;

            var page = MainClass.MainMenu.AddSubMenu(MainClass.Logo, "ColourBlind", "ColourBlind Options", true, true, null, "", MainClass.Logo, true).Item1;

            var Access = page.AddCollapsibleButtonGroup("Colour Blind Assist", true);

            ToggleButton Protanopia = null; // red
            ToggleButton Deuteranopia = null; // green
            ToggleButton Tritanopia = null; // blue

            Protanopia = Access.AddToggleButton("Protanopia", "Red-Green", b =>
            {
                MainClass.Config.InternalConfig.Protanopia = b;
                MainClass.Config.InternalConfig.Deuteranopia = false;
                MainClass.Config.InternalConfig.Tritanopia = false;

                Comp.AssistMode = b ? ColourBlindComponent.AssistType.Protanopia : ColourBlindComponent.AssistType.Normal;

                Deuteranopia.SetToggleState(false, false);
                Tritanopia.SetToggleState(false, false);
            }, MainClass.Config.InternalConfig.Protanopia);

            Deuteranopia = Access.AddToggleButton("Deuteranopia", "Red–Green-Yellow", b =>
            {
                MainClass.Config.InternalConfig.Protanopia = false;
                MainClass.Config.InternalConfig.Deuteranopia = b;
                MainClass.Config.InternalConfig.Tritanopia = false;

                Comp.AssistMode = b ? ColourBlindComponent.AssistType.Deuteranopia : ColourBlindComponent.AssistType.Normal;

                Protanopia.SetToggleState(false, false);
                Tritanopia.SetToggleState(false, false);
            }, MainClass.Config.InternalConfig.Deuteranopia);

            Tritanopia = Access.AddToggleButton("Tritanopia", "Green-Blue", b =>
            {
                MainClass.Config.InternalConfig.Protanopia = false;
                MainClass.Config.InternalConfig.Deuteranopia = false;
                MainClass.Config.InternalConfig.Tritanopia = b;

                Comp.AssistMode = b ? ColourBlindComponent.AssistType.Tritanopia : ColourBlindComponent.AssistType.Normal;

                Protanopia.SetToggleState(false, false);
                Deuteranopia.SetToggleState(false, false);
            }, MainClass.Config.InternalConfig.Tritanopia);

            page.AddSlider("Strength", "Change the assist strength", val =>
            {
                MainClass.Config.InternalConfig.AssistStrength = val;

                Comp.Strength = val;
            }, 0f, 1f, MainClass.Config.InternalConfig.AssistStrength, false, false);

            if (MainClass.Config.InternalConfig.Protanopia)
            {
                Comp.AssistMode = ColourBlindComponent.AssistType.Protanopia;
            }
            else if (MainClass.Config.InternalConfig.Deuteranopia)
            {
                Comp.AssistMode = ColourBlindComponent.AssistType.Deuteranopia;
            }
            else if (MainClass.Config.InternalConfig.Tritanopia)
            {
                Comp.AssistMode = ColourBlindComponent.AssistType.Tritanopia;
            }

            Comp.Strength = MainClass.Config.InternalConfig.AssistStrength;
        }
    }
}
