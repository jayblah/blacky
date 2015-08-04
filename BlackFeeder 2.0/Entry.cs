namespace BlackFeeder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;
    using Menu = LeagueSharp.Common.Menu;

    internal class Entry
    {
        #region Static Fields

        #region SpellList

        public static List<ChampWrapper> ListChamp = new List<ChampWrapper>()
                                                         {
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Blitzcrank",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Bard",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "DrMundo",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.R }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Draven",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Evelynn",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Garen",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.Q }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Hecarim",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.E }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Karma",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.E }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Kayle",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Kennen",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.E }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Lulu",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "MasterYi",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.R }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Nunu",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Olaf",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.R }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Orianna",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Poppy",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Quinn",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.R }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Rammus",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.Q }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Rumble",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Ryze",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.R }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Shyvana",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Singed",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.R }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Sivir",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.R }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Skarner",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Sona",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.E }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Teemo",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Trundle",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Twitch",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.Q }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Udyr",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.E }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Volibear",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.Q }
                                                                 },
                                                             new ChampWrapper
                                                                 {
                                                                     Name = "Zilean",
                                                                     SpellSlots = new List<SpellSlot>() { SpellSlot.W, SpellSlot.E }
                                                                 }
                                                         };
        #endregion

        public static Menu Menu;

        private static readonly bool[] BoughtItems = { false, false, false, false };

        private static string[] deaths;

        private static Obj_AI_Hero player;
        private static SpellSlot ghostSlot, healSlot;

        private static readonly Vector3 TopVector3 = new Vector3(2122, 12558, 53);
        private static readonly Vector3 BotVector3 = new Vector3(12608, 2380, 52);
        private static readonly Vector3 PurpleSpawn = new Vector3(14286f, 14382f, 172f);
        private static readonly Vector3 BlueSpawn = new Vector3(416f, 468f, 182f);

        public static bool TopVectorReached;
        public static bool BotVectorReached;

        private static int lastLaugh;
        private static double lastTouchdown;
        private static double timeDead;

        #endregion

        #region OnLoad

        public static void OnLoad(EventArgs args)
        {
            try
            {
                deaths = new[]
                         {
                             "/all XD", "kek", "sorry lag", "/all gg", "help pls", "nooob wtf", "team???", "/all gg my team sucks",
                             "/all matchmaking sucks", "i can't carry dis", "wtf how?", "wow rito nerf pls",
                             "/all report enemys for drophacks", "tilidin y u do dis", "kappa", "amk", "/all einfach mal leben genießen amk"
                         };

                player = ObjectManager.Player;
                ghostSlot = player.GetSpellSlot("SummonerHaste");
                healSlot = player.GetSpellSlot("SummonerHeal");

                ShowNotification("BlackFeeder by blacky - Loaded", Color.Crimson, 10000);

                InitializeMenu.Load();
                Game.OnUpdate += OnUpdate;
                Game.OnEnd += OnEnd;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region OnUpdate

        private static void OnUpdate(EventArgs args)
        {
            if (InitializeMenu.Menu.Item("Feeding.Activated").GetValue<bool>())
            {
                Feed();
            }

            if (player.IsDead || player.InFountain())
            {
                TopVectorReached = false;
                BotVectorReached = false;
            }
            else
            {
                if (player.Distance(BotVector3) <= 300)
                {
                    BotVectorReached = true;
                }

                if (player.Distance(TopVector3) <= 300)
                {
                    TopVectorReached = true;
                }
            }
        }

        #endregion

        #region OnEnd

        private static void OnEnd(EventArgs args)
        {
            Game.Say("/all Good game guys, well played.");
        }

        #endregion

        #region Items

        private static void Items()
        {
            if (player.InShop() && player.Gold >= 325 && !BoughtItems[0])
            {
                player.BuyItem(ItemId.Boots_of_Speed);
                BoughtItems[0] = true;
            }

            if (player.InShop() && player.Gold >= 475 && BoughtItems[0] && !BoughtItems[1])
            {
                player.BuyItem(ItemId.Boots_of_Mobility);
                BoughtItems[1] = true;
            }

            if (player.InShop() && player.Gold >= 475 && BoughtItems[1] && !BoughtItems[2])
            {
                player.BuyItem(ItemId.Boots_of_Mobility_Enchantment_Homeguard);
                BoughtItems[2] = true;
            }

            if (player.InShop() && player.Gold >= 950 && BoughtItems[2] && !BoughtItems[3])
            {
                player.BuyItem(ItemId.Aether_Wisp);
                BoughtItems[3] = true;
            }

            if (player.InShop() && player.Gold >= 1100 && BoughtItems[3])
            {
                player.BuyItem(ItemId.Zeal);
            }
        }

        #endregion

        #region Feed

        private static void Feed()
        {
            var feedingMode = InitializeMenu.Menu.Item("Feeding.FeedMode").GetValue<StringList>().SelectedIndex;

            switch (feedingMode)
            {
                case 0:
                    {
                        if (player.Team == GameObjectTeam.Order)
                        {
                            player.IssueOrder(GameObjectOrder.MoveTo, PurpleSpawn);
                        }
                        else
                        {
                            player.IssueOrder(GameObjectOrder.MoveTo, BlueSpawn);
                        }
                    }
                    break;
                case 1:
                    {
                        if (player.Team == GameObjectTeam.Order)
                        {
                            if (!BotVectorReached)
                                player.IssueOrder(GameObjectOrder.MoveTo, BotVector3);
                            else if (BotVectorReached)
                                player.IssueOrder(GameObjectOrder.MoveTo, PurpleSpawn);
                        }
                        else
                        {
                            if (!BotVectorReached)
                                player.IssueOrder(GameObjectOrder.MoveTo, BotVector3);
                            else if (BotVectorReached)
                                player.IssueOrder(GameObjectOrder.MoveTo, PurpleSpawn);
                        }
                    }
                    break;
                case 2:
                    {
                        if (player.Team == GameObjectTeam.Order)
                        {
                            if (!TopVectorReached)
                                player.IssueOrder(GameObjectOrder.MoveTo, TopVector3);
                            else if (TopVectorReached)
                                player.IssueOrder(GameObjectOrder.MoveTo, PurpleSpawn);
                        }
                        else
                        {
                            if (!TopVectorReached)
                                player.IssueOrder(GameObjectOrder.MoveTo, TopVector3);
                            else if (TopVectorReached)
                                player.IssueOrder(GameObjectOrder.MoveTo, PurpleSpawn);
                        }
                    }
                    break;
            }

            if (InitializeMenu.Menu.Item("Spells.Activated").GetValue<bool>())
            {
                Spells();
            }

            if (InitializeMenu.Menu.Item("Messages.Activated").GetValue<bool>())
            {
                Messages();
            }

            if (InitializeMenu.Menu.Item("Laugh.Activated").GetValue<bool>())
            {
                Laughing();
            }

            if (InitializeMenu.Menu.Item("Items.Activated").GetValue<bool>())
            {
                Items();
            }
        }

        #endregion  

        #region Spells

        private static void Spells()
        {
            if (player.Distance(PurpleSpawn) < 600 | player.Distance(BlueSpawn) < 600)
            {
                return;
            }

            if (ghostSlot != SpellSlot.Unknown && player.Spellbook.CanUseSpell(ghostSlot) == SpellState.Ready)
            {
                player.Spellbook.CastSpell(ghostSlot);
            }

            if (healSlot != SpellSlot.Unknown && player.Spellbook.CanUseSpell(healSlot) == SpellState.Ready)
            {
                player.Spellbook.CastSpell(healSlot);
            }

            var entry = ListChamp.FirstOrDefault(h => h.Name == ObjectManager.Player.ChampionName);

            if (entry == null)
            {
                return;
            }

            var slots = entry.SpellSlots;

            foreach (var slot in slots)
            {
                player.Spellbook.LevelSpell(slot);
                if (player.Spellbook.CanUseSpell(slot) == SpellState.Ready)
                {
                    player.Spellbook.CastSpell(slot, player);
                }
            }
        }

        #endregion

        #region Laughing

        private static void Laughing()
        {
            if (Environment.TickCount <= lastLaugh + 2500)
            {
                return;
            }

            Game.Say("/l");
            lastLaugh = Environment.TickCount;
        }

        #endregion

        #region Messages

        private static void Messages()
        {
            if (player.IsDead && Game.Time - timeDead > 80)
            {
                var r = new Random();
                Game.Say(deaths[r.Next(0,17)]);
                timeDead = Game.Time;
            }

            if (player.Team == GameObjectTeam.Chaos && player.Distance(BlueSpawn) < 600)
            {
                if (Game.Time - lastTouchdown > 80)
                {
                    Game.Say("/all TOUCHDOWN!");
                    lastTouchdown = Game.Time;
                }
            }

            if (player.Team == GameObjectTeam.Order && player.Distance(PurpleSpawn) < 600)
            {
                if (Game.Time - lastTouchdown > 80)
                {
                    Game.Say("/all TOUCHDOWN!");
                    lastTouchdown = Game.Time;
                }
            }
        }

        #endregion

        #region Notifications Credits to Beaving.

        public static Notification ShowNotification(string message, Color color, int duration = -1, bool dispose = true)
        {
            var notif = new Notification(message).SetTextColor(color);
            Notifications.AddNotification(notif);
            if (dispose)
            {
                Utility.DelayAction.Add(duration, () => notif.Dispose());
            }
            return notif;
        }

        #endregion
    }
}