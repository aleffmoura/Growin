#### Primeiro lugar quero agradecer pela oportunidade de participar do processo seletivo, foi um desafio muito interessante.

## step-by-step

* abrir terminal para execução dos comandos
* executar: 'git clone https://github.com/aleffmoura/Growin.git'
* para execução do backend, entrar na pasta 'Growin\Backend'
* modificar as variaveis de ambiente em 'docker-compose.yml'
* a variavel: 'Workers__OrderStatusCancelWorker__NextIteratorInSeconds: ' representa a iteração do worker, caso queira deixar o ciclo de execção do worker maior ou menor, aconselhado deixar pelo menos metade do valor da variavel abaixo, para manter um ciclo dentro da validade de uma encomenda
* a variavel: 'Workers__OrderStatusCancelWorker__CancellOrderInSeconds: ' representa a partir de quantos segundos a encomenda pode ser cancelada, ou seja uma encomenda criada a 'x segundos atras'
*  e executar o comando 'docker compose up' para subir todas as dependencias.
*  sera criado o banco ja com um produto de 100 adicionado, sera criado tambem um pgadmin, caso queira visualizar os dados no banco e será implantada a api e o worker.
* para vizualizar o swagger o endpoint está em: https://localhost:7113/swagger/index.html

com isso todo o backend será criado e pronto para uso.

* frontend está em 'Growin\Frontend\growin-app'
* entrar na pasta via terminal e executar os comandos:
* npm install
* npm run dev
* front vai esta em: http://localhost:3000/

de resto acredito que todos os criterios de aceitaçáo foram atendidos.

### nota:
Como não tive muito tempo o frontend deixou um pouco a desejar, mas acredito que sirva a seu proposito, peço que deem uma olhada no backend que é mais minha especialidade, mas estou sempre disposto a evoluir.
Pensei em fazer algo real time para atualizar os produtos e encomendas no frontend, mas acredito que com oque fiz no projeto consigo passar meu conhecimento.

Até a proxima.