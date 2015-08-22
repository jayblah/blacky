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


namespace BlackZilean
{
    public static class Program
    {
        private const string ChampionName = "Zilean";
        private static Obj_AI_Hero _player;
        private static readonly List<Spell> SpellList = new List<Spell>();
        private static readonly Dictionary<Spells, Spell> Spells = new Dictionary<Spells, Spell>
                                                                {
                                                                    { Spells.Q, new Spell(SpellSlot.Q, 675) }, 
                                                                    { Spells.Q1, new Spell(SpellSlot.Q, 1100) }, 
                                                                    { Spells.W, new Spell(SpellSlot.W, 1000) }, 
                                                                    { Spells.E, new Spell(SpellSlot.E, 425) }, 
                                                                    { Spells.R, new Spell(SpellSlot.R, 1400) }
                                                                };
        private static SpellSlot _ignite;
        private static Menu _menu;
        private static Orbwalking.Orbwalker _orbwalker;
        private static ManaManager _manaManager;

        private static HitChance CustomHitChance
        {
            get { return GetHitchance(); }
        }

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

            _timeBomb = new Spell(SpellSlot.Q, 900);
            _rewind = new Spell(SpellSlot.W, 0);
            _timeWarp = new Spell(SpellSlot.E, 700);
            _chronoshift = new Spell(SpellSlot.R, 900);

            Spells[SpellSlot.Q].SetTargetted(0.25f, 2000);
            Spells[SpellSlot.W].SetSkillshot(0.25f, 300, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Spells[SpellSlot.E].SetSkillshot(0.0f, 90, 1200, false, SkillshotType.SkillshotLine);
            Spells[SpellSlot.R].SetSkillshot(0.25f, 250, float.MaxValue, false, SkillshotType.SkillshotCircle);

            SpellList.AddRange(new[] { _timeBomb, _timeWarp });

            _timeBomb.SetSkillshot(0.30f, 210f, 2000f, false, SkillshotType.SkillshotCircle);
            _ignite = _player.GetSpellSlot("summonerdot");

            _manaManager = new ManaManager();

            CreateMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;

            ShowNotification("BlackZilean by blacky - Loaded", Color.Crimson, 10000);
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
                case Orbwalking.OrbwalkingMode.Mixed:
                    OnHarass();
                    break;
            }

            Killsteal();
            OnImmobile();
            SelfUlt();
            AllyUlt();

            if (_menu.Item("useFlee").GetValue<KeyBind>().Active)
            {
                Flee();
            }
        }

        #endregion

        #region OnImmobile

        private static void OnImmobile()
        {
            if (_menu.Item("miscImmobile").GetValue<bool>())
            {
                foreach (var pred in
                    HeroManager.Enemies.Where(
                        hero => hero.IsValidTarget() && hero.Distance(_player.Position) <= _timeBomb.Range)
                        .Select(target => _timeBomb.GetPrediction(target))
                        .Where(pred => pred.Hitchance == HitChance.Immobile))
                {
                    _timeBomb.Cast(pred.CastPosition);
                }
            }
        }

        #endregion

        #region Combo

        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(_timeBomb.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid)
                return;

            var qCombo = _menu.Item("useQ").GetValue<bool>();
            var wCombo = _menu.Item("useW").GetValue<bool>();
            var eCombo = _menu.Item("useE").GetValue<bool>();
            var useIgnite = _menu.Item("useIgnite").GetValue<bool>();

            if (qCombo && _timeBomb.IsReady() && _player.Distance(target) <= _timeBomb.Range)
            {
                _timeBomb.CastIfHitchanceEquals(target, CustomHitChance);
            }

            if (eCombo && _timeWarp.IsReady() && _player.Distance(target) <= _timeWarp.Range)
            {
                _timeWarp.Cast(target);
            }

