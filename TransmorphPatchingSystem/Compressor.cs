using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TransmorphPatchingSystem
{
    public class Compressor
    {
        public static string SerializeCompressText(object obj)
        {
            var serialized = Serialize(obj);
            var compressed = Compress(serialized);
            string text = Convert.ToBase64String(compressed);
            return text;
        }
        public static T DecompressDeserializeText<T>(string text)
        {
            var compressed = Convert.FromBase64String(text);
            var serialized = Decompress(compressed);
            T output = Deserialize<T>(serialized);
            return output;
        }




        public static byte[] SerializeCompress(object obj) => Compress(Serialize(obj));
        public static byte[] Serialize(object obj)
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }


        public static T DecompressDeserialize<T>(byte[] input) => Deserialize<T>(Decompress(input));
        public static T Deserialize<T>(byte[] input)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream(input);
            var output = (T)formatter.Deserialize(stream);
            return output;
        }

        public static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        public static void RunTest()
        {
            //ensures the compression and decompression mechanism works

            bool success = true;

            List<byte> data = new List<byte>();
            for (int i = 0; i < 256; i++)
                data.Add((byte)i);


            byte[] input = data.ToArray();

            var compressed = Compress(input);
            var output = Decompress(compressed);

            if (input.Length != output.Length)
                success = false;

            for (int i = 0; i < 256; i++)
            {
                if (input[i] != output[i])
                    success = false;
            }


            if (!success)
                throw new Exception("TEST FAILED");
        }

        public static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }

        public static string CompressText(byte[] frameData)
        {
            var compressed = Compress(frameData);
            string text = Convert.ToBase64String(compressed);
            return text;
        }

        public static byte[] DecompressText(string text)
        {
            var compressed = Convert.FromBase64String(text);
            return Decompress(compressed);
        }
    }
}
