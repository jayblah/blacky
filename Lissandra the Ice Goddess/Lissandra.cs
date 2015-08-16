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
        public static ManaManager ManaManager;
        public static Orbwalking.Orbwalker Orbwalker;

        private const string ChampionName = "Lissandra";
        private static Obj_AI_Hero player;

        private static HitChance CustomHitChance
        {
            get { return GetHitchance(); }
        }

        #region E Related

        private static bool eActive = false;

        public static bool EActive
        {
            get { return eActive && EDuration > 0; }
        }

        public static int ECastTime = 0;

        public static float EDuration
        {
            get { return (ECastTime + 1500 - Environment.TickCount) / 1000f; }
        }

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

                ManaManager = new ManaManager();
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

                if (EDuration > -1)
                {
                    eActive = false;
                }
                else
                {
                    ECastTime = Environment.TickCount;
                    eActive = true;
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
                    OnCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    OnHarass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Waveclear();
                    break;
            }

            if (Menu.Item("flee.activated").GetValue<KeyBind>().Active)
            {
                OnFlee();
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

        private static void OnCombo()
        {
            //TODO The All In Combo.
            var comboTarget = TargetSelector.GetTarget(SkillsHandler.QShard.Range,
                TargetSelector.DamageType.Magical);

            if (comboTarget.IsValidTarget())
            {
                if (GetMenuValue<bool>("lissandra.combo.useQ") 
                    && comboTarget.IsValidTarget(SkillsHandler.QShard.Range) 
                    && SkillsHandler.Spells[SpellSlot.Q].IsReady())
                {
                    var predictionPosition = SkillsHandler.GetQPrediction(comboTarget);
                    if (predictionPosition != null)
                    {
                        //Found a valid Q prediction
                        SkillsHandler.Spells[SpellSlot.Q].Cast((Vector3) predictionPosition);
                    }
                }

                if (GetMenuValue<bool>("lissandra.combo.useW")
                    && comboTarget.IsValidTarget(SkillsHandler.Spells[SpellSlot.W].Range)
                    && SkillsHandler.Spells[SpellSlot.W].IsReady())
                {
                    SkillsHandler.Spells[SpellSlot.W].Cast();
                }

                if (GetMenuValue<bool>("lissandra.combo.useE")
                    && comboTarget.IsValidTarget(SkillsHandler.Spells[SpellSlot.E].Range)
                    && SkillsHandler.Spells[SpellSlot.E].IsReady())
                {
                    if (!EActive)
                    {
                        var comboTargetPosition = Prediction.GetPrediction(comboTarget,
                            TimeToEEnd(ObjectManager.Player.ServerPosition, comboTarget.ServerPosition)).UnitPosition;
                        //TODO This will probably fail horribly because it's such a long delay ayy lmao
                        if (comboTargetPosition.IsSafePositionEx() && comboTargetPosition.PassesNoEIntoEnemiesCheck())
                        {
                            SkillsHandler.Spells[SpellSlot.E].Cast(comboTargetPosition);
                        }
                    }
                    else
                    {
                        if (CurrentEPosition.Distance(comboTarget.ServerPosition) <= 450f || CurrentEPosition.Distance(EEnd) <= 100f)
                        {
                            if (CurrentEPosition.IsSafePositionEx() && CurrentEPosition.PassesNoEIntoEnemiesCheck())
                            {
                                SkillsHandler.Spells[SpellSlot.E].Cast();
                            }
                        }
                    }
                }
            }
        }

        private static float TimeToEEnd(Vector3 from, Vector3 To)
        {
            return (Vector3.Distance(from, To) / SkillsHandler.Spells[SpellSlot.E].Speed) * 1000f; 
        }
        #endregion

        #region Harass

        private static void OnHarass()
        {
            var qtarget = TargetSelector.GetTarget(SkillsHandler.Spells[SpellSlot.Q].Range, TargetSelector.DamageType.Magical);
            var etarget = TargetSelector.GetTarget(SkillsHandler.Spells[SpellSlot.E].Range, TargetSelector.DamageType.Magical);
            if (qtarget == null || !qtarget.IsValid || !ManaManager.CanHarass())
            {
                return;
            }

            var qHarass = Menu.Item("harass.useQ").GetValue<bool>();
            var wHarass = Menu.Item("harass.useW").GetValue<bool>();
            var eHarass = Menu.Item("harass.useE").GetValue<bool>();

            if (qHarass && SkillsHandler.Spells[SpellSlot.Q].IsReady() && SkillsHandler.Spells[SpellSlot.Q].IsInRange(qtarget))
            {
                SkillsHandler.Spells[SpellSlot.Q].CastIfHitchanceEquals(qtarget, CustomHitChance);
            }

            if (wHarass && SkillsHandler.Spells[SpellSlot.W].IsReady() && SkillsHandler.Spells[SpellSlot.W].IsInRange(qtarget))
            {
                    SkillsHandler.Spells[SpellSlot.W].Cast();
            }

            if (eHarass && !EActive && SkillsHandler.Spells[SpellSlot.E].IsReady() && SkillsHandler.Spells[SpellSlot.E].IsInRange(etarget))
            {
                SkillsHandler.Spells[SpellSlot.E].CastIfHitchanceEquals(etarget, CustomHitChance);
            }
        }

        #endregion

        #region Flee

        private static void OnFlee()
        {
            // TODO
            Orbwalking.Orbwalk(null, Game.CursorPos);
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

        #region GetHitChance

        private static HitChance GetHitchance()
        {
            switch (Menu.Item("misc.hitChance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.Medium;
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

        #region Utility Methods

        public static T GetMenuValue<T>(String menuItem)
        {
            return Menu.Item(menuItem).GetValue<T>();
        }
        #endregion

    }
}