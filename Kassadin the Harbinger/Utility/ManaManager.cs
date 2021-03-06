﻿using LeagueSharp;
using LeagueSharp.Common;

namespace Kassadin_the_Harbinger.Utility
{
    internal class ManaManager
    {
        private readonly Obj_AI_Hero player;
        private Menu menu;

        public ManaManager()
        {
            this.player = ObjectManager.Player;
        }

        public void AddToMenu(ref Menu attachMenu)
        {
            this.menu = attachMenu;
            this.CreateMenu();
        }

        public void CreateMenu()
        {
            var manaMenu = new Menu("[KH] Mana Manager", "ManaManager");
            {
                manaMenu.AddItem(new MenuItem("kassadin.harass.Mana", "Mana % for Harass").SetValue(new Slider(25)));
                manaMenu.AddItem(new MenuItem("kassadin.laneclear.Mana", "Mana % for Wave & Jungleclear").SetValue(new Slider(40)));
            }

            this.menu.AddSubMenu(manaMenu);
        }

        public bool CanUseSpell(Spell spell)
        {
            return this.player.Mana >= this.player.Spellbook.GetSpell(spell.Slot).ManaCost;
        }

        public bool CanDoCombo()
        {
            return this.player.Mana >=
                   this.player.Spellbook.GetSpell(SpellSlot.Q).ManaCost + this.player.Spellbook.GetSpell(SpellSlot.W).ManaCost +
                   this.player.Spellbook.GetSpell(SpellSlot.E).ManaCost + this.player.Spellbook.GetSpell(SpellSlot.R).ManaCost;
        }

        public bool CanHarass()
        {
            return !(this.player.ManaPercent <= this.menu.Item("kassadin.harass.Mana").GetValue<Slider>().Value);
        }

        public bool CanLaneclear()
        {
            return !(this.player.ManaPercent <= this.menu.Item("kassadin.laneclear.Mana").GetValue<Slider>().Value);
        }
    }
}