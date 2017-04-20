﻿namespace CompleteLastHitMarker.Abilities.Passive
{
    using System.Collections.Generic;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions;

    using Interfaces;

    using Units.Base;

    using Utils;

    [Ability(AbilityId.bounty_hunter_jinada)]
    internal class Jinada : DefaultPassive
    {
        public Jinada(Ability ability)
            : base(ability)
        {
            for (var i = 0u; i < ability.MaximumLevel; i++)
            {
                Damage[i] = Ability.AbilitySpecialData.First(x => x.Name == "crit_multiplier").GetValue(i) / 100;
            }
        }

        public override float GetBonusDamage(Hero hero, KillableUnit unit, IEnumerable<IPassiveAbility> abilities)
        {
            if (!CanDoDamage(hero, unit, abilities))
            {
                return 0;
            }

            if (unit.UnitType == UnitType.Tower)
            {
                return 0;
            }

            return hero.MinimumDamage * Damage[Level - 1] - hero.MinimumDamage;
        }

        public override bool IsValid(Hero hero, KillableUnit unit)
        {
            return base.IsValid(hero, unit) && Ability.CanBeCasted();
        }
    }
}