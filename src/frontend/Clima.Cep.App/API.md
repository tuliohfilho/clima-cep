# 📡 Documentação da API - Frontend Clima CEP

## Visão Geral

Esta documentação descreve como o frontend React do Clima CEP se integra com o backend. A aplicação utiliza **Axios** para comunicação HTTP com a API RESTful do backend.

## Configuração da API

### URL Base

A URL base da API é configurada através da variável de ambiente `REACT_APP_API_URL`:

```env
REACT_APP_API_URL=http://localhost:5000/api
```

No Docker Compose, é configurada como:
```env
REACT_APP_API_URL=http://clima-cep-api:80/api
```

### Client HTTP

O arquivo `src/features/cep/services/cepService.js` configura o cliente Axios:

```javascript
import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';
```

## Endpoints da API

### 1. CEP Service

#### GET /api/cep/:cep
**Descrição:** Busca informações de localização por CEP

**Parâmetros:**
- `cep` (string): CEP sem formatação (8 dígitos)

**Exemplo de Requisição:**
```javascript
GET /api/cep/01310100
```

**Resposta Esperada (200):**
```json
{
  "cep": "01310100",
  "street": "Avenida Paulista",
  "neighborhood": "Bela Vista",
  "city": "São Paulo",
  "state": "SP",
  "latitude": -23.561414,
  "longitude": -46.656139
}
```

**Erros Possíveis:**
- 400: CEP inválido
- 404: CEP não encontrado

---

#### GET /api/weather?latitude={lat}&longitude={lon}
**Descrição:** Busca dados de clima para coordenadas específicas

**Parâmetros (Query String):**
- `latitude` (number): Latitude da localização
- `longitude` (number): Longitude da localização

**Exemplo de Requisição:**
```javascript
GET /api/weather?latitude=-23.561414&longitude=-46.656139
```

**Resposta Esperada (200):**
```json
{
  "temperature": 28.5,
  "feels_like": 27.8,
  "condition": "Partly Cloudy",
  "humidity": 65,
  "pressure": 1013,
  "wind_speed": 3.2,
  "clouds": 45
}
```

**Erros Possíveis:**
- 400: Parâmetros inválidos
- 503: Serviço de clima indisponível

---

#### GET /api/cep/:cep/weather
**Descrição:** Busca informações de localização E clima em uma única requisição

**Parâmetros:**
- `cep` (string): CEP sem formatação (8 dígitos)

**Exemplo de Requisição:**
```javascript
GET /api/cep/01310100/weather
```

**Resposta Esperada (200):**
```json
{
  "location": {
    "cep": "01310100",
    "street": "Avenida Paulista",
    "neighborhood": "Bela Vista",
    "city": "São Paulo",
    "state": "SP",
    "latitude": -23.561414,
    "longitude": -46.656139
  },
  "weather": {
    "temperature": 28.5,
    "feels_like": 27.8,
    "condition": "Partly Cloudy",
    "humidity": 65,
    "pressure": 1013,
    "wind_speed": 3.2,
    "clouds": 45
  }
}
```

**Erros Possíveis:**
- 400: CEP inválido
- 404: CEP não encontrado
- 503: Serviço de clima indisponível

---

### 2. History Service

#### GET /api/history
**Descrição:** Obtém o histórico de buscas

**Parâmetros:**
- Nenhum

**Exemplo de Requisição:**
```javascript
GET /api/history
```

**Resposta Esperada (200):**
```json
[
  {
    "id": "uuid-1",
    "cep": "01310100",
    "city": "São Paulo",
    "state": "SP",
    "timestamp": "2024-01-15T10:30:00Z",
    "location": { /* ... */ },
    "weather": { /* ... */ }
  },
  {
    "id": "uuid-2",
    "cep": "20040020",
    "city": "Rio de Janeiro",
    "state": "RJ",
    "timestamp": "2024-01-15T09:15:00Z",
    "location": { /* ... */ },
    "weather": { /* ... */ }
  }
]
```

**Erros Possíveis:**
- 500: Erro do servidor

---

#### POST /api/history
**Descrição:** Adiciona uma busca ao histórico

**Body (JSON):**
```json
{
  "cep": "01310100",
  "location": {
    "cep": "01310100",
    "street": "Avenida Paulista",
    "neighborhood": "Bela Vista",
    "city": "São Paulo",
    "state": "SP",
    "latitude": -23.561414,
    "longitude": -46.656139
  },
  "weather": {
    "temperature": 28.5,
    "feels_like": 27.8,
    "condition": "Partly Cloudy",
    "humidity": 65,
    "pressure": 1013,
    "wind_speed": 3.2,
    "clouds": 45
  }
}
```

**Resposta Esperada (201):**
```json
{
  "id": "uuid-novo",
  "cep": "01310100",
  "city": "São Paulo",
  "state": "SP",
  "timestamp": "2024-01-15T10:35:00Z",
  "location": { /* ... */ },
  "weather": { /* ... */ }
}
```

