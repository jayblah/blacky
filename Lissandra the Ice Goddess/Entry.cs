using System;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using Menu = LeagueSharp.Common.Menu;

namespace Lissandra_the_Ice_Goddess
{
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

                if (player.ChampionName != ChampionName)
                {
                    return;
                }

                ShowNotification("Lissandra the Ice Goddess", Color.DeepSkyBlue, 10000);
                ShowNotification("by blacky & Asuna - Loaded", Color.DeepSkyBlue, 10000);

                DamageIndicator.Initialize(GetComboDamage);
                DamageIndicator.Enabled = true;
                DamageIndicator.DrawingColor = Color.Green;

                InitializeMenu.Load();
                InitializeSkills.Load();
                AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
                Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
                Drawing.OnDraw += InitializeDrawings.OnDraw;
                Game.OnUpdate += OnUpdate;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region OnEnemyGapcloser

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("misc.gapcloseW").GetValue<bool>() && InitializeSkills.Spells[SpellSlot.W].IsReady() && InitializeSkills.Spells[SpellSlot.W].IsInRange(gapcloser.Sender))
                InitializeSkills.Spells[SpellSlot.W].Cast();
        }

        #endregion

        #region OnInterruptableTarget

        private static void OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Menu.Item("misc.interruptR").GetValue<bool>())
            {
                if (args.DangerLevel == Interrupter2.DangerLevel.High
                    && sender.IsValidTarget(InitializeSkills.Spells[SpellSlot.R].Range)
                    && InitializeSkills.Spells[SpellSlot.R].IsReady())
                {
                    InitializeSkills.Spells[SpellSlot.R].CastOnUnit(sender);
                }
            }

        }

        #endregion

        #region OnUpdate

        private static void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Waveclear();
                    break;
            }
        }

        #endregion

        #region Combo

        private static void Combo()
        {
            
        }

        #endregion

        #region Harass

        private static void Harass()
        {

        }

        #endregion

        #region Waveclear

        private static void Waveclear()
        {

        }

        #endregion

        #region GetComboDamage

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (InitializeSkills.Spells[SpellSlot.Q].IsReady())
            {
                damage += player.GetSpellDamage(enemy, SpellSlot.Q);
            }

            if (InitializeSkills.Spells[SpellSlot.W].IsReady())
            {
                damage += player.GetSpellDamage(enemy, SpellSlot.W);
            }

            if (InitializeSkills.Spells[SpellSlot.E].IsReady())
            {
                damage += player.GetSpellDamage(enemy, SpellSlot.E);
            }

            if (InitializeSkills.Spells[SpellSlot.R].IsReady())
            {
                damage += player.GetSpellDamage(enemy, SpellSlot.R);
            }

            return (float)damage;
        }

        #endregion

        #region GetIgniteDamage

        private static float GetIgniteDamage(Obj_AI_Hero target)
        {
            if (InitializeSkills.Ignite == SpellSlot.Unknown || player.Spellbook.CanUseSpell(InitializeSkills.Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
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