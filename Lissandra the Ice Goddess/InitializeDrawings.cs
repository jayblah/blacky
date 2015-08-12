using System;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace Lissandra_the_Ice_Goddess
{
    internal class InitializeDrawings
    {
        public static void OnDraw(EventArgs args)
        {
            var drawOff = Entry.Menu.Item("drawing.drawingsOff").GetValue<bool>();
            var drawQ = Entry.Menu.Item("drawing.drawQ").GetValue<Circle>();
            var drawW = Entry.Menu.Item("drawing.drawW").GetValue<Circle>();
            var drawE = Entry.Menu.Item("drawing.drawE").GetValue<Circle>();
            var drawR = Entry.Menu.Item("drawing.drawR").GetValue<Circle>();
            var drawDamage = Entry.Menu.Item("drawing.drawDamage").GetValue<Circle>();

            if (drawOff || ObjectManager.Player.IsDead)
            {
                return;
            }

            DamageIndicator.DrawingColor = drawDamage.Color;
            DamageIndicator.Enabled = drawDamage.Active;

            if (drawQ.Active)
                if (InitializeSkills.Spells[SpellSlot.Q].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, InitializeSkills.Spells[SpellSlot.Q].Range, Color.Aqua);

            if (drawW.Active)
                if (InitializeSkills.Spells[SpellSlot.W].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, InitializeSkills.Spells[SpellSlot.W].Range, Color.Aqua);

            if (drawE.Active)
                if (InitializeSkills.Spells[SpellSlot.E].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, InitializeSkills.Spells[SpellSlot.E].Range, Color.Aqua);

            if (drawR.Active)
                if (InitializeSkills.Spells[SpellSlot.R].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, InitializeSkills.Spells[SpellSlot.R].Range, Color.Aqua);
        }
    }
}
