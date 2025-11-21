# Sistema de Consulta de CEP e Clima

## Visão Geral do Projeto

Este projeto consiste em uma aplicação *full-stack* desenvolvida para consultar informações de **CEP** e **Clima**, utilizando APIs públicas. A solução é composta por um **Backend** em **.NET 8** (C#) e um **Frontend** em **React** (TypeScript), ambos containerizados com **Docker** e orquestrados via **Docker Compose**.

O objetivo principal é demonstrar a integração de serviços externos com foco em **resiliência**, **boas práticas de desenvolvimento** (como DDD e Problem Details - RFC 7807), **testes automatizados** e **experiência do usuário (UX)**.

## Arquitetura da Solução

A solução é dividida em dois serviços principais:

| Serviço | Tecnologia | Descrição |
| :--- | :--- | :--- |
| **Backend (API)** | .NET 8 (C#) | API RESTful responsável por: 1. Consultar CEP com fallback (BrasilAPI e ViaCEP). 2. Persistir CEPs consultados em banco de dados em memória. 3. Consultar clima e previsão (Open-Meteo) baseado nos CEPs salvos. |
| **Frontend (Web)** | React 19+ (TypeScript) | Aplicação Single Page Application (SPA) que fornece a interface para o usuário: 1. Formulário de consulta de CEP. 2. Exibição dos dados de endereço e clima. 3. Gerenciamento de estado de servidor (TanStack Query) e formulários (React Hook Form). |

## Funcionalidades Principais

### Backend (.NET 8)

*   **Consulta de CEP com Fallback:** Utiliza a BrasilAPI como provedor primário e ViaCEP como fallback em caso de falha ou indisponibilidade.
*   **Persistência em Memória:** Armazena os CEPs consultados em um banco de dados em memória (SQLite ou MongoDB in-memory) para uso posterior na consulta de clima.
*   **Consulta de Clima:** Utiliza as coordenadas (Latitude/Longitude) dos CEPs salvos para consultar o clima atual e a previsão (1 a 7 dias) via Open-Meteo.
*   **Resiliência:** Implementação de políticas de **Retry** e **Circuit Breaker** (usando `Microsoft.Extensions.Http.Resilience` ou Polly) para chamadas a APIs externas.
*   **Tratamento de Erros Consistente:** Todos os erros são retornados no formato **Problem Details (RFC 7807)**.
*   **Observabilidade:** Logs estruturados e correlação de requisições.
*   **Documentação:** Swagger UI disponível para documentação da API.

### Frontend (React)

*   **Interface Amigável:** Formulário de consulta de CEP com validação em tempo real.
*   **Integração de Clima:** Exibição do clima atual e previsão após a consulta de CEP.
*   **Responsividade:** Layout adaptável para dispositivos móveis, tablets e desktops.
*   **Gerenciamento de Estado:** Uso de **TanStack Query** para cache e gerenciamento de estado de servidor.
*   **Histórico:** Persistência das últimas consultas de CEP no `localStorage`.

## Pré-requisitos

Para executar a aplicação localmente utilizando a containerização, você precisará ter instalado:

1.  **[Docker](https://docs.docker.com/get-docker/)**: Para construir e executar os containers.
2.  **[Docker Compose](https://docs.docker.com/compose/install/)**: Para orquestrar os serviços de Backend e Frontend.

## Como Executar a Aplicação (Docker Compose)

A maneira mais rápida e recomendada de iniciar a aplicação é utilizando o `docker-compose`.

### 1. Estrutura de Diretórios e Localização dos Dockerfiles

O projeto segue a seguinte estrutura de diretórios, com o `docker-compose.yml` na raiz e os Dockerfiles dentro de seus respectivos projetos:

```
.
├── docker-compose.yml  <-- Arquivo de orquestração
└── src/
    ├── backend/
    │   └── Clima.Cep.Api/
    │       └── Dockerfile  <-- Dockerfile do Backend (.NET 8)
    └── frontend/
        └── Clima.Cep.App/
            └── Dockerfile  <-- Dockerfile do Frontend (React)
```

### 2. Iniciar os Serviços

Navegue até o diretório raiz do projeto (onde o arquivo `docker-compose.yml` está localizado) e execute o comando abaixo.

O comando `--build` é essencial na primeira execução ou após qualquer alteração no código-fonte, pois ele garante que as imagens Docker mais recentes sejam construídas antes de iniciar os containers.

```bash
docker-compose up --build
```

| Serviço | Porta Exposta | URL de Acesso |
| :--- | :--- | :--- |
| **Frontend (React)** | `3000` (ou outra configurada) | `http://localhost:3000` |
| **Backend (API .NET)** | `5137` | `http://localhost:5137` |

### 3. Acessar a Aplicação

Após a inicialização bem-sucedida (o que pode levar alguns minutos na primeira vez devido ao download das imagens base e ao build), abra seu navegador e acesse:

[http://localhost:3000](http://localhost:3000)

### 4. Parar e Limpar

Para parar a execução dos containers, pressione `Ctrl + C` no terminal onde o `docker-compose up` está rodando.

Para remover os containers, as redes e os volumes criados pelo `docker-compose`, execute:

```bash
docker-compose down --remove-orphans
```

## Detalhes da Containerização

### Configuração do Docker Compose (`docker-compose.yml`)

O arquivo `docker-compose.yml` orquestra a construção e a execução dos dois serviços, definindo as dependências e o mapeamento de portas.

**Localização:** Raiz do Projeto (`docker-compose.yml`)

```yaml
version: '3.8'

services:
  clima-cep-api:
    build:
      context: .
      dockerfile: src/backend/Clima.Cep.Api/Dockerfile
    container_name: clima-cep-api
    ports:
      - "5137:80"
    environment:
      - ASPNETCORE_URLS=http://+:80
    networks:
      - climacepnetwork

  clima-cep-app:
    build:
      context: ./src/frontend/Clima.Cep.App
      dockerfile: Dockerfile
    container_name: clima-cep-app
    ports:
      - "3000:3000"
    environment:
      - REACT_APP_API_URL=http://clima-cep-api:80/api
      - NODE_ENV=production
    networks:
      - climacepnetwork
    depends_on:
      - clima-cep-api

networks:
  climacepnetwork:
    driver: bridge
```

> **Nota:** O serviço de Backend (`clima-cep-api`) expõe a porta `80` internamente, que é mapeada para a porta `5137` do seu host.

### Dockerfile do Backend (`src/backend/Clima.Cep.Api/Dockerfile`)

O Dockerfile do Backend utiliza uma abordagem *multi-stage* para garantir que a imagem final de produção seja o menor possível, contendo apenas os binários necessários e o runtime do .NET.

**Localização:** `src/backend/Clima.Cep.Api/Dockerfile`

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /build

COPY ./src/backend/. ./src

RUN dotnet restore src/Clima.Cep.Api/Clima.Cep.Api.csproj
RUN dotnet publish src/Clima.Cep.Api/Clima.Cep.Api.csproj -c Release -o src/Clima.Cep.Api/dist

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build-env /build/src/Clima.Cep.Api/dist ./

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://*:80
EXPOSE 80

ENTRYPOINT ["dotnet", "Clima.Cep.Api.dll"]
```

### Dockerfile do Frontend (React) - Sugestão de Correção

O rascunho fornecido é baseado em Angular. Abaixo está uma **sugestão de correção** para um Dockerfile de React, mantendo a estrutura *multi-stage* e a utilização do Nginx para servir os arquivos estáticos, assumindo que o comando de build do React gera os arquivos na pasta `build` ou `dist`.

**Localização:** `src/frontend/Clima.cep.App/Dockerfile`

```dockerfile
FROM node:18-alpine AS builder

WORKDIR /app

COPY package.json package-lock.json* yarn.lock* ./

RUN npm ci || npm install

COPY . .

RUN npm run build

FROM node:18-alpine

WORKDIR /app

RUN npm install -g serve

COPY --from=builder /app/build ./build

EXPOSE 3000

ENV REACT_APP_API_URL=http://localhost:5000/api

CMD ["serve", "-s", "build", "-l", "3000"]
```

## Testes da API (Backend)

Você pode testar os endpoints da API diretamente, mesmo sem o Frontend, utilizando ferramentas como `curl` ou Postman.

### 1. Consulta de CEP (GET)

Consulta um CEP e retorna o endereço, utilizando o fallback se necessário.

**Endpoint:** `GET /cep/{cep}`

```bash
# Exemplo de uso:
curl -s http://localhost:5137/cep/01001000 | jq .
```

### 2. Persistir CEP (POST)

Consulta um CEP e o salva no banco de dados em memória.

**Endpoint:** `POST /cep`

```bash
# Exemplo de uso:
curl -s -X POST http://localhost:5137/cep \
-H "Content-Type: application/json" \
-d '{"zipCode":"01001000"}' | jq .
```

### 3. Consulta de Clima (GET)

Consulta o clima e a previsão (padrão 3 dias) para todos os CEPs salvos.

**Endpoint:** `GET /weather?days={dias}`

```bash
# Exemplo de uso:
curl -s "http://localhost:5137/weather?days=5" | jq .
```

## Tecnologias e Padrões Adotados

| Camada | Tecnologia/Padrão | Detalhe |
| :--- | :--- | :--- |
| **Backend** | .NET 8 (C#) | Framework moderno e de alto desempenho. |
| **Backend** | Problem Details (RFC 7807) | Padronização de mensagens de erro. |
| **Backend** | Resiliência (Polly/Resilience) | Políticas de Retry e Circuit Breaker. |
| **Frontend** | React 19+ (TypeScript) | Biblioteca moderna para construção de interfaces. |
| **Frontend** | TanStack Query | Gerenciamento de estado de servidor e cache. |
| **Frontend** | React Hook Form + Zod | Gerenciamento e validação de formulários. |
| **Geral** | Docker Multi-Stage Build | Otimização do tamanho das imagens de produção. |
| **Geral** | Docker Compose | Orquestração e execução simplificada dos serviços. |
