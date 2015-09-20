using System;
using System.Linq;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Menu = LeagueSharp.Common.Menu;

namespace EnemyRange
{
    internal class EnemyRange
    {
        #region Static Fields

        public static Menu Menu;

        public static List<Obj_AI_Minion> AzirSoldiers = new List<Obj_AI_Minion>();

        public static List<Obj_AI_Minion> GangplankBarrels = new List<Obj_AI_Minion>();

        public static Vector3 BallPosition { get; set; }

        #endregion

        #region OnLoad

        public static void OnLoad(EventArgs args)
        {
            try
            {
                BallPosition =
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(x => x.IsEnemy && x.CharData.BaseSkinName.Equals("oriannaball"))
                        .Select(x => x.Position)
                        .FirstOrDefault();

                ShowNotification("Enemy Range#", Color.DarkOrange, 10000);
                ShowNotification("by blacky - Loaded", Color.DarkOrange, 10000);

                MenuGenerator.Load();
                Drawing.OnDraw += OnDraw;
                GameObject.OnCreate += OnCreate;
                GameObject.OnDelete += OnDelete;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            int i = 0;
            foreach (var soldier in AzirSoldiers)
            {
                if (soldier.NetworkId == sender.NetworkId)
                {
                    AzirSoldiers.RemoveAt(i);
                    return;
                }
                i++;
            }

            int i2 = 0;
            foreach (var barrel in GangplankBarrels)
            {
                if (barrel.NetworkId == sender.NetworkId)
                {
                    GangplankBarrels.RemoveAt(i2);
                    return;
                }
                i2++;
            }
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "AzirSoldier" && sender.IsEnemy)
            {
                var soldier = sender as Obj_AI_Minion;
                if (soldier != null && soldier.SkinName == "AzirSoldier")
                    AzirSoldiers.Add(soldier);
            }

            if (sender.Name == "Barrel" && sender.IsEnemy)
            {
                var barrel = sender as Obj_AI_Minion;
                if (barrel != null)
                    GangplankBarrels.Add(barrel);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            var drawOff = Menu.Item("er.drawing.drawingsOff").GetValue<bool>();
            var drawAzirSoldier = Menu.Item("er.drawing.azir").GetValue<Circle>();
            var drawOriannaW = Menu.Item("er.drawing.orianna.W").GetValue<Circle>();
            var drawOriannaR = Menu.Item("er.drawing.orianna.R").GetValue<Circle>();
            var drawGpBarrel = Menu.Item("er.drawing.gangplank").GetValue<Circle>();

            if (drawOff || ObjectManager.Player.IsDead)
            {
                return;
            }

            if (drawAzirSoldier.Active)
            {
                foreach (var soldier in AzirSoldiers)
                {
                    if (soldier.IsValid && !soldier.IsDead)
                    {
                        Render.Circle.DrawCircle(soldier.Position, 250, Color.DarkOrange);
                    }
                }
            }

            if (drawOriannaW.Active)
            {
                Render.Circle.DrawCircle(BallPosition, 250, Color.DarkOrange);
            }

            if (drawOriannaR.Active)
            {
                Render.Circle.DrawCircle(BallPosition, 325, Color.DarkOrange);
            }

            if (drawGpBarrel.Active)
            {
                foreach (var barrel in GangplankBarrels)
                {
                    if (barrel.IsValid && !barrel.IsDead)
                    {
                        Render.Circle.DrawCircle(barrel.Position, 100, Color.DarkOrange);
                    }
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
                LeagueSharp.Common.Utility.DelayAction.Add(duration, () => notif.Dispose());
            }
            return notif;
        }

        #endregion
    }
}
