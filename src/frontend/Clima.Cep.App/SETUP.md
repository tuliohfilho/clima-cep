# 🚀 Guia de Setup - Clima CEP Frontend

## Estrutura do Projeto

O projeto foi estruturado seguindo um padrão **features-based architecture**, onde cada feature encapsula seus próprios:
- Components
- Hooks (lógica de negócio + state management)
- Services (comunicação com API)
- Pages (telas principais)

```
src/
├── features/
│   ├── cep/                    # Feature de busca por CEP
│   │   ├── components/         # Componentes reutilizáveis
│   │   │   ├── CepSearchForm.jsx
│   │   │   ├── CepSearchForm.css
│   │   │   ├── CepResult.jsx
│   │   │   └── CepResult.css
│   │   ├── hooks/              # Custom hooks
│   │   │   └── useCep.js       # Lógica de busca + estado
│   │   ├── services/           # API integration
│   │   │   └── cepService.js   # Chamadas HTTP
│   │   └── pages/              # Páginas principais
│   │       ├── CepPage.jsx
│   │       └── CepPage.css
│   │
│   └── history/                # Feature de histórico
│       ├── components/         # Componentes reutilizáveis
│       │   ├── HistoryList.jsx
│       │   ├── HistoryList.css
│       │   ├── HistoryDetail.jsx
│       │   └── HistoryDetail.css
│       ├── hooks/              # Custom hooks
│       │   └── useHistory.js   # Lógica de histórico + estado
│       ├── services/           # API integration
│       │   └── historyService.js # Chamadas HTTP
│       └── pages/              # Páginas principais
│           ├── HistoryPage.jsx
│           └── HistoryPage.css
│
├── shared/                     # Componentes compartilhados
│   ├── components/
│   └── utils/
│
├── App.jsx                     # Componente raiz com rotas
├── App.css                     # Estilos globais
└── index.jsx                   # Ponto de entrada

public/
├── index.html                  # HTML principal
└── favicon.ico
```

## 🎯 Características Implementadas

### ✅ Módulo CEP
- ✅ Busca de CEP com validação
- ✅ Exibição de informações de localização
- ✅ Integração com clima em tempo real
- ✅ Formatação automática (XXXXX-XXX)
- ✅ Histórico local no localStorage
- ✅ Tratamento de erros

### ✅ Módulo Histórico
- ✅ Listagem de buscas anteriores
- ✅ Visualização detalhada de cada busca
- ✅ Remoção de itens do histórico
- ✅ Limpeza completa do histórico
- ✅ Sincronização com backend
- ✅ Interface responsiva

## 🔄 Fluxo de Dados

```
Usuario Input
    ↓
CepSearchForm Component
    ↓
useCep Hook (lógica + estado)
    ↓
cepService (chamada à API)
    ↓
API Backend
    ↓
Resposta com dados
    ↓
CepResult Component (exibição)
    ↓
localStorage (persistência)
```

## 🚀 Como Usar

### Desenvolvimento Local

```bash
# 1. Instalar dependências
npm install

# 2. Copiar arquivo de ambiente
cp .env.example .env

# 3. Editar .env com a URL da API
REACT_APP_API_URL=http://localhost:5000/api

# 4. Iniciar desenvolvimento
npm start
```

### Com Docker

```bash
# Build
docker build -t clima-cep-app .

# Run
docker run -p 3000:3000 -e REACT_APP_API_URL=http://localhost:5000/api clima-cep-app
```

### Com Docker Compose (Recomendado)

```bash
# A partir da raiz do projeto
docker-compose up -d

# Acessar em http://localhost:3000
```

## 📦 Dependências Principais

```json
{
  "react": "^18.2.0",           // Framework frontend
  "react-dom": "^18.2.0",       // React DOM renderer
  "react-router-dom": "^6.20.0",// Roteamento
  "axios": "^1.6.2",            // Cliente HTTP
  "zustand": "^4.4.1"           // State management (opcional)
}
```

## 🔌 Endpoints da API Esperados

A aplicação espera os seguintes endpoints do backend:

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/cep/:cep` | Buscar localização por CEP |
| GET | `/api/weather` | Buscar clima (lat, lon) |
| GET | `/api/cep/:cep/weather` | Buscar CEP + clima |
| GET | `/api/history` | Obter histórico |
| POST | `/api/history` | Adicionar ao histórico |
| GET | `/api/history/:id` | Detalhes do histórico |
| DELETE | `/api/history/:id` | Remover do histórico |
| DELETE | `/api/history` | Limpar histórico |

## 🎨 Paleta de Cores

- **Primário**: `#667eea` (Roxo)
- **Secundário**: `#764ba2` (Roxo escuro)
- **Sucesso**: Verde
- **Erro**: `#c33` (Vermelho)
- **Fundo**: `#f5f5f5` (Cinza claro)
- **Texto**: `#333` (Cinza escuro)

## 📱 Responsividade

A aplicação é responsiva para:
- Desktop: 1920px+
- Tablet: 768px - 1024px
- Mobile: < 768px

## 🧪 Scripts Disponíveis

```bash
# Iniciar desenvolvimento
npm start

# Build para produção
npm run build

# Rodar testes
npm test

# Eject (cuidado!)
npm run eject
```

## 📋 Boas Práticas Implementadas

✅ **Separação de Responsabilidades**
- Services para API
- Hooks para lógica
- Components para UI

✅ **State Management**
- Hooks customizados
- localStorage para persistência
- Zustand como opção

✅ **Performance**
- Code splitting automático
- Lazy loading de rotas
- Memoização onde necessário

✅ **Segurança**
- Validação de input
- Tratamento de erros
- Sanitização de dados

✅ **UX/UI**
- Formulários intuitivos
- Feedback visual
- Carregamento com spinner
- Mensagens de erro claras

✅ **Responsividade**
- Mobile-first approach
- Grid layout flexível
- Media queries

## 🔒 Variáveis de Ambiente

```env
# URL da API backend
REACT_APP_API_URL=http://localhost:5000/api

# Ambiente
NODE_ENV=development|production
```

## 💡 Dicas de Desenvolvimento

1. **Adicionar novo serviço**
   ```javascript
   // src/features/novo/services/novoService.js
   const novoService = {
     getData: async () => { /* ... */ }
   };
   ```

2. **Criar novo hook**
   ```javascript
   // src/features/novo/hooks/useNovo.js
   const useNovo = () => {
     const [data, setData] = useState(null);
     // ...
     return { data };
   };
   ```

3. **Adicionar novo componente**
   ```javascript
   // src/features/novo/components/NovoComponent.jsx
   const NovoComponent = ({ prop }) => {
     return <div>{prop}</div>;
   };
   ```

## 🐛 Troubleshooting

| Problema | Solução |
|----------|---------|
| Porta 3000 em uso | Mudar porta: `PORT=3001 npm start` |
| CORS errors | Verificar URL da API em .env |
| Módulos não encontrados | Rodar `npm install` |
| Build muito lento | Verificar node_modules, fazer `npm ci` |

## 📚 Recursos Úteis

- [React Docs](https://react.dev)
- [React Router](https://reactrouter.com)
- [Axios](https://axios-http.com)
- [Zustand](https://github.com/pmndrs/zustand)

## ✅ Checklist de Deploy

- [ ] Variáveis de ambiente configuradas
- [ ] `.env` não versionado
- [ ] Build sem erros: `npm run build`
- [ ] Docker image built com sucesso
- [ ] API endpoint correto
- [ ] CORS configurado no backend
- [ ] Testes executados
- [ ] Performance checada (Lighthouse)

---

**Desenvolvido com ❤️ para Clima CEP**
