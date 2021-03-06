﻿using System.Windows;

namespace Domain0.Desktop.Views
{
    /// <summary>
    /// BindingProxy for XAML data binding through resources
    /// https://stackoverflow.com/questions/22073740/binding-visibility-for-datagridcolumn-in-wpf/22074985#22074985
    /// </summary>
    public class BindingProxy : Freezable
    {
        #region Overrides of Freezable

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        #endregion

        public object Data
        {
            get => (object) GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object),
                typeof(BindingProxy));
    }
}