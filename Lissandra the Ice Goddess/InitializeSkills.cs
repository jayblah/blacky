using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace Lissandra_the_Ice_Goddess
{
    public class InitializeSkills
    {
        public static SpellSlot Ignite;

        public static readonly Dictionary<SpellSlot, Spell> Spells = new Dictionary<SpellSlot, Spell>
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q, 700) },
            { SpellSlot.W, new Spell(SpellSlot.W, 440) },
            { SpellSlot.E, new Spell(SpellSlot.E, 1050) },
            { SpellSlot.R, new Spell(SpellSlot.R, 550) }
        };

        public static void Load()
        {
            Spells[SpellSlot.Q].SetSkillshot(0.25f, 75, 2250, true, SkillshotType.SkillshotLine);
            Spells[SpellSlot.W].Delay = 0.25f;
            Spells[SpellSlot.E].SetSkillshot(0.25f, 110, 850, false, SkillshotType.SkillshotLine);
            Spells[SpellSlot.R].SetSkillshot(0.25f, 690, 800, false, SkillshotType.SkillshotCircle);

            Ignite = ObjectManager.Player.GetSpellSlot("summonerdot");
        }
    }
}
