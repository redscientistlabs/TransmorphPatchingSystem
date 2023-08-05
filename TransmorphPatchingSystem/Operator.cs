using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransmorphPatchingSystem
{
    internal class Operator
    {
        static byte[] inputData = null;
        static Dictionary<ushort, int> ushortInputKnowledge = new Dictionary<ushort, int>();
        static List<ushort> orderedUshortInputKnowledge = new List<ushort>();

        static byte[] outputData = null;
        static string outputName = null;
        static Dictionary<ushort, int> ushortOutputKnowledge = new Dictionary<ushort, int>();

        internal static void LoadInput(string input)
        {
            inputData = File.ReadAllBytes(input);
        }

        internal static Tesseract GenerateTesseract()
        {
            Tesseract TPS = new Tesseract();

            TPS.targetFilename = Path.GetFileName(outputName);
            TPS.targetSize = outputData.Length;

            TPS.targetChecksum = CalculateChecksum(outputData);

            int goodHits = 0;
            int badHits = 0;

            for(int i = 0; i < outputData.Length; i++)
            {
                if(i%2 == 0)
                {
                    byte[] buffer = new byte[2];
                    Buffer.BlockCopy(outputData, i, buffer, 0, 2);

                    ushort data = BitConverter.ToUInt16(buffer, 0);

                    if(ushortInputKnowledge.TryGetValue(data, out int inputPosition))
                    {

                        TPS.FatPointers.Add(inputPosition);

                        goodHits++;
                    }
                    else
                    {

                        TPS.FatPointers.Add(-1);

                        var trans = GetTransformation(data);
                        TPS.FatDiffPointers.Add(trans.pointer);
                        TPS.FatDiffValues.Add(trans.diff);

                        badHits++;
                    }
                }
            }
            double totalSearches = goodHits + badHits;
            double goodBadRatio = Convert.ToDouble(goodHits) / totalSearches;

            TPS.pointers = TPS.FatPointers.ToArray();
            TPS.diffPointers = TPS.FatDiffPointers.ToArray();
            TPS.diffValues = TPS.FatDiffValues.ToArray();

            return TPS;
        }

        internal static void ProcessTesseract(Tesseract TPS, string output)
        {
            byte[] vessel = new byte[TPS.targetSize];

            int pointerIterator = 0;
            int subPointerIterator = 0;

            for(int i = 0; i < vessel.Length; i++)
            {
                if(i%2 == 0)
                {
                    var pointer = TPS.pointers[pointerIterator];

                    if (pointer == -1)
                    {
                        var diffPointer = TPS.diffPointers[subPointerIterator];
                        var diffValue = TPS.diffValues[subPointerIterator];

                        byte[] buffer = new byte[2];
                        Buffer.BlockCopy(inputData, diffPointer, buffer, 0, 2);

                        //buffer = buffer.Reverse().ToArray(); //flip endianess

                        var fetchedValue = Convert.ToInt32(BitConverter.ToUInt16(buffer, 0));

                        var finalValue = fetchedValue - diffValue;
                        var finalData = BitConverter.GetBytes(Convert.ToUInt16(finalValue));

                        vessel[i] = finalData[0];
                        vessel[i+1] = finalData[1];

                        subPointerIterator++;
                    }
                    else
                    {
                        byte[] buffer = new byte[2];
                        Buffer.BlockCopy(inputData, pointer, buffer, 0, 2);

                        ushort fetchedValue = BitConverter.ToUInt16(buffer, 0);
                        var finalData = BitConverter.GetBytes(fetchedValue);

                        vessel[i] = finalData[0];
                        vessel[i + 1] = finalData[1];
                    }

                    pointerIterator++;
                }
            }

            File.WriteAllBytes(output, vessel);
        }

        private static int CalculateChecksum(byte[] outputData)
        {
            return -1;
        }

        private static (int pointer, int diff) GetTransformation(ushort data)
        {
            var altVal = orderedUshortInputKnowledge.First(it => it > data);

            var pointer = ushortInputKnowledge[altVal];
            var diff = altVal - data;

            return (pointer, diff);
        }

        internal static void LoadOutput(string output)
        {
            outputData = File.ReadAllBytes(output);
            outputName = output;
        }

        internal static void ProbeInput()
        {
            ushortInputKnowledge.Clear();

            for (int i=0; i<inputData.Length; i++)
            {
                if(i%2 == 0)
                {
                    byte[] buffer = new byte[2];
                    Buffer.BlockCopy(inputData, i, buffer, 0, 2);

                    ushort data = BitConverter.ToUInt16(buffer, 0);
                    ushortInputKnowledge[data] = i;
                }

            }

            orderedUshortInputKnowledge = ushortInputKnowledge.Keys.OrderBy(k => k).ToList();

            new object();
        }

        internal static void ProbeOutput()
        {
            ushortOutputKnowledge.Clear();

            for (int i = 0; i < outputData.Length; i++)
            {
                if (i % 2 == 0)
                {
                    byte[] buffer = new byte[2];
                    Buffer.BlockCopy(outputData, i, buffer, 0, 2);

                    ushort data = BitConverter.ToUInt16(buffer,0);
                    ushortOutputKnowledge[data] = i;
                }

            }


            new object();
        }
    }
}
