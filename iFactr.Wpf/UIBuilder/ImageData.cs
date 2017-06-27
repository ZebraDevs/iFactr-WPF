using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using iFactr.Core;
using MonoCross;
using MonoCross.Utilities;

namespace iFactr.Wpf
{
    public class ImageData : IImageData
    {
        public BitmapImage Source { get; private set; }

        private string filePath;

        public ImageData(BitmapImage source, string filePath = null)
        {
            Source = source;
            this.filePath = filePath;
        }

        public byte[] GetBytes()
        {
            if (Source == null)
            {
                return new byte[0];
            }

            byte[] array = new byte[Source.PixelWidth * Source.PixelHeight * 4];
            Source.CopyPixels(array, Source.PixelWidth * 4, 0);
            return array;
        }

        public IExifData GetExifData()
        {
            return new ExifData(Source, filePath);
        }

        public void Save(string filePath, ImageFileFormat format)
        {
            BitmapEncoder encoder = format == ImageFileFormat.JPEG ? (BitmapEncoder)new JpegBitmapEncoder() : new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(Source));

            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                System.IO.File.WriteAllBytes(filePath, stream.ToArray());
            }
        }
    }

    public class ExifData : IExifData
    {
        public double Aperture { get; private set; }

        public int ColorSpace { get; private set; }

        public double DPIHeight { get; private set; }

        public double DPIWidth { get; private set; }

        public DateTime DateTime { get; private set; }

        public DateTime DateTimeDigitized { get; private set; }

        public DateTime DateTimeOriginal { get; private set; }

        public string ExposureProgram { get; private set; }

        public double ExposureTime { get; private set; }

        public double FNumber { get; private set; }

        public int Flash { get; private set; }

        public double FocalLength { get; private set; }

        public string Manufacturer { get; private set; }

        public string Model { get; private set; }

        public int Orientation { get; private set; }

        public double PixelHeight { get; private set; }

        public double PixelWidth { get; private set; }

        public double ShutterSpeed { get; private set; }

        public double XResolution { get; private set; }

        public double YResolution { get; private set; }

        private BitmapMetadata metadata;

        public ExifData(BitmapImage source, string filePath = null)
        {
            if (source == null)
            {
                return;
            }

            metadata = BitmapFrame.Create(source.UriSource ?? new Uri(filePath, UriKind.RelativeOrAbsolute)).Metadata as BitmapMetadata;
            if (metadata != null)
            {
                Aperture = GetQuery<double>("System.Photo.Aperture");
                ColorSpace = GetQuery<string>("System.Image.ColorSpace").TryParseInt32();
                DateTime = GetQuery<DateTime>("/app1/{ushort=0}/{ushort=34665}/{ushort=306}");
                DateTimeDigitized = GetQuery<DateTime>("/app1/{ushort=0}/{ushort=34665}/{ushort=36868}");
                DateTimeOriginal = GetQuery<DateTime>("/app1/{ushort=0}/{ushort=34665}/{ushort=36867}");
                ExposureProgram = GetQuery<string>("System.Photo.ExposureProgram");
                ExposureTime = GetQuery<double>("System.Photo.ExposureTime");
                Flash = GetQuery<string>("System.Photo.Flash").TryParseInt32();
                FNumber = GetQuery<double>("System.Photo.FNumber");
                FocalLength = GetQuery<double>("System.Photo.FocalLength");
                Manufacturer = GetQuery<string>("System.Photo.CameraManufacturer");
                Model = GetQuery<string>("System.Photo.CameraModel");
                Orientation = GetQuery<string>("System.Photo.Orientation").TryParseInt32();
                ShutterSpeed = GetQuery<double>("System.Photo.ShutterSpeed");
                XResolution = GetQuery<double>("System.Image.HorizontalResolution");
                YResolution = GetQuery<double>("System.Image.VerticalResolution");
            }

            DPIWidth = source.DpiX;
            DPIHeight = source.DpiY;
            PixelHeight = source.PixelHeight;
            PixelWidth = source.PixelWidth;
        }

        public IDictionary<string, object> GetRawData()
        {
            var dictionary = new Dictionary<string, object>();
            ExtractExifData(metadata, dictionary);
            return dictionary;
        }

        private T GetQuery<T>(string tag)
        {
            var value = metadata.GetQuery(tag);
            if (value == null)
            {
                return default(T);
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)value.ToString();
            }

            if (typeof(T) == typeof(DateTime))
            {
                if (value is System.Runtime.InteropServices.ComTypes.FILETIME)
                {
                    var filetime = (System.Runtime.InteropServices.ComTypes.FILETIME)value;
                    long highBits = filetime.dwHighDateTime;
                    highBits = highBits << 32;

                    return (T)(object)DateTime.FromFileTime(highBits + (long)(uint)filetime.dwLowDateTime);
                }

                return (T)(object)DateTime.ParseExact(value.ToString(), "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
            }

            try
            {
                return (T)value;
            }
            catch
            {
                return default(T);
            }
        }

        private void ExtractExifData(BitmapMetadata data, Dictionary<string, object> dictionary)
        {
            if (data == null)
            {
                return;
            }

            foreach (var tag in data)
            {
                var query = data.GetQuery(tag);
                var bmp = query as BitmapMetadata;
                if (bmp != null)
                {
                    ExtractExifData(bmp, dictionary);
                    continue;
                }

                try
                {
                    string name = tag;
                    int index = tag.IndexOf('=');
                    if (index >= 0)
                    {
                        name = tag.Substring(index + 1);
                        name = name.Remove(name.IndexOf('}'));

                        ushort value = 0;
                        if (ushort.TryParse(name, out value))
                        {
                            name = ((ExifTags)value).ToString();
                        }
                    }

                    dictionary[name] = query;
                }
                catch
                {
                    dictionary[tag] = query;
                }

                
            }
        }
    }

    public enum ExifTags : ushort
    {
        // primary tags
        DPIWidth = 0x100,
        DPIHeight = 0x101,
        BitsPerSample = 0x102,
        Compression = 0x103,
        PhotometricInterpretation = 0x106,
        ImageDescription = 0x10E,
        Manufacturer = 0x10F,
        Model = 0x110,
        StripOffsets = 0x111,
        Orientation = 0x112,
        SamplesPerPixel = 0x115,
        RowsPerStrip = 0x116,
        StripByteCounts = 0x117,
        XResolution = 0x11A,
        YResolution = 0x11B,
        PlanarConfiguration = 0x11C,
        ResolutionUnit = 0x128,
        TransferFunction = 0x12D,
        Software = 0x131,
        DateTime = 0x132,
        Artist = 0x13B,
        WhitePoint = 0x13E,
        PrimaryChromaticities = 0x13F,
        JPEGInterchangeFormat = 0x201,
        JPEGInterchangeFormatLength = 0x202,
        YCbCrCoefficients = 0x211,
        YCbCrSubSampling = 0x212,
        YCbCrPositioning = 0x213,
        ReferenceBlackWhite = 0x214,
        Copyright = 0x8298,

        // EXIF tags
        ExposureTime = 0x829A,
        FNumber = 0x829D,
        ExposureProgram = 0x8822,
        SpectralSensitivity = 0x8824,
        ISOSpeedRatings = 0x8827,
        OECF = 0x8828,
        ExifVersion = 0x9000,
        DateTimeOriginal = 0x9003,
        DateTimeDigitized = 0x9004,
        ComponentsConfiguration = 0x9101,
        CompressedBitsPerPixel = 0x9102,
        ShutterSpeed = 0x9201,
        Aperture = 0x9202,
        Brightness = 0x9203,
        ExposureBias = 0x9204,
        MaxAperture = 0x9205,
        SubjectDistance = 0x9206,
        MeteringMode = 0x9207,
        LightSource = 0x9208,
        Flash = 0x9209,
        FocalLength = 0x920A,
        SubjectArea = 0x9214,
        MakerNote = 0x927C,
        UserComment = 0x9286,
        SubsecTime = 0x9290,
        SubsecTimeOriginal = 0x9291,
        SubsecTimeDigitized = 0x9292,
        FlashpixVersion = 0xA000,
        ColorSpace = 0xA001,
        PixelWidth = 0xA002,
        PixelHeight = 0xA003,
        RelatedSoundFile = 0xA004,
        FlashEnergy = 0xA20B,
        SpatialFrequencyResponse = 0xA20C,
        FocalPlaneXResolution = 0xA20E,
        FocalPlaneYResolution = 0xA20F,
        FocalPlaneResolutionUnit = 0xA210,
        SubjectLocation = 0xA214,
        ExposureIndex = 0xA215,
        SensingMethod = 0xA217,
        FileSource = 0xA300,
        SceneType = 0xA301,
        CFAPattern = 0xA302,
        CustomRendered = 0xA401,
        ExposureMode = 0xA402,
        WhiteBalance = 0xA403,
        DigitalZoomRatio = 0xA404,
        FocalLengthIn35mmFilm = 0xA405,
        SceneCaptureType = 0xA406,
        GainControl = 0xA407,
        Contrast = 0xA408,
        Saturation = 0xA409,
        Sharpness = 0xA40A,
        DeviceSettingDescription = 0xA40B,
        SubjectDistanceRange = 0xA40C,
        ImageUniqueID = 0xA420,

        // GPS tags
        GPSVersionID = 0x0,
        GPSLatitudeRef = 0x1,
        GPSLatitude = 0x2,
        GPSLongitudeRef = 0x3,
        GPSLongitude = 0x4,
        GPSAltitudeRef = 0x5,
        GPSAltitude = 0x6,
        GPSTimestamp = 0x7,
        GPSSatellites = 0x8,
        GPSStatus = 0x9,
        GPSMeasureMode = 0xA,
        GPSDOP = 0xB,
        GPSSpeedRef = 0xC,
        GPSSpeed = 0xD,
        GPSTrackRef = 0xE,
        GPSTrack = 0xF,
        GPSImgDirectionRef = 0x10,
        GPSImgDirection = 0x11,
        GPSMapDatum = 0x12,
        GPSDestLatitudeRef = 0x13,
        GPSDestLatitude = 0x14,
        GPSDestLongitudeRef = 0x15,
        GPSDestLongitude = 0x16,
        GPSDestBearingRef = 0x17,
        GPSDestBearing = 0x18,
        GPSDestDistanceRef = 0x19,
        GPSDestDistance = 0x1A,
        GPSProcessingMethod = 0x1B,
        GPSAreaInformation = 0x1C,
        GPSDateStamp = 0x1D,
        GPSDifferential = 0x1E
    }
}
