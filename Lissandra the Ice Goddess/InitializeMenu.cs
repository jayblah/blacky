using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace Lissandra_the_Ice_Goddess
{

    public class InitializeMenu
    {
        public static void Load()
        {
            Entry.Menu = new Menu("Lissandra - Ice Goddess", "LissandraIceGoddess", true);

            var owMenu = new Menu("[IG] Orbwalker", "orbwalker");
            Entry.Orbwalker = new Orbwalking.Orbwalker(owMenu);
            Entry.Menu.AddSubMenu(owMenu);

            var tsMenu = new Menu("Target Selector", "Target.Selector");
            TargetSelector.AddToMenu(tsMenu);

            var comboMenu = Entry.Menu.AddSubMenu(new Menu("[IG] Combo", "ComboMenu"));
            {
                comboMenu.AddItem(new MenuItem("combo.useQ", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("combo.useW", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("combo.useE", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("combo.useR", "Use R").SetValue(true));

                var comboOptionsMenu = new Menu("Combo - Options", "ComboOptionsMenu");
                {
                    comboOptionsMenu.AddItem(new MenuItem("combo.options.useIgnite", "Use Ignite").SetValue(true));

                    comboMenu.AddSubMenu(comboOptionsMenu);
                }
            }

            var harassMenu = Entry.Menu.AddSubMenu(new Menu("[IG] Harass", "HarassMenu"));
            {
                harassMenu.AddItem(new MenuItem("harass.useQ", "Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("harass.useE", "Use E").SetValue(true));
            }

            var waveclearMenu = Entry.Menu.AddSubMenu(new Menu("[IG] Waveclear", "WaveclearMenu"));
            {
                waveclearMenu.AddItem(new MenuItem("waveclear.useQ", "Use Q").SetValue(true));
                waveclearMenu.AddItem(new MenuItem("waveclear.useQ", "Use W").SetValue(true));
                waveclearMenu.AddItem(new MenuItem("waveclear.useQ", "Use E").SetValue(true));
            }

            var fleeMenu = Entry.Menu.AddSubMenu(new Menu("[IG] Flee", "FleeMenu"));
            {
                fleeMenu.AddItem(new MenuItem("flee.activated", "Flee Activated").SetValue(new KeyBind('G', KeyBindType.Press)));
            }

            var miscMenu = Entry.Menu.AddSubMenu(new Menu("[IG] Miscellaneous", "MiscMenu"));
            {
                miscMenu.AddItem(new MenuItem("misc.gapcloseW", "Use W against gapclosers").SetValue(true));
                miscMenu.AddItem(new MenuItem("misc.interruptR", "Use R to interrupt dangerous spells").SetValue(true));
            }

            var drawingMenu = Entry.Menu.AddSubMenu(new Menu("[IG] Drawings", "DrawingMenu"));
            {
                drawingMenu.AddItem(new MenuItem("drawing.drawQ", "Draw Q").SetValue(new Circle()));
                drawingMenu.AddItem(new MenuItem("drawing.drawW", "Draw W").SetValue(new Circle()));
                drawingMenu.AddItem(new MenuItem("drawing.drawE", "Draw E").SetValue(new Circle()));
                drawingMenu.AddItem(new MenuItem("drawing.drawR", "Draw R").SetValue(new Circle()));
                drawingMenu.AddItem(new MenuItem("drawing.drawDamage", "Draw Damage").SetValue(new Circle()));
                drawingMenu.AddItem(new MenuItem("drawing.drawingsOff", "Turn drawings off").SetValue(false));
            }

            Entry.Menu.AddItem(new MenuItem("seperator", ""));
            Entry.Menu.AddItem(new MenuItem("by.blacky.and.Asuna", "Made by blacky & Asuna"));

            Entry.Menu.AddToMainMenu();
        }
    }
}