using NAudio.Wave;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vosk;
using System.Diagnostics;
using System.IO;

namespace LeeVoskExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WaveInEvent waveIn;

        public MainWindow()
        {
            InitializeComponent();
            Debug.WriteLine("Start MainWindow");
            string modelPath = AppDomain.CurrentDomain.BaseDirectory + "vosk-model-small-ko-0.22"; // 모델 경로
            Vosk.Vosk.SetLogLevel(0);
            var model = new Model(modelPath);
            var recognizer = new VoskRecognizer(model, 16000.0f);

            waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(16000, 1) // 16kHz Mono
            };

            waveIn.DataAvailable += (s, e) =>
            {
                if (recognizer.AcceptWaveform(e.Buffer, e.BytesRecorded))
                {
                    var result = JObject.Parse(recognizer.Result());
                    Debug.WriteLine("Recognized: " + result["text"]);
                }
            };

            DemoBytes(model);
        }

        public static void DemoBytes(Model model)
        {
            // Demo byte buffer
            VoskRecognizer rec = new VoskRecognizer(model, 16000.0f);
            rec.SetMaxAlternatives(0);
            rec.SetWords(true);
            using (Stream source = File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + "test.wav"))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (rec.AcceptWaveform(buffer, bytesRead))
                    {
                        Debug.WriteLine(rec.Result());
                    }
                    else
                    {
                        Debug.WriteLine(rec.PartialResult());
                    }
                }
            }
            Debug.WriteLine(rec.FinalResult());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Recode Start");
            waveIn.StartRecording();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Recode Stop");
            waveIn.StopRecording();
        }
    }
}
