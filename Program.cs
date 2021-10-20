using System;
using System.Collections.Generic;
using static test1.Base45;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using PeterO.Cbor;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace test1
{
    class Program
    {
        //bron = Install-Package origo.dgc -Version 1.0.1.6   

        static void Main(string[] args)
        {
            string codeData = "HC1:NCFM70P90T9WTWGSLKC 4+79MHEJZGQW79ADABB0XK:ICSW82F3Z0RUIE2F3Z0M JEY50.FK6ZK7:EDOLOPCF8F7460H8.+A$Y9G:67Y8.*9W09.1BETAKN9/%6.G8NB8YK4WJCT3E0H8XJC +DXJCCWENF6OF63W5NW6.96%JCKQEV+AQIB.JCBECB1A-:8$96646746-Q6307Q$D.UDRYA 96NF6L/5SW6VX6KQE*709WEQDD+Q6TW6FA7C466KCN9E%961A69L6QW6B46JPCT3E5JDNA73467464W51S6..DX%DZJC3/DWY8VKE5$C4WEI3D.8E7$C5$C$34JEC5UD9Z9*KE1ECW.C9WE0Y8.HAGY83UAI3DIWET6AITA$HAP6ABZA2S7RB8XB9$PC5$CUZCY$5Y$527B:VDORSZ7WGN707LC/E-VK*QS2RIC$G2G6OTT-ABGW9WNBRYKZ7N$TJEO6I*NQ8T6G2NNIK-7EOBVER-LN4+Q5.UHXOK.0NQ30AB8CF";
            string tested = "HC1:NCFOXN%TSMAHN-H%OCHOS80JS3NL73:D4+OV-36HD7AOMOW4S2S**J4G5W/JT3FF/8X*G3M9BM9Z0BZW4V/AY733J7%2HV77ADFYRVNDF.93$PN-*0X37*090GVVNNGM5V.499TP+M5*K*U3*96846A$Q 76UW62U10%MPF65ZMNH6LK92R5QV1O2R0NLD+9 BLXE6UC65ZM176NF675IPF5$5QA46/Q6576PR6PF5RBQ746B46O1N646RM9XC5.Q69L6-96QW6U46%E5 NPC71AL6ZO66X69/9-3AKI63ZMLEQZ76UW6*E99Q9E$BDZIE9J/MJFZI*IB*NIZ0KA42BKBTKBA4229BCWKXSJGZI8DJC0J*PITQTA.SGD32OIZ0K%GA+ESCQSETC%ESISTR SR63+NTWVBDKBYLDN4DE1D-NSLFUKQ9B.UP-1AZJS9JE6F*ZJKE7+3G3UUS.77SU1QUB5JPN2R*O55OOQC*3JSH53SFN*46PBMZL+H2%-T$LVVV1Y:D3T3AP7BFPI7SYM0/KO+DG";
            string recovered = "HC1:NCFOXN%TSMAHN-H%OCHOS80JS3NL73.G47X5+T9ZKEOGIOVEZVVM*4555%2VCV4*XUA2P-FHT-HNTI4L6F$SYQ1WWB8VFJT9JQOVV3GP8XVHJZBMP8/V7PRK3P3QUKP2W:PIWIG4SISSQWVHWVH+ZE+R55%HIMIJH95R76YB8L49Q8: KNH42 J8FFZ.CY.C KE5KD1FEP.B5IAXMFU*GSHGRKMXGG%DBZI9$JAQJKM*GX2MZJKOHKSKE MCAOI8%MVYEQ KX*O%OKI%KI$N+CGDPIHG4OCGDPQIX0S%O+*P3-SY$N%VEY5LV39JDKM.SY$NPJGL8R$8RZQJ*8P/-3*4C 347CPFRMLNKNM8POCJPG56H$RH1%3MN9HDB$-PF$5V7OVWMHOS10GKFW916U0SHK4.4JD S*QLDMC*D5VFV8WK:.7P4CMEV/RO9-0%W1SBTEU5T-1$YOZ3W4OVTY1U%OYXE";
            // The base45 encoded data shoudl begin with HC1
            if (codeData.StartsWith("HC1:"))
            {
                string base45CodedData = recovered.Substring(4);

                // Base 45 decode data
                byte[] base45DecodedData = Base45Decoding(Encoding.GetEncoding("UTF-8").GetBytes(base45CodedData));

                // zlib decompression
                byte[] uncompressedData = ZlibDecompression(base45DecodedData);

                byte[] coseContent = getCoseContent(uncompressedData);

                string json = getJson(coseContent);

                JsonModel jsonModel = JsonConvert.DeserializeObject<JsonModel>(json);
                Boolean magbinnen = jsonModel.SafeAndVacc();
                Boolean magbinnen2 = jsonModel.TestandSafe();
                Boolean magbinnen3 = jsonModel.Recovered();
                Console.WriteLine(magbinnen);
                Console.WriteLine(magbinnen2);
                Console.WriteLine(magbinnen3);
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
