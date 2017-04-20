﻿namespace CompleteLastHitMarker.Abilities.Passive
{
    using System.Collections.Generic;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;

    using Interfaces;

    using Units.Base;

    using Utils;

    [Ability(AbilityId.item_quelling_blade)]
    internal class QuellingBlade : DefaultPassive
    {
        private readonly float meleeBonus;

        private readonly float rangedBonus;

        public QuellingBlade(Ability ability)
            : base(ability)
        {
            DamageType = DamageType.Physical;

            meleeBonus = ability.AbilitySpecialData.First(x => x.Name == "damage_bonus").Value;
            rangedBonus = ability.AbilitySpecialData.First(x => x.Name == "damage_bonus_ranged").Value;

            DoesNotStackWith.Add(AbilityId.kunkka_tidebringer);
            DoesNotStackWith.Add(AbilityId.item_iron_talon);
        }

        public override float GetBonusDamage(Hero hero, KillableUnit unit, IEnumerable<IPassiveAbility> abilities)
        {
            if (!CanDoDamage(hero, unit, abilities))
            {
                return 0;
            }

            if (unit.UnitType != UnitType.Creep)
            {
                return 0;
            }

            return hero.IsMelee ? meleeBonus : rangedBonus;
        }
    }
}