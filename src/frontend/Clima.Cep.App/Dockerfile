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
