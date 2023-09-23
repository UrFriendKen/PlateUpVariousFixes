using Kitchen;
using KitchenMods;
using Unity.Entities;
using UnityEngine;

namespace KitchenVariousFixes
{
    public class PatchController : GenericSystemBase, IModSystem
    {
        private static PatchController _instance;
        protected override void Initialise()
        {
            base.Initialise();
            _instance = this;
        }

        protected override void OnUpdate()
        {
        }

        internal static bool TryGetBounds(out Bounds bounds)
        {
            bounds = default;
            if (_instance == null || !_instance.TryGetSingletonEntity<SLayout>(out Entity singletonEntity) || !_instance.Require(singletonEntity, out CBounds cBounds))
                return false;

            bounds = cBounds.Bounds;
            return true;
        }
    }
}
