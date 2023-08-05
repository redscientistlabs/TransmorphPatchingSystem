using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace TransmorphPatchingSystem
{
    [Serializable()]
    public class Tesseract
    {
        public int targetSize = 0;
        public int targetChecksum = 0;

        public int[] pointers { get; set; } = null;
        public int[] diffPointers { get; set; } = null;
        public int[] diffValues { get; set; } = null;
        public string targetFilename { get; set; } = null;

        [NonSerialized()]
        public List<int> FatPointers = new List<int>();
        [NonSerialized()]
        public List<int> FatDiffPointers = new List<int>();
        [NonSerialized()]
        public List<int> FatDiffValues = new List<int>();

        public static Tesseract LoadTesseract(string input)
        {

            try
            {
                var bytes = File.ReadAllBytes(input);
                var TPS = Compressor.DecompressDeserialize<Tesseract>(bytes);
                return TPS;
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            

        }

        public static void SaveTesseract(Tesseract TPS, string output)
        {

            try
            {
                var data = Compressor.SerializeCompress(TPS);
                File.WriteAllBytes(output, data);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
        }
    }
}