            if (wCombo && target.HasBuff("ZileanQEnemyBomb"))
            {
                _rewind.Cast();
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
            var target = TargetSelector.GetTarget(_timeBomb.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid)
                return;

            var qHarass = _menu.Item("useQHarass").GetValue<bool>();
            var eHarass = _menu.Item("useEHarass").GetValue<bool>();

            if (qHarass && _timeBomb.IsReady() && _player.Distance(target) <= _timeBomb.Range)
            {
                _timeBomb.CastIfHitchanceEquals(target, CustomHitChance);
            }

            if (eHarass && _timeWarp.IsReady() && _player.Distance(target) <= _timeWarp.Range)
            {
                _timeWarp.Cast(target);
            }
        }

        #endregion

        #region WaveClear

        private static void WaveClear()
        {
            var minion = MinionManager.GetMinions(_player.ServerPosition, _timeBomb.Range).FirstOrDefault();
            if (minion == null || minion.Name.ToLower().Contains("ward")) return;

            var qWc = _menu.Item("useQWC").GetValue<bool>();
            var wWc = _menu.Item("useWWC").GetValue<bool>();

            var farmLocation = MinionManager.GetBestCircularFarmLocation(MinionManager.GetMinions(_timeBomb.Range, MinionTypes.All, MinionTeam.Enemy).Select(m => m.ServerPosition.To2D()).ToList(), _timeBomb.Width, _timeBomb.Range);

            if (qWc && minion.IsValidTarget() && _timeBomb.IsReady())
            {
                _timeBomb.Cast(farmLocation.Position);
            }

            if (wWc & !_timeBomb.IsReady())
            {
                _rewind.Cast();
            }
        }

        #endregion

        #region Flee

        private static void Flee()
        {
            Orbwalking.Orbwalk(null, Game.CursorPos);

            if (_timeWarp.IsReady())
            {
                _timeWarp.Cast(_player);
            }

            if (_rewind.IsReady() && !_timeWarp.IsReady())
            {
                _rewind.Cast();
            }
        }

        #endregion

        #region SelfUlt
        // took something from you jQuery because i was too lazy :p
        private static void SelfUlt()
        {
            var ultSelf = _menu.Item("ultSelf").GetValue<bool>();
            var ultSelfHp = _menu.Item("ultSelfHP").GetValue<Slider>().Value;

            if (_player.IsRecalling() || _player.InFountain())
                return;

            if (ultSelf && (_player.Health / _player.MaxHealth) * 100 <= ultSelfHp && _chronoshift.IsReady() && _player.CountEnemiesInRange(650) > 0)
            {
                _chronoshift.Cast(_player);
            }
        }

        #endregion

        #region AllyUlt
        // took something from you jQuery because i was too lazy :p
        private static void AllyUlt()
        {
            var ultAlly = _menu.Item("ultAlly").GetValue<bool>();
            var ultAllyHp = _menu.Item("ultAllyHP").GetValue<Slider>().Value;

            foreach (var aChamp in ObjectManager.Get<Obj_AI_Hero>().Where(aChamp => aChamp.IsAlly && !aChamp.IsMe))
            {
                var allys = _menu.Item("ultCastAlly" + aChamp.BaseSkinName);

                if (_player.InFountain() || _player.IsRecalling())
                    return;

                if (ultAlly && ((aChamp.Health / aChamp.MaxHealth) * 100 <= ultAllyHp) && _chronoshift.IsReady() &&
                    _player.CountEnemiesInRange(900) > 0 && (aChamp.Distance(_player.Position) <= _chronoshift.Range))
                {
                    if (allys != null && allys.GetValue<bool>())
                    {
                        _chronoshift.Cast(aChamp);
                    }
                }
            }
        }

        #endregion

        #region KillSteal

