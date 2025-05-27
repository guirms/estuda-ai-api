# Estuda Aí

## 🧠 Descrição

Projeto desenvolvido no contexto da disciplina de Projeto Integrador, com foco em organização de estudos e tarefas acadêmicas.  
Permite gerir quadros (boards), cartões de tarefa (cards) e usuários com autenticação.

## ⚙️ Principais Funcionalidades

- Cadastro e login de utilizadores;
- Criação e gestão de quadros de estudo;
- Organização de tarefas por status: A Fazer, Fazendo, Feito;
- Atualização de status com drag-and-drop;
- Autenticação com token JWT;
- API REST estruturada em camadas (Presentation, Application, Domain, Infrastructure).

## 🧼 Problemas Detectados na Versão Original

- Controllers com lógica repetida e validações manuais;
- Métodos longos e sem coesão;
- Nomes genéricos e pouco descritivos;
- Alto acoplamento entre camadas;
- Ausência de testes e linter;
- Números mágicos e lógica duplicada.

## 🛠️ Estratégia de Refatoração

1. **Extração de Métodos**: Centralização de validações e tratamento de erros;
2. **Divisão de Responsabilidades**: Separação de lógica por classes e perfis;
3. **Eliminação de Código Duplicado**: Métodos reutilizáveis extraídos em helpers;
4. **Melhoria de Nomes**: Métodos e variáveis mais descritivos;
5. **Desacoplamento**: Uso de injeção de dependência e interfaces;
6. **Constantes**: Substituição de valores fixos por nomes simbólicos;
7. **Documentação**: Adição de comentários e documentação do projeto via README.

## 🧪 Testes

- Testes unitários implementados para `UserService`, `BoardService` e `CardService`;
- Cobertura aproximada: 50% dos fluxos principais (registo, login, alterações de status);
- Uso de xUnit e Moq.

## 🔧 Interface Fluente

- Implementada com FluentValidation:
  - `.RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email inválido")`
- Validações encadeadas para legibilidade e reuso.

## 🎨 Estilização com Linter

- Uso de `StyleCop` para padronização de código C#;
- Regras aplicadas:
  - Nomeação de variáveis e métodos;
  - Organização por regiões;
  - Comentários obrigatórios em métodos públicos.

## 🔄 Instalação e Execução

1. Clona o repositório:
git clone https://github.com/seu-usuario/seu-repositorio.git

2. Abre o projeto no Visual Studio;

3. Restaura os pacotes NuGet:

4. Botão direito na solução > “Restore NuGet Packages”;

5. Executa o projeto com:
dotnet run --project Presentation.Web

