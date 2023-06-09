﻿using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;

namespace KitchenVariousFixes
{
    public class SplittableDepletedDelaySystem : GameSystemBase, IModSystem
    {
        internal struct CSplittableDepletedDelay : IComponentData, IModComponent
        {
            public bool IsFirstFrame;
            public float TimeRemaining;
            public int SplitCount;
        }

        EntityQuery Items;

        internal static SplittableDepletedDelaySystem instance;

        protected override void Initialise()
        {
            base.Initialise();
            instance = this;
            Items = GetEntityQuery(new QueryHelper()
                .All(typeof(CSplittableItem), typeof(CSplittableDepletedDelay)));
        }
        protected override void OnUpdate()
        {
            using NativeArray<Entity> items = Items.ToEntityArray(Allocator.Temp);
            using NativeArray<CSplittableItem> splits = Items.ToComponentDataArray<CSplittableItem>(Allocator.Temp);
            using NativeArray<CSplittableDepletedDelay> delays = Items.ToComponentDataArray<CSplittableDepletedDelay>(Allocator.Temp);

            float dt = Time.DeltaTime;

            for (int i = 0; i < items.Length; i++)
            {
                Entity item = items[i];
                CSplittableItem split = splits[i];
                CSplittableDepletedDelay delay = delays[i];

                if (delay.TimeRemaining < 0f)
                {
                    split.RemainingCount = delay.SplitCount;
                    Set(item, split);
                    EntityManager.RemoveComponent<CSplittableDepletedDelay>(item);
                    continue;
                }

                if (delay.IsFirstFrame)
                {
                    delay.IsFirstFrame = false;
                    split.RemainingCount = delay.SplitCount + 1;
                    Set(item, split);
                }
                delay.TimeRemaining -= dt;
                Set(item, delay);
            }
        }
    }
}
