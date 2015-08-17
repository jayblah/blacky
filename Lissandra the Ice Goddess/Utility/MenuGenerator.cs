using LeagueSharp.Common;

namespace Lissandra_the_Ice_Goddess.Utility
{

    public class MenuGenerator
    {
        public static void Load()
        {
            Lissandra.Menu = new Menu("Lissandra - Ice Goddess", "lissandra", true);

            var owMenu = new Menu("[IG] Orbwalker", "orbwalker");
            Lissandra.Orbwalker = new Orbwalking.Orbwalker(owMenu);
            Lissandra.Menu.AddSubMenu(owMenu);

            var tsMenu = new Menu("Target Selector", "Target.Selector");
            TargetSelector.AddToMenu(tsMenu);

            var comboMenu = Lissandra.Menu.AddSubMenu(new Menu("[IG] Combo", "lissandra.combo"));
            {
                comboMenu.AddItem(new MenuItem("lissandra.combo.useQ", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("lissandra.combo.useW", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("lissandra.combo.useE", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("lissandra.combo.useR", "Use R").SetValue(true));

                var comboOptionsMenu = new Menu("Combo - Options", "lissandra.combo.options");
                {
                    comboOptionsMenu.AddItem(new MenuItem("combo.options.useIgnite", "Use Ignite").SetValue(true));

                    comboMenu.AddSubMenu(comboOptionsMenu);
                }
            }

            var harassMenu = Lissandra.Menu.AddSubMenu(new Menu("[IG] Harass", "lissandra.harass"));
            {
                harassMenu.AddItem(new MenuItem("lissandra.harass.useQ", "Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("lissandra.harass.useW", "Use W").SetValue(false));
                harassMenu.AddItem(new MenuItem("lissandra.harass.useE", "Use E").SetValue(true));
            }

            var waveclearMenu = Lissandra.Menu.AddSubMenu(new Menu("[IG] Waveclear", "lissandra.waveclear"));
            {
                waveclearMenu.AddItem(new MenuItem("lissandra.waveclear.useQ", "Use Q").SetValue(true));
                waveclearMenu.AddItem(new MenuItem("lissandra.waveclear.useW", "Use W").SetValue(true));
                waveclearMenu.AddItem(new MenuItem("lissandra.waveclear.useE", "Use E").SetValue(true));
            }

            var fleeMenu = Lissandra.Menu.AddSubMenu(new Menu("[IG] Flee", "lissandra.flee"));
            {
                fleeMenu.AddItem(new MenuItem("lissandra.flee.activated", "Flee Activated").SetValue(new KeyBind('G', KeyBindType.Press)));
            }

            var miscMenu = Lissandra.Menu.AddSubMenu(new Menu("[IG] Miscellaneous", "lissandra.misc"));
            {
                miscMenu.AddItem(new MenuItem("lissandra.misc.gapcloseW", "Use W against gapclosers").SetValue(true));
                miscMenu.AddItem(new MenuItem("lissandra.misc.interruptR", "Use R to interrupt dangerous spells").SetValue(true));
                miscMenu.AddItem(new MenuItem("lissandra.misc.saveR", "Save my Ass with R").SetValue(true));

                miscMenu.AddItem(new MenuItem("lissandra.misc.hitChance", "Prediction Hitchance").SetValue(new StringList(new[] { "Low", "Medium", "High", "Very High" }, 3)));
            }

            Lissandra.ManaManager.AddToMenu(ref Lissandra.Menu);

            var drawingMenu = Lissandra.Menu.AddSubMenu(new Menu("[IG] Drawings", "lissandra.drawing"));
            {
                drawingMenu.AddItem(new MenuItem("lissandra.drawing.drawQ", "Draw Q").SetValue(new Circle()));
                drawingMenu.AddItem(new MenuItem("lissandra.drawing.drawW", "Draw W").SetValue(new Circle()));
                drawingMenu.AddItem(new MenuItem("lissandra.drawing.drawE", "Draw E").SetValue(new Circle()));
                drawingMenu.AddItem(new MenuItem("lissandra.drawing.drawR", "Draw R").SetValue(new Circle()));
                drawingMenu.AddItem(new MenuItem("lissandra.drawing.drawDamage", "Draw Damage").SetValue(new Circle()));
                drawingMenu.AddItem(new MenuItem("lissandra.drawing.drawingsOff", "Turn drawings off").SetValue(false));
            }

            Lissandra.Menu.AddItem(new MenuItem("seperator", ""));
            Lissandra.Menu.AddItem(new MenuItem("by.blacky.and.Asuna", "Made by blacky & Asuna"));

            Lissandra.Menu.AddToMainMenu();
        }
    }
}