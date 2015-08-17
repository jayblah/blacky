using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Lissandra_the_Ice_Goddess.Utility.Damage
{
    class DamagePrediction
    {
        public delegate void OnKillableDelegate(Obj_AI_Hero sender, Obj_AI_Hero target, SpellData SData);

        public static event OnKillableDelegate OnTargettedSpellWillKill;

        private static int HealthBuffer = 15; //Safety check

        static DamagePrediction()
        {
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(sender is Obj_AI_Hero) || !(args.Target is Obj_AI_Hero))
            {
                return;
            }

            var senderHero = (Obj_AI_Hero) sender;
            var targetHero = (Obj_AI_Hero) args.Target;

            var predictedDamage = Orbwalking.IsAutoAttack(args.SData.Name)
                ? senderHero.GetAutoAttackDamage(targetHero, true)
                : senderHero.GetSpellDamage(targetHero, args.SData.Name);

            if (predictedDamage > targetHero.Health + HealthBuffer)
            {
                if (OnTargettedSpellWillKill != null)
                {
                    OnTargettedSpellWillKill(senderHero, targetHero, args.SData);
                }
            }
        }
    }
}
