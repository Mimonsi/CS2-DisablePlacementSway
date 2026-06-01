using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Game.Tools;
using Colossal.IO.AssetDatabase;

namespace DisablePlacementSway
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(DisablePlacementSway)}.{nameof(Mod)}")
            .SetShowsErrorsInUI(false);

        private Setting m_Setting;
        private AnimationSystem m_VanillaAnimationSystem;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));

            AssetDatabase.global.LoadSettings(nameof(DisablePlacementSway), m_Setting, new Setting(this));

            updateSystem.UpdateAfter<DisableSwaySystem>(SystemUpdatePhase.ModificationEnd);

            var system = updateSystem.World.GetOrCreateSystemManaged<DisableSwaySystem>();
            m_VanillaAnimationSystem = updateSystem.World.GetOrCreateSystemManaged<AnimationSystem>();

            void ApplyState(bool disableSway)
            {
                system.Enabled = disableSway;
                m_VanillaAnimationSystem.Enabled = !disableSway;
            }

            ApplyState(m_Setting.DisablePlacementSway);
            m_Setting.OnDisablePlacementSwayChanged = ApplyState;
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }
    }
}