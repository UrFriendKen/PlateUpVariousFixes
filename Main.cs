using HarmonyLib;
using Kitchen;
using KitchenData;
using KitchenMods;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

// Namespace should have "Kitchen" in the beginning
namespace KitchenVariousFixes
{
    public class Main : IModInitializer
    {
        public const string MOD_GUID = "IcedMilo.PlateUp.VariousFixes";
        public const string MOD_NAME = "VariousFixes";
        public const string MOD_VERSION = "0.1.6";

        protected readonly Dictionary<int, int> ItemProviders = new Dictionary<int, int>();
        
        Harmony harmony;
        static List<Assembly> PatchedAssemblies = new List<Assembly>();

        public Main()
        {
            if (harmony == null)
                harmony = new Harmony(MOD_GUID);
            Assembly assembly = Assembly.GetExecutingAssembly();
            if (assembly != null && !PatchedAssemblies.Contains(assembly))
            {
                harmony.PatchAll(assembly);
                PatchedAssemblies.Add(assembly);
            }
        }

        private void PopulateMissingDedicatedProviders()
        {
            HashSet<int> checkedItems = new HashSet<int>() { 0 };
            foreach (Appliance appliance in GameData.Main.Get<Appliance>())
            {
                IEnumerable<CItemProvider> providers = appliance.Properties.Where(property => property is CItemProvider).Cast<CItemProvider>();
                if (providers.Count() == 0)
                    continue;

                CItemProvider provider = providers.First();
                int itemId = provider.DefaultProvidedItem == 0? provider.ProvidedItem : provider.DefaultProvidedItem;
                if (checkedItems.Contains(itemId))
                    continue;

                checkedItems.Add(itemId);

                if (!GameData.Main.TryGet(itemId, out Item item, warn_if_fail: true))
                    continue;

                if (item.DedicatedProvider != null)
                    continue;

                item.DedicatedProvider = appliance;
                Main.LogInfo($"Updated DedicatedProvider for {item.name} ({item.ID}) - {item.DedicatedProvider.name}");
            }
        }

        public void PostActivate(KitchenMods.Mod mod)
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
        }

        public void PreInject()
        {
            PopulateMissingDedicatedProviders();
        }

        public void PostInject() { }

        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
