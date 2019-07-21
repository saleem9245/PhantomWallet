using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Phantasma.Blockchain;
using Phantasma.Cryptography;
using Phantasma.RpcClient.DTOs;
using Phantasma.Numerics;
using Phantasma.VM;
using Phantasma.Core.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Phantom.Wallet.Helpers
{
    public static class SendUtils
    {
        public static List<ChainDto> SelectShortestPath(string from, string to, List<string> paths, List<ChainDto> phantasmaChains)
        {
            var finalPath = "";
            foreach (var path in paths)
            {
                if (path.IndexOf(from, StringComparison.Ordinal) != -1 && path.IndexOf(to, StringComparison.Ordinal) != -1)
                {
                    if (finalPath == "")
                    {
                        finalPath = path;
                    }
                    else if (path.Count(d => d == ',') < finalPath.Count(d => d == ','))
                    {
                        finalPath = path;
                    }
                }
            }
            var listStrLineElements = finalPath.Split(',').ToList();
            List<ChainDto> chainPath = new List<ChainDto>();
            foreach (var element in listStrLineElements)
            {
                chainPath.Add(phantasmaChains.Find(p => p.Name == element.Trim()));
            }
            return chainPath;
        }

        public static bool IsTxHashValid(string data)
        {
            return Hash.TryParse(data, out Hash result);
        }

        public static T ParseEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }

        public static List<object> BuildParamList(string parameters)
        {
            JObject jsonparam = JsonConvert.DeserializeObject<JObject>(parameters);
            List<object> paramList = new List<object>();

            foreach (var param in jsonparam)
            {
                foreach (var param2 in param.Value)
                {
                    string name = (string) param2["name"];
                    string type = (string) param2["type"];
                    string vmtype = (string) param2["vmtype"];
                    string input = (string) param2["input"];
                    string info = (string) param2["info"];

                    VMObject result = new VMObject();

                    switch (vmtype)
                    {
                        case "Object":
                            // for now, we assume every Object is an address
                            // complex object creation will follow new ABI
                            result.SetValue(Address.FromText(input));
                            break;
                        case "Number":
                            BigInteger bigInt = new BigInteger();
                            bool res = BigInteger.TryParse(input, out bigInt);
                            if (!res) 
                            {
                                throw new Exception($"Could not parse {input} to BigInteger");
                            } else
                            {
                                result.SetValue(bigInt);
                            }
                            break;
                        case "Timestamp":
                            DateTime date = DateTime.ParseExact(input, "MM/dd/yyyy HH:mm:ss", 
                                    System.Globalization.CultureInfo.InvariantCulture);
                            result.SetValue(date);
                            break;
                        case "Bool":
                            result.SetValue(bool.Parse(input));
                            break;
                        case "String":
                            result.SetValue(input);
                            break;
                        case "Bytes":
                            result.SetValue(Encoding.UTF8.GetBytes(input), VMType.Bytes);
                            break;
                        case "Enum":
                            uint enumNumber = Convert.ToUInt32(input);
                            byte[] barr = BitConverter.GetBytes(enumNumber);
                            result.SetValue(barr, VMType.Enum);
                            break;
                        default:
                            throw new Exception($"invalid vmtype: {vmtype} for {input}");
                    }

                    if (result != null)
                    {
                        paramList.Add(result);
                    } 
                    else 
                    {
                        Console.WriteLine($"Could not create parameter from object: Object[name: {name} type: {type} vmtype: {vmtype} input: {input} info: {info} ]");
                    }
                }
            }

            return paramList;
        }

        public static List<ChainDto> GetShortestPath(string from, string to, List<ChainDto> phantasmaChains)
        {
            var vertices = new List<string>();
            var edges = new List<Tuple<string, string>>();

            var children = new Dictionary<string, List<ChainDto>>();
            foreach (var chain in phantasmaChains)
            {
                var childs = phantasmaChains.Where(p => p.ParentAddress.Equals(chain.Address));
                if (childs.Any())
                {
                    children[chain.Name] = childs.ToList();
                }
            }

            foreach (var chain in phantasmaChains)
            {
                vertices.Add(chain.Name);
                if (children.ContainsKey(chain.Name))
                {
                    foreach (var child in children[chain.Name])
                    {
                        edges.Add(new Tuple<string, string>(chain.Name, child.Name));
                    }
                }
            }
            var graph = new Graph<string>(vertices, edges);

            var shortestPath = Algorithms.ShortestPathFunction(graph, from);

            List<string> allpaths = new List<string>();
            foreach (var vertex in vertices)
            {
                allpaths.Add(string.Join(", ", shortestPath(vertex)));
            }

            foreach (var allpath in allpaths)
            {
                Debug.WriteLine(allpath);
            }

            return SelectShortestPath(from, to, allpaths, phantasmaChains);
        }
    }
}
