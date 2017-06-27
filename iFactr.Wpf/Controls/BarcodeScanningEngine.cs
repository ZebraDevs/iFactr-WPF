using System;
using System.Windows.Input;
using iFactr.Core;

#if SILVERLIGHT
using iFactr.Core.Layers;
using MonoCross.Navigation;
using Telerik.Windows.Controls.Design;
#endif

namespace iFactr.Wpf
{
    public class BarcodeScanningEngine
    {
        public BarcodeScanningEngine(bool startActive) { IsActive = startActive; }
        private readonly KeyConverter _keyInterpreter = new KeyConverter();
        public static BarcodeScanningEngine Instance { get; set; }

        public static void ActivateScanner(bool activate)
        {
            if (activate)
            {
                if (Instance == null)
                {
                    Instance = new BarcodeScanningEngine(true);
                }
                else
                {
                    if (PopoverPane.PopoverWindow != null)
                        PopoverPane.PopoverWindow.KeyUp -= Instance.ScanLayerKeyboardHandler;
                    WpfFactory.Instance.MainWindow.KeyUp -= Instance.ScanLayerKeyboardHandler;

                    lock (Instance)
                    {
                        Instance = new BarcodeScanningEngine(true);
                    }
                }

                if (PopoverPane.PopoverWindow != null)
                    PopoverPane.PopoverWindow.KeyUp += Instance.ScanLayerKeyboardHandler;
                WpfFactory.Instance.MainWindow.KeyUp += Instance.ScanLayerKeyboardHandler;
            }
            else if (Instance != null)
            {
                if (PopoverPane.PopoverWindow != null)
                    PopoverPane.PopoverWindow.KeyUp -= Instance.ScanLayerKeyboardHandler;
                WpfFactory.Instance.MainWindow.KeyUp -= Instance.ScanLayerKeyboardHandler;

                if (!Instance.IsActive) return;
                lock (Instance)
                {
                    Instance.IsActive = false;
                }
            }
        }

        public string ScannedValues
        {
            get
            {
                string s = null;
                if (_scannedValues != null)
                {
                    s = String.Copy(_scannedValues);
                }
                return s;
            }
        }
        public void AddChar(Key key)
        {
            string stringValue = CheckIfValidCharAndReturnStringValue(key);
            if (!String.IsNullOrEmpty(stringValue)) { _scannedValues += stringValue; }
        }
        private string _scannedValues;

        public bool IsActive { get; set; }

        private string CheckIfValidCharAndReturnStringValue(Key key)
        {
            string newValue = null;
            if (key >= Key.A && key <= Key.Z ||
                key >= Key.NumPad0 && key <= Key.NumPad9 ||
                key >= Key.D0 && key <= Key.D9 ||
                key == Key.Enter ||
                key == Key.Space
#if !SILVERLIGHT
 || key == Key.OemPipe
#endif
)
            {
                switch (key)
                {
#if !SILVERLIGHT
                    case Key.OemPipe:
                        newValue = "|";
                        break;
#endif
                    case Key.Enter:
                        newValue = "\r\n";
                        break;
                    case Key.Space:
                        newValue = " ";
                        break;
                    default:
                        newValue = _keyInterpreter.ConvertToString(key);
                        break;
                }
            }

            return newValue;
        }


        public void ScanLayerKeyboardHandler(object sender, KeyEventArgs e)
        {
            lock (this)
            {
                if (IsActive) { AddChar(e.Key); }
            }
        }

        internal static string ScannedBarcodes
        {
            get
            {
                var result = String.Empty;
                if (Instance != null)
                {
                    lock (Instance)
                    {
                        result = Instance.ScannedValues;
                        Instance = new BarcodeScanningEngine(true);
                    }
                }
                return result;
            }
        }
    }
}