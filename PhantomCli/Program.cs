using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using Phantasma.Cryptography;
using Phantasma.Numerics;
using Phantasma.VM;
using Phantasma.Storage;
using Phantom.Wallet.Controllers;

using Newtonsoft.Json;


namespace PhantomCli
{
    class PhantomCli
    {
        private static KeyPair keyPair {get; set;} = null;

        private static AccountController AccountController { get; set; }

        public static void SetupControllers()
        {
            AccountController = new AccountController();
        }


        static void Main(string[] args)
        {
            string version = "0.1";
            var prompt = "phantom> ";
            var startupMsg = "PhantomCli " + version;

            SetupControllers();
            List<string> completionList = new List<string>(lCommands.Keys); 
            Prompt.Run(
                ((command, listCmd, lists) =>
                {

                    string command_main = command.Split(new char[] { ' ' }).First();
                    string[] arguments = command.Split(new char[] { ' ' }).Skip(1).ToArray();
                    if (lCommands.ContainsKey(command_main))
                    {
                        Tuple<Action<string[]>, string> cmd = null;
                        lCommands.TryGetValue(command_main, out cmd);
                        Action<string[]> function_to_execute = cmd.Item1;
                        function_to_execute(arguments);
                    }
                    else
                        Console.WriteLine("Command '" + command_main + "' not found");

                    return null;
                }), prompt, startupMsg, completionList);
        }

        private static Dictionary<string, Tuple<Action<string[]>, string>> lCommands = 
            new Dictionary<string, Tuple<Action<string[]>, string>>()
        {
            { "help",   new Tuple<Action<string[]>, string>(HelpFunc,       "test")},
            { "exit",   new Tuple<Action<string[]>, string>(Exit,           "test")},
            { "clear",  new Tuple<Action<string[]>, string>(Clear,          "test")},
            { "wallet", new Tuple<Action<string[]>, string>(CopyFunc,       "test")},
            { "tx",     new Tuple<Action<string[]>, string>(Transaction,    "test")},
            { "invoke", new Tuple<Action<string[]>, string>(InvokeFunc,          "test")}
        };

        private static void Transaction(string[] obj)
        {
            string txHash = string.Join("", obj);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(AccountController
                    .GetTxConfirmations(txHash).Result, Formatting.Indented);
            Console.WriteLine(json);
        }

        private static void InvokeFunc(string[] obj)
        {
            string contract = obj[0];
            string method = obj[1];
            KeyPair kp = GetLoginKey();
            var result = AccountController.InvokeContractGeneric(kp, contract, method).Result;
            if (result == null) {
                Console.WriteLine("Node returned null...");
                return;
            }
            byte[] decodedResult = Base16.Decode(result);

            VMObject output = Serialization.Unserialize<VMObject>(decodedResult);
            Console.WriteLine("Result: " + output.ToObject());

        }

        private static void Clear(string[] obj)
        {
            Console.Clear();
        }

        private static void Exit(string[] obj)
        {
            Environment.Exit(0);
        }

        private static void CopyFunc(string[] obj)
        {
            GetLoginKey();
        }

        private static KeyPair GetLoginKey(bool changeWallet=false)
        {
            if (keyPair == null && !changeWallet) 
            {
                Console.Write("Enter private key: ");
                var wif = Console.ReadLine();
                var kPair = KeyPair.FromWIF(wif);
                keyPair = kPair;
            }
            return keyPair;
        }


        public static void HelpFunc(string[] args)
        {
            Console.WriteLine("===== SOME MEANINGFULL HELP ==== ");
        }
    }
}
