# CORRECTIONS.md

**Repositório:** ValidationCodeExamples  
**URL:** https://github.com/medcomdk/ValidationCodeExamples  
**Branch base analisada:** main (commit 22822a7f7028fc89b51fad5a5b980e24c1b86548)  
**Fork alvo:** https://github.com/admeduardovieira/ValidationCodeExamples  
**Data:** 2026-11-08  
**Agente:** Kilo (REA 6.7.0), identidade Git admeduardovieira / admeduardovieira@users.noreply.github.com  

---

## Correções propostas e status

| ID | Título | Severidade | Status | Localização | Risco | PR sugerido |
|----|--------|------------|--------|-------------|-------|-------------|
| REA-015 | Substituir `Coding.First()` por `FirstOrDefault()` com null-check | P2 | Proposta (não implementada) | `FirelySDKDemo/Program.cs:84` | Baixo | `fix/rea-015-coding-first-or-default` |

---

## Detalhes por correção

### REA-015 — `Coding.First()` → `FirstOrDefault()` com null-check

**Problema:** `Program.cs:84` chama `issue.Details?.Coding.First().CodeElement?.Value`. Se a lista `Coding` estiver vazia, `.First()` lança `InvalidOperationException`.

**Evidência:** `Program.cs:84` — `Console.WriteLine($"IssueCode: {issue} - Issue details {issue.Details?.Text} - Error Code : {issue.Details?.Coding.First().CodeElement?.Value}");`

**Causa-raiz:** uso de `.First()` em coleção que pode estar vazia.

**Correção proposta:** substituir `.First()` por `.FirstOrDefault()` e verificar null antes de acessar `.CodeElement?.Value`.

**Arquivos envolvidos:** `FirelySDKDemo/FirelySDKDemo/Program.cs`

**Teste de validação:** adicionar teste que gere uma issue de validação sem Coding e confirme que não lança exceção ao imprimir.

**Risco:** baixo.

**Benefício:** evitar InvalidOperationException na impressão de issues.

**Prioridade:** imediata.

---

## Problemas não corrigidos e riscos remanescentes

- A correção acima está em status "Proposta" nesta etapa (Fase 1 concluída). Nenhuma alteração aplicada ao código-fonte ainda.
- Limitação: dotnet não instalado; não foi possível executar `dotnet build`/`dotnet test` para validar dinamicamente.
- Problema recorrente documentado (conformsTo não suportado) está em correção pelos mantenedores; não é alvo desta auditoria.

---

## Registros de relatório

- Base PT: `REVERSE_ENGINEERING_REPORT.md` e `CORRECTIONS.md` (este arquivo).
- Traduções: `REVERSE_ENGINEERING_REPORT.en.md`, `REVERSE_ENGINEERING_REPORT.uk.md`, `CORRECTIONS.en.md`, `CORRECTIONS.uk.md`.
- Todos na raiz do clone de trabalho / área de auditoria, ao lado dos relatórios-base em português.

**Nota de tradução (uk):** Tradução técnica assistida automaticamente; recomenda-se revisão por falante nativo antes de uso como referência normativa. Não houve revisão por nativo obtida.
