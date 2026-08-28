#!/usr/bin/env bash
# Pre-PR check: build, tests, lint, security.
# Exits non-zero if anything fails.

set -euo pipefail

ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"

echo "🔍 Backend..."
cd backend
dotnet restore --verbosity quiet
dotnet build --no-restore --nologo -warnaserror
dotnet test --no-build --nologo --verbosity quiet || { echo "❌ Backend tests failed"; exit 1; }
dotnet list package --vulnerable --include-transitive 2>&1 | tee /tmp/vuln.txt
[ ! -s /tmp/vuln.txt ] || grep -q "0 vulnerable" /tmp/vuln.txt || echo "⚠️  Vulnerabilities in .NET packages"
cd ..

echo "🎨 Frontend..."
cd frontend
npm ci --silent
npm run lint || { echo "❌ Frontend lint failed"; exit 1; }
npm run build || { echo "❌ Frontend build failed"; exit 1; }
npm audit --production 2>&1 | tee /tmp/npm-audit.txt
[ ! -s /tmp/npm-audit.txt ] || grep -qE "0 vulnerabilities" /tmp/npm-audit.txt || echo "⚠️  Vulnerabilities in npm packages"
cd ..

echo "✅ OK"
