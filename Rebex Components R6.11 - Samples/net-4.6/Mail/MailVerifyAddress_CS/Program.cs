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
using System.Configuration;
using System.Text;
using Rebex.Net;
using Rebex.Mime.Headers;

namespace Rebex.Samples
{
    /// <summary>
    /// Command line utility to checks validity of e-mail addresses.
    /// Parses specified e-mail addresses and resolves thair domain MX records.
    /// </summary>
    class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            // set the trial license key
            Rebex.Licensing.Key = Rebex.Samples.TrialKey.Key;


            if (args.Length == 0)
            {
                // display syntax if no parameter is specified
                string applicationName = AppDomain.CurrentDomain.FriendlyName;
                Console.WriteLine("=====================================================================");
                Console.WriteLine(" {0} ", applicationName);
                Console.WriteLine("=====================================================================");		
                Console.WriteLine("");
                Console.WriteLine("Command line utility to check validity of e-mail addresses.");
                Console.WriteLine("Parses specified e-mail addresses and resolves their domain MX records.");
                Console.WriteLine("");		
                Console.WriteLine("The program is a sample for Rebex Secure Mail for .NET component.");
                Console.WriteLine("For more info, see http://www.rebex.net/secure-mail.net/");
                Console.WriteLine("");

                Console.WriteLine("Syntax: {0} address1 [address2 [address3] ...]", applicationName);
                return 1;
            }


            // variables to hold total counts
            int total = 0;
            int ok = 0;

            // iterate through the supplied list of arguments
            for (int i=0; i<args.Length; i++)
            {
                // parse each argument into a address list
                MailAddressCollection list = new MailAddressCollection (args[i]);

                // check each address validity and resolve MX
                foreach (MailAddress box in list)
                {
                    total++;
                    if (box.Host == "")
                    {
                        Console.WriteLine ("Address '{0}': Invalid address.", box);
                        continue;
                    }

                    try
                    {
                        // resolve and display MX records, if available
                        string[] mailServers = Smtp.ResolveDomainMX(box.Host);

                        if (mailServers.Length == 0)
                        {
                            Console.WriteLine ("Address '{0}' has no MX records.", box);
                        }
                        else
                        {
                            Console.WriteLine ("Address '{0}' resolves to: {1}.", box, string.Join (", ",mailServers));
                            ok++;
                        }
                    }
                    catch (NotSupportedException x)
                    {
                        Console.WriteLine("Sample feature is not supported. Reason: {0}", x.Message);
                    }
                    catch (Exception x)
                    {
                        Console.WriteLine ("Address '{0}': {1}", box, x.Message);
                    }
                }	
            }

            // display statistics
            Console.WriteLine ();
            Console.WriteLine ("Resolved {1} out of {1} domains.", ok, total);
            return 0;
        }
    }
}
