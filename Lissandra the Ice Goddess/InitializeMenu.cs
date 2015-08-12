using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace Lissandra_the_Ice_Goddess
{

    public class InitializeMenu
    {
        public static void Load()
        {
            Entry.Menu = new Menu("Lissandra - Ice Goddess", "LissandraIceGoddess", true);

            var owMenu = new Menu("Orbwalker", "orbwalker");
            Entry.Orbwalker = new Orbwalking.Orbwalker(owMenu);
            Entry.Menu.AddSubMenu(owMenu);

            var tsMenu = new Menu("Target Selector", "Target.Selector");
            TargetSelector.AddToMenu(tsMenu);

            var comboMenu = Entry.Menu.AddSubMenu(new Menu("Combo Options", "ComboMenu"));
            {
                comboMenu.AddItem(new MenuItem("combo.useQ", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("combo.useW", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("combo.useE", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("combo.useR", "Use R").SetValue(true));
            }

            var harassMenu = Entry.Menu.AddSubMenu(new Menu("Harass Options", "HarassMenu"));
            {
                harassMenu.AddItem(new MenuItem("harass.useQ", "Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("harass.useE", "Use E").SetValue(true));
            }

            var drawingMenu = Entry.Menu.AddSubMenu(new Menu("Drawing Options", "DrawingMenu"));
            {
                drawingMenu.AddItem(new MenuItem("drawing.drawQ", "Draw Q").SetValue(new Circle(true, Color.Aquamarine)));
            }

            Entry.Menu.AddItem(new MenuItem("seperator", ""));
            Entry.Menu.AddItem(new MenuItem("by.blacky.and.Asuna", "Made by blacky & Asuna"));

            Entry.Menu.AddToMainMenu();
        }
    }
}