**Erros Possíveis:**
- 400: Dados inválidos
- 500: Erro do servidor

---

#### GET /api/history/:id
**Descrição:** Obtém detalhes de uma busca específica

**Parâmetros:**
- `id` (string): ID único do histórico

**Exemplo de Requisição:**
```javascript
GET /api/history/uuid-1
```

**Resposta Esperada (200):**
```json
{
  "id": "uuid-1",
  "cep": "01310100",
  "city": "São Paulo",
  "state": "SP",
  "timestamp": "2024-01-15T10:30:00Z",
  "location": {
    "cep": "01310100",
    "street": "Avenida Paulista",
    "neighborhood": "Bela Vista",
    "city": "São Paulo",
    "state": "SP",
    "latitude": -23.561414,
    "longitude": -46.656139
  },
  "weather": {
    "temperature": 28.5,
    "feels_like": 27.8,
    "condition": "Partly Cloudy",
    "humidity": 65,
    "pressure": 1013,
    "wind_speed": 3.2,
    "clouds": 45
  }
}
```

**Erros Possíveis:**
- 404: Histórico não encontrado
- 500: Erro do servidor

---

#### DELETE /api/history/:id
**Descrição:** Remove uma entrada do histórico

**Parâmetros:**
- `id` (string): ID único do histórico

**Exemplo de Requisição:**
```javascript
DELETE /api/history/uuid-1
```

**Resposta Esperada (204):**
```
Sem conteúdo
```

**Erros Possíveis:**
- 404: Histórico não encontrado
- 500: Erro do servidor

---

#### DELETE /api/history
**Descrição:** Limpa todo o histórico

**Parâmetros:**
- Nenhum

**Exemplo de Requisição:**
```javascript
DELETE /api/history
```

**Resposta Esperada (204):**
```
Sem conteúdo
```

**Erros Possíveis:**
- 500: Erro do servidor

---

## Exemplo de Uso no Frontend

### Buscar CEP com Clima

```javascript
import cepService from './services/cepService';

// No componente ou hook
const handleSearch = async (cep) => {
  try {
    const data = await cepService.searchCepWithWeather(cep);
    console.log('Dados:', data);
    // data.location e data.weather
  } catch (error) {
    console.error('Erro:', error.message);
  }
};
```

### Carregar Histórico

```javascript
import historyService from './services/historyService';

// No hook
useEffect(() => {
  const loadHistory = async () => {
    try {
      const data = await historyService.getHistory();
      setHistoryData(data);
    } catch (error) {
      console.error('Erro:', error.message);
    }
  };
  
  loadHistory();
}, []);
```

### Tratamento de Erros

```javascript
try {
  const data = await cepService.searchCepWithWeather('01310100');
} catch (error) {
  if (error.response) {
    // Erro do servidor (4xx, 5xx)
    console.error('Status:', error.response.status);
    console.error('Mensagem:', error.response.data.message);
  } else if (error.request) {
    // Requisição feita mas sem resposta
    console.error('Sem resposta do servidor');
  } else {
    // Erro na configuração
    console.error('Erro:', error.message);
  }
}
```

## Headers Esperados

### Request
```
Content-Type: application/json
Accept: application/json
```

### Response
```
Content-Type: application/json
```

## Códigos de Status HTTP

| Código | Significado |
|--------|-------------|
| 200 | OK - Requisição bem-sucedida |
| 201 | Created - Recurso criado |
| 204 | No Content - Sucesso sem retorno |
| 400 | Bad Request - Dados inválidos |
| 404 | Not Found - Recurso não encontrado |
| 500 | Internal Server Error - Erro do servidor |
| 503 | Service Unavailable - Serviço indisponível |

## CORS

A aplicação frontend rodando em `http://localhost:3000` necessita que o backend tenha CORS configurado para aceitar requisições de:

```
http://localhost:3000
http://localhost:*
```

## Timeout

O Axios está configurado com timeout padrão. Para requisições longas, o backend deve responder dentro de um tempo razoável (recomendado: 30 segundos).

## Rate Limiting

Se o backend implementar rate limiting, a aplicação tratará corretamente os erros 429 (Too Many Requests).

## Autenticação

Atualmente, a API não requer autenticação. Se implementada no futuro:
- Bearer Token no header Authorization
- Cookie-based sessions
- OAuth2

## Logging

Para debugging, ative logs no console:

```javascript
// No arquivo de serviço
if (process.env.NODE_ENV === 'development') {
  console.log('Requisição:', config);
}
```

## Versionamento da API

A URL base é `/api` (v1 implícita). Para versões futuras:
```
/api/v2/cep/:cep
/api/v3/history
```

---

**Última atualização:** Janeiro 2024
