﻿namespace ItemManager.Core
{
    using System;
    using System.Collections.Generic;

    using Menus;

    using Modules.AbilityHelper;
    using Modules.AutoUsage;
    using Modules.CourierHelper;
    using Modules.GoldSpender;
    using Modules.ItemSwapper;
    using Modules.RecoveryAbuse;
    using Modules.ShrineHelper;
    using Modules.Snatcher;

    internal class Bootstrap
    {
        private readonly List<IDisposable> disposables = new List<IDisposable>();

        public void OnClose()
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }

            disposables.Clear();
        }

        public void OnLoad()
        {
            var manager = new Manager();
            var menu = new MenuManager();

            disposables.Add(manager);
            disposables.Add(menu);

            disposables.Add(new ItemSwapper(manager, menu.ItemSwapMenu));
            disposables.Add(new CourierHelper(manager, menu.CourierHelperMenu));
            disposables.Add(new Snatcher(manager, menu.SnatcherMenu));
            disposables.Add(new GoldSpender(manager, menu.GoldSpenderMenu));
            disposables.Add(new ShrineHelper(manager, menu.ShrineHelperMenu));
            disposables.Add(new RecoveryAbuse(manager, menu.RecoveryMenu, menu.AbilityHelperMenu.Tranquil));
            disposables.Add(new AutoSoulRing(manager, menu.AutoUsageMenu.SoulRing));
            disposables.Add(new PowerTreadsSwitcher(manager, menu.AutoUsageMenu.PowerTreads, menu.RecoveryMenu));
            disposables.Add(new AutoBottle(manager, menu.AutoUsageMenu.Bottle, menu.RecoveryMenu));
            disposables.Add(new TranquilDrop(manager, menu.AbilityHelperMenu.Tranquil));
            disposables.Add(new BlinkAdjustment(manager, menu.AbilityHelperMenu.Blink));
            disposables.Add(new AutoArcaneBoots(manager, menu.AutoUsageMenu.ArcaneBoots));
            disposables.Add(new AutoDewarding(manager, menu.AutoUsageMenu.Deward));
        }
    }
}