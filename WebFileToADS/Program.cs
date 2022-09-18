using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern SafeFileHandle CreateFile(
           string lpFileName,
           EFileAccess dwDesiredAccess,
           EFileShare dwShareMode,
           IntPtr lpSecurityAttributes,
           ECreationDisposition dwCreationDisposition,
           EFileAttributes dwFlagsAndAttributes,
           IntPtr hTemplateFile);

        static void usage()
        {
            Console.WriteLine("program.exe <FileURL> <DestFilePath:ADSName>");
        }
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {

                usage();
            }
            else {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string url = args[0];
            string destfile = args[1];

            Console.WriteLine("PARAM URL: " + url);
            Console.WriteLine("PARAM DESTFILE: " + destfile);

            WebClient wc = new WebClient();

            Console.WriteLine("Téléchargement des données: " + url);
            
            Byte[] datas = null;
            try
            {
                datas = wc.DownloadData(url);
            }
            catch(Exception ex)
            {
                Console.WriteLine("error");
                Console.WriteLine(ex.Message);
            }


            CreateFileWithAlternateDataStream(destfile, datas);
            }

        }

        static void CreateFileWithAlternateDataStream(string destFile, byte[] datas)
        {
            var sfh = CreateFile(destFile,
                   EFileAccess.GenericRead | EFileAccess.GenericWrite,
                    EFileShare.Read,
                    IntPtr.Zero,
                    ECreationDisposition.CreateAlways,
                    EFileAttributes.Normal,
                    IntPtr.Zero);

            if (sfh.IsInvalid)
            {
                Console.WriteLine("Error:");
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
            
            Console.WriteLine("Ecriture de l'ADS");
            try
            {
                FileStream fs = new FileStream(sfh, FileAccess.Write);
            
                fs.Write(datas, 0, datas.Length);
                Console.WriteLine("Datas written");
                Console.WriteLine("Fermeture des flux");
                fs.Close();
                sfh.Close();
           
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }

[Flags]
enum EFileAccess : uint
{
    GenericRead = 0x80000000,
    GenericWrite = 0x40000000,
    GenericExecute = 0x20000000,
    GenericAll = 0x10000000
}

[Flags]
public enum EFileShare : uint
{
    None = 0x00000000,
    Read = 0x00000001,
    Write = 0x00000002,
    Delete = 0x00000004
}

public enum ECreationDisposition : uint
{
    New = 1,
    CreateAlways = 2,
    OpenExisting = 3,
    OpenAlways = 4,
    TruncateExisting = 5
}

[Flags]
public enum EFileAttributes : uint
{
    Normal = 0x00000080
}