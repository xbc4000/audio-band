﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using AudioBand.AudioSource;
using AudioBand.Models;
using AudioBand.ViewModels;
using MahApps.Metro.Controls.Dialogs;

namespace AudioBand.Views.Wpf
{
    internal partial class SettingsWindow
    {
        internal event EventHandler Saved;

        public IEnumerable<TextAlignment> TextAlignValues { get; } = Enum.GetValues(typeof(TextAlignment)).Cast<TextAlignment>();
        public ObservableCollection<CustomLabelVM> TextAppearancesCollection { get; set; }
        public AudioSourceSettingsCollectionVM AudioSourceSettingsViewModel { get; }

        private List<CustomLabelVM> _deletedTextAppearances;
        private List<CustomLabelVM> _addedTextAppearances;
        private bool _cancelEdit = true;

        internal SettingsWindow()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            InitializeComponent();
        }

        // Problem loading xceed.wpf.toolkit assembly normally
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (!args.Name.StartsWith("Xceed.Wpf.Toolkit"))
            {
                return null;
            }

            var dir = DirectoryHelper.BaseDirectory;
            // name is in this format Xceed.Wpf.Toolkit, Version=3.4.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
            var asmName = args.Name.Substring(0, args.Name.IndexOf(',')) + ".dll";
            var filename = Path.Combine(dir, asmName);

            return !File.Exists(filename) ? null : Assembly.LoadFrom(filename);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            // if closed by x button
            if (_cancelEdit)
            {

            }
            else
            {
                Saved?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            _cancelEdit = false;
            Close();
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            _cancelEdit = true;
            Close();
        }

        private async void ResetSettingOnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var res = await ShowConfirmationDialog("Reset Setting", "Are you sure you want to reset this setting to the default values?");
            if (res != MessageDialogResult.Affirmative)
            {
                return;
            }

            var resettableObj = e.Parameter as IResettableObject;
            resettableObj.Reset();
        }

        private async Task<MessageDialogResult> ShowConfirmationDialog(string title, string message)
        {
            var dialogSettings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No",
                AnimateHide = false,
                AnimateShow = false
            };

            return await this.ShowMessageAsync(title, message, MessageDialogStyle.AffirmativeAndNegative, dialogSettings);
        }
    }
}
