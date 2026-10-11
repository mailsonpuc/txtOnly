# txtOnly Frontend

Frontend em React, Vite e TypeScript para a API REST existente em `BackEnd/txtOnly.API`. A interface é textual e responsiva, com navegação em estilo de terminal Linux.

## Executar

```sh
yarn install
cp .env.example .env
yarn dev
```

Configure `VITE_API_URL` como a URL base da API incluindo `/api`, por exemplo `http://localhost:5011/api`. A API em desenvolvimento permite a origem padrão `http://localhost:5173`; se o Vite usar outra porta, ajuste `Cors:AllowedOrigins` no backend.

Verificações disponíveis: `yarn typecheck`, `yarn lint` e `yarn build`.

## Recursos conectados

- Cadastro e login em `POST /api/Auth/register` e `POST /api/Auth/login`; a sessão renova o access token pelo endpoint `POST /api/Auth/refresh-token`.
- Feed global paginado, criação, edição e exclusão lógica de publicações pelas rotas `/api/Posts`.
- Histórico por perfil em `/api/Posts/user/{userId}/pagination`; bio lida/editada em `/api/Users/{userId}`.
- Comentários e curtidas usam as rotas paginadas de `/api/Comments` e `/api/Likes`.
- Diretório paginado, seguidores e seguindo usam `/api/Users/pagination` e `/api/Follows`.

Após o primeiro login, o frontend cria o perfil de domínio em `/api/Users` usando o GUID do claim `sub` do JWT. Isso é necessário porque cadastro Identity e perfil social são registros separados no backend.

## Limites da API

- Os DTOs públicos incluem o `userName` da conta Identity associado ao mesmo GUID. O username vem do registro Identity, sem criar um nome paralelo no perfil social.
- Não existe endpoint de timeline baseada em quem a pessoa segue. O feed disponível é global.
- Posts e comentários não incluem contagens agregadas ou indicador de curtida do usuário atual. A interface consulta os endpoints paginados de comentários/likes para montar esses estados.
- A API não verifica propriedade do post nas ações `PUT /api/Posts/{id}` e `DELETE /api/Posts/{id}`. A interface só mostra editar/excluir ao autor, mas o backend precisa impor essa autorização para garantir segurança.
- A API não oferece pesquisa de usuários; o diretório é paginado.