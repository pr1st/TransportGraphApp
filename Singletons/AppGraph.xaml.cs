using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using LiteDB;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;

namespace TransportGraphApp.Singletons {
    public partial class AppGraph : UserControl {
        private static AppGraph _instance;
        public static AppGraph Instance => _instance ??= new AppGraph();


        private AppGraph() {
            InitializeComponent();
        }

        private TransportSystem _transportSystem;
        private IList<City> _cities;
        private IList<Road> _roads;

        public TransportSystem GetSelectedSystem => _transportSystem;

        public void SelectSystem(TransportSystem ts) {
            if (ts == null) {
                _transportSystem = null;
                _cities = null;
                _roads = null;
                AppWindow.Instance.SystemSelected(false);
            }
            else {
                _transportSystem = ts;
                _cities = AppDataBase.Instance
                    .GetCollection<City>()
                    .Find(c => c.TransportSystemId == ts.Id)
                    .ToList();
                _roads = AppDataBase.Instance
                    .GetCollection<Road>()
                    .Find(r => r.TransportSystemId == ts.Id)
                    .ToList();
                AppWindow.Instance.SystemSelected(true);
            }

            DrawGraph();
        }

        public void UpdateSystem() {
            var db = AppDataBase.Instance;
            _transportSystem = db.GetCollection<TransportSystem>().FindById(_transportSystem.Id);
            _cities = AppDataBase.Instance
                .GetCollection<City>()
                .Find(c => c.TransportSystemId == _transportSystem.Id)
                .ToList();
            _roads = AppDataBase.Instance
                .GetCollection<Road>()
                .Find(r => r.TransportSystemId == _transportSystem.Id)
                .ToList();
            DrawGraph();
        }

        private void DrawGraph() {
            GraphPanel.Children.Clear();
            if (_transportSystem != null) {
                var prev = new Label() {
                    Content = "Скоро здесь будет рисоваться граф, а пока"
                };
                var nameLabel = new Label() {
                    Content = $"Название: {_transportSystem.Name}"
                };
                var cityLabel = new Label() {
                    Content = $"В сети присутсвует {_cities.Count} различных населенных пунктов"
                };
                var roadLabel = new Label() {
                    Content = $"В сети присутсвует {_roads.Count} различных маршрутов"
                };
            
                GraphPanel.Children.Add(prev);
                GraphPanel.Children.Add(nameLabel);
                GraphPanel.Children.Add(cityLabel);
                GraphPanel.Children.Add(roadLabel);
            }
            else {
                var label = new Label() {
                    Content = "Никакая транспортная система еще не выбрана"
                };
                GraphPanel.Children.Add(label);
            }
        }
    }
}