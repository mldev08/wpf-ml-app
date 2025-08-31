using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace progress
{
    public partial class TrainModelWindow : Window
    {
        private MainWindow parentWindow;
        private string selectedFilePath = string.Empty;
        private string currentModelName;

        public TrainModelWindow(MainWindow parent)
        {
            InitializeComponent();
            parentWindow = parent;
            currentModelName = parent.ModelComboBox.SelectedItem?.ToString();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = parent;
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                SelectedFileTextBox.Text = selectedFilePath;
                parentWindow.AppendOutput($"Выбран файл: {selectedFilePath}");
            }
        }

        private void StartTrainingButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedFilePath))
            {
                parentWindow.AppendOutput("Выберите файл перед началом обучения.");
                return;
            }

            if (string.IsNullOrWhiteSpace(currentModelName))
            {
                parentWindow.AppendOutput("Название модели не указано. Добавьте модель перед обучением.");
                return;
            }

            try
            {
                string pythonPath = @"C:\Users\matvey\AppData\Local\Programs\Python\Python311\python.exe";
                string scriptName = "script.py";
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string scriptPath = Path.Combine(baseDirectory, scriptName);


                if (!File.Exists(scriptPath))
                {
                    parentWindow.AppendOutput($"Скрипт {scriptPath} не найден.");
                    return;
                }


                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = pythonPath,
                        Arguments = $"{scriptPath} \"{selectedFilePath}\" \"{currentModelName}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrWhiteSpace(output))
                    parentWindow.AppendOutput(output);
                if (!string.IsNullOrWhiteSpace(errors))
                    parentWindow.AppendOutput($"Ошибки:\n{errors}");

                this.Close();

            }
            catch (Exception ex)
            {
                parentWindow.AppendOutput($"Ошибка: {ex.Message}");
            }
        }
    }
}
