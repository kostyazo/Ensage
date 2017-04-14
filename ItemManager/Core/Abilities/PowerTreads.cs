﻿namespace ItemManager.Core.Abilities
{
    using Attributes;

    using Base;

    using Ensage;

    [Ability(AbilityId.item_power_treads)]
    internal class PowerTreads : UsableAbility
    {
        private readonly Ensage.Items.PowerTreads powerTreads;

        public PowerTreads(Ability ability, Manager manager)
            : base(ability, manager)
        {
            powerTreads = (Ensage.Items.PowerTreads)ability;
            DefaultAttribute = powerTreads.ActiveAttribute;
        }

        public Attribute ActiveAttribute => powerTreads.ActiveAttribute;

        public Attribute DefaultAttribute { get; private set; }

        public void ChangeDefaultAttribute()
        {
            switch (powerTreads.ActiveAttribute)
            {
                case Attribute.Strength:
                    DefaultAttribute = Attribute.Intelligence;
                    break;
                case Attribute.Intelligence:
                    DefaultAttribute = Attribute.Agility;
                    break;
                case Attribute.Agility:
                    DefaultAttribute = Attribute.Strength;
                    break;
            }

            SetSleep(300);
        }

        public void SwitchTo(Attribute attribute, bool queue = false)
        {
            switch (attribute)
            {
                case Attribute.Strength:
                    if (ActiveAttribute == Attribute.Intelligence)
                    {
                        powerTreads.UseAbility(queue);
                        powerTreads.UseAbility(queue);
                    }
                    else if (ActiveAttribute == Attribute.Agility)
                    {
                        powerTreads.UseAbility(queue);
                    }
                    break;
                case Attribute.Intelligence:
                    if (ActiveAttribute == Attribute.Agility)
                    {
                        powerTreads.UseAbility(queue);
                        powerTreads.UseAbility(queue);
                    }
                    else if (ActiveAttribute == Attribute.Strength)
                    {
                        powerTreads.UseAbility(queue);
                    }
                    break;
                case Attribute.Agility:
                    if (ActiveAttribute == Attribute.Strength)
                    {
                        powerTreads.UseAbility(queue);
                        powerTreads.UseAbility(queue);
                    }
                    else if (ActiveAttribute == Attribute.Intelligence)
                    {
                        powerTreads.UseAbility(queue);
                    }
                    break;
            }

            SetSleep(300);
        }

        public override void Use(bool queue = false)
        {
        }
    }
}