        private static void Killsteal()
        {
            if (_menu.Item("miscKillsteal").GetValue<bool>())
            {
                foreach (PredictionOutput pred in
                    from target in HeroManager.Enemies.Where(hero => hero.IsValidTarget(_timeBomb.Range))
                    let prediction = _timeBomb.GetPrediction(target)
                    let timeBombDamage = GetComboDamage(target)
                    where target.Health <= timeBombDamage && prediction.Hitchance >= HitChance.Medium
                    select prediction)
                {
                    _timeBomb.Cast(pred.CastPosition);
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
                keybindings.AddItem(new MenuItem("useFlee", "Flee").SetValue(new KeyBind('G', KeyBindType.Press)));
                _menu.AddSubMenu(keybindings);
            }

            var combo = new Menu("Combo Options", "combo");
            {
                combo.AddItem(new MenuItem("useQ", "Use Time Bomb (Q)").SetValue(true));
                combo.AddItem(new MenuItem("useW", "Use Rewind (W)").SetValue(true));
                combo.AddItem(new MenuItem("useE", "Use Time Warp (E)").SetValue(true));
                combo.AddItem(new MenuItem("useIgnite", "Use Ignite").SetValue(true));
                _menu.AddSubMenu(combo);
            }

            var harass = new Menu("Harass Options", "harass");
            {
                harass.AddItem(new MenuItem("useQHarass", "Use Time Bomb (Q)").SetValue(true));
                harass.AddItem(new MenuItem("useEHarass", "Use Time Warp (E)").SetValue(false));
                _menu.AddSubMenu(harass);
            }

            var waveclear = new Menu("Waveclear Options", "waveclear");
            {
                waveclear.AddItem(new MenuItem("useQWC", "Use Time Bomb (Q)").SetValue(true));
                waveclear.AddItem(new MenuItem("useWWC", "Use Rewind (W)").SetValue(true));
                _menu.AddSubMenu(waveclear);
            }

            _manaManager.AddToMenu(ref _menu);

            var ult = new Menu("Ult Options", "ult");
            {
                ult.AddItem(new MenuItem("ultAlly", "Use Chronoshift (R) on Ally").SetValue(true));
                ult.AddItem(new MenuItem("ultAllyHP", "Health % to Chronoshift (R) Ally")).SetValue(new Slider(25, 1, 100));
                ult.AddItem(new MenuItem("sep1", ""));
                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsAlly && !hero.IsMe))
                    ult.AddItem(new MenuItem("ultCastAlly" + hero.BaseSkinName, hero.BaseSkinName).SetValue(true));
                ult.AddItem(new MenuItem("sep2", ""));
                ult.AddItem(new MenuItem("ultSelf", "Use Chronoshift (R) on self").SetValue(true));
                ult.AddItem(new MenuItem("ultSelfHP", "Health % to Chronoshift (R) self")).SetValue(new Slider(25, 1, 100));
                _menu.AddSubMenu(ult);
            }
            var misc = new Menu("Misc Options", "misc");
            {
                misc.AddItem(new MenuItem("miscImmobile", "Time Bomb (Q) on immobile").SetValue(true));
                misc.AddItem(new MenuItem("miscKillsteal", "Time Bomb (Q) to Killsteal").SetValue(true));
                misc.AddItem(
                    new MenuItem("hitChanceSetting", "Hitchance").SetValue(
                        new StringList(new[] { "Low", "Medium", "High", "Very High" }, 3)));
                _menu.AddSubMenu(misc);
            }

            var drawings = new Menu("Drawing Options", "drawings");
            {
                drawings.AddItem(new MenuItem("drawRangeQR", "Q / R range").SetValue(new Circle(true, Color.Aquamarine)));
                drawings.AddItem(new MenuItem("drawRangeE", "E range").SetValue(new Circle(true, Color.Aquamarine)));
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

        #region GetHitChance

        private static HitChance GetHitchance()
        {
            switch (_menu.Item("hitChanceSetting").GetValue<StringList>().SelectedIndex)
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

        #region GetComboDamage

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (_timeBomb.IsReady())
            {
                damage += _player.GetSpellDamage(enemy, SpellSlot.Q);
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
