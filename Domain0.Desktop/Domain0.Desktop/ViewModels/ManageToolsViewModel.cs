﻿using Domain0.Api.Client;
using Domain0.Desktop.Extensions;
using Domain0.Desktop.Properties;
using Domain0.Desktop.Services;
using MahApps.Metro;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Ui.Wpf.Common;
using Ui.Wpf.Common.ViewModels;
using Application = System.Windows.Application;

namespace Domain0.Desktop.ViewModels
{
    public class ManageToolsViewModel : ViewModelBase
    {
        private readonly IDomain0Service _domain0;
        private readonly IAuthenticationContext _authContext;

        public ManageToolsViewModel(
            IShell shell,
            IDomain0Service domain0,
            IAuthenticationContext authContext)
        {
            _domain0 = domain0;
            _authContext = authContext;

            LogoutCommand = ReactiveCommand
                .Create(Logout)
                .DisposeWith(Disposables);
            ReloadCommand = ReactiveCommand
                .CreateFromTask(Reload)
                .DisposeWith(Disposables);
            CopyTokenToClipboardCommand = ReactiveCommand
                .Create(CopyTokenToClipboard)
                .DisposeWith(Disposables);

            OpenUsersCommand = ReactiveCommand
                .Create(shell.ShowUsers)
                .DisposeWith(Disposables);
            OpenRolesCommand = ReactiveCommand
                .Create(shell.ShowRoles)
                .DisposeWith(Disposables);
            OpenPermissionsCommand = ReactiveCommand
                .Create(shell.ShowPermissions)
                .DisposeWith(Disposables);
            OpenApplicationsCommand = ReactiveCommand
                .Create(shell.ShowApplications)
                .DisposeWith(Disposables);
            OpenEnvironmentsCommand = ReactiveCommand
                .Create(shell.ShowEnvironments)
                .DisposeWith(Disposables);
            OpenMessagesCommand = ReactiveCommand
                .Create(shell.ShowMessages)
                .DisposeWith(Disposables);

            AccentColors = ThemeManager.Accents
                .Select(a => new AccentColorData { Name = a.Name, ColorBrush = a.Resources["AccentBaseColorBrush"] as Brush })
                .ToList();
            AppThemes = ThemeManager.AppThemes
                .GroupBy(x => x.Resources)
                .Select(x => x.First())
                .Select(a => new AppThemeData { Name = a.Name, BorderColorBrush = a.Resources["BlackColorBrush"] as Brush, ColorBrush = a.Resources["WhiteColorBrush"] as Brush })
                .ToList();
        }

        public List<AccentColorData> AccentColors { get; set; }
        public List<AppThemeData> AppThemes { get; set; }

        public ReactiveCommand<Unit, Unit> LogoutCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ReloadCommand { get; set; }
        public ReactiveCommand<Unit, Unit> CopyTokenToClipboardCommand { get; set; }

        public ReactiveCommand<Unit, Unit> OpenUsersCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenRolesCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenPermissionsCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenApplicationsCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenEnvironmentsCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenMessagesCommand { get; set; }


        private void Logout()
        {
            _domain0.Reconnect();
        }

        private Task Reload()
        {
            return _domain0.LoadModel();
        }

        private void CopyTokenToClipboard()
        {
            Clipboard.SetText($"Bearer {_authContext.Token}");
        }
    }


    public class AccentColorData
    {
        public string Name { get; set; }
        public Brush BorderColorBrush { get; set; }
        public Brush ColorBrush { get; set; }

        private readonly Lazy<ReactiveCommand<Unit, Unit>> _changeAccentCommand;
        public ReactiveCommand<Unit, Unit> ChangeAccentCommand => _changeAccentCommand.Value;

        public AccentColorData()
        {
            _changeAccentCommand = new Lazy<ReactiveCommand<Unit, Unit>>(() => ReactiveCommand.Create(DoChangeTheme));
        }

        protected virtual void DoChangeTheme()
        {
            Settings.Default.AccentColor = Name;
            Settings.Default.Save();

            ThemeManager.ChangeAppStyle(Application.Current,
                ThemeManager.GetAccent(Settings.Default.AccentColor),
                ThemeManager.GetAppTheme(Settings.Default.AppTheme));
        }
    }

    public class AppThemeData : AccentColorData
    {
        protected override void DoChangeTheme()
        {
            Settings.Default.AppTheme = Name;
            Settings.Default.Save();

            ThemeManager.ChangeAppStyle(Application.Current,
                ThemeManager.GetAccent(Settings.Default.AccentColor),
                ThemeManager.GetAppTheme(Settings.Default.AppTheme));
        }
    }

}
