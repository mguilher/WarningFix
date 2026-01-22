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


![requests premium](data%3Aimage%2Fpng%3Bbase64%2CiVBORw0KGgoAAAANSUhEUgAAAywAAAB1CAYAAACs52U%2FAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAABi%2BSURBVHhe7d3%2Frx5VXsBx%2FwGT3S2lSLtpWsqXtrRdCvTSdguh5csStkZlwdVQIbVBDQlsWEC6ZXfjJiSuIAJ1I5oKfokxEkPSKHFjY%2BIPZDGpiKZoYsBf2JBA1KRGxf2B5DifM3NmPuecz8w8z9zn3jvPve8fXuE%2Bc2bOl8%2FM7Xw%2BzPPc58d%2B%2FLMbHAAAAACMEQULAAAAgNGiYAEAAAAwWhQsAAAAAEaLggUAAADAaFGwAAAAABgtChYAAAAAo0XBAgAAAGC0KFgAAAAAjBYFCwAAAIDRomABAAAAMFoULAAAAABGi4IFAAAAwGhRsAAAAAAYLQoWAAAAAKNFwQIAAABgtChYAAAAAIwWBQsAAACA0aJgAQAAADBaFCwAAAAARouCBQAAAMBoUbAAAAAAGC0KFgAAAACjRcECAAAAYLQoWAAAAACM1kwLls9s3O427L7bbTz4kNt8%2B1Nuy12%2FBgAAAGBkJFeXnF1yd8nhrdx%2BLGZSsMgiNy4cKxZ%2Bym3cd8xduuMOt27Lgvvc5%2FcAAAAAGBnJ1SVnl9zd5%2FBFLj%2FWwmXRBcv67Ud8lXb53vvMYAAAAAAYN8nlJaeX3N7K%2BVfSogoWeYS0%2Bdavu%2FVX3WIuHAAAAMB8kJxecnvJ8a3cf6UMLlik%2BpIF8dYvAAAAYHWQ3N4%2FkBjRk5ZBBYu8v80%2FMuLJCgAAALCqSI4vuf5YPtMyqGCRD%2BXwmRUAAABgdZJcX3J%2BqxZYblMXLFJpyV8SsBYGAAAAYHWQnH8MT1mmLlj896zsO2YuCgAAAMDqIDn%2FGD6AP3XBIl8wI3%2Bz2VoUAAAAgNXBf09LkftbNcFymrpgkW%2FF5C%2BDAQAAAKub%2F4thRe5v1QTLaeqCRf5igLUgAAAArC07rvuiW7j5TnfotqMwSGwkRlbs5oXk%2FlZNsJwoWAAAADCVjdtu8Mn49QeOuKv3LLgt13wBBomNxEhiJTGzYjl2FCwAAACYO5KA77rxkJmkIyexkphZsRw7ChYAAADMFXmLkzw1sBJztJOYzePbwyhYAAAAMFfkSQFvA5uexGwen7JQsAAAAGCuyIfJrYQc%2FSR2VkzHjIIFAAAAc4WCZTgKlmEoWAAAADAxCpbhKFiGGUXBctnOO932e37TLTz5A3frc%2F%2Fh3fTED4ptz7oNfKs%2BAADAaFCwDEfBMsyKFizrtx10Nz761%2B620z9yh1%2F8H7f%2F5Ntuzy%2F%2Bqbf%2FG%2F%2Fgtx156f%2Fc3of%2Fwl2y9SazDwAAACwfCpbhKFiGWbGCZd3WBXfwW%2F%2FsC5LtX3nOv073ueSK%2FW7HfS%2F6gmb%2Fyb936zZfn%2B0DAACA5UPBMhwFyzArVrDs%2FeXXfbGy7a5vmO3a1T%2F1jC9adj3wqtkOAACA5UHBMtxiC5bLdn7JXXn3N802cdWXv%2BX3sdqGWrMFy8a99%2FgCZMdXns%2FaDn77X9z%2Bp%2F8p277rF15xh1%2F4b7dhx%2B1ZW%2BrZ85%2B6i%2BdfMbe%2Fd%2FZUtn1RTl9wFz84505YbavMibMfmXENfPsnRey9C%2B7ZrvYoZqfcax%2BE4z51b55Wxz1yzr1n9DU1OU8t81rr%2FHlZI9fweJW%2FA9G1r%2Fnfg4%2Fca48YbaPWs65Or7g3zTUvps8lsph%2Fpxa%2B7w4V90O5J9ae%2Bb7bZO07lTNu3%2BkP3e4Fq02cdLuf%2BZHbd9Rqm61Nxz90tz1%2BxmzzfAzecdv8a5l397y2PV7EqKu%2FmemL4do1tGB55I2P3cW3X83bXn7XXfzhOfdIun0Ftc51kRZbsFxxx5P%2B34lrf%2F53sjbZJm2TPAyYxpotWPY8%2BMfu1uf%2B063bsi9rk7eJWQXLpdcccUde%2FMRtv%2Fe3sraccaOTG8pSJGUULCVfEDQ37CwJTtp1URn1G50nSUxmkaS1JT7ACskS3BEm4UPMdF1rqWAJyfoszWvBklrJomF1FCxS4B06ftJsG4qCZbjFFixi9wN%2FUBYt9%2F9uvU1%2Blm3SpvedhTVbsBz6zvvu%2Bof%2F0mzrctOv%2Fp1bePItsy0VJ9dLeIOjYLHbkht49nRLtUtbc24kSamOK2I7kydii0kmgKUw08R%2BRGa6LgqWxaFgWTwKljYULMPNomARoWjZ%2BdXThd9esmJFrNmC5fDzF81HWVuPfM0%2FRTnwzQtZm9jz4B%2B5W777kdmWUze7tKjw%2F7e%2FeQtSV6ItuhL10Pezso%2FVn5%2BHGivMw7rBJdv8uPWx7TfD%2FK1uyY3e99vMQd%2FoO8eI4nTBvdYVBz9GM2YcMyvxaJKOaF%2Fpx8dIjmlfc6zsq5mrGqvrXIvW85CsRfWhY90X%2B3Jt56r5pTEQIQ7xGrJzlPWRrFmvK7smq2N0LIz9265L81yelv3Kvsr1y%2Faq787zVh1%2FtppL6Dc6T3Gc%2FNO40BbmWcdZjxsf13ldFMK6dP%2F6XMbnXa%2BpI%2FY%2Bfk2bmVSb12S4DtrWE9ZbvZ5kHKtPNVY%2BRnNM6HeqmA1aV5dkzbXQZ%2Fk6zCk6X35sdYyeW%2FjdqPepxkmuyfj8pzHW6ynazqa%2FN0kc0%2FlokyTrR2Wf8JYxvW9ZdJTb07aQbMt%2FQ7tOvvOCxRcWZl%2BprnELR9%2BJ2nZnBYueUzGH4zoGal4%2BNs1%2B4a1yWQEUjVfIxuqKYbpuHZMQw2bfRjVPP%2FfiuPptfElskkItGqs4ZrcqJvLCLj9H8Vrjufm3yqm%2BN2XnKewfx39IMbO0BcvT7vUffureern8b%2Fg9euvl%2BBjfV%2FgdK%2Fp86e1P3ftvPB3193qxrTn2VfdW2F%2BkBZIcU7e%2F617P5hofX481pVkVLGLP8T%2Bpz%2BNSFStizRYs8nawXcfiX2Ihb%2FtaeOJNH%2Fi0Tew69vv%2BO1qsNkt5IysTvfpm429c%2BiZY3ljqm25vwpaoboTx8fF4zY2uvMmVr%2BObrtDJrx%2BzTs6659CdNKfjFMlG9XPnGH5dKg4hQeq68eokKtrPSjz0vPTNvdxP1qRj0646Vo9nzj0%2Bp438POg4%2BJ%2BjY%2FU5nLBgydauhbWrffx8u%2Fooj2nGTdaQXJPl8SpGSf9%2B%2F6kKlqKvaH%2BZfzgmnVuqOr7rfOn56J8LJ4rEt%2Fy56ic9LplH1zghLnHcqrikcSjG1f3asU%2FOQzHH8LuWyeJczVdt80VBvXZ9XU06TtpniFlz7v0YdYyq%2FYfGTF5Pva4ues1avP4wp%2FSc1K%2BTNdTXbHpN63VLH8W9o77uZIzoXOj4p2tMxk%2Fmm0mT8kKTQFaJZZoMh6S2OHZfvW%2FZlh2rk3Of7BqFQfHaJ8v1OFbyrHSNG41R7uvXV%2FdVzqtJwkNSbc%2FLKhqiufnxdHtLHNpiKK8fT9Zd75uP3aj6UTHLxy5ft8U4FB9h%2Fzzm8fFZbOV16E%2F%2FXNh09Ez9c%2FqEJX590m072rRNajkKlouffOxef6Jqf%2BKce1%2B99v3ogqMqNqKCpXgdFTnFtuZ1WXzUr%2F3%2B77qXwr5%2BvGIO9VyT%2Ff1rNb8pzLJgCU9ZBAVLYhYFizxBufFrf2O2yaMtCbzVtu%2Bxv3UHTr1jttmqG4m6OeYJZkFuaGGf7IZb3ayim5mij62YYxhtcb9dCUloj%2BcV5OPlfeXz6R7DWsNUcYiSBD2foOMmLsf6ceQ4%2BUdLJwMJ43xlfZv7KNHc9bH2HHUcumPfEzPPHqP9OilY66ljVv2sz4XfX8c%2FGVPv33v9p%2Bey7EvHoHvN%2BbWQx1D6rPaRuZnnrueamuC6yOeZHp%2F2H7a3xT6PRausn3hu%2BT56vZOOk%2FcpsY7W3HPu0z46Yyavp15XF%2Bsci745FdS6%2Bv8taxtH0XPWMbParfXV14jaFviEXiWhESNhThJTLU54rWRbJ8BtPwdyfNu8Ynpc6%2B1H0bys%2BUcxSOeSr6NvvHiM6WIYz8WKYWDEzDqXMpafqxXjeP59BUu%2BVmmv5ifjtJyv9Dh5HY8zveV5whLv0zxB6WsvXkf92fT%2B0bGVaK5Gf9KeHjOJWRUsvCWsxywKlmvvP%2BMOP%2F9f7pJtB6Lt%2BgnL1iOPxW1X3uy%2FSHLnz30v2t4nvlkZN0%2FRc7PpTMCMm5c1Zpl0l5o2uVFWY0U3NNkeH1Oyb6r5DTm9Aas5TDSGHaf2ONj7y7zKbVZCIMdY65HtZUyidbXd8K3koRAdayUQETU%2F2bfuz5p3QY3ZF%2FvOa8drj3XoN%2BtDxrfOXZh3FhOZk15%2FMqbev%2Ff6z2PSnGdr%2F1R6fDkXaz3R%2FPw2fZx9buq5ZDFo2lvjmsbFx6JtLol6LLWervOexdm4DqJ9OuLWOk7ep76uPB2nmcVsmnV1sc9x2Wez3bzeZAy%2FFmP8QnxMyzjZuS7n3DpeWFPvNZJYZMHik8%2FinlmrE1E72Zb9ywRYJ8Oyb9KP15ast41rJ%2BU6Ec%2BT8sLggsUer7foSAsWn%2Bzr9XQcWzPGzvqp%2BLHaz8dkBUv5s9V%2FPYd6%2FHgcPYbe5vdtK9x6LEnBUm%2B3C5KmQLCfbvQXLPFbzES5f%2Ft4YU7%2BZ3VczVpLj1kULKFY4UP3HWZRsPzEnp%2F038EiXwqpt4fPsEjADzz9j1Hbtff%2Fni9YLtt5Z7S9T5pQpq%2B9%2BuZW%2FZzcTM0bVGDc5Jsx8pulNR9pD%2F8tt8c35D75mtpu9OW%2B5Vq6x8j71MfG%2B5pJSaFZk9HekrRIrJvYqfnpc6S19NOM3b6PFtbbjC%2Fb7XX5c17FIY9THPvOa8drj13oN%2BujLRZBdk3KnPT6kzH1%2Fkas4vHzayuKdbZ%2Fqv%2F4Vn5u4VjrGlfrmuC6yOfZcr79WKrfrtgrMlZrHLL5GWNH%2B1jrLbWPk%2Fcpa46u155zL6aK2dTr6tJ1PppYmNdbva6yj%2Fh3NI2ZEdv0d0jPOfv9qraFdtl3wmvEW0TBkiaiccJrJch5Atz83JaY57rGbU2Ow7zSYiFsG1Sw2OP5mNZjdMcwm09fsVNL51mIxk0Z%2B1fbwvy7C5ZyrfHxLfwamnmbMar4MVvn3G5owdL25CMuZPqeoKRvzxLlMe0FS95n3xMW2VbPKSqoFmexBcvW2x4rixX%2BrHG3WRQs4rqH%2FrwM6peeMtu1q49%2Bx%2B%2B7%2B8E%2FNNu7yI0pu0FHN6f0hqaSE3ntb1QdiYdx82rGTG%2BEZd%2FZfM5fKLbHN3B%2FE57wppfu62%2FG9bjFmGru%2BubeNYZv03PqiUPWl76BG6%2FjhKGS3Oj1udPzjpXnL2pLx9bJRhs%2FdpHMJftlcUiuj3Tdcey75h1U8%2B%2BIXd6HnYTV5PjovMqc9RrK4%2BtrPNo%2FXl9%2B3qU9Tu5kzTqp7F5zfnx2vpQTZy8kvz%2Fquk6ux%2Fhc9F8XbXH1ayn2bdak49UV%2B2JO6Xza4pBdk8k5yfZJ1j7ROHmfsm%2F278%2BsYiavp11Xx7kXfrykPf23o9xHX1PltRHGzPqY4JpO1xn1UR3frKmKWz1G%2Bbr19zM1uGBJE%2BDydVyw6NdpchofP3ni2j2u70evx69PzaN6nR0%2FsGApix3dXh7fJOjdBUvUV3g9tGDJxo75wk3H2M9d7e9fN7Er56LGSNq1TcffUfOM5x0XLMUc1Wd2dCzK9skMLliqYiMqDpLPp9RPQnTBIQWD%2BoyJLyay9q6CJX0qE8%2BjfIIyzWdYhltswbJh553uqi9%2F22wT0sYXRxZmVbCs27rgv3NFnrRc8zPfNb%2BT5ZIrDrgdP%2FuS%2F4Xdf%2FJtt27z9dk%2BfXTSW%2FM3SbnBlDrbi5t5%2FBdlEtENv5Ql2vVYRRKUzae8uVo3tzL5VdrmUN8wS%2BVf5Ak34LjNmmvdJrKbdLW9Lw6FeK1xApC1Z%2F3kSUOITRhfzzuWrDFJcPIkylL1YawvXleSfHXGvjq2I2bh%2BPKvDIV%2BupOnkopNpZ5Xdk3Kvnr91Zht%2B3de%2F%2Fl5kmtIx6R7zfnxIo1xPR89l0IzTtVP%2BMtOXnqOu6%2BLfJ4qLiGprVi%2Fs02%2FYV7JeJ3XrPrd83NIzomIrlsdt0nHyfuUNUdrya6VpO9pYlZtm2pd%2Fvym5y2WXRvJtVXOKfwVvVL672n079wE13R6jt87fyH%2BNyS6Popj%2FV%2FN0%2Btou0YMIaGPdCTMOsGsEt7Sh27f4zr5ro4tEtm8X5En3D6hrvctqEQ%2B0jluk2h7xVy3JUVBvOZijv4veIW55fOq%2B2spMuL5pAVDTwx9uzr28XfUXIxja1bBIuL%2BRGuMizWkTz90%2B6HjZ9pjEZjXQjKvOt7lWuLzbBdAfYYXLKJM%2Fpvfj%2FTtXdXTkDeqoqFjn7qPorCInpJkBUv6tq533VvJU5WovTj2peipTyEUMbV0TpNZbMGyEtZ0wSLWbzvobnj0nP%2BlkW%2Bx3%2F%2FUef%2Bni79w4s%2F8h%2BsPv%2FC%2FvqDZ%2Bytn3SVbbzL7AGbDSKyWxUqNO%2B%2BsJBNrUV5E9ZMCxvqfRMBySwuWebG4gqVP%2FvatfkOOWRkULMOsaMESyKOr7fe%2B4L8UUv7k8S2%2F8bH%2F8P01P%2F3rbsP228xjgFnySU%2FP%2FxFfGhQsw1CwoDR1weKf6nDtYBwoWCzTFx%2FZW7pGjIJlmFEULMCKqd%2Fa0f22lKVDwTIMBQtKvQVL8rZCihWMCQWLpb9gid%2FeJeajWBEULMNQsAAAAGBiS1uwrG4ULMNQsAAAAGBiFCzDUbAMQ8ECAACAiS3cfKe7es%2BCmZCjncRMYmfFdMwoWAAAADBXdlz3RXf9gSNmUo52EjOJnRXTMaNgAQAAwNyRJwW7bjxkJubISazm8emKoGABAADA3Nm47QafgMtTA94e1k5iIzGSWEnMrFiOHQULAAAA5pa8xUmScfkwOXISm3l8G5hGwTJm%2Fvs5%2BHv9MyffiVB%2FQaN8l8bw7yCRb6ue9huuAQAAMDkKliU09bcfY3lEBctsSQHz3tlTZhsAAACmR8GyhChYRoqCBQAAYG5QsCwR%2F1ahTxrNW47KtyA1bRfcs8mxDdm3eUtYKIB032lyHI9b9V0l6K9VbfVcZHu9b%2FzWMz9W3Ra%2FZSoaI0r847W1Je5WUu%2F7rIu7rhjFMfGyt3gV7WertVkFY7T%2FKffaB8368hiXY%2Bt46Lk3RWnZT9gnjScAAACGoWBZQk0yG7aViXiW8Lb%2B3%2F44OQ9Jc1xwNO1x0i%2F7n2sKFn1cvU0VAmkSf76ZUzTHaL%2Bi7fQr1c%2Fl2poxjMIiSPqI9%2B2LkdFv1F95vFmoBOla04JFrSMULvV85FgVN7%2B%2FGssqxgAAADAcBcsSSpPZPFEXHYl90pb1p5Nt%2FwF9%2FSRCMcbNE2vpq2Ueuu8kYa8ZY8h87eQ9LhKiY3tjNEnB0hbPSrS%2FUbCk5yxab%2Fc5oWABAACYLQqWJZQms3nBIZLkPTJFwWIm%2BpWsrTyufOtSLCoiojaVtNdtydyi%2FSvZetX%2BVZsk%2Ba0Fg6djtMwFixRrUewoWAAAAJYTBcsSypLfrHAQkjC3JdhTFCwDnrDUxUkq3b%2Btb7%2B9mp8ckxUaHeo%2BZY1JMdQZIwoWAACAtYSCZQnlBYYku3FC6%2FfJEvSgOzmOk%2B3yZ90efYYlHUO2tRQ46Tj%2BdbXvibMXVDGg51eurbUIMvjkvihE4gS%2FL0blOpv2cv%2BxFizyOj5nAAAAmAYFy5KqkukokW%2B2eWkhEelOjtNkuy5a0r6tgqVQFiLG%2Fskc3zt%2FIfkMS9MWFSj%2BqUnT1vuXsnxf1j49MYrGKeYVrS%2BOmWkJC5ZmbuU%2BFCwAAACLQ8GCldNSSAEAAAABBQtWSPkUJXpCAwAAACQoWLDs%2FNukimIl%2FuwKAAAAkKNgAQAAADBaFCwAAAAARouCBQAAAMBoUbAAAAAAGC0KFgAAAACjRcECAAAAYLQoWAAAAACM1lwWLJtvf8qt27JgLggAAADA6iA5v%2BT%2BVk2wnKYuWDYefMhduuMOc1EAAAAAVgfJ%2BSX3t2qC5TR1wbJh991u475j5qIAAAAArA6S80vub9UEy2nqguUzG7e7zbefMhcFAAAAYHWQnF9yf6smWE5TFyxi48Ixd%2Fne%2B8yFAQAAAJhvkutLzm%2FVAsttUMEilZb8xYD1V91iLhAAAADAfJIcX3L9MTxdEYMKFrF%2B%2BxG3%2Bdav8xfDAAAAgFXC%2F2WwIseXXN%2BqAVbC4IJFyIdw%2FIJ40gIAAADMNcnpJbcfwwfttUUVLEKqL3lkxGdaAAAAgPkkubz%2FyMeInqwEiy5YhLy%2FTT6UI39JQP78mfzNZt4qBgAAAIyT5Or%2Be1aK3N3n8EUuP5bPrKRmUrAEskj%2FPS0HH%2FLfiilVGgAAAIBxkVxdcnbJ3cdaqAQzLVgAAAAAYJYoWAAAAACMFgULAAAAgNGiYAEAAAAwWhQsAAAAAEaLggUAAADAaFGwAAAAABgtChYAAAAAo0XBAgAAAGC0KFgAAAAAjBYFCwAAAIDRomABAAAAMFoULAAAAABGi4IFAAAAwGhRsAAAAAAYrZkWLJ%2B98nZ36eFn3OVf%2FSv3%2BV%2F6V7f50X8HAAAAMDKSq0vOLrm75PBWbj8WMylYPnftvX7BVjAAAAAAjJvk8pLTW7n%2BSlt0wbLhru%2FVC93y8L%2B5bfeccZsWHnDrNl9n7g8AAABgZUmuLjm75O6Sw4d8XnJ7a%2F%2BVs8H9P7n5ZVy0KlRgAAAAAElFTkSuQmCC)


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
