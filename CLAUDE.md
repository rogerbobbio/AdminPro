# Claude Agents & Instructions for ProyectInit

Welcome to **ProyectInit**!

## 📖 Complete Guide

👉 **Read everything here:** [`docs/base-standards.md`](docs/base-standards.md)

This file contains:
- ✅ Reading order for all project documentation
- ✅ Core principles & workflows
- ✅ All 3 specialized agents (Backend, Frontend, Product)
- ✅ Available skills & when to use them
- ✅ DDD 4-layer architecture
- ✅ Spec-driven development workflow
- ✅ Critical rules & security guidelines
- ✅ Project structure & quick start checklist

---

🚀 **Let's build something great!**

## graphify

This project has a knowledge graph at graphify-out/ with god nodes, community structure, and cross-file relationships.

Rules:
- For codebase questions, first run `graphify query "<question>"` when graphify-out/graph.json exists. Use `graphify path "<A>" "<B>"` for relationships and `graphify explain "<concept>"` for focused concepts. These return a scoped subgraph, usually much smaller than GRAPH_REPORT.md or raw grep output.
- If graphify-out/wiki/index.md exists, use it for broad navigation instead of raw source browsing.
- Read graphify-out/GRAPH_REPORT.md only for broad architecture review or when query/path/explain do not surface enough context.
- After modifying code, run `graphify update .` to keep the graph current (AST-only, no API cost).
