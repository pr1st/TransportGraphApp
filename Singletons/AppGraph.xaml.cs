using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
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

        public GraphConfig GraphConfig { get; set; }

        public TransportSystem GetSelectedSystem => _transportSystem;

        public void SelectSystem(TransportSystem ts) {
            if (ts == null) {
                _transportSystem = null;
                _cities = null;
                _roads = null;
                GraphConfig = null;
                AppWindow.Instance.SystemSelected(false);
                DrawGraph();
            }
            else {
                _transportSystem = ts;
                AppWindow.Instance.SystemSelected(true);
                UpdateData();
                DrawGraph();
            }
        }

        public void UpdateSystem() {
            _transportSystem = AppDataBase.Instance.GetCollection<TransportSystem>().FindById(_transportSystem.Id);
            UpdateData();
            DrawGraph();
        }

        private void UpdateData() {
            _cities = AppDataBase.Instance
                .GetCollection<City>()
                .Find(c => c.TransportSystemId == _transportSystem.Id)
                .ToList();
            _roads = AppDataBase.Instance
                .GetCollection<Road>()
                .Find(r => r.TransportSystemId == _transportSystem.Id)
                .ToList();
            if (_cities.Count == 0) {
                GraphConfig = GraphConfig.GetDefault;
            }
            else {
                var minY = _cities.Min(c => c.Latitude);
                var maxY = _cities.Max(c => c.Latitude);
                var minX = _cities.Min(c => c.Longitude);
                var maxX = _cities.Max(c => c.Longitude);

                if (maxY - minY <= 0) {
                    minY = -180;
                    maxY = 180;
                }

                if (maxX - minX <= 0) {
                    minX = -180;
                    maxX = 180;
                }

                GraphConfig = new GraphConfig() {
                    BackGroundColor = Colors.Bisque,
                    MinX = minX,
                    MaxX = maxX,
                    MinY = minY,
                    MaxY = maxY,
                    CircleRadius = 7.5,
                    CircleColor = Colors.Yellow,
                    EdgeThickness = 2
                };   
            }
        }

        public void DrawGraph() {
            GraphPanel.Children.Clear();
            GraphPanel.Height = AppWindow.Instance.Height - 86;

            if (_transportSystem != null) {
                var cfg = GraphConfig;
                GraphPanel.Background = new SolidColorBrush(cfg.BackGroundColor);

                if (_cities.Count <= 0) return;

                
                foreach (var r in _roads) {
                    var from = _cities.First(c => r.FromCityId == c.Id);
                    var to = _cities.First(c => r.ToCityId == c.Id);
                    
                    var fromPoint = FromCityToPixelXy(from);
                    var toPoint = FromCityToPixelXy(to);
                    
                    var line = new Line() {
                        X1 = fromPoint.X,
                        Y1 = fromPoint.Y,
                        X2 = toPoint.X,
                        Y2 = toPoint.Y,
                        Stroke = Brushes.Black,
                        ToolTip = $"{from.Name} - {to.Name}",
                        StrokeThickness = cfg.EdgeThickness
                    };
                    GraphPanel.Children.Add(line);
                }
                
                foreach (var c in _cities) {
                    var ellipse = new Ellipse() {
                        Height = cfg.CircleRadius * 2,
                        Width = cfg.CircleRadius * 2,
                        Fill = new SolidColorBrush(cfg.CircleColor),
                        Stroke = Brushes.Black,
                        ToolTip = c.Name
                    };

                    var point = FromCityToPixelXy(c);

                    ellipse.Margin = new Thickness(point.X - cfg.CircleRadius, point.Y - cfg.CircleRadius, 0, 0);
                    GraphPanel.Children.Add(ellipse);
                }
            }
            else {
                GraphPanel.Background = Brushes.Bisque;
                var label = new Label() {
                    Content = "Никакая транспортная система еще не выбрана"
                };
                GraphPanel.Children.Add(label);
            }
        }

        private Point FromCityToPixelXy(City c) {
            var cfg = GraphConfig;
            
            var x = (c.Longitude - cfg.MinX) / (cfg.MaxX - cfg.MinX);
            var pixelX = x * (GraphPanel.Width - cfg.CircleRadius * 2) + cfg.CircleRadius;
            
            var y = (c.Latitude - cfg.MinY) / (cfg.MaxY - cfg.MinY);
            var pixelY = y * (GraphPanel.Height - cfg.CircleRadius * 2) + cfg.CircleRadius;
            pixelY = GraphPanel.Height - pixelY;
            
            return new Point(pixelX, pixelY);
        }

        public IList<City> GetCities() => _cities;
    }

    public class GraphConfig {
        public Color BackGroundColor { get; set; }

        public double MinX { get; set; }
        public double MaxX { get; set; }

        public double MinY { get; set; }
        public double MaxY { get; set; }

        public double CircleRadius { get; set; }
        public Color CircleColor { get; set; }

        public double EdgeThickness { get; set; }

        public static GraphConfig GetDefault => new GraphConfig() {
            BackGroundColor = Colors.Bisque,
            MinX = -180,
            MaxX = 180,
            MinY = -180,
            MaxY = 180,
            CircleRadius = 7.5,
            CircleColor = Colors.Yellow,
            EdgeThickness = 2
        };
    }
}