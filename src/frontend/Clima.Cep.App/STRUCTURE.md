# 🏗️ Estrutura Completa do Projeto React - Clima CEP

## 📂 Hierarquia de Diretórios

```
src/frontend/Clima.Cep.App/
│
├── 📄 package.json               ← Dependências e scripts do projeto
├── 📄 Dockerfile                 ← Imagem Docker para produção
├── 📄 .dockerignore              ← Arquivos ignorados no Docker
├── 📄 .gitignore                 ← Arquivos ignorados no Git
├── 📄 .env.example               ← Variáveis de ambiente (exemplo)
├── 📄 .eslintrc.json             ← Configuração ESLint
├── 📄 .prettierrc                ← Configuração Prettier
│
├── 📖 README.md                  ← Documentação principal
├── 📖 SETUP.md                   ← Guia de setup e desenvolvimento
├── 📖 API.md                     ← Documentação de endpoints da API
│
├── 📁 public/
│   └── 📄 index.html             ← HTML principal da aplicação
│
└── 📁 src/
    │
    ├── 📄 index.jsx              ← Ponto de entrada da aplicação
    ├── 📄 App.jsx                ← Componente raiz com rotas
    ├── 📄 App.css                ← Estilos globais da aplicação
    │
    ├── 📁 features/              ← Feature-based modules
    │   │
    │   ├── 📁 cep/               ⭐ MÓDULO CEP
    │   │   ├── 📁 components/    ← Componentes reutilizáveis
    │   │   │   ├── 📄 CepSearchForm.jsx
    │   │   │   ├── 📄 CepSearchForm.css
    │   │   │   ├── 📄 CepResult.jsx
    │   │   │   └── 📄 CepResult.css
    │   │   │
    │   │   ├── 📁 hooks/         ← Lógica de negócio + State
    │   │   │   └── 📄 useCep.js
    │   │   │
    │   │   ├── 📁 services/      ← Integração com API
    │   │   │   └── 📄 cepService.js
    │   │   │
    │   │   └── 📁 pages/         ← Páginas principais
    │   │       ├── 📄 CepPage.jsx
    │   │       └── 📄 CepPage.css
    │   │
    │   └── 📁 history/           ⭐ MÓDULO HISTÓRIA
    │       ├── 📁 components/    ← Componentes reutilizáveis
    │       │   ├── 📄 HistoryList.jsx
    │       │   ├── 📄 HistoryList.css
    │       │   ├── 📄 HistoryDetail.jsx
    │       │   └── 📄 HistoryDetail.css
    │       │
    │       ├── 📁 hooks/         ← Lógica de negócio + State
    │       │   └── 📄 useHistory.js
    │       │
    │       ├── 📁 services/      ← Integração com API
    │       │   └── 📄 historyService.js
    │       │
    │       └── 📁 pages/         ← Páginas principais
    │           ├── 📄 HistoryPage.jsx
    │           └── 📄 HistoryPage.css
    │
    └── 📁 shared/                ← Código compartilhado
        ├── 📁 components/        ← Componentes reutilizáveis globais
        │   └── (vazio - pronto para uso)
        │
        └── 📁 utils/             ← Utilitários e helpers
            └── (vazio - pronto para uso)
```

## 🔄 Fluxo de Dados

### Busca de CEP
```
CepSearchForm
    ↓ (input do usuário)
CepPage (usa useCep)
    ↓
useCep Hook
    ↓ (chamada ao serviço)
cepService.searchCepWithWeather()
    ↓ (HTTP GET)
Backend API
    ↓
Response com location + weather
    ↓
Armazenar em state
    ↓
Salvar em localStorage
    ↓
CepResult Component (exibição)
```

### Visualizar Histórico
```
HistoryPage (usa useHistory)
    ↓ (useEffect na montagem)
useHistory Hook
    ↓
historyService.getHistory()
    ↓ (HTTP GET)
Backend API
    ↓
Response com lista de histórico
    ↓
Armazenar em state
    ↓
HistoryList Component (exibição)
```

## 📊 Arquitetura de Pastas

```
features/
├── feature1/
│   ├── components/    ← Componentes React com CSS
│   ├── hooks/         ← Custom hooks (useFetch, useState, etc)
│   ├── services/      ← Chamadas HTTP com Axios
│   └── pages/         ← Páginas da feature
│
└── feature2/
    ├── components/
    ├── hooks/
    ├── services/
    └── pages/

shared/
├── components/        ← Componentes usados por múltiplas features
└── utils/             ← Funções helper, constantes, etc
```

## 🎯 Separação de Responsabilidades

### Services (Comunicação com API)
```javascript
// cepService.js
export const searchCepWithWeather = async (cep) => {
  // ✅ Apenas lógica HTTP
  // ✅ Tratamento de erros de rede
  // ✅ Transformação de dados se necessário
}
```

### Hooks (Lógica de Negócio + State)
```javascript
// useCep.js
export const useCep = () => {
  // ✅ Chamar o service
  // ✅ Gerenciar estado com useState
  // ✅ Salvar em localStorage
  // ✅ Lógica de negócio
  // ✅ Retornar dados e funções para o componente
}
```

