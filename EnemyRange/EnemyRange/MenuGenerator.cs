using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace EnemyRange
{
    using System.Linq;

    public class MenuGenerator
    {
        public static void Load()
        {
            EnemyRange.Menu = new Menu("Enemy Range#", "enemy_range", true);

            EnemyRange.Menu.AddItem(new MenuItem("enemy.range.enabled", "Enemy Range# Enabled").SetValue(true));

            var drawingMenu = EnemyRange.Menu.AddSubMenu(new Menu("[ER] Drawings", "er.drawing"));
            {
                foreach (var hero in HeroManager.Enemies.Where(hero => hero.IsEnemy && hero.ChampionName == "Orianna"))
                {
                    drawingMenu.AddItem(
                        new MenuItem(string.Format("er.drawing.{0}.W", hero.ChampionName.ToLower()), "Draw Orianna W Range")
                            .SetValue(new Circle(true, Color.DarkOrange)));
                    drawingMenu.AddItem(
                        new MenuItem(string.Format("er.drawing.{0}.R", hero.ChampionName.ToLower()), "Draw Orianna R Range")
                            .SetValue(new Circle(true, Color.DarkOrange)));
                }
                foreach (var hero in HeroManager.Enemies.Where(hero => hero.IsEnemy && hero.ChampionName == "Azir"))
                {
                    drawingMenu.AddItem(
                        new MenuItem(string.Format("er.drawing.{0}", hero.ChampionName.ToLower()), "Draw Azir Soldier Range")
                            .SetValue(new Circle(true, Color.DarkOrange)));
                }
                foreach (var hero in HeroManager.Enemies.Where(hero => hero.IsEnemy && hero.ChampionName == "Heimerdinger"))
                {
                    drawingMenu.AddItem(
                        new MenuItem(string.Format("er.drawing.{0}", hero.ChampionName.ToLower()), "Draw Heimerdinger Turret Range")
                            .SetValue(new Circle(true, Color.DarkOrange)));
                }
                foreach (var hero in HeroManager.Enemies.Where(hero => hero.IsEnemy && hero.ChampionName == "Gangplank"))
                {
                    drawingMenu.AddItem(
                        new MenuItem(string.Format("er.drawing.{0}", hero.ChampionName.ToLower()), "Draw GP Barrel Range")
                            .SetValue(new Circle(true, Color.DarkOrange)));
                }
                foreach (var hero in HeroManager.Enemies.Where(hero => hero.IsEnemy && hero.ChampionName == "Maokai"))
                {
                    drawingMenu.AddItem(
                        new MenuItem(string.Format("er.drawing.{0}", hero.ChampionName.ToLower()), "Draw Maokai Sapling Range")
                            .SetValue(new Circle(true, Color.DarkOrange)));
                }
                foreach (var hero in HeroManager.Enemies.Where(hero => hero.IsEnemy && hero.ChampionName == "Zyra"))
                {
                    drawingMenu.AddItem(
                        new MenuItem(string.Format("er.drawing.{0}", hero.ChampionName.ToLower()), "Draw Zyra Plant Range")
                            .SetValue(new Circle(true, Color.DarkOrange)));
                }

                drawingMenu.AddItem(new MenuItem("er.drawing.drawingsOff", "Turn drawings off").SetValue(false));
            }

            EnemyRange.Menu.AddItem(new MenuItem("seperator", ""));
            EnemyRange.Menu.AddItem(new MenuItem("by.blacky", "Made by blacky"));

            EnemyRange.Menu.AddToMainMenu();
        }
    }
}