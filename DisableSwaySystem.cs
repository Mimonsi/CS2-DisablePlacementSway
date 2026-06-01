using Game;
using Game.Objects;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace DisablePlacementSway
{
    [UpdateAfter(typeof(AnimationSystem))]
    public partial class DisableSwaySystem : GameSystemBase
    {
        private EntityQuery m_Query;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_Query = GetEntityQuery(
                ComponentType.ReadWrite<Animation>(),
                ComponentType.ReadOnly<Transform>());
            RequireForUpdate(m_Query);
        }

        protected override void OnUpdate()
        {
            if (!Setting.Instance.DisablePlacementSway)
                return;
            var job = new SnapSwayJob
            {
                AnimationType = SystemAPI.GetComponentTypeHandle<Animation>(false),
                TransformType = SystemAPI.GetComponentTypeHandle<Transform>(true),
            };
            Dependency = job.ScheduleParallel(m_Query, Dependency);
        }

        [BurstCompile]
        private struct SnapSwayJob : IJobChunk
        {
            public ComponentTypeHandle<Animation> AnimationType;
            [ReadOnly] public ComponentTypeHandle<Transform> TransformType;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                var animations = chunk.GetNativeArray(ref AnimationType);
                var transforms = chunk.GetNativeArray(ref TransformType);

                for (int i = 0; i < animations.Length; i++)
                {
                    var anim = animations[i];
                    var tr = transforms[i];

                    anim.m_SwayVelocity = float3.zero;
                    anim.m_SwayPosition = float3.zero;
                    anim.m_TargetPosition = tr.m_Position;
                    anim.m_Position = tr.m_Position;
                    anim.m_Rotation = tr.m_Rotation;

                    animations[i] = anim;
                }
            }
        }
    }
}
