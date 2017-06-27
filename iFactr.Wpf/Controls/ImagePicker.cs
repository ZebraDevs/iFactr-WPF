using System;
using System.Collections.Generic;
using iFactr.Core;
using iFactr.Core.Layers;

#if SILVERLIGHT
using System.Windows.Controls;
#else
using System.IO;
using System.Windows.Forms;
using MonoCross.Utilities;
#endif

namespace iFactr.Wpf
{
    public class ImagePicker
    {
        public const string Camera = "camera";
        public const string Gallery = "gallery";
        public const string CallbackUri = "callback";

        public string ImageUri { get; set; }
        public string ImageId { get; set; }

        public ImagePicker(iLayer.NavigationContext navContext, Dictionary<string, string> parameters = null)
        {
            string callbackUri = null;
            bool camera = false;
            bool gallery = true;

            if (parameters != null)
            {
                if (parameters.ContainsKey(CallbackUri))
                {
                    callbackUri = parameters[CallbackUri];
                }
                if (parameters.ContainsKey(Camera))
                {
                    bool.TryParse(parameters[Camera], out camera);
                }
                if (parameters.ContainsKey(Gallery))
                {
                    bool.TryParse(parameters[Gallery], out gallery);
                }
            }

            //TODO: If camera and gallery are both enabled, allow user to pick.

            if (camera)
            {
                //TODO: Camera support.
            }

            if (gallery)
            {
                var openFileDialog1 = new OpenFileDialog
                {
#if !SILVERLIGHT
                    Filter = "Image Files (*.png, *.jpg, *.jpeg, *.tif, *.tiff, *.gif)|*.png;*.jpg;*.jpeg;*.tif;*.tiff,*.gif|All Files (*.*)|*.*",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
#endif
                    Multiselect = false,
                };

#if SILVERLIGHT
                try
                {
                    if (openFileDialog1.ShowDialog() != true) return;
                }
                catch (Exception)
                {
                    return;
                }
                var fileStream = openFileDialog1.File.OpenRead();
                var extension = openFileDialog1.File.Extension.Trim(new[] { '.' });
                ImageUri = openFileDialog1.File.Name;
#else
                iApp.Factory.StopBlockingUserInput();
                if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
                var location = new FileInfo(openFileDialog1.FileName);
                ImageUri = location.FullName;
                var extension = location.Extension.Trim(new[] { '.' });
                var fileStream = new MemoryStream(iApp.File.Read(ImageUri, EncryptionMode.NoEncryption));
#endif
                ImageId = WpfFactory.Instance.StoreImage(fileStream, extension);
                fileStream.Close();

                if (callbackUri != null)
                {
                    WpfFactory.Instance.IsBusy = true;
                    iApp.SetNavigationContext(navContext);
                    iApp.Navigate(callbackUri, new Dictionary<string, string> { { "PhotoImage", ImageId }, });
                }
            }
        }
    }
}