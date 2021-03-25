# ConsoleRoutine

Projeto Console

Descrição: Esse projeto é o responsavel por essas atividades:
1. Acesse o método GetItemFila da api desenvolvida no Item anterior. Caso o método retorne um
objeto, obter todas as moedas e períodos correspondentes.
</br>
1.1. Para cada moeda/período retornada da api, acessar o arquivo DadosMoeda.csv (mesmo
diretório de execução) e trazer todas as moedas/datas que estejam dentro do período
(Inclusive) da moeda que está sendo tratada.
1.2. Com a lista de moedas/datas, buscar todos os valores de cotação (vlr_cotacao) no arquivo
DadosCotacao.csv utilizando o de-para descrito no item 4 (Tabela de de-para) para obter as
cotações.
1.3. Gerar um arquivo de resultado (apenas com as moedas/datas consultadas) com o nome
Resultado_aaaammdd_HHmmss.csv no mesmo formato do arquivo DadosMoeda.csv.
Porém com uma coluna a mais (VL_COTACAO) contendo o valor de cotação correspondente
(obtido do arquivo DadosCotacao.csv) para cada moeda/data consultada.
1.4. Encerrar o processamento e aguardar o próximo ciclo de verificação (2 minutos)
