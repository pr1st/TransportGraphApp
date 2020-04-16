using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;

namespace TransportGraphApp.Dialogs {
    public partial class EntityDialog : Window {
        public EntityDialog() {
            InitializeComponent();
            Owner = App.Window;
            Icon = AppResources.GetAppIcon;
        }
    }

    public class GenericEntityDialog<T> where T : IAppModel {
        private readonly EntityDialog _dialog = new EntityDialog();
        private GenericEntityListControl<T> _entityList;
    
        private readonly IList<UIElement> _propertyRows = new List<UIElement>();
        private readonly IList<ThreadStart> _onAddWindowFunctions = new List<ThreadStart>();
        private readonly IList<Action<T>> _onUpdateWindowFunctions = new List<Action<T>>();
        private readonly Dictionary<string, Func<T, object>> _columnMatcher = new Dictionary<string, Func<T, object>>();
        
        private Button _addButton = new Button() {
            Content = "Добавить",
            Margin = new Thickness(5,5,5,5)
        };
        
        private Button _updateButton = new Button() {
            Content = "Обновить"
        };
        private Button _removeButton = new Button() {
            Content = "Удалить",
            Margin = new Thickness(0, 0, 5, 0),
        };
    
        public string Title { get; set; }
        public string ListTitle { get; set; }
        public string OpenAddNewItemWindowButtonTitle { get; set; }
        public string AddNewItemWindowTitle { get; set; }
        public string UpdateItemWindowTitle { get; set; }
        
        public Func<bool> AddItemFunction { get; set; }
        public Func<T, bool> UpdateItemFunction { get; set; }
        public Func<T, bool> RemoveItemFunction { get; set; }
        
        public Func<IEnumerable<T>> UpdateCollectionFunction { get; set; }
    
    
        public void AddProperty<TU>(TU propertyRow, ThreadStart onAddWindow, Action<T> onUpdateWindow) where TU : UIElement {
            _propertyRows.Add(propertyRow);
            _onAddWindowFunctions.Add(onAddWindow);
            _onUpdateWindowFunctions.Add(onUpdateWindow);
        }
    
        public void AddColumn(string name, Func<T, object> columnSupplier) {
            _columnMatcher[name] = columnSupplier;
        }
    
        public void ShowDialog() {
            _dialog.Title = Title;
            ComponentUtils.InsertIconToPanel(_dialog.InfoPanel, AppResources.GetInfoIcon, "Для большей информации об атрибуте, наведите на название этого атрибута");
            
            _entityList = new GenericEntityListControl<T>(ListTitle, _columnMatcher, DisplayUpdate);
            _dialog.ListPanel.Children.Add(_entityList.GetUiElement());

            foreach (var p in _propertyRows) {
                _dialog.PropertiesPanel.Children.Add(p);
            }
            
            ComponentUtils.InsertIconToButton(_dialog.AddButton, AppResources.GetAddItemIcon, OpenAddNewItemWindowButtonTitle);
            _dialog.AddButton.Click += (sender, args) => DisplayNew();
    
            _addButton.Click += (sender, args) => {
                if (!AddItemFunction.Invoke()) return;
                _entityList.SetSource(UpdateCollectionFunction.Invoke());
                DisplayNew();
            };
            _updateButton.Click += (sender, args) => {
                var selected = _entityList.Selected;
                if (!UpdateItemFunction.Invoke(selected)) return;

                var updatedList = UpdateCollectionFunction.Invoke().ToList();
                _entityList.SetSource(updatedList);
                
                _entityList.Selected = updatedList.First(t => t.Id == selected.Id);
            };
            _removeButton.Click += (sender, args) => {
                if (!RemoveItemFunction.Invoke(_entityList.Selected)) return;
                _entityList.SetSource(UpdateCollectionFunction.Invoke());
                DisplayNew();
            };

            _entityList.SetSource(UpdateCollectionFunction.Invoke());
            
            _dialog.ShowDialog();
        }
    
        private void DisplayNew() {
            _dialog.LabelPanel.Content = AddNewItemWindowTitle;
            foreach (var f in _onAddWindowFunctions) {
                f.Invoke();
            }

            _entityList.Selected = default;
            
            _dialog.ButtonsPanel.Children.Clear();
            _dialog.ButtonsPanel.Children.Add(_addButton);
            _dialog.VisibilityPanel.Visibility = Visibility.Visible;
        }
    
        private void DisplayUpdate(T entity) {
            _dialog.LabelPanel.Content = UpdateItemWindowTitle;
            foreach (var f in _onUpdateWindowFunctions) {
                f.Invoke(entity);
            }
            
            _dialog.ButtonsPanel.Children.Clear();
            _dialog.ButtonsPanel.Children.Add(_removeButton);
            _dialog.ButtonsPanel.Children.Add(_updateButton);
            _dialog.VisibilityPanel.Visibility = Visibility.Visible;
        }
    }
}