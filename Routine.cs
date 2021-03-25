using ConsoleRoutine.Models;
using System;
using System.Net.Http;
using RestSharp;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Text.Json;
using System.Data.OleDb;
using System.Linq;
using System.Threading;
using System.IO;
using System.Text;
using System.Reflection;

namespace ConsoleRoutine
{
    public class Routine
    {
        Logs logs = new Logs();
        public async Task<CoinModel> CallApiAsync()
        {
            try
            {

                Console.WriteLine("Configurando a requisição ....");
                RestSharp.RestClient client = new RestClient("http://localhost:44357/");
                // Request 
                RestSharp.RestRequest request = new RestSharp.RestRequest("api/GetItemFila", RestSharp.Method.GET, RestSharp.DataFormat.None);
                //request.AddJsonBody(mytest);
                Console.WriteLine("Solicitando a requisição ....");
                // Invoke
                RestSharp.IRestResponse response = await client.ExecuteAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("Solicitação realizada com sucesso !!");
                    Console.WriteLine("Deserelizando a string ....");
                    string unescapedJsonString = JsonConvert.DeserializeObject<string>(response.Content);
                    var deserializedResult = JsonConvert.DeserializeObject<CoinModel>(unescapedJsonString);
                    Console.WriteLine("Chamada finalizada !!");
                    logs.RegisterLog("Chamada finalizada !!");
                    return deserializedResult;
                }
                Console.WriteLine("Retorno da chamada foi nulo !!");
                logs.RegisterLog("Retorno da chamada foi nulo !!");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao solicita a requisição");
                logs.RegisterLog("Erro ao solicita a requisição");
                return null;
            }
        }
        public async Task<string> ValidationCoinQuotationAsync()
        {
            // Lista
            CoinModel coinModelReturn = CallApiAsync().Result;
            List<CurrencyData> listCurrency = new List<CurrencyData>();
            try
            {
                if (coinModelReturn != null)
                {
                    ListCurrencyData(coinModelReturn);
                }

                return "Finalizado";
            }
            catch
            {
                Console.WriteLine("Erro");
                return "Finalizado";
            }
        }
        public void ListCurrencyData(CoinModel coinModel)
        {
            var caminho = System.Environment.CurrentDirectory;
            StreamReader stream = new StreamReader(caminho+@"\Excel\DadosMoeda.csv");
            List<CurrencyData> listCurrencyData = new List<CurrencyData>();
            string linha = null;
            try
            {
                Console.WriteLine("Iniciando a leitura  do arquivo DadosMoeda.csv .....");
                while ((linha = stream.ReadLine()) != null)
                {
                    string[] linhaSeparada = linha.Split(';');
                    listCurrencyData.Add(new CurrencyData()
                    {
                        ID_MOEDA = linhaSeparada[0],
                        DATA_REF = Convert.ToDateTime(linhaSeparada[1])
                    });
                }
                if (listCurrencyData.Count() > 0)
                {
                    Console.WriteLine("Leitura finalizada, adicionado a lista !");
                    logs.RegisterLog("Leitura finalizada, adicionado a lista !");
                    listCurrencyData = listCurrencyData.Where(l => l.DATA_REF >= coinModel.data_inicio && l.DATA_REF <= coinModel.data_fim).ToList();

                    if (listCurrencyData.Count > 0)
                    {
                        ListFromToQuotation(listCurrencyData);
                    }
                    else
                    {
                        Console.WriteLine("Lista listCurrencyData nula !");
                        logs.RegisterLog("Lista listCurrencyData nula !");
                    }
                }
            }
            catch
            {
                Console.WriteLine("Erro");
            }

            finally
            {
                stream.Close();
            }
        }
        public void ListFromToQuotation(List<CurrencyData> listCoinModel)
        {
            var caminho = System.Environment.CurrentDirectory;
            StreamReader stream = new StreamReader(caminho + @"\Excel\DePara.csv");
            List<FromToQuotation> listFromToQuotation = new List<FromToQuotation>();
            List<FromToQuotation> listFromToQuotationFilter = new List<FromToQuotation>();
            string linha = null;
            try
            {
                Console.WriteLine("Iniciando a leitura  do arquivo DePara.csv .....");
                while ((linha = stream.ReadLine()) != null)
                {
                    string[] linhaSeparada = linha.Split(';');
                    listFromToQuotation.Add(new FromToQuotation()
                    {
                        ID_MOEDA = linhaSeparada[0],
                        cod_cotacao = linhaSeparada[1]
                    });
                }
                if (listFromToQuotation.Count() > 0)
                {
                    Console.WriteLine("Leitura do arquivo DePara.csv finalizada, adicionado a lista !");
                    logs.RegisterLog("Leitura do arquivo DePara.csv finalizada, adicionado a lista !");
                    foreach (var item in listCoinModel)
                    {
                        if (listFromToQuotation.Any(l => l.ID_MOEDA == item.ID_MOEDA))
                        {
                            FromToQuotation fromToQuotation = listFromToQuotation.Where(l => l.ID_MOEDA == item.ID_MOEDA).FirstOrDefault();
                            listFromToQuotationFilter.Add(new FromToQuotation()
                            {
                                ID_MOEDA = item.ID_MOEDA,
                                cod_cotacao = fromToQuotation.cod_cotacao
                            });
                        }
                    }
                    if (listFromToQuotationFilter.Count() > 0)
                    {
                        ListValueQuotation(listFromToQuotationFilter);
                    }
                    else
                    {
                        Console.WriteLine("Lista FromToQuatation nula !!");
                        logs.RegisterLog("Lista FromToQuatation nula !!");
                    }
                }
            }
            catch
            {
                Console.WriteLine("Erro");
            }
            finally
            {
                stream.Close();
            }
        }
        public void ListValueQuotation(List<FromToQuotation> fromToQuotation)
        {
            var caminho = System.Environment.CurrentDirectory;
            StreamReader stream = new StreamReader(caminho + @"\Excel\DadosCotacao.csv");
            List<QuotationData> listQuotationData = new List<QuotationData>();
            List<CurrencyQuotationResult> currencyQuotationResult = new List<CurrencyQuotationResult>();
            string linha = null;
            try
            {
                Console.WriteLine("Iniciando a leitura  do arquivo DadosCotacao.csv .....");
                while ((linha = stream.ReadLine()) != null)
                {
                    string[] linhaSeparada = linha.Split(';');
                    listQuotationData.Add(new QuotationData()
                    {
                        vlr_cotacao = linhaSeparada[0],
                        cod_cotacao = linhaSeparada[1],
                        dat_cotacao = Convert.ToDateTime(linhaSeparada[2]),
                    });
                }
                if (listQuotationData.Count() > 0)
                {
                    Console.WriteLine("Leitura do arquivo DadosCotacao.csv finalizada, adicionado a lista !");
                    logs.RegisterLog("Leitura do arquivo DadosCotacao.csv finalizada, adicionado a lista !");
                    foreach (var i in fromToQuotation)
                    {
                        if (listQuotationData.Any(l => l.cod_cotacao == i.cod_cotacao))
                        {
                            foreach (var x in listQuotationData.Where(l => l.cod_cotacao == i.cod_cotacao).ToList())
                            {
                                currencyQuotationResult.Add(new CurrencyQuotationResult()
                                {
                                    ID_MOEDA = i.ID_MOEDA,
                                    DATA_REF = x.dat_cotacao,
                                    VL_COTACAO = x.vlr_cotacao
                                });
                            }
                        }
                    }

                    if (currencyQuotationResult.Count() > 0)
                    {
                        saveResulQuotation(currencyQuotationResult);
                    }
                    else
                    {
                        Console.WriteLine("Lista currencyQuotationResult nula !!");
                        logs.RegisterLog("Lista currencyQuotationResult nula !!");
                    }
                }
            }
            catch
            {
                Console.WriteLine("Erro");
                Thread.Sleep(2000);
            }

            finally
            {
                stream.Close();
            }
        }
        public void saveResulQuotation(List<CurrencyQuotationResult> currencyQuotationResult)
        {
            var datenow = DateTime.Now;
            var caminho = System.Environment.CurrentDirectory;
            string strFilePath = caminho + @"\Resultado_" + datenow.Year + datenow.Month + datenow.Day + "_" + datenow.Hour + datenow.Minute + datenow.Second + ".csv";
            string strSeperator = ",";
            StringBuilder sbOutput = new StringBuilder();
            try
            {
                // Create and write the csv file
                Console.WriteLine("Gerando arquivo ....");
                File.WriteAllText(strFilePath, sbOutput.ToString());
                foreach (var item in currencyQuotationResult)
                {
                    sbOutput.AppendLine(string.Join(strSeperator, item.ID_MOEDA + ";" + item.DATA_REF + ";" + item.VL_COTACAO));
                }
                // To append more lines to the csv file
                File.AppendAllText(strFilePath, sbOutput.ToString());
                logs.RegisterLog("Arquivo CSV Gerado");
                Console.WriteLine("Arquivo CSV Gerado ");
                Console.ReadKey();
            }
            catch
            {
                logs.RegisterLog("Erro ao gerar arquivo !");
                Console.WriteLine("Erro ao gerar arquivo !");
            }
        }

    }
}
