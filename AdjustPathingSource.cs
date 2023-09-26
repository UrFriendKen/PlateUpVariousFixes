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
                SPerformTableUpdate.DefaultPathingSource = new Vector3(Mathf.Min(Bounds.min.x - 7, -15f), 0f, 0f);
                Main.LogInfo($"SPerformTableUpdate.DefaultPathingSource inside Bounds! Updated to {SPerformTableUpdate.DefaultPathingSource}");
            }
        }
    }
}
