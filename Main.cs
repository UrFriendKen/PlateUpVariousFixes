using Kitchen;
using KitchenData;
using KitchenLib;
using KitchenMods;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

// Namespace should have "Kitchen" in the beginning
namespace KitchenVariousFixes
{
    public class Main : BaseMod, IModSystem
    {
        // GUID must be unique and is recommended to be in reverse domain name notation
        // Mod Name is displayed to the player and listed in the mods menu
        // Mod Version must follow semver notation e.g. "1.2.3"
        public const string MOD_GUID = "IcedMilo.PlateUp.VariousFixes";
        public const string MOD_NAME = "VariousFixes";
        public const string MOD_VERSION = "0.1.1";
        public const string MOD_AUTHOR = "IcedMilo";
        public const string MOD_GAMEVERSION = ">=1.1.5";
        // Game version this mod is designed for in semver
        // e.g. ">=1.1.3" current and all future
        // e.g. ">=1.1.3 <=1.2.3" for all from/until

        public static AssetBundle Bundle;

        protected readonly Dictionary<int, int> ItemProviders = new Dictionary<int, int>();

        public Main() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            PopulateMissingDedicatedProviders();
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

        protected override void OnUpdate()
        {
        }

        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
        }
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
