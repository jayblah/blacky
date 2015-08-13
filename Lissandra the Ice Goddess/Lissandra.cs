using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Lissandra_the_Ice_Goddess.Evade;
using Color = System.Drawing.Color;
using Menu = LeagueSharp.Common.Menu;
using Lissandra_the_Ice_Goddess.Handlers;
using Lissandra_the_Ice_Goddess.Utility;
using SharpDX;

namespace Lissandra_the_Ice_Goddess
{
    internal class Lissandra
    {
        #region Static Fields

        public static Menu Menu;
        public static Orbwalking.Orbwalker Orbwalker;

        private const string ChampionName = "Lissandra";
        private static Obj_AI_Hero player;

        public static Vector3 EStart { get; set; }
        public static Vector3 EEnd { get; set; }

        public static float EStartTick { get; set; }

        public static float EEndTick
        {
            get { return (EStartTick + 1500) / 1000f; }
        }

        public static Vector3 CurrentEPosition
        {
            get
            {
                var currentPoint =(float) Math.Floor(
                        ((Environment.TickCount - EStartTick) / 1000f - SkillsHandler.Spells[SpellSlot.E].Delay) *
                        SkillsHandler.Spells[SpellSlot.E].Speed);
                return EStart.Extend(EEnd,  currentPoint < SkillsHandler.Spells[SpellSlot.E].Range ? currentPoint : SkillsHandler.Spells[SpellSlot.E].Range); 
            }
        }

        public static float TimeFromCurrentToEnd
        {
            get { return (Vector3.Distance(CurrentEPosition, EEnd) / SkillsHandler.Spells[SpellSlot.E].Speed) * 1000f; }
        }
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
                DamageIndicator.DrawingColor = Color.GreenYellow;

                MenuGenerator.Load();
                SkillsHandler.Load();
                AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
                Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
                Drawing.OnDraw += DrawHandler.OnDraw;
                Game.OnUpdate += OnUpdate;
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

                EStartTick = -1;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Event Delegates
        static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && ObjectManager.Player.GetSpellSlot(args.SData.Name) == SpellSlot.E)
            {
                if (EEndTick < Utils.TickCount)
                {
                    EStart = Vector3.Zero;
                    EEnd = Vector3.Zero;
                    EStartTick = -1;
                }
                else
                {
                    EStart = args.Start;
                    EEnd = player.ServerPosition.Extend(args.End, SkillsHandler.Spells[SpellSlot.E].Range);
                    EStartTick = Utils.TickCount;
                }
                
            }
        }

        #region OnEnemyGapcloser

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("misc.gapcloseW").GetValue<bool>() && SkillsHandler.Spells[SpellSlot.W].IsReady()
                && SkillsHandler.Spells[SpellSlot.W].IsInRange(gapcloser.Sender))
            {
                SkillsHandler.Spells[SpellSlot.W].Cast();
            }
        }

        #endregion

        #region OnInterruptableTarget

        private static void OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Menu.Item("misc.interruptR").GetValue<bool>())
            {
                if (args.DangerLevel == Interrupter2.DangerLevel.High
                    && sender.IsValidTarget(SkillsHandler.Spells[SpellSlot.R].Range)
                    && SkillsHandler.Spells[SpellSlot.R].IsReady())
                {
                    SkillsHandler.Spells[SpellSlot.R].CastOnUnit(sender);
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

            OnUpdateMethods();
        }

        #endregion

        #endregion

        #region OnUpdateMethods

        private static void OnUpdateMethods()
        {
            CheckEvade();
        }

        private static void CheckEvade()
        {
            if (EStart != Vector3.Zero && 
                EEnd != Vector3.Zero &&
                EvadeHelper.EvadeDetectedSkillshots.Any(
                    skillshot => 
                        skillshot.SpellData.IsDangerous 
                     && skillshot.SpellData.DangerValue >= 3
                     && skillshot.IsAboutToHit((int)TimeFromCurrentToEnd, EEnd)))
            {
                if (CurrentEPosition.IsSafePosition())
                {
                    SkillsHandler.Spells[SpellSlot.E].Cast();
                }
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

        public static float GetComboDamage(Obj_AI_Base target)
        {
            var damage = 0d;

            if (SkillsHandler.Spells[SpellSlot.Q].IsReady())
            {
                damage += player.GetSpellDamage(target, SpellSlot.Q);
            }

            if (SkillsHandler.Spells[SpellSlot.W].IsReady())
            {
                damage += player.GetSpellDamage(target, SpellSlot.W);
            }

            if (SkillsHandler.Spells[SpellSlot.E].IsReady())
            {
                damage += player.GetSpellDamage(target, SpellSlot.E);
            }

            if (SkillsHandler.Spells[SpellSlot.R].IsReady())
            {
                damage += player.GetSpellDamage(target, SpellSlot.R);
            }

            return (float)damage;
        }

        #endregion

        #region GetIgniteDamage

        private static float GetIgniteDamage(Obj_AI_Base target)
        {
            if (SkillsHandler.Ignite == SpellSlot.Unknown || player.Spellbook.CanUseSpell(SkillsHandler.Ignite) != SpellState.Ready)
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
                LeagueSharp.Common.Utility.DelayAction.Add(duration, () => notif.Dispose());
            }
            return notif;
        }

        #endregion
    }
}