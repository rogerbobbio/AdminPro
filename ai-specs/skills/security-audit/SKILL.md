---
name: security-audit
description: OWASP top 10 security audit adapted to .NET 10 + Angular 22.
---

# Security Audit (OWASP)

## Backend .NET 10

| Risk | Verify |
|------|--------|
| **A01 Broken Access Control** | `[Authorize]` on controllers, per-role policies |
| **A02 Cryptographic Failures** | JWT secret ≥ 256 bits in env var, BCrypt, HTTPS |
| **A03 Injection** | Parameterized EF Core, FluentValidation, escape HTML |
| **A04 Insecure Design** | Result pattern, rate limiting, multi-tenant if applicable |
| **A05 Security Misconfig** | `appsettings.Production.json` not in git, restrictive CORS |
| **A06 Vulnerable Components** | `dotnet list package --vulnerable` in CI |
| **A07 Auth Failures** | Refresh tokens, short access expiration (15-60min) |
| **A08 Data Integrity** | Validate JWT signatures, anti-forgery tokens |
| **A09 Logging Failures** | Do not log passwords, JWT, PII |
| **A10 SSRF** | Validate external URLs, allowlist on webhooks |

## Frontend Angular 22

| Risk | Verify |
|------|--------|
| XSS | Never `[innerHTML]` without sanitizing |
| CSRF | SameSite=Strict cookie, anti-forgery token |
| Token storage | **Never localStorage** → prefer httpOnly cookie |
| Secrets | Do not expose API keys in `environment.ts` |
| Dependencies | `npm audit --production` in CI |

## Commands

```bash
dotnet list package --vulnerable --include-transitive
npm audit --production
```
