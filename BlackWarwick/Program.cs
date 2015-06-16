// 
// LeagueSharp.Common is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with LeagueSharp.Common.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace BlackWarwick
{
    public static class Program
    {
        private const string ChampionName = "Warwick";
        private static Obj_AI_Hero _player;
        private static readonly List<Spell> SpellList = new List<Spell>();
        private static Spell _hungeringStrike, _huntersCall, _infiniteDuress;
        private static SpellSlot _ignite;
        private static Menu _menu;
        private static Orbwalking.Orbwalker _orbwalker;
        private static ManaManager _manaManager;

        #region Main

        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        #endregion

        #region OnGameLoad

        private static void Game_OnGameLoad(EventArgs args)
        {
            _player = ObjectManager.Player;
            if (_player.ChampionName != ChampionName)
            {
                return;
            }

            DamageIndicator.Initialize(GetComboDamage);
            DamageIndicator.Enabled = true;
            DamageIndicator.DrawingColor = Color.Green;

            _hungeringStrike = new Spell(SpellSlot.Q, 400);
            _huntersCall = new Spell(SpellSlot.W, 1250);
            _infiniteDuress = new Spell(SpellSlot.R, 700);

            SpellList.AddRange(new[] { _hungeringStrike, _huntersCall, _infiniteDuress});

            _ignite = _player.GetSpellSlot("summonerdot");

            _manaManager = new ManaManager();

            CreateMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.OnAttack += OnAttack;

            ShowNotification("BlackWarwick by blacky - Loaded", Color.Crimson, 10000);
            ShowNotification("ManaManager by iJabba", Color.Crimson, 10000);
        }

        #endregion

        #region OnAttack

        private static void OnAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!_huntersCall.IsReady())
            {
                return;
            }

            if (((_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && _menu.Item("useW").GetValue<bool>() ||
                  _orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && _menu.Item("useWHarass").GetValue<bool>())) &&
                target is Obj_AI_Hero ||
                (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && _menu.Item("useWWC").GetValue<bool>() && target is Obj_AI_Minion))
            {
                _huntersCall.Cast();
            }
        }

        #endregion

        #region OnDraw

        private static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SpellList)
            {
                var circleEntry = _menu.Item("drawRange" + spell.Slot).GetValue<Circle>();
                if (circleEntry.Active && !_player.IsDead)
                {
                    Render.Circle.DrawCircle(_player.Position, spell.Range, circleEntry.Color);
                }
            }

            Circle damageCircle = _menu.Item("drawDamage").GetValue<Circle>();

            DamageIndicator.DrawingColor = damageCircle.Color;
            DamageIndicator.Enabled = damageCircle.Active;
        }

        #endregion

        #region OnGameUpdate

        private static void Game_OnGameUpdate(EventArgs args)
        {
            switch (_orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    WaveClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    OnHarass();
                    break;
            }

            Killsteal();
        }

        #endregion

        #region Combo

        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(_infiniteDuress.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid)
                return;

            var qCombo = _menu.Item("useQ").GetValue<bool>();
            var rCombo = _menu.Item("useR").GetValue<bool>();
            var useRSmite = _menu.Item("useRSmite").GetValue<bool>();
            var useIgnite = _menu.Item("useIgnite").GetValue<bool>();

            if (useRSmite && _player.GetSpellSlot(GetSmiteName()).IsReady() && rCombo &&
                _menu.Item("DontUlt" + target.BaseSkinName) != null &&
                _menu.Item("DontUlt" + target.BaseSkinName).GetValue<bool>() == false)
            {
                _player.Spellbook.CastSpell(_player.GetSpellSlot(GetSmiteName()), target);
            }

            if (rCombo && _infiniteDuress.IsReady() && _player.Distance(target) <= _infiniteDuress.Range)
            {
                if (_menu.Item("DontUlt" + target.BaseSkinName) != null &&
                    _menu.Item("DontUlt" + target.BaseSkinName).GetValue<bool>() == false)
                {
                    _infiniteDuress.Cast(target);
                }
            }

            if (qCombo && _hungeringStrike.IsReady() && _player.Distance(target) <= _hungeringStrike.Range)
            {
                _hungeringStrike.Cast(target);
            }

            if (_player.Distance(target) <= 600 && GetIgniteDamage(target) >= target.Health && useIgnite)
            {
                _player.Spellbook.CastSpell(_ignite, target);
            }
        }

        #endregion

        #region Harass

        private static void OnHarass()
        {
            var target = TargetSelector.GetTarget(_hungeringStrike.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid)
                return;

            var qHarass = _menu.Item("useQHarass").GetValue<bool>();

            if (qHarass && _hungeringStrike.IsReady() && _player.Distance(target) <= _hungeringStrike.Range)
            {
                _hungeringStrike.Cast(target);
            }
        }

        #endregion

        #region WaveClear

        private static void WaveClear()
        {
            var minion = MinionManager.GetMinions(_player.ServerPosition, _hungeringStrike.Range).FirstOrDefault();
            if (minion == null || minion.Name.ToLower().Contains("ward")) return;

            var qWc = _menu.Item("useQWC").GetValue<bool>();

            if (qWc && minion.IsValidTarget() && _hungeringStrike.IsKillable(minion) && _hungeringStrike.IsReady())
            {
                _hungeringStrike.Cast(minion);
            }
        }

        #endregion

        #region KillSteal

        private static void Killsteal()
        {
            if (_menu.Item("killstealQ").GetValue<bool>())
            {
                var hungeringStrikeTarget = _hungeringStrike.GetTarget();

                if (hungeringStrikeTarget != null && _hungeringStrike.IsKillable(hungeringStrikeTarget) &&
                    _hungeringStrike.IsReady())
                {
                    _hungeringStrike.Cast(hungeringStrikeTarget);
                }
            }

            if (_menu.Item("killstealR").GetValue<bool>())
            {
                var infiniteDuressTarget = _infiniteDuress.GetTarget();

                if (infiniteDuressTarget != null && _infiniteDuress.IsKillable(infiniteDuressTarget) &&
                    _infiniteDuress.IsReady())
                {
                    _hungeringStrike.Cast(infiniteDuressTarget);
                }
            }

            if (_menu.Item("killstealIgnite").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(600, TargetSelector.DamageType.True);

                if (_player.Distance(target) <= 600 && GetIgniteDamage(target) >= target.Health)
                {
                    _player.Spellbook.CastSpell(_ignite, target);
                }
            }

            if (_menu.Item("killstealSmite").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(760, TargetSelector.DamageType.True);

                if (_player.Distance(target) <= 760 && _player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Smite) >= target.Health)
                {
                    _player.Spellbook.CastSpell(_player.GetSpellSlot(GetSmiteName()), target);
                }
            }
        }

        #endregion

        #region CreateMenu

        private static void CreateMenu()
        {
            _menu = new Menu("Black" + ChampionName, "black" + ChampionName, true);

            var targetSelectorMenu = new Menu("Target Selector", "ts");
            _menu.AddSubMenu(targetSelectorMenu);
            TargetSelector.AddToMenu(targetSelectorMenu);

            var orbwalkingMenu = new Menu("Orbwalking", "orbwalk");
            _menu.AddSubMenu(orbwalkingMenu);
            _orbwalker = new Orbwalking.Orbwalker(orbwalkingMenu);

            var keybindings = new Menu("Key Bindings", "keybindings");
            {
                keybindings.AddItem(new MenuItem("useCombo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
                keybindings.AddItem(new MenuItem("useHarass", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));
                keybindings.AddItem(new MenuItem("useWC", "Waveclear").SetValue(new KeyBind('V', KeyBindType.Press)));
                _menu.AddSubMenu(keybindings);
            }

            var combo = new Menu("Combo Options", "combo");
            {
                combo.AddItem(new MenuItem("useQ", "Use Hungering Strike (Q)").SetValue(true));
                combo.AddItem(new MenuItem("useW", "Use Hunters Call (W)").SetValue(true));
                combo.AddItem(new MenuItem("useR", "Use Infinite Duress (R)").SetValue(true));
                combo.AddItem(new MenuItem("useRSmite", "Use Smite before Infinite Duress (R)").SetValue(true));
                combo.AddItem(new MenuItem("useIgnite", "Use Ignite").SetValue(true));
                _menu.AddSubMenu(combo);
            }

            var harass = new Menu("Harass Options", "harass");
            {
                harass.AddItem(new MenuItem("useQHarass", "Use Hungering Strike (Q").SetValue(true));
                harass.AddItem(new MenuItem("useWHarass", "Use Hunters Call (W)").SetValue(true));
                _menu.AddSubMenu(harass);
            }

            var waveclear = new Menu("Waveclear Options", "waveclear");
            {
                waveclear.AddItem(new MenuItem("useQWC", "Use Hungering Strike (Q").SetValue(true));
                waveclear.AddItem(new MenuItem("useWWC", "Use Hunters Call (W)").SetValue(true));
                _menu.AddSubMenu(waveclear);
            }

            _manaManager.AddToMenu(ref _menu);

            var ult = new Menu("Ult Options", "ult");
            {
                ult.AddItem(new MenuItem("DontUlt", "Dont use R on"));
                ult.AddItem(new MenuItem("sep0", ""));
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != _player.Team))
                {
                    ult.AddItem(new MenuItem("DontUlt" + enemy.BaseSkinName, enemy.BaseSkinName).SetValue(false));
                }
                ult.AddItem(new MenuItem("sep1", ""));
                _menu.AddSubMenu(ult);
            }

            var misc = new Menu("Misc Options", "misc");
            {
                var killsteal = new Menu("Killsteal Options", "killsteal");
                {
                    killsteal.AddItem(new MenuItem("killstealQ", "Hungering Strike (Q) to Killsteal").SetValue(true));
                    killsteal.AddItem(new MenuItem("killstealR", "Infinite Duress (R) to Killsteal").SetValue(false));
                    killsteal.AddItem(new MenuItem("killstealIgnite", "Ignite to Killsteal").SetValue(true));
                    killsteal.AddItem(new MenuItem("killstealSmite", "Smite to Killsteal").SetValue(true));
                }
                _menu.AddSubMenu(misc);
            }

            var drawings = new Menu("Drawing Options", "drawings");
            {
                drawings.AddItem(new MenuItem("drawRangeQ", "Q range").SetValue(new Circle(true, Color.Aquamarine)));
                drawings.AddItem(new MenuItem("drawRangeW", "W range").SetValue(new Circle(true, Color.Aquamarine)));
                drawings.AddItem(new MenuItem("drawRangeR", "R range").SetValue(new Circle(true, Color.Aquamarine)));
                drawings.AddItem(new MenuItem("drawDamage", "Draw Spell Damage").SetValue(new Circle(true, Color.GreenYellow)));
                _menu.AddSubMenu(drawings);
            }

            _menu.AddToMainMenu();
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

        #region GetSmiteName

        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };


        private static string GetSmiteName()
        {
            if (SmiteBlue.Any(a => Items.HasItem(a)))
            {
                return "s5_summonersmiteplayerganker";
            }
            if (SmiteRed.Any(a => Items.HasItem(a)))
            {
                return "s5_summonersmiteduel";
            }
            if (SmiteGrey.Any(a => Items.HasItem(a)))
            {
                return "s5_summonersmitequick";
            }
            if (SmitePurple.Any(a => Items.HasItem(a)))
            {
                return "itemsmiteaoe";
            }
            return "summonersmite";
        }

        #endregion

        #region GetComboDamage

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (_infiniteDuress.IsReady())
            {
                damage += _player.GetSpellDamage(enemy, SpellSlot.R);
            }

            if (_hungeringStrike.IsReady())
            {
                damage += _player.GetSpellDamage(enemy, SpellSlot.Q);
            }

            if (_player.GetSpellSlot(GetSmiteName()).IsReady())
            {
                damage += _player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Smite);
            }

            return (float)damage;
        }

        #endregion

        #region GetIgniteDamage

        private static float GetIgniteDamage(Obj_AI_Hero target)
        {
            if (_ignite == SpellSlot.Unknown || _player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)_player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        #endregion
    }
}
