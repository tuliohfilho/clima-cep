# Estágio 1: Build
FROM node:18-alpine AS builder

WORKDIR /app

# Copiar arquivos de dependências
COPY package.json package-lock.json* yarn.lock* ./

# Instalar dependências
RUN npm ci || npm install

# Copiar código fonte
COPY . .

# Build da aplicação
RUN npm run build

# Estágio 2: Runtime
FROM node:18-alpine

WORKDIR /app

# Instalar serve para servir a aplicação
RUN npm install -g serve

# Copiar build do estágio anterior
COPY --from=builder /app/build ./build

# Expor porta
EXPOSE 3000

# Variáveis de ambiente
ENV REACT_APP_API_URL=http://localhost:5000/api

# Comando para iniciar a aplicação
CMD ["serve", "-s", "build", "-l", "3000"]
