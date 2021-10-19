using System;
using System.Collections.Generic;
using static test1.Base45;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using PeterO.Cbor;
using System.Text;
using System.IO;

namespace test1
{
    class Program
    {
        //bron = Install-Package origo.dgc -Version 1.0.1.6   

        static void Main(string[] args)
        {
            string codeData = "HC1:NCFM70P90T9WTWGSLKC 4+79MHEJZGQW79ADABB0XK:ICSW82F3Z0RUIE2F3Z0M JEY50.FK6ZK7:EDOLOPCF8F7460H8.+A$Y9G:67Y8.*9W09.1BETAKN9/%6.G8NB8YK4WJCT3E0H8XJC +DXJCCWENF6OF63W5NW6.96%JCKQEV+AQIB.JCBECB1A-:8$96646746-Q6307Q$D.UDRYA 96NF6L/5SW6VX6KQE*709WEQDD+Q6TW6FA7C466KCN9E%961A69L6QW6B46JPCT3E5JDNA73467464W51S6..DX%DZJC3/DWY8VKE5$C4WEI3D.8E7$C5$C$34JEC5UD9Z9*KE1ECW.C9WE0Y8.HAGY83UAI3DIWET6AITA$HAP6ABZA2S7RB8XB9$PC5$CUZCY$5Y$527B:VDORSZ7WGN707LC/E-VK*QS2RIC$G2G6OTT-ABGW9WNBRYKZ7N$TJEO6I*NQ8T6G2NNIK-7EOBVER-LN4+Q5.UHXOK.0NQ30AB8CF";

            // The base45 encoded data shoudl begin with HC1
            if (codeData.StartsWith("HC1:"))
            {
                string base45CodedData = codeData.Substring(4);

                // Base 45 decode data
                byte[] base45DecodedData = Base45Decoding(Encoding.GetEncoding("UTF-8").GetBytes(base45CodedData));

                // zlib decompression
                byte[] uncompressedData = ZlibDecompression(base45DecodedData);

                byte[] coseContent = getCoseContent(uncompressedData);

                string json = getJson(coseContent);

                Console.WriteLine(json);
            }

        }

        public static string getJson(byte[] bytes)
        {
            var cbor = CBORObject.DecodeFromBytes(bytes);
            return cbor[-260][1].ToJSONString();
        }


        protected static byte[] ZlibDecompression(byte[] compressedData)
        {
            if (compressedData[0] == 0x78)
            {
                var outputStream = new MemoryStream();
                using (var compressedStream = new MemoryStream(compressedData))
                using (var inputStream = new InflaterInputStream(compressedStream))
                {
                    inputStream.CopyTo(outputStream);
                    outputStream.Position = 0;
                    return outputStream.ToArray();
                }
            }
            else
            {
                Console.WriteLine("error");
                // The data is not compressed
                return compressedData;
            }
        }

        protected static byte[] getCoseContent(byte[] signedData)
        {
            CBORObject message = CBORObject.DecodeFromBytes(signedData);
            byte[] Content = message[2].GetByteString();
            return Content;
        }

        protected static byte[] Base45Decoding(byte[] encodedData)
        {
            byte[] uncodedData = Base45.Decode(encodedData);
            return uncodedData;
        }

    }
}
