# Skill: Artifact Design

## Purpose
Design guidance and fundamentals for building polished UI artifacts — Angular components, pages, dashboards — with deliberate visual identity. Applies to any deliverable rendered in a browser.

---

## When to Load
Load this skill when asked to:
- Design or build a new Angular page, component, or dashboard
- Create a visual prototype or mockup
- Review UI quality before delivery
- Make design decisions about palette, typography, or layout

---

## Role
Approach every design task as the **design lead at a small studio known for versatility** — calibrate the visual treatment to what the task actually calls for, make deliberate choices, and avoid templated designs.

---

## Calibrate Treatment First

Before writing any code, read the request and choose the right treatment:

| Request type | Treatment |
|---|---|
| Document, plan, memo, demo | **Utilitarian** — polished typography, considered spacing, proper palette. No flashy hero. |
| Landing page, app, tool the user will share | **Editorial** — distinctive point of view, opinionated calls, one real aesthetic risk. |

**When unsure:** a well-composed page is never wrong; an over-designed visual identity sometimes is.

---

## Fundamentals (Apply to Everything)

### Honor What Exists First
1. Look for the existing design system: CLAUDE.md, token files, component styles
2. When one exists, apply it — never override
3. Precedence: **user's words → project's existing system → your choices**

### Ground It in the Subject
- Pin one concrete subject, its audience, and the page's single job
- Draw distinctive choices from the subject's own world — its materials, instruments, vernacular
- Build with **real content**, never placeholder text

### Typography
- Inline fonts as `@font-face` data URI (CDN font URLs are blocked by CSP)
- Keep running text near **65 characters wide**
- Set a type scale and stay on it
- Headings: `text-wrap: balance`
- Uppercase labels: add `letter-spacing`
- Digits in columns: `font-variant-numeric: tabular-nums`
- Pair 2+ typefaces: one characterful display face used with restraint + one complementary body face

### Color
- Choose neutrals deliberately — a grey with a slight hue bias toward the accent reads as chosen; pure mid-grey reads as unconsidered
- Palette: 4–6 named hex values
- Semantic color (good/warning/critical) is separate from the accent hue

### Layout
- Use flex or grid with `gap` for sibling groups — not per-element margins
- Wide content (tables, code, diagrams): wrap in `overflow-x: auto`
- Avoid: everything centered, `border-radius` everywhere, emoji as section markers

### Avoid AI-Generated Design Clichés
Do not default to:
- Warm cream (`#F4F1EA`) + serif + terracotta
- Near-black + lone acid-green pop
- Purple-to-blue gradient hero
- Inter or Space Grotesk as the "safe" face
- Numbered markers (01/02/03) when content isn't actually a sequence

### Clean Code
- Watch selector specificities — avoid class conflicts canceling spacing
- Close every non-void element, double-quote attributes
- Visible keyboard focus state
- Respect `prefers-reduced-motion`
- For decorative graphics: prefer Canvas/WebGL over hand-authored SVG paths

### Copy & Writing
- Write from the user's side of the screen — name things by what people recognize
- Active voice: a button says exactly what happens ("Publish", not "Submit")
- Errors explain what went wrong and how to fix it — no apologies, no vagueness
- Specific beats clever

---

## Design Plan (Required Before Building)

Before writing code, produce a compact design plan with:

```
Color:    4-6 named hex values with roles (background, surface, text, accent, semantic)
Type:     Display face + body face + utility face (if needed), with size scale
Layout:   1-2 sentences describing the layout concept
```

Then build following the plan exactly, deriving every decision from it.

---

## UI vs Document Mode

### When it's a document/report
- Craft is in typography, spacing, hierarchy
- Read top-to-bottom flow

### When it's a dashboard/tool
- Craft shifts to **information design**
- Surface summary before detail
- Encode state in form (pill, chip, severity stripe) not just numbers
- Semantic color encodes what needs attention at a glance
- What's interactive must **look** interactive
- Sparklines and charts: area fill, faint grid, emphasized endpoint

---

## Editorial Mode (Landing Pages, Apps, Showcases)

Activate when the treatment calls for a distinctive point of view:

1. **Review the design plan** against the subject — if any part reads like a generic default, revise it
2. **Hero = thesis**: open with the most characteristic thing in the subject's world
3. **Typography carries personality**: pair faces deliberately, not the families you'd reach for on any other project
4. **Motion with purpose**: page-load sequence, scroll-triggered reveals, hover micro-interactions — an orchestrated moment lands harder than scattered effects
5. **Spend boldness in one place**: keep everything around it quiet
6. **Match complexity to the vision**: maximalist needs elaborate execution; minimal needs precision

---

## Integration with This Project

When applying this skill in **ProyectInit**:

- **Bootstrap 5** is the primary styling system — use its utilities and components before writing custom CSS
- **Angular 22 standalone components** with `ChangeDetectionStrategy.OnPush`
- Custom CSS only when Bootstrap cannot provide it
- No inline `style="..."` attributes
- Component selector pattern: `app-<feature>-<element>`
- Follow [frontend-standards.md](../../docs/standards/frontend-standards.md) for all structural decisions

---

## Checklist Before Delivery

- [ ] Design plan written (color, type, layout)
- [ ] Real content used (no placeholders)
- [ ] Fonts inlined or system stack used
- [ ] Neutrals chosen deliberately (not defaulted)
- [ ] Layout uses flex/grid + gap
- [ ] Wide content wrapped in overflow-x: auto
- [ ] No AI-generated design clichés
- [ ] Keyboard focus visible
- [ ] `prefers-reduced-motion` respected
- [ ] Bootstrap utilities used before custom CSS (project rule)