### Components (Apresentação)
```javascript
// CepSearchForm.jsx
export const CepSearchForm = ({ onSearch, loading }) => {
  // ✅ Apenas UI/UX
  // ✅ Chamar callbacks recebidos como props
  // ✅ Não fazer chamadas HTTP diretas
  // ✅ Renderizar dados recebidos como props
}
```

### Pages (Orquestração)
```javascript
// CepPage.jsx
export const CepPage = () => {
  // ✅ Usar hooks para lógica
  // ✅ Combinar múltiplos componentes
  // ✅ Estrutura a página
  // ✅ Passa dados para components
}
```

## 🔄 Ciclo de Vida Comum

```
1. Usuário interage com formulário (CepSearchForm)
   ↓
2. Componente Page recebe evento (CepPage)
   ↓
3. Page chama função do Hook (useCep)
   ↓
4. Hook chama Service (cepService)
   ↓
5. Service faz requisição HTTP (Axios)
   ↓
6. Hook atualiza estado (setState)
   ↓
7. Page re-renderiza com novos dados
   ↓
8. Components exibem dados (CepResult)
```

## 💾 Estado da Aplicação

### Local (Hook State)
```javascript
const [cepData, setCepData] = useState(null);
const [loading, setLoading] = useState(false);
const [error, setError] = useState(null);
```

### Persistente (localStorage)
```javascript
localStorage.setItem('cepSearchHistory', JSON.stringify(data));
```

### Futuro (Zustand Store - opcional)
```javascript
const store = create((set) => ({
  data: [],
  setData: (data) => set({ data })
}));
```

## 🎨 Sistema de Estilos

Cada componente possui:
- **ComponentName.jsx** - Lógica React
- **ComponentName.css** - Estilos específicos

Estilos globais em `App.css`

```css
/* App.css */
.navbar { }          ← Estilos globais
.main-content { }
.footer { }

/* CepSearchForm.css */
.cep-search-form { } ← Estilos do componente
.btn { }
```

## 📦 Tamanho do Projeto

```
src/
├── features/
│   ├── cep/        ~300 linhas
│   └── history/    ~350 linhas
├── shared/         ~100 linhas
├── App.jsx         ~50 linhas
└── index.jsx       ~15 linhas

Total: ~800 linhas de código (sem comentários)
```

## 🚀 Como Adicionar uma Nova Feature

1. **Criar pasta da feature**
   ```bash
   mkdir -p src/features/nova-feature/{components,hooks,services,pages}
   ```

2. **Criar service**
   ```javascript
   // src/features/nova-feature/services/novaService.js
   export const getData = async () => { }
   ```

3. **Criar hook**
   ```javascript
   // src/features/nova-feature/hooks/useNova.js
   export const useNova = () => { }
   ```

4. **Criar componentes**
   ```javascript
   // src/features/nova-feature/components/NovoComponent.jsx
   export const NovoComponent = () => { }
   ```

5. **Criar página**
   ```javascript
   // src/features/nova-feature/pages/NovaPage.jsx
   export const NovaPage = () => { }
   ```

6. **Adicionar rota em App.jsx**
   ```javascript
   <Route path="/nova-feature" element={<NovaPage />} />
   ```

## 🔍 Estrutura de Componente Exemplo

```javascript
// src/features/cep/components/CepSearchForm.jsx
import React from 'react';
import './CepSearchForm.css';

/**
 * Componente de formulário para busca de CEP
 * @param {Function} onSearch - Callback quando formulário é enviado
 * @param {boolean} loading - Estado de carregamento
 */
const CepSearchForm = ({ onSearch, loading }) => {
  const [input, setInput] = React.useState('');

  const handleSubmit = (e) => {
    e.preventDefault();
    onSearch(input);
  };

  return (
    <form onSubmit={handleSubmit}>
      <input
        value={input}
        onChange={(e) => setInput(e.target.value)}
        disabled={loading}
      />
      <button type="submit" disabled={loading}>
        {loading ? 'Buscando...' : 'Buscar'}
      </button>
    </form>
  );
};

export default CepSearchForm;
```

## 📚 Arquivos de Referência

| Arquivo | Propósito |
|---------|-----------|
| `package.json` | Dependências e scripts |
| `Dockerfile` | Containerização |
| `.env.example` | Variáveis de ambiente |
| `README.md` | Documentação |
| `SETUP.md` | Guia de desenvolvimento |
| `API.md` | Documentação de endpoints |
| `STRUCTURE.md` | Este arquivo |

## ✅ Checklist de Desenvolvimento

- [ ] Feature criada em `src/features/`
- [ ] Service implementado em `services/`
- [ ] Hook customizado em `hooks/`
- [ ] Componentes criados em `components/`
- [ ] Página criada em `pages/`
- [ ] Rota adicionada em `App.jsx`
- [ ] Estilos CSS organizados
- [ ] Componentes testados localmente
- [ ] Erros tratados corretamente
- [ ] Código documentado com comments

---

**Este arquivo descreve a estrutura completa do projeto React Clima CEP.**
