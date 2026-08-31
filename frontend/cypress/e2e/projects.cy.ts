describe('Projects', () => {
  it('creates, views, edits, and deletes a project', () => {
    const projectName = `Test Project ${Date.now()}`;
    const updatedName = `${projectName} Updated`;

    cy.visit('/proyectos');
    cy.contains('a', 'Nuevo Proyecto').click();

    cy.get('[data-testid="input-nombre"]').type(projectName);
    cy.get('[data-testid="btn-guardar"]').click();

    cy.url().should('match', /\/proyectos\/\d+$/);
    cy.contains('h1', projectName).should('be.visible');

    cy.get('[data-testid="btn-agregar-bd"]').click();
    cy.get('[data-testid="modal-input-nombre"]').type('SalesDb');
    cy.get('[data-testid="modal-btn-guardar"]').click();
    cy.contains('SalesDb').should('be.visible');

    cy.contains('a', 'Editar').click();
    cy.get('[data-testid="input-nombre"]').clear().type(updatedName);
    cy.get('[data-testid="btn-guardar"]').click();
    cy.contains('h1', updatedName).should('be.visible');

    cy.visit('/proyectos');
    cy.contains(updatedName).should('be.visible');

    cy.contains('.project-card', updatedName).click();
    cy.url().should('match', /\/proyectos\/\d+$/);
    cy.url().then((detailUrl) => {
      const projectId = detailUrl.split('/').pop();
      cy.request('DELETE', `/api/projects/${projectId}`);
    });

    cy.visit('/proyectos');
    cy.contains(updatedName).should('not.exist');
  });
});
