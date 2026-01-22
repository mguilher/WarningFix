# WarningFix

Ferramenta de linha de comando para automatizar a correção de warnings em projetos .NET utilizando IA (GitHub Copilot ou OpenAI).

## Descrição

O WarningFix analisa arquivos de log de build do MSBuild/Visual Studio(Output), identifica warnings de compilação C# e gera prompts otimizados para que agentes de IA possam corrigir automaticamente os problemas no código-fonte.

## Funcionalidades

- **Parser de Warnings**: Analisa logs de build e extrai informações detalhadas sobre warnings (código, mensagem, arquivo, linha, coluna)
- **Estatísticas**: Agrupa e exibe estatísticas sobre os warnings encontrados
- **Geração de Prompts**: Cria prompts inteligentes com instruções específicas para cada tipo de warning
- **Integração com IA**: 
  - GitHub Copilot SDK (modo padrão)
  - OpenAI/GitHub Models API (modo alternativo)
- **Ferramentas de Edição**: Conjunto de ferramentas para que o agente de IA possa modificar arquivos

## Requisitos

- **.NET 10** ou superior
- **GitHub Copilot** (para modo Copilot SDK)
- **API Key do GitHub Models** (para modo OpenAI - opcional)

## Dependências

| Pacote | Versão | Descrição |
|--------|--------|-----------|
| GitHub.Copilot.SDK | 0.1.14 | SDK para integração com GitHub Copilot |
| Microsoft.Agents.AI | 1.0.0-preview | Framework de agentes IA da Microsoft |
| Microsoft.Agents.AI.OpenAI | 1.0.0-preview | Integração OpenAI para Microsoft Agents |
| Microsoft.Extensions.Configuration | 10.0.2 | Gerenciamento de configuração |
| Microsoft.Extensions.Logging.Console | 10.0.2 | Logging no console |
| OpenAI | 2.8.0 | Cliente OpenAI para .NET |
| StreamJsonRpc | 2.23.32-alpha | Comunicação JSON-RPC |
| JetBrains.Annotations | 2025.2.4 | Anotações para análise de código |

## Instalação

```bash
git clone https://github.com/mguilher/WarningFix.git
cd WarningFix
dotnet build
```

## Uso

### Via linha de comando

```bash
# Passando o arquivo de log como argumento
dotnet run --project WarningFix -- "caminho/para/build.log"

# Modo interativo (sem argumentos)
dotnet run --project WarningFix
# Digite o caminho do arquivo quando solicitado
```

### Formato do arquivo de log

O arquivo de log deve conter warnings no formato padrão do MSBuild:

```
16>D:\GitHub\Backend\MyApi\Service.cs(110,36,110,60): warning CS8601: Possible null reference assignment.
```

Também suporta warnings de NuGet:

```
D:\GitHub\Backend\MyApi\MyApi.csproj : warning NU1903: Package 'System.Text.Json' has known vulnerabilities.
```

## Warnings Customizados

A ferramenta possui instruções específicas pré-configuradas para os seguintes warnings. Você pode **personalizar estas instruções** no arquivo `CreatePrompt.cs` para atender às necessidades específicas do seu projeto:

| Código | Descrição |
|--------|-----------|
| CS0105 | Diretiva using duplicada |
| CS0108 | Membro oculta membro herdado |
| CS0168 | Variável declarada mas não utilizada |
| CS8073 | Resultado da expressão é sempre o mesmo |
| CS8600 | Conversão de valor possivelmente nulo para tipo não-nulo |
| CS8601 | Possível atribuição de referência nula |
| CS8602 | Desreferência de referência possivelmente nula |
| CS8603 | Possível retorno de referência nula |
| CS8604 | Possível argumento de referência nula |
| CS8618 | Propriedade não-nula não inicializada |
| CS8619 | Incompatibilidade de nulabilidade em valor |
| CS8625 | Não é possível converter null para tipo não-nulo |
| CS8629 | Tipo de valor nullable pode ser nulo |
| CS8632 | Falta anotação de tipo de referência nullable |
| CS8765 | Incompatibilidade de nulabilidade no tipo de retorno |

> 🔧 **Personalizando**: Para adicionar novos warnings ou modificar as instruções existentes, edite o `switch` em `CreatePrompt.cs`. Cada instrução guia o agente de IA sobre como corrigir o warning específico.

## Configuração

### Modo GitHub Copilot (padrão)

Para utilizar o modo GitHub Copilot SDK, é necessário ter o **GitHub Copilot CLI** instalado e configurado.

#### Instalação do Copilot CLI

Siga as instruções oficiais em: https://docs.github.com/en/copilot/how-tos/set-up/install-copilot-cli

```bash
# Instalar via npm
npm install -g @githubnext/github-copilot-cli

# Autenticar
github-copilot-cli auth
```

> 💡 **Dica de Economia**: Ao usar o GitHub Copilot SDK com modelos padrões (como `gpt-4.1`), você economiza requests premium da sua assinatura Copilot, pois esses modelos são incluídos no plano base.


![requests premium](data:image/png;base64,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)


### Modo OpenAI/GitHub Models (alternativo)

Para usar o modo alternativo com GitHub Models API, descomente as linhas relevantes em `Program.cs` e configure a variável de ambiente:

```bash
# Windows
set GITHUB_API_KEY=seu_token_aqui

# Linux/macOS
export GITHUB_API_KEY=seu_token_aqui
```

## Estrutura do Projeto

```
WarningFix/
├── Program.cs                    # Ponto de entrada da aplicação
├── WarningParser.cs              # Parser de warnings do build log
├── WarningObject.cs              # Modelo de dados para warnings
├── CreatePrompt.cs               # Gerador de prompts para IA
├── Agent/
│   ├── AgentWarning.cs           # Agente usando OpenAI/GitHub Models
│   └── Tools/
│       └── FileSystemTools.cs    # Ferramentas de manipulação de arquivos
└── Copilot/
    └── CopilotAgentWarning.cs    # Agente usando GitHub Copilot SDK
```

## Considerações

- **Backup**: Faça backup do seu código antes de executar correções automáticas
- **Revisão**: Revise as alterações feitas pelo agente de IA antes de fazer commit
- **Warnings CS**: Apenas warnings com prefixo "CS" são processados para correção
- **Escopo**: A ferramenta modifica arquivos diretamente no sistema de arquivos
- **Preview**: Alguns pacotes utilizados estão em versão preview

> ⚠️ **Aviso Importante**: 
> - Execute esta ferramenta **apenas em projetos que possuam cópia dos arquivos ou estejam versionados** (Git, SVN, etc.). A ferramenta modifica arquivos diretamente e alterações incorretas podem ser difíceis de reverter sem um sistema de controle de versão.
> - Algumas bibliotecas utilizadas estão em **versão preview** e podem sofrer alterações que causem incompatibilidades em versões futuras. Verifique a compatibilidade antes de atualizar os pacotes.
> - Se você receber um erro de JSON-RPC logo após a instalação do GitHub Copilot CLI, tente **reiniciar o Visual Studio ou o terminal**. Isso geralmente resolve o problema de comunicação inicial.

## Licença

Este projeto está sob licença MIT.

## Contribuição

Contribuições são bem-vindas! Abra uma issue ou pull request no [repositório](https://github.com/mguilher/WarningFix).
