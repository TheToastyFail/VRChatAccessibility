using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;

namespace VRChatAccessibility
{
    [RegisterTypeInIl2Cpp]
    public class ColourBlindComponent : MonoBehaviour
    {
        public ColourBlindComponent(IntPtr ptr) : base(ptr){}

        public enum AssistType
        {
            Normal,
            Deuteranopia,
            Protanopia,
            Tritanopia
        }

        public AssistType AssistMode;

        public float Strength = 1.0f;

        public Material Material;

        public bool IsLinear => QualitySettings.activeColorSpace == ColorSpace.Linear;

        void Start()
        {
            // Disable if we don't support image effects
            if (!SystemInfo.supportsImageEffects)
            {
                MelonLogger.Error("Image Effects are not supported on this platform.");
                enabled = false;
                return;
            }

            // Disable if we don't support render textures
            if (!SystemInfo.supportsRenderTextures)
            {
                MelonLogger.Error("RenderTextures are not supported on this platform.");
                enabled = false;
                return;
            }

            if (Material == null)
            {
                MelonLogger.Error("Material Is Null!");
                enabled = false;
                return;
            }

            if (Material.shader == null)
            {
                MelonLogger.Error("Shader Is Null!");
                enabled = false;
                return;
            }

            // Disable the image effect if the shader can't
            // run on the users graphics card
            if (!Material.shader.isSupported)
            {
                MelonLogger.Error("Unsupported shader.");
                enabled = false;
                return;
            }
        }

        void OnDestroy()
        {
            if (Material)
            {
                DestroyImmediate(Material);
            }
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (Material.shader == null || AssistMode == AssistType.Normal)
            {
                Graphics.Blit(source, destination);
                return;
            }

            Material.SetFloat("_Strength", Strength);
            Graphics.Blit(source, destination, Material, ((int)AssistMode - 1) + (IsLinear ? 3 : 0));
        }
	}
}
