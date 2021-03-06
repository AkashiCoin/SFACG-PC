﻿using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SFACGPC.Objects.Primitive {
    public static class UiHelper {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy,
                                               uint uFlags);

        public static void Disable(this FrameworkElement element) {
            element.IsEnabled = false;
        }

        public static void Enable(this FrameworkElement element) {
            element.IsEnabled = true;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject {
            if (depObj != null)
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++) {
                    var child = VisualTreeHelper.GetChild(depObj, i);
                    if (child is T dependencyObject) yield return dependencyObject;

                    foreach (var childOfChild in FindVisualChildren<T>(child)) yield return childOfChild;
                }
        }

        public static T DataContext<T>(this FrameworkElement element) {
            return (T)element.DataContext;
        }

        public static void SetImageSource(object img, ImageSource imgSource) {
            Application.Current.Dispatcher?.Invoke(() => ((Image)img).Source = imgSource);
        }

        public static void ReleaseImage(object img) {
            Application.Current.Dispatcher?.Invoke(() => ((Image)img).Source = null);
        }

        public static ObservableCollection<T> NewItemsSource<T>(ItemsControl itemsControl) {
            var collection = new ObservableCollection<T>();
            SetItemsSource(itemsControl, collection);
            return collection;
        }

        public static void SetItemsSource(ItemsControl itemsControl, IEnumerable itemSource) {
            itemsControl.ItemsSource = itemSource;
        }

        public static void ReleaseItemsSource(ItemsControl listView) {
            listView.ItemsSource = null;
        }

        public static void StartDoubleAnimationUseCubicEase(object sender, string path, double from, double to,
                                                            int milliseconds) {
            var sb = new Storyboard();
            var doubleAnimation = new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(milliseconds)) {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(doubleAnimation, (DependencyObject)sender);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(path));
            sb.Children.Add(doubleAnimation);
            sb.Begin();
        }

        public static T GetDataContext<T>(this object sender) {
            if (sender is FrameworkElement element) return element.DataContext<T>();

            throw new NotSupportedException($"parameter must be derive class of {nameof(FrameworkElement)}");
        }

        public static T GetResources<T>(this FrameworkElement element, string name) {
            return (T)element.Resources[name];
        }

        public static void CloseControls(params UIElement[] controls) {
            foreach (var control in controls) CloseControl(control);
        }

        public static void CloseControl(this UIElement control) {
            try {
                ((dynamic)control).IsOpen = false;
            } catch (RuntimeBinderException) {
                // ignore
            }
        }

        public static void OpenControl(this UIElement control) {
            try {
                ((dynamic)control).IsOpen = true;
            } catch (RuntimeBinderException) {
                // ignore
            }
        }

        public static void Scroll(ScrollViewer sender, MouseWheelEventArgs e) {
            if (e.Delta > 0) {
                sender.LineUp();
                sender.LineUp();
            } else {
                sender.LineDown();
                sender.LineDown();
            }
        }
    }

    public class PopupHelper {
        public static readonly DependencyProperty PopupPlacementTargetProperty =
            DependencyProperty.RegisterAttached("PopupPlacementTarget", typeof(DependencyObject), typeof(PopupHelper),
                                                new PropertyMetadata(null, OnPopupPlacementTargetChanged));

        public static DependencyObject GetPopupPlacementTarget(DependencyObject obj) {
            return (DependencyObject)obj.GetValue(PopupPlacementTargetProperty);
        }

        public static void SetPopupPlacementTarget(DependencyObject obj, DependencyObject value) {
            obj.SetValue(PopupPlacementTargetProperty, value);
        }

        private static void OnPopupPlacementTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue != null) {
                var popupPopupPlacementTarget = e.NewValue as DependencyObject;
                var pop = d as Popup;

                var w = Window.GetWindow(popupPopupPlacementTarget ?? throw new InvalidOperationException());
                if (null != w) {
                    w.LocationChanged += (sender, args) => {
                        if (pop != null) {
                            var offset = pop.HorizontalOffset;
                            pop.HorizontalOffset = offset + 1;
                            pop.HorizontalOffset = offset;
                        }
                    };

                    w.SizeChanged += (sender, args) => {
                        var mi = typeof(Popup).GetMethod("UpdatePosition",
                                                         BindingFlags.NonPublic | BindingFlags.Instance);
                        try {
                            mi?.Invoke(pop, null);
                        } catch {
                            // ignored
                        }
                    };
                }
            }
        }
    }

    public abstract class BindableBase : INotifyPropertyChanged {
        /// <summary>
        /// Multicast event for property change notifications.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Checks if a property already matches a desired value.  Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null) {
            if (Equals(storage, value)) return false;

            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }
    }

    internal class DelegateCommand : ICommand {
        private readonly Action _execute;

        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of the RelayCommand class that 
        /// can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
        public DelegateCommand(Action execute)
            : this(execute, null) {
        }

        /// <summary>
        /// Initializes a new instance of the RelayCommand class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
        public DelegateCommand(Action execute, Func<bool> canExecute) {
            if (execute == null) {
                throw new ArgumentNullException(nameof(execute));
            }

            this._execute = execute;
            this._canExecute = canExecute;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "This cannot be an event")]
        public void RaiseCanExecuteChanged() {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        [DebuggerStepThrough]
        public bool CanExecute(object parameter) {
            return this._canExecute == null || this._canExecute();
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked. 
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored.</param>
        public void Execute(object parameter) {
            if (this.CanExecute(parameter)) {
                this._execute();
            }
        }
    }
}
