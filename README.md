# GestaoPedidosAPI

API de gestão de pedidos construída com .NET 10, Clean Architecture, CQRS/MediatR, EF Core + SQLite e autenticação JWT.

> Para esse projeto foi escolhido Controllers (.NET Core Web API) por experiência do desenvolvedor com aplicações do tipo utilizando controllers e mais fácil implementação de Clean Architecture no mesmo.

---

## Pré-requisitos

- .NET 10 SDK 
- Docker Desktop

---

Clone o repositório
git clone https://github.com/seu-usuario/GestaoPedidosAPI.git
cd GestaoPedidosAPI

Rode a aplicação (migrations são aplicadas automaticamente na inicialização)

Acesse o Swagger em: [http://localhost:5048/swagger](http://localhost:5048/swagger)

## Autenticação

Todos os endpoints de pedidos exigem autenticação JWT. Para obter o token:

`/auth/login`
verbo: POST
```Body raw
{
  "email": "dev@martech.com",
  "password": "Senha@123"
}
```

token retornado no header nas próximas requisições:
```
Authorization: Bearer {token}
```

## Endpoints

| Verbo | Rota | Descrição 
| POST | `/auth/login` | Autenticação 
| GET | `/api/orders` | Lista pedidos paginados 
| GET | `/api/orders/{id}` | Busca pedido por ID 
| POST | `/api/orders` | Cria novo pedido 
| PATCH | `/api/orders/{id}/cancel` | Cancela um pedido 