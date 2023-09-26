using Kitchen;
using KitchenMods;
using UnityEngine;

namespace KitchenVariousFixes
{
    public class AdjustPathingSource : NightSystem, IModSystem
    {
        protected override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnUpdate()
        {
            if (Bounds.Contains(SPerformTableUpdate.DefaultPathingSource))
            {
                SPerformTableUpdate.DefaultPathingSource = GetFrontDoor(get_external_tile: true);
                Main.LogInfo($"SPerformTableUpdate.DefaultPathingSource inside Bounds! Updated to {SPerformTableUpdate.DefaultPathingSource}");
            }
        }
    }
}
