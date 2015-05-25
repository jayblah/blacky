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


namespace BlackPoppy
{
    public static class Program
    {
        private const string ChampionName = "Poppy";
        private static Obj_AI_Hero _player;
        private static readonly List<Spell> SpellList = new List<Spell>();
        private static Spell _devastatingBlow, _paragonOfDemacia, _heroicCharge, _diplomaticImmunity;
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

            _devastatingBlow = new Spell(SpellSlot.Q, 0);
            _paragonOfDemacia = new Spell(SpellSlot.W, 0);
            _heroicCharge = new Spell(SpellSlot.E, 525);
            _diplomaticImmunity = new Spell(SpellSlot.R, 900);

            SpellList.AddRange(new[] { _heroicCharge, _diplomaticImmunity });

            _ignite = _player.GetSpellSlot("summonerdot");

            _manaManager = new ManaManager();

            CreateMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Orbwalking.OnAttack += OnAttack;

            ShowNotification("BlackPoppy by blacky - Loaded", Color.Crimson, 10000);
            ShowNotification("ManaManager by iJabba", Color.Crimson, 10000);
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
            }
        }

        #endregion

        #region OnInterrupt

        private static void OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (_menu.Item("miscInterrupt").GetValue<bool>() && _heroicCharge.IsReady() && _heroicCharge.CanCast(sender))
            {
                _heroicCharge.CastOnUnit(sender);
            }
        }

        #endregion

        #region OnGapcloser

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (_menu.Item("miscGapcloser").GetValue<bool>() && _heroicCharge.IsReady() && _heroicCharge.CanCast(gapcloser.Sender) && WallCheck(_player, gapcloser.Sender))
            {
                _heroicCharge.CastOnUnit(gapcloser.Sender);
            }
        }

        #endregion

        #region OnAttack

        private static void OnAttack(AttackableUnit unit, AttackableUnit target)
        {
            Obj_AI_Hero targetQ = target as Obj_AI_Hero;
            if (unit.IsMe && _devastatingBlow.IsReady() && _menu.Item("useQ").GetValue<bool>())
            {
                if (targetQ.IsValidTarget())
                {
                    _devastatingBlow.Cast();
                }
            }
        }

        #endregion

        #region Combo

        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(_diplomaticImmunity.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid)
                return;

            //var qCombo = _menu.Item("useQ").GetValue<bool>();
            var wCombo = _menu.Item("useW").GetValue<bool>();
            var eCombo = _menu.Item("useE").GetValue<bool>();
            var rCombo = _menu.Item("useR").GetValue<bool>();
            var useIgnite = _menu.Item("useIgnite").GetValue<bool>();

            if (rCombo && _player.Distance(target.Position) < _diplomaticImmunity.Range &&
                ObjectManager.Get<Obj_AI_Hero>().Count(hero => hero.IsValidTarget(_diplomaticImmunity.Range)) >=
                _menu.Item("useRLogic").GetValue<Slider>().Value && _menu.Item("DontUlt" + target.BaseSkinName) != null &&
                _menu.Item("DontUlt" + target.BaseSkinName).GetValue<bool>() == false)
            {
                UltLogic();
            }

            if (eCombo && _heroicCharge.IsReady() && _player.Distance(target.Position) < _heroicCharge.Range)
            {
                if (wCombo)
                {
                    _paragonOfDemacia.Cast();
                }
                ELogic();
            }

            /*
            if (qCombo && _devastatingBlow.IsReady() && _player.Distance(target.Position) < _devastatingBlow.Range)
            {
                _devastatingBlow.Cast();
            }
             */

            if (_player.Distance(target) <= 600 && GetIgniteDamage(target) >= target.Health && useIgnite)
            {
                _player.Spellbook.CastSpell(_ignite, target);
            }
        }

        #endregion

        #region WaveClear

        private static void WaveClear()
        {
            var minion = MinionManager.GetMinions(_player.ServerPosition, _devastatingBlow.Range).FirstOrDefault();
            if (minion == null || minion.Name.ToLower().Contains("ward")) return;

            var qWc = _menu.Item("useQWC").GetValue<bool>();
            var eWc = _menu.Item("useEWC").GetValue<bool>();

            if (qWc && minion.IsValidTarget() && _devastatingBlow.IsReady())
            {
                _devastatingBlow.Cast();
            }

            if (eWc && _heroicCharge.IsReady() && WallCheck(_player, minion))
            {
                _heroicCharge.Cast();
            }
        }

        #endregion

        #region UltLogic

        private static void UltLogic()
        {
            Obj_AI_Hero newtarget = null;
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(_diplomaticImmunity.Range)))
            {
                if (newtarget == null)
                {
                    newtarget = hero;
                }
                else
                {
                    if (hero.Health > newtarget.Health && hero.BaseAttackDamage < newtarget.BaseAttackDamage)
                    {
                        newtarget = hero;
                    }
                }
            }
            _diplomaticImmunity.Cast(newtarget);
        }

        #endregion

        #region ELogic

        private static void ELogic()
        {
            foreach (
                var hero in from hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(_heroicCharge.Range))
                            let prediction = _heroicCharge.GetPrediction(hero)
                            where NavMesh.GetCollisionFlags(
                                prediction.UnitPosition.To2D()
                                    .Extend(ObjectManager.Player.ServerPosition.To2D(), -300)
                                    .To3D())
                                .HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(
                                    prediction.UnitPosition.To2D()
                                        .Extend(ObjectManager.Player.ServerPosition.To2D(),
                                            -(300 / 2))
                                        .To3D())
                                    .HasFlag(CollisionFlags.Wall)
                            select hero)
            {
                _heroicCharge.Cast(hero);
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
                keybindings.AddItem(new MenuItem("useWC", "Waveclear").SetValue(new KeyBind('V', KeyBindType.Press)));
                _menu.AddSubMenu(keybindings);
            }

            var combo = new Menu("Combo Options", "combo");
            {
                combo.AddItem(new MenuItem("useQ", "Use Devastating Blow (Q)").SetValue(true));
                combo.AddItem(new MenuItem("useW", "Use Paragon of Demacia  (W)").SetValue(true));
                combo.AddItem(new MenuItem("useE", "Use Heroic Charge (E)").SetValue(true));
                combo.AddItem(new MenuItem("useR", "Use Diplomatic Immunity (R)").SetValue(true));
                combo.AddItem(new MenuItem("useRLogic", "Enemies in Range to use Ult").SetValue(new Slider(2, 1, 5)));
                combo.AddItem(new MenuItem("useIgnite", "Use Ignite").SetValue(true));
                _menu.AddSubMenu(combo);
            }

            var waveclear = new Menu("Waveclear Options", "waveclear");
            {
                waveclear.AddItem(new MenuItem("useQWC", "Use Devastating Blow (Q)").SetValue(true));
                waveclear.AddItem(new MenuItem("useEWC", "Use Heroic Charge (E)").SetValue(true));
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
                misc.AddItem(new MenuItem("miscInterrupt", "Heroic Charge (E) to Interrupt").SetValue(true));
                misc.AddItem(new MenuItem("miscGapcloser", "Heroic Charge (E) on Gapcloser near Walls").SetValue(true));
                _menu.AddSubMenu(misc);
            }

            var drawings = new Menu("Drawing Options", "drawings");
            {
                drawings.AddItem(new MenuItem("drawRangeE", "E range").SetValue(new Circle(true, Color.Aquamarine)));
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

        #region WallCheck

        public static bool WallCheck(Obj_AI_Base player, Obj_AI_Base enemy)
        {
            var distance = player.Position.Distance(enemy.Position);
            for (int i = 1; i < 6; i++)
            {
                if (player.Position.Extend(enemy.Position, distance + 60 * i).IsWall())
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region GetComboDamage

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (_devastatingBlow.IsReady())
            {
                damage += _player.GetSpellDamage(enemy, SpellSlot.Q);
            }

            if (_heroicCharge.IsReady())
            {
                damage += _player.GetSpellDamage(enemy, SpellSlot.E);
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
