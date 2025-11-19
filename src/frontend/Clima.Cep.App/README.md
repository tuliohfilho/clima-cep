# Clima CEP - React Application

Aplicação frontend em React para buscar informações de localização por CEP e dados climáticos em tempo real.

## 📋 Estrutura do Projeto

```
Clima.Cep.App/
├── public/
│   └── index.html
├── src/
│   ├── features/
│   │   ├── cep/
│   │   │   ├── components/
│   │   │   │   ├── CepSearchForm.jsx
│   │   │   │   └── CepResult.jsx
│   │   │   ├── hooks/
│   │   │   │   └── useCep.js
│   │   │   ├── services/
│   │   │   │   └── cepService.js
│   │   │   └── pages/
│   │   │       └── CepPage.jsx
│   │   └── history/
│   │       ├── components/
│   │       │   ├── HistoryList.jsx
│   │       │   └── HistoryDetail.jsx
│   │       ├── hooks/
│   │       │   └── useHistory.js
│   │       ├── services/
│   │       │   └── historyService.js
│   │       └── pages/
│   │           └── HistoryPage.jsx
│   ├── shared/
│   │   ├── components/
│   │   └── utils/
│   ├── App.jsx
│   ├── App.css
│   └── index.jsx
├── Dockerfile
├── .dockerignore
├── package.json
└── README.md
```

## 🚀 Funcionalidades

### Módulo CEP
- **Busca de CEP**: Busca informações de localização por CEP
- **Informações de Clima**: Exibe clima em tempo real para a localização
- **Histórico Local**: Mantém histórico de buscas no localStorage
- **Formatação Automática**: Formata automaticamente o CEP digitado (XXXXX-XXX)

### Módulo Histórico
- **Visualização de Histórico**: Lista todas as buscas anteriores
- **Detalhes da Busca**: Exibe informações completas de localização e clima
- **Remover do Histórico**: Remove itens individuais
- **Limpar Histórico**: Limpa todo o histórico de buscas
- **Sincronização**: Sincroniza com a API do backend

## 💻 Tecnologias Utilizadas

- **React 18.2.0**: Frontend framework
- **React Router 6.20.0**: Roteamento
- **Axios 1.6.2**: Cliente HTTP
- **Zustand 4.4.1**: Gerenciamento de estado (opcional)
- **CSS Módulos**: Estilização

## 🔧 Instalação e Uso

### Desenvolvimento Local

1. **Instalar dependências**
   ```bash
   npm install
   ```

2. **Configurar variáveis de ambiente**
   ```bash
   cp .env.example .env
   # Editar .env com a URL da API
   ```

3. **Iniciar aplicação**
   ```bash
   npm start
   ```

   A aplicação estará disponível em `http://localhost:3000`

4. **Build para produção**
   ```bash
   npm run build
   ```

### Com Docker

1. **Build da imagem**
   ```bash
   docker build -t clima-cep-app .
   ```

2. **Executar container**
   ```bash
   docker run -p 3000:3000 -e REACT_APP_API_URL=http://localhost:5000/api clima-cep-app
   ```

### Com Docker Compose

```bash
docker-compose up -d
```

A aplicação estará disponível em `http://localhost:3000`

## 📁 Arquitetura

### Features-based Architecture
A aplicação segue uma arquitetura baseada em features, onde cada feature (CEP, Histórico) contém:

- **Components**: Componentes reutilizáveis da feature
- **Hooks**: Hooks customizados para lógica de negócio e state management
- **Services**: Chamadas à API e integração com backend
- **Pages**: Páginas principais da feature

### Separação de Responsabilidades

- **Services**: Comunicação com a API
- **Hooks**: Gerenciamento de estado e lógica de negócio
- **Components**: Apresentação e interação com usuário
- **Pages**: Combinação de componentes em páginas

## 🎨 Componentes Principais

### CepPage
Página principal para busca de CEP
- CepSearchForm: Formulário de busca
- CepResult: Exibe resultado da busca

### HistoryPage
Página de histórico de buscas
- HistoryList: Lista de buscas anteriores
- HistoryDetail: Detalhes da busca selecionada

## 🔌 API Endpoints Esperados

A aplicação espera os seguintes endpoints da API:

```
GET  /api/cep/:cep              - Buscar localização por CEP
GET  /api/weather               - Buscar clima (params: latitude, longitude)
GET  /api/cep/:cep/weather      - Buscar localização com clima
GET  /api/history               - Obter histórico
POST /api/history               - Adicionar ao histórico
GET  /api/history/:id           - Obter detalhes do histórico
DELETE /api/history/:id         - Remover do histórico
DELETE /api/history             - Limpar todo histórico
```

## 🌐 Variáveis de Ambiente

```env
# URL base da API
REACT_APP_API_URL=http://localhost:5000/api

# Ambiente (development, production)
NODE_ENV=production
```

## 📱 Responsividade

A aplicação é totalmente responsiva e foi desenvolvida com mobile-first approach, suportando:
- Desktop (1920px+)
- Tablet (768px - 1024px)
- Mobile (< 768px)

## 🧪 Testes

```bash
npm test
```

## 📦 Build e Deployment

```bash
npm run build
```

Os arquivos prontos para produção estarão em `./build`

## 🤝 Contribuindo

1. Create uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
2. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
3. Push para a branch (`git push origin feature/AmazingFeature`)
4. Abra um Pull Request

## 📄 Licença

Este projeto é parte do Clima CEP.

## 📞 Suporte

Para suporte, abra uma issue no repositório do projeto.
