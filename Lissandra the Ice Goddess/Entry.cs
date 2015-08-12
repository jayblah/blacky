using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Lissandra_the_Ice_Goddess
{

    using Color = System.Drawing.Color;
    using Menu = LeagueSharp.Common.Menu;

    internal class Entry
    {
        #region Static Fields

        public static Menu Menu;
        public static Orbwalking.Orbwalker Orbwalker;

        private const string ChampionName = "Lissandra";
        private static Obj_AI_Hero player;

        #endregion

        #region OnLoad

        public static void OnLoad(EventArgs args)
        {
            try
            {
                player = ObjectManager.Player;

                ShowNotification("Ice Goddess by blacky & Asuna - Loaded", Color.Crimson, 10000);

                InitializeMenu.Load();
                InitializeSkills.Load();
                Drawing.OnDraw += OnDraw;
                Game.OnUpdate += OnUpdate;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region OnDraw

        private static void OnDraw(EventArgs args)
        {
            var drawQ = Menu.Item("drawing.drawQ").GetValue<Circle>();

            if (drawQ.Active)
            {
                Render.Circle.DrawCircle(player.Position, InitializeSkills.Spells[SpellSlot.Q].Range, drawQ.Color);
            }
        }

        #endregion

        #region OnUpdate

        private static void OnUpdate(EventArgs args)
        {

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