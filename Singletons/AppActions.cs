using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TransportGraphApp.Actions;

namespace TransportGraphApp.Singletons {
    internal class AppActions {
        private static AppActions _instance;

        public static AppActions Instance => _instance ??= new AppActions();

        private Dictionary<IAppAction, IList<UIElement>> _mappedElementsToActions { get; }
            = new Dictionary<IAppAction, IList<UIElement>>();

        private AppActions() {
            var actions = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => typeof(IAppAction).IsAssignableFrom(t) && !t.IsInterface)
                .Select(t => Activator.CreateInstance(t, true))
                .Cast<IAppAction>();
            foreach (var appAction in actions) {
                _mappedElementsToActions.Add(appAction, new List<UIElement>());
            }
        }

        public void AddElementToAction<T>(Button element) where T : IAppAction {
            var (action, elements) = _mappedElementsToActions
                .First(kv => kv.Key.GetType().IsAssignableFrom(typeof(T)));
            elements.Add(element);
            element.Click += (sender, args) => action.Invoke();
        }

        public void AddElementToAction<T>(MenuItem element) where T : IAppAction {
            var (action, elements) = _mappedElementsToActions
                .First(kv => kv.Key.GetType().IsAssignableFrom(typeof(T)));
            elements.Add(element);
            element.Click += (sender, args) => action.Invoke();
        }


        public T GetAction<T>() where T : IAppAction {
            return (T) _mappedElementsToActions
                .First(kv => kv.Key.GetType().IsAssignableFrom(typeof(T)))
                .Key;
        }

        public void AppStateChanged() {
            foreach (var (action, uiElements) in _mappedElementsToActions) {
                foreach (var uiElement in uiElements) {
                    uiElement.IsEnabled = action.IsAvailable();
                }
            }
        }
    }
}