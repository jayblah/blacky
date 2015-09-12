﻿using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Kassadin_the_Harbinger.Handlers
{
    public class SkillsHandler
    {
        public static SpellSlot IgniteSlot { get { return ObjectManager.Player.GetSpellSlot("summonerdot"); } }

        public static readonly Dictionary<SpellSlot, Spell> Spells = new Dictionary<SpellSlot, Spell>
                                                                         {
                                                                             { SpellSlot.Q, new Spell(SpellSlot.Q, 650) },
                                                                             { SpellSlot.W, new Spell(SpellSlot.W, 150) },
                                                                             { SpellSlot.E, new Spell(SpellSlot.E, 400) },
                                                                             { SpellSlot.R, new Spell(SpellSlot.R, 500) }
                                                                         };

        public static void Load()
        {
            Spells[SpellSlot.Q].SetTargetted(0.5f, 1400f);
            Spells[SpellSlot.E].SetSkillshot(0.5f, 10f, float.MaxValue, false, SkillshotType.SkillshotCone);
            Spells[SpellSlot.R].SetSkillshot(0.5f, 150f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }
    }
}