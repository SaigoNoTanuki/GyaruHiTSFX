using BepInEx;
using HarmonyLib;
using REPOLib;
using System.IO;
using UnityEngine;

namespace GyaruHitSFX
{
    [BepInPlugin("SaigoNoTanuki.GyaruHitSFX", "GyaruHitSFX", "1.0.0")]
    [BepInDependency("REPOLib", BepInDependency.DependencyFlags.HardDependency)]
    public class GyaruHitSFX : BaseUnityPlugin
    {
        internal static AudioClip ?gyaruFX;

        private void Awake()
        {
            string pluginDir = Path.GetDirectoryName(Info.Location);
            string bundlePath = Path.Combine(pluginDir, "gyarufxbundle");

            BundleLoader.LoadBundle(
                bundlePath,
                OnBundleLoaded,
                loadContents: false
            );

            Logger.LogInfo("[GyaruHitSFX] Plugin loaded and initialized.");
            var harmony = new Harmony("SaigoNoTanuki.GyaruHitSFX");
            harmony.PatchAll();
        }

        private void OnBundleLoaded(AssetBundle bundle)
        {
            gyaruFX = bundle.LoadAsset<AudioClip>("gyaru");

            if (gyaruFX == null)
            {
                Logger.LogError("[GyaruHitSFX] Failed to load AudioClip from bundle.");
            }
            else
            {
                Logger.LogInfo("[GyaruHitSFX] AudioClip loaded successfully.");
            }

            Debug.Log("Bundle Loaded");
        }
        

        [HarmonyPatch(typeof(PlayerHealth), "Hurt")]
        public static class SFXPatcher
        {
            [HarmonyPrefix]
            public static void Prefix(PlayerHealth __instance, int damage)
            {
                if (gyaruFX == null) return;
                AudioSource.PlayClipAtPoint(gyaruFX, __instance.transform.position);
            }
        }
    }
}