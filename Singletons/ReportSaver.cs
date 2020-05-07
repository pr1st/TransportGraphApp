using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using TransportGraphApp.Models;

namespace TransportGraphApp.Singletons {
    public class ReportSaver {
        // todo

        private string EnterFileName(string templateName, string filter) {
            var dialog = new SaveFileDialog() {
                InitialDirectory = Directory.GetCurrentDirectory(),
                Filter = filter,
                FileName = templateName
            };
            return dialog.ShowDialog() != true ? null : dialog.FileName;
        }
        
        public void MinimalReport(AlgorithmResult res) {
            // todo
            ComponentUtils.ShowMessage("Еще не реализованно", MessageBoxImage.Error);
        }

        public void JsonReport(AlgorithmResult res) {
            // todo
            ComponentUtils.ShowMessage("Еще не реализованно", MessageBoxImage.Error);
        }

        public void FullReport(AlgorithmResult res) {
            // todo
            ComponentUtils.ShowMessage("Еще не реализованно", MessageBoxImage.Error);
        }

        public void MatrixReport(AlgorithmResult res) {
            var fileName = EnterFileName("matrix-report", "Csv file (*.csv)|*.csv");
            if (fileName == null) return;
            File.WriteAllText(fileName, ReportUtils.CreateMatrix(res));
            // todo check results
            ComponentUtils.ShowMessage("Файл успешно создан", MessageBoxImage.Information);
        }
    }

    internal static class ReportUtils {
        public static string CreateMatrix(AlgorithmResult res) {
            var sb = new StringBuilder("Название");
            sb.Append(",Индекс");
            for (var i = 0; i < res.Nodes.Count; i++) {
                sb.Append($",{i}");
            }
            sb.AppendLine();
            
            for (var i = 0; i < res.Nodes.Count; i++) {
                var node = res.Nodes[i];
                sb.Append($"\"{res.GetCityName(node)}\"");
                sb.Append($",{i}");
                for (var j = 0; j < res.Nodes.Count; j++) {
                    var toNode = res.Nodes[j];
                    if (i == j) {
                        sb.Append(",-1");
                        continue;
                    }

                    var found = node.Weights.FirstOrDefault(w => w.From == toNode);
                    if (found == null) {
                        sb.Append(",-1");
                        continue;
                    }
                    // todo make nicer format
                    sb.Append($",{found.Weight.Value}");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}