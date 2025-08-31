using System.Collections.Generic;
using System.Text.Json;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace progress
{
    public partial class MainWindow : Window
    {
        private string modelsFilePath = "models.json";
        private List<string> models = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            LoadModels();
            UpdateModelComboBox();
        }

        private void TrainModelButton_Click(object sender, RoutedEventArgs e)
        {
            TrainModelWindow trainModelWindow = new TrainModelWindow(this);
            trainModelWindow.ShowDialog();
        }

        private void PredictionButton_Click(object sender, RoutedEventArgs e)
        {
            var predictionWindow = new PredictionWindow(this);
            predictionWindow.ShowDialog();
        }

        public void AppendOutput(string message)
        {
            OutputTextBox.AppendText(message + "\n");
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            OutputTextBox.Clear();
        }

        private void AddModelButton_Click(object sender, RoutedEventArgs e)
        {
            Window inputWindow = new Window
            {
                Title = "Добавить модель",
                Height = 150,
                Width = 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };

            // Создаём элементы интерфейса
            Grid grid = new Grid { Margin = new Thickness(10) };

            TextBox textBox = new TextBox
            {
                VerticalAlignment = VerticalAlignment.Top,
                Height = 30,
                Text = "Введите название модели...",
                Foreground = Brushes.Gray
            };

            textBox.GotFocus += (o, args) =>
            {
                if (textBox.Text == "Введите название модели...")
                {
                    textBox.Text = "";
                    textBox.Foreground = Brushes.Black;
                }
            };
            textBox.LostFocus += (o, args) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = "Введите название модели...";
                    textBox.Foreground = Brushes.Gray;
                }
            };

            Button okButton = new Button
            {
                Content = "Добавить",
                Width = 110,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 1, 10)
            };

            Button cancelButton = new Button
            {
                Content = "Отмена",
                Width = 110,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 0, 10)
            };

            grid.Children.Add(textBox);
            grid.Children.Add(okButton);
            grid.Children.Add(cancelButton);

            inputWindow.Content = grid;

            string modelName = null;
            okButton.Click += (o, args) =>
            {
                modelName = textBox.Text;
                inputWindow.DialogResult = true;
            };

            cancelButton.Click += (o, args) =>
            {
                inputWindow.DialogResult = false;
            };


            if (inputWindow.ShowDialog() == true)
            {
                if (!string.IsNullOrWhiteSpace(modelName))
                {
                    models.Add(modelName);
                    SaveModels(); 
                    UpdateModelComboBox();
                    ModelComboBox.SelectedItem = modelName; 
                    OutputTextBox.AppendText($"Добавлена модель: {modelName}\n");
                }
                else
                {
                    OutputTextBox.AppendText("Название модели не может быть пустым.\n");
                }
            }
        }

        private void RemoveModelButton_Click(object sender, RoutedEventArgs e)
        {
            if (ModelComboBox.SelectedItem != null)
            {
                string selectedModel = ModelComboBox.SelectedItem.ToString();
                models.Remove(selectedModel);


                string modelFilePath = Path.Combine(@"C:\saved_models", $"{selectedModel}.pkl");
                if (File.Exists(modelFilePath))
                {
                    File.Delete(modelFilePath);
                }

                SaveModels();
                UpdateModelComboBox();
                AppendOutput($"Модель '{selectedModel}' удалена.");
            }
            else
            {
                AppendOutput("Выберите модель для удаления.");
            }
        }

        private void UpdateModelComboBox()
        {
            ModelComboBox.ItemsSource = null;
            ModelComboBox.ItemsSource = models;

            string lastModel = LoadLastModel();
            if (!string.IsNullOrEmpty(lastModel) && models.Contains(lastModel))
            {
                ModelComboBox.SelectedItem = lastModel;
            }
        }

        private void ModelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModelComboBox.SelectedItem != null)
            {
                string selectedModel = ModelComboBox.SelectedItem.ToString();
                SaveLastModel(selectedModel);
            }
        }

        private void SaveLastModel(string modelName)
        {
            try
            {
                File.WriteAllText("last_model.json", JsonSerializer.Serialize(modelName));
            }
            catch (Exception ex)
            {
                OutputTextBox.AppendText($"Ошибка сохранения последней модели: {ex.Message}\n");
            }
        }

        private string LoadLastModel()
        {
            try
            {
                if (File.Exists("last_model.json"))
                {
                    return JsonSerializer.Deserialize<string>(File.ReadAllText("last_model.json"));
                }
            }
            catch (Exception ex)
            {
                OutputTextBox.AppendText($"Ошибка загрузки последней модели: {ex.Message}\n");
            }
            return null;
        }

        private void LoadModels()
        {
            if (File.Exists(modelsFilePath))
            {
                try
                {
                    string json = File.ReadAllText(modelsFilePath);
                    models = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
                }
                catch (Exception ex)
                {
                    AppendOutput($"Ошибка загрузки моделей: {ex.Message}");
                }
            }
        }

        private void SaveModels()
        {
            try
            {
                string json = JsonSerializer.Serialize(models);
                File.WriteAllText(modelsFilePath, json);
            }
            catch (Exception ex)
            {
                AppendOutput($"Ошибка сохранения моделей: {ex.Message}");
            }
        }
    }
}
