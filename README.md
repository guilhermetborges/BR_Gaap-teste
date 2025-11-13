Projeto – API ASP.NET Core + Frontend SAPUI5

Aplicação Full Stack desenvolvida como desafio técnico, contendo:

Backend: ASP.NET Core + Entity Framework Core + SQLite

Frontend: SAPUI5

Testes: xUnit

Arquitetura do Projeto

```

BR_Gaap-teste/
├── BRGBackend/                         # API ASP.NET Core
│   ├── Controllers/
│   ├── Data/
│   ├── Migrations/
│   ├── Models/
│   ├── appsettings.json
│   ├── BRGBackend.csproj
│   └── Program.cs
│
├── BRGBackend.Tests/                   # Testes de integração (xUnit)
│   ├── TodosControllerTests.cs
│   └── BRGBackend.Tests.csproj
│
└── frontend/                           # Aplicação SAPUI5
    └── webapp/
        ├── view/
        ├── controller/
        ├── model/
        ├── css/
        ├── manifest.json
        ├── Component.js
        └── index.html
```
        


-------------------------------------
 Como Rodar o Backend
 1️⃣ Restaurar dependências
dotnet restore

 2️⃣ Criar o banco via Entity Framework
dotnet ef database update

 3️⃣ Rodar a API
cd backend

dotnet run 

📄 Swagger

Acesse a documentação da API em:

 http://localhost:5241/swagger/index.html

---------------------------------------------

🧪 Como Rodar os Testes


1️⃣Entrar no projeto de testes:
- cd BRGBackend.Tests


2️⃣ Rodar os testes:
dotnet test


------------------------------------------------
🌐 Como Rodar o Frontend SAPUI5
 1️⃣ Instalar UI5 CLI (se ainda não tiver)
npm install --global @ui5/cli

 2️⃣ Entrar na pasta do frontend
cd frontend

 3️⃣ Rodar o servidor SAPUI5
npx ui5 serve --open index.html
(**para funcionar o backend tambem tem que estar rodando_

 Endpoint do frontend
http://localhost:8080/index.html#

---------------------------------------------

 Endpoints da API
🔹 GET /todos

Lista tarefas com paginação, filtro e ordenação.


Parâmetros
Param	    Tipo	  Descrição
page	    int  	  Número da página
pageSize	int	  Itens por página
title	  string	  Filtro por título
sort	  string	  Campo de ordenação


Exemplo
GET http://localhost:5241/todos?page=1&pageSize=10&title=test&sort=title&order=asc


🔹 GET /todos/{id}

Retorna detalhes de uma tarefa.

GET http://localhost:5241/todos/1

🔹 PUT /todos/{id}

Atualiza o campo completed.

Body
{
  "completed": true
}


Exemplo
PUT http://localhost:5241/todos/10

curl -X PUT "http://localhost:5241/todos/10" ^
     -H "Content-Type: application/json" ^
     -d "{\"completed\": false}"

🔹 POST /sync

Importa tarefas de:

https://jsonplaceholder.typicode.com/todos


E salva no SQLite.


------------------------------------------

 Regra de Negócio

Cada usuário (userId) só pode ter até 5 tarefas incompletas.
Se tentar marcar mais uma como incompleta:

HTTP 400
{
  "message": "O usuário já possui 5 tarefas incompletas."
}


📄 Exemplos de Requests
✔ Listar tarefas (com filtros)
GET http://localhost:5241/todos?page=2&pageSize=10&title=qui&sort=userId&order=desc

✔ Obter uma tarefa
GET http://localhost:5241/todos/15

✔ Atualizar conclusão
PUT http://localhost:5241/todos/3


Body:

{
  "completed": false
}


 Sincronizar dados externos
POST http://localhost:5241/sync

----------------------------------------------

 Scripts Utilizados no Projeto
▶ Backend
Ação	Script
Restaurar dependências	dotnet restore
Criar banco EF	dotnet ef database update
acessar pasta  cd backend 
Rodar API  |	dotnet run 
Rodar Swagger	http://localhost:5241/swagger/index.html

🌐 Frontend
Instalar UI5	npm install -g @ui5/cli
Acessar pasta	cd frontend
Rodar servidor	npx ui5 serve --open index.html
Testar frontend	http://localhost:8080/index.html#
🧪 Testes
Ação	Script
Entrar no projeto	cd BRGBackend.Tests
Rodar testes	dotnet test
