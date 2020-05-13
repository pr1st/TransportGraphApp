using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using Microsoft.Win32;
using TransportGraphApp.Graph;
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
            var fileName = EnterFileName("minimal-report", "Txt file (*.txt)|*.txt");
            if (fileName == null) return;
            File.WriteAllText(fileName, ReportUtils.MinimalReport(res));
            ComponentUtils.ShowMessage("Результат в файл успешно записан", MessageBoxImage.Information);
        }

        public void JsonReport(AlgorithmResult res) {
            var fileName = EnterFileName("json-report", "Json file (*.json)|*.json");
            if (fileName == null) return;
            File.WriteAllText(fileName, ReportUtils.JsonReport(res));
            ComponentUtils.ShowMessage("Результат в файл успешно записан", MessageBoxImage.Information);
        }

        public void FullReport(AlgorithmResult res) {
            var fileName = EnterFileName("full-report", "Txt file (*.txt)|*.txt");
            if (fileName == null) return;
            File.WriteAllText(fileName, ReportUtils.FullReport(res));
            ComponentUtils.ShowMessage("Результат в файл успешно записан", MessageBoxImage.Information);
        }

        public void MatrixReport(AlgorithmResult res) {
            var fileName = EnterFileName("matrix-report", "Csv file (*.csv)|*.csv");
            if (fileName == null) return;
            File.WriteAllText(fileName, ReportUtils.MatrixReport(res));
            ComponentUtils.ShowMessage("Результат в файл успешно записан", MessageBoxImage.Information);
        }
    }

    internal static class ReportUtils {
        public static string MatrixReport(AlgorithmResult res) {
            var sb = new StringBuilder("Название");
            sb.Append(",Индекс");
            for (var i = 0; i < res.Nodes.Count; i++) {
                sb.Append($",{i}");
            }

            sb.AppendLine();

            for (var i = 0; i < res.Nodes.Count; i++) {
                var fromNode = res.Nodes[i];
                sb.Append($"\"{fromNode.Name}\"");
                sb.Append($",{i}");
                for (var j = 0; j < res.Nodes.Count; j++) {
                    var toNode = res.Nodes[j];
                    if (i == j) {
                        sb.Append(",-1");
                        continue;
                    }

                    var found = fromNode.Weights.FirstOrDefault(w => toNode.Equals(w.From));
                    if (found == null) {
                        sb.Append(",-1");
                        continue;
                    }

                    sb.Append($",{found.Weight.Value:F}");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static string MinimalReport(AlgorithmResult res) {
            var sb = new StringBuilder("Минимальный отчет результата работы алгоритма");
            sb.AppendLine();
            sb.AppendLine($"Дата выполнения: {res.RunDate}");
            sb.AppendLine($"Использованный алгоритм: {res.AlgorithmConfig.AlgorithmType.GetDescription()}");
            sb.AppendLine("В каждой строчке названию города сопоставляется значение транспортной доступности");
            foreach (var node in res.Nodes) {
                sb.Append(node.Name)
                    .Append(" ")
                    .Append(WeightValueWithExtension(node, res.AlgorithmConfig.AlgorithmType))
                    .AppendLine();
            }

            return sb.ToString();
        }

        private static string WeightValueWithExtension(Node node, AlgorithmType algorithmType) {
            var weight = node.MinWeight().Weight.Value;
            switch (algorithmType) {
                case AlgorithmType.Time when node.IsCentral:
                    return "0 д. 0 ч. 0 м.";
                case AlgorithmType.Time: {
                    var time = (int) weight;
                    var d = time / (60 * 24);
                    time -= d * 60 * 24;
                    var h = time / 60;
                    time -= h * 60;
                    var m = time;
                    return $"{d} д. {h} ч. {m} м.";
                }
                case AlgorithmType.Length:
                case AlgorithmType.ComplexCost:
                case AlgorithmType.Cost:
                    return $"{weight:F2} у.е.";
                default: throw new NotImplementedException();
            }
        }

        public static string FullReport(AlgorithmResult res) {
            var sb = new StringBuilder("Полный отчет результата работы алгоритма");
            sb.AppendLine();
            sb.AppendLine($"Дата выполнения: {res.RunDate}");
            sb.AppendLine($"Использованный алгоритм: {res.AlgorithmConfig.AlgorithmType.GetDescription()}");
            sb.AppendLine($"Использованный метод: {res.AlgorithmConfig.MethodType.GetDescription()}");

            sb.AppendLine("Использованные траснпортные системы:");
            foreach (var transportSystem in res.AlgorithmConfig.TransportSystems) {
                sb.AppendLine($"\t{transportSystem.Name}");
            }
            sb.AppendLine("Использованные тэги для определения центральных населенных пунктов:");
            foreach (var cityTag in res.AlgorithmConfig.CityTags) {
                sb.AppendLine($"\t{cityTag.Name}");
            }
            sb.AppendLine("Исключенные типы дорог из работы алгоритма:");
            foreach (var roadType in res.AlgorithmConfig.RoadTypes) {
                sb.AppendLine($"\t{roadType.Name}");
            }

            sb.AppendLine("Далее будут перечисленны города и значения их транспортной доступности");
            foreach (var node in res.Nodes) {
                sb.AppendLine("{");
                sb.AppendLine($"\tНазвание: {node.Name}");
                var isCenter = node.IsCentral ? "да" : "нет";
                sb.AppendLine($"\tЯвляется центральным: {isCenter}");
                if (!node.IsCentral) {
                    sb.AppendLine($"\tЗначение доступности: {WeightValueWithExtension(node, res.AlgorithmConfig.AlgorithmType)}");
                    
                    var minWeight = node.MinWeight();
                    var a = minWeight;
                    sb.Append($"\t{node.Name}");
                    while (!a.From.IsCentral) {
                        GraphWeight next;
                        if (res.AlgorithmConfig.AlgorithmType == AlgorithmType.Cost ||
                            res.AlgorithmConfig.AlgorithmType == AlgorithmType.Length) {
                            next = a.From.Weights[0];
                        }
                        else {
                            next = a.From.Weights.First(w => a.FromTime.Value == w.Time.Value);
                        }
                        sb.Append($" ->(+{(a.Weight - next.Weight).Value:F2}) {a.From.Name}");
                        a = next;
                    }
                    sb.AppendLine($" ->(+{(a.Weight).Value}) {a.From.Name}");
                }

                sb.AppendLine("}");
            }
            
            return sb.ToString();
        }

        public static string JsonReport(AlgorithmResult res) {
            var options = new JsonSerializerOptions {WriteIndented = true, IgnoreNullValues = true};
            options.Converters.Add(new AlgorithmTypeOffsetConverter());
            options.Converters.Add(new MethodTypeOffsetConverter());
            var jsonString = JsonSerializer.Serialize(res, options);
            return jsonString;
        }
    }

    internal class AlgorithmTypeOffsetConverter : JsonConverter<AlgorithmType> {
        public override AlgorithmType Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) {
            var s = reader.GetString();
            foreach (var algType in Enum.GetValues(typeof(AlgorithmType))) {
                if (((AlgorithmType) algType).GetDescription() == s) {
                    return (AlgorithmType) algType;
                }
            }

            throw new Exception();
        }

        public override void Write(
            Utf8JsonWriter writer,
            AlgorithmType value,
            JsonSerializerOptions options) =>
            writer.WriteStringValue(value.GetDescription());
    }
    
    internal class MethodTypeOffsetConverter : JsonConverter<MethodType> {
        public override MethodType Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) {
            var s = reader.GetString();
            foreach (var algType in Enum.GetValues(typeof(MethodType))) {
                if (((MethodType) algType).GetDescription() == s) {
                    return (MethodType) algType;
                }
            }

            throw new Exception();
        }

        public override void Write(
            Utf8JsonWriter writer,
            MethodType value,
            JsonSerializerOptions options) =>
            writer.WriteStringValue(value.GetDescription());
    }
}