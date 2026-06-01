using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;

namespace DisablePlacementSway
{
    [FileLocation("ModsData/" + nameof(DisablePlacementSway) + "/" + nameof(DisablePlacementSway))]
    public class Setting : ModSetting
    {
        public static Setting Instance;
        public const string kSection = "Main";
        public Setting(IMod mod) : base(mod)
        {
        }



        [SettingsUISection(kSection)] public bool DisablePlacementSway { get; set; } = true;
        

        public override void SetDefaults()
        {
            DisablePlacementSway = true;
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Disable Placement Sway" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DisablePlacementSway)), "Disable Placement Sway" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.DisablePlacementSway)),
                    $"Check this box to disable the object sway when placing them"
                },
            };
        }

        public void Unload()
        {
        }
    }
}