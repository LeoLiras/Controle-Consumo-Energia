using System;
using System.Data;
using System.Security.Cryptography.X509Certificates;

public class Program
{
    //Recebe os dados e a data inicial, calcula os dias em que o consumo ficou acima ou abaixo da meta, armazena as datas das ocorrências, calcula a média de custo e consumo.
    public static void consumoEnergia(){

        //Kw/h
        //energia_piso é o consumo ideal, o teto é a meta de consumo total.
        double energia_teto = 350, energia_piso = 100, media_consumo_dia = 0, maior_consumo = -999, menor_consumo = 999;
        List<double> consumo = new List<double> ();

        //Auxiliares
        int contador = 0, contador_maior_consumo = 0, contador_menor_consumo = 0;
        int aux_input = 0;

        //R$
        double custo_kwh = 0.74, media_custo_dia = 0;
        int dias_acima = 0, dias_abaixo = 0, dias_na_media = 0;

        //Para receber a data inicial
        String input_data;
        
        //Recebe a data atual apenas para inicializar, a data inicial será inserida pelo usuário.
        DateTime data_atual = DateTime.Now;

        List <DateTime> datas_teto = new List <DateTime> ();
        List <DateTime> datas_piso = new List <DateTime> ();
        List <DateTime> datas_media = new List<DateTime> ();

        //Variáveis para armazenar as últimas datas em que o maior e o menor consumo foram registrados.
        DateTime data_maior_consumo = data_atual, data_menor_consumo = data_atual, data_aux = data_atual;

        Console.WriteLine("================Bem vindo!================");
        Console.WriteLine("Por favor, insira os valores, ou -1 para sair.");

        try{
            //Recebe a data de início do programa
            Console.WriteLine("Insira uma data (mês/dia/ano): ex. 11/23/2023");
            input_data = Console.ReadLine();
            data_atual = DateTime.Parse(input_data);
            data_aux = DateTime.Parse(input_data);
        }catch(System.FormatException e){
            Console.WriteLine("\nData inválida.");
            Console.WriteLine("Reinicinado...\n");
            Thread.Sleep(1000);
            consumoEnergia();
        }
        
        try{
            //Recebe os dados do usuário para 365 dias
            while(contador < 365 && aux_input != -1){
            
                //Recebe os dados
                Console.WriteLine("\nInsira o seu consumo em Kw/h neste dia. (" + (contador + 1) + " / 365)");
                aux_input = int.Parse(Console.ReadLine());

                if(aux_input == -1){
                    break;
                }else{
                    consumo.Add(aux_input);
                }
                
                //Verifica se o consumo do dia está acima ou abaixo da meta
                if(consumo[contador] > energia_teto){
                    datas_teto.Add(data_atual);
                    dias_acima++;
                }else if(consumo[contador] <= energia_piso){
                    datas_piso.Add(data_atual);
                    dias_abaixo++;
                }else{
                    datas_media.Add(data_atual);
                    dias_na_media++;
                }

                contador++;
                data_atual = data_atual.AddDays(1);
        }
            //Calcula quantas vezes o menor e maior valores foram inseridos.
            for(int i = 0; i < consumo.Count; i ++){
                if(consumo[i] == consumo.Max()){
                    contador_maior_consumo++;
                    data_maior_consumo = data_aux;
                }else if(consumo[i] == consumo.Min()){
                    contador_menor_consumo++;
                    data_menor_consumo = data_aux;
                }

                data_aux = data_aux.AddDays(1);
            }

            //Calcula as médias de custo e consumo diários
            media_consumo_dia = consumo.Sum() / consumo.Count;
            media_custo_dia = media_consumo_dia * custo_kwh;

            double estimativa_mes = estimativaMes(media_custo_dia);
            double estimativa_ano = estimativaAno(media_custo_dia);

            //Melhoria: Criar classe Relatório e usar esses parâmetros como atributos.
            relatorio(consumo, media_consumo_dia, media_custo_dia, maior_consumo, menor_consumo, contador_maior_consumo, contador_menor_consumo, data_maior_consumo, 
                        data_menor_consumo, datas_teto, datas_piso, datas_media, dias_acima, dias_abaixo, dias_na_media, estimativa_mes, estimativa_ano);

            }catch(System.FormatException e){
                Console.WriteLine("\nInsira apenas valores inteiros positivos ou -1 para sair.");
                Console.WriteLine("Reniciando o programa...\n");
                Thread.Sleep(1000);
                consumoEnergia();
            }
    }

