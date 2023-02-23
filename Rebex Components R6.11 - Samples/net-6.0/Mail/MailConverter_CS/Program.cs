//  
//   Rebex Sample Code License
// 
//   Copyright 2023, REBEX CR s.r.o.
//   All rights reserved.
//   https://www.rebex.net/
// 
//   Permission to use, copy, modify, and/or distribute this software for any
//   purpose with or without fee is hereby granted.
// 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//   EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//   OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//   NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
//   HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//   WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//   FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//   OTHER DEALINGS IN THE SOFTWARE.
// 

using System;
using System.IO;
using Rebex.Mail;

namespace Rebex.Samples
{
    /// <summary>
    /// This program converts mail messages between different formats.
    /// See https://www.rebex.net/mail-converter/ for a more powerful version of this app with extra features,
    /// such as conversion of multiple files.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // set the trial license key
            Rebex.Licensing.Key = Rebex.Samples.TrialKey.Key;


            if (args.Length != 3)
            {
                ShowHelp();
                return;
            }

            string sourcePath = args[1];
            string targetPath = args[2];

            // check if the sourcePath exists
            if (!File.Exists(sourcePath))
            {
                Console.WriteLine("Cannot find the source file.");
                return;
            }

            MailFormat format;
            switch (args[0].ToLowerInvariant())
            {
                case "-tomime":
                    format = MailFormat.Mime;
                    break;

                case "-tomsg":
                    format = MailFormat.OutlookMsg;
                    break;

                default:
                    Console.WriteLine("Invalid format specified.");
                    return;
            }

            // convert the mail message
            MailMessage mail = new MailMessage();
            mail.Load(sourcePath);
            mail.Save(targetPath, format);
        }

        /// <summary>
        /// Shows the app help.
        /// </summary>
        private static void ShowHelp()
        {
            string applicationName = AppDomain.CurrentDomain.FriendlyName;
            Console.WriteLine("=====================================================================");
            Console.WriteLine(" {0} ", applicationName);
            Console.WriteLine("=====================================================================");
            Console.WriteLine();
            Console.WriteLine("Converts e-mail messages between different formats.");
            Console.WriteLine();
            Console.WriteLine("The program is a sample for Rebex Secure Mail for .NET component.");
            Console.WriteLine("For more info, see http://www.rebex.net/secure-mail.net/");

            Console.WriteLine();
            Console.WriteLine("Syntax: MailConverter.exe -tomime|-tomsg sourcepath targetpath");

            Console.WriteLine();
            Console.WriteLine(@"Example: MailConverter.exe -tomsg C:\mail.eml C:\mail.msg");
            Console.WriteLine(@"Example: MailConverter.exe -tomime C:\mail.msg C:\mail.eml");
        }
    }
}
