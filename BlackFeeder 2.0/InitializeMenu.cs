using LeagueSharp.Common;

namespace BlackFeeder
{
    public class InitializeMenu
    {
        public static Menu Menu;

        public static void Load()
        {
            Menu = new Menu("BlackFeeder 2.0", "BlackFeeder", true);

            Menu.AddItem(new MenuItem("Feeding.Activated", "Feeding Activated").SetValue(true));
            Menu.AddItem(new MenuItem("Feeding.FeedMode", "Feeding Mode:").SetValue(new StringList(new[] { "Middle Lane", "Bottom Lane", "Top Lane" })));

            var feedingMenu = Menu.AddSubMenu(new Menu("Feeding Options", "FeedingMenu"));
            {
                feedingMenu.AddItem(new MenuItem("Spells.Activated", "Spells Activated").SetValue(true));
                feedingMenu.AddItem(new MenuItem("Messages.Activated", "Messages Activated").SetValue(true));
                feedingMenu.AddItem(new MenuItem("Laugh.Activated", "Laugh Activated").SetValue(true));
                feedingMenu.AddItem(new MenuItem("Items.Activated", "Items Activated").SetValue(true));
            }

            Menu.AddItem(new MenuItem("seperator", ""));
            Menu.AddItem(new MenuItem("by.blacky", "Made by blacky"));

            Menu.AddToMainMenu();
        }
    }
}