    //Calcula a estimativa de custo mensal futuro com base nos dados inseridos;
    //Recebe a média_custo_dia, calculada na função consumoEnergia();
    public static double estimativaMes(double media_custo_dia){

        //Estimativa mensal obtida multiplicando-se a média diária pela média aritimética entre o número de dias possíveis em um mês (28, 30 e 31)
        double estimativa_custo_mes = media_custo_dia * 29.7;

        return estimativa_custo_mes;
    }

    //Calcula a estimativa de custo anual futuro com base nos dados inseridos;
    //Recebe a média_custo_dia, calculada na função consumoEnergia();
    public static double estimativaAno(double media_custo_dia){

        double estimativa_custo_ano = 0;

        //Multiplica o gasto diário conforme o número de dias do respectivo mês
        for(int i = 1; i <= 12; i++){
            if(i == 2){
                estimativa_custo_ano += (media_custo_dia * 28);
            }else if(i == 1 || i == 3 || i == 5 || i == 7 || i == 8 || i == 10 || i == 12){
                estimativa_custo_ano += (media_custo_dia * 31);
            }else if(i == 4 || i == 6 || i == 9 || i == 11){
                estimativa_custo_ano += (media_custo_dia * 30);
            }
        }

        return estimativa_custo_ano;
    }

    //Plota os resultados e estimativas no console.
    //Recebe todos os dados que serão plotados.
    public static void relatorio(List<double> consumo,double media_consumo_dia, double media_custo_dia, double maior, double menor, int contador_maior, 
                        int contador_menor, DateTime data_maior, DateTime data_menor, List<DateTime> datas_teto, List<DateTime> datas_piso, List<DateTime> datas_media
                        , int dias_acima, int dias_abaixo, int dias_na_media, double estimativa_mes, double estimativa_ano){
        
        //Plota relatório de gastos no console.
        Console.WriteLine("============================================================");
        Console.WriteLine("\n\nRelatório de Consumo:\n");
        Console.WriteLine("Média de Consumo diário: " + Math.Round(media_consumo_dia, 2) + " Kw/h");
        Console.WriteLine("Media custo diário: R$ " + Math.Round(media_custo_dia, 2));
        Console.WriteLine("-----------");

        Console.WriteLine("Maior consumo registrado: " + consumo.Max() + " Kw/h");
        string data_maior_formatada = data_maior.ToString("dd/MM/yyyy");
        Console.WriteLine("Data: " + data_maior_formatada);
        Console.WriteLine("Ocorrências: " + contador_maior + "\n");
        Console.WriteLine("Menor consumo registrado: " + consumo.Min() + " Kw/h");
        string data_menor_formatada = data_menor.ToString("dd/MM/yyyy");
        Console.WriteLine("Data: " + data_menor_formatada);
        Console.WriteLine("Ocorrências: " + contador_menor);

        Console.WriteLine("-----------");

        Console.WriteLine("Dias acima do teto: " + dias_acima);
        Console.Write("Datas: ");

        for(int i = 0; i < datas_teto.Count; i++){
            string data = datas_teto[i].ToString("dd/MM/yyyy");
            Console.Write(data + ", ");
        }
        Console.WriteLine("\n");

        Console.WriteLine("Dias abaixo do piso: " + dias_abaixo);
        Console.Write("Datas: ");

        for(int i = 0; i < datas_piso.Count; i++){
            string data = datas_piso[i].ToString("dd/MM/yyyy");
            Console.Write(data + ", ");
        }
        Console.WriteLine("\n");

        Console.WriteLine("Dias na média estimada: " + dias_na_media);
        Console.Write("Datas: ");

        for(int i = 0; i < datas_media.Count; i++){
            string data = datas_media[i].ToString("dd/MM/yyyy");
            Console.Write(data + ", ");
        }
        
        Console.WriteLine("\n-----------\n");

        Console.WriteLine("Estimativas de gastos futuros: \n");
        Console.WriteLine("Estimativa de gasto mensal: R$" + Math.Round(estimativa_mes, 2));
        Console.WriteLine("Estimativa de gasto anual: R$" + Math.Round(estimativa_ano, 2)); 
    }
	public static void Main()
	{
        //inicia o programa
		consumoEnergia();
	}
}