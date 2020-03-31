using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
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
            var stringRowControl = new StringRowControl() {
                TitleValue = "Str",
                Value = "123"
            };
            var stringRowControl2 = new StringRowControl() {
                TitleValue = "Str2",
            };
            var latitudeLongitudeRowControl = new LatitudeLongitudeRowControl() {
                Latitude = 123.3,
                Longitude = -23.2
            };
            var latitudeLongitudeRowControl2 = new LatitudeLongitudeRowControl();
            var stringWithHelpRowControl1 = new StringWithHelpRowControl() {
                TitleValue = "HALP1",
                HelpingValues = new List<string>() {
                    "Test1",
                    "Test2",
                    "test3",
                    "ne test"
                }
            };
            var stringTableRowControl = new StringTableRowControl() {
                TitleValue = "tablet",
                Value = new List<string>() {
                    "A1",
                    "Crunchy"
                },
                IsViable = s => s != ""
            };
            var positiveDoubleRowControl = new PositiveDoubleRowControl() {
                TitleValue = "Pos",
                Value = 123123.33
            };
            var positiveDoubleRowControl2 = new PositiveDoubleRowControl() {
                TitleValue = "Pos"
            };
            var stringTableRowControl2 = new StringTableRowControl() {
                TitleValue = "tablet2",
                IsViable = s => s != ""
            };
            var trueFalseRowControl = new TrueFalseRowControl() {
                TitleValue = "True",
                Value = true
            };
            var trueFalseRowControl2 = new TrueFalseRowControl() {
                TitleValue = "True(NO)"
            };
            var stringWithHelpRowControl2 = new StringWithHelpRowControl() {
                TitleValue = "HALP",
                HelpingValues = new List<string>() {
                    "Test1",
                    "Test2",
                    "test3",
                    "ne test"
                }
            };
            var button = new Button();
            ComponentUtils.InsertIconToButton(button, AppResources.GetAddItemIcon, "Adding");
            GraphPanel.Children.Add(stringRowControl);
            GraphPanel.Children.Add(stringRowControl2);
            GraphPanel.Children.Add(latitudeLongitudeRowControl);
            GraphPanel.Children.Add(latitudeLongitudeRowControl2);
            GraphPanel.Children.Add(stringWithHelpRowControl1);
            GraphPanel.Children.Add(stringTableRowControl);
            GraphPanel.Children.Add(positiveDoubleRowControl);
            
            GraphPanel.Children.Add(positiveDoubleRowControl2);
            GraphPanel.Children.Add(stringTableRowControl2);
            GraphPanel.Children.Add(trueFalseRowControl);
            GraphPanel.Children.Add(trueFalseRowControl2);
            
            GraphPanel.Children.Add(stringWithHelpRowControl2);
            GraphPanel.Children.Add(button);
            button.Click += (sender, args) => {
                Console.WriteLine("Pressed");
            };
            // GraphPanel.Children.Clear();
            // if (_transportSystem != null) {
            //     var prev = new Label() {
            //         Content = "Скоро здесь будет рисоваться граф, а пока"
            //     };
            //     var nameLabel = new Label() {
            //         Content = $"Название: {_transportSystem.Name}"
            //     };
            //     var cityLabel = new Label() {
            //         Content = $"В сети присутсвует {_cities.Count} различных населенных пунктов"
            //     };
            //     var roadLabel = new Label() {
            //         Content = $"В сети присутсвует {_roads.Count} различных маршрутов"
            //     };
            //
            //     GraphPanel.Children.Add(prev);
            //     GraphPanel.Children.Add(nameLabel);
            //     GraphPanel.Children.Add(cityLabel);
            //     GraphPanel.Children.Add(roadLabel);
            // }
            // else {
            //     var label = new Label() {
            //         Content = "Никакая транспортная система еще не выбрана"
            //     };
            //     GraphPanel.Children.Add(label);
            // }
        }
    }
}