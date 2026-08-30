describe('Dashboard', () => {
  it('loads the dashboard with stat cards and modules', () => {
    cy.visit('/');

    cy.contains('h1', 'Dashboard').should('be.visible');

    cy.get('.stat-card').should('have.length', 4);

    cy.contains('.mod-item', 'Gestión de Proyectos').should('be.visible');
    cy.contains('.mod-item', 'Catálogo de Servicios').should('be.visible');
  });

  it('navigates to the Proyectos placeholder when clicking its module card', () => {
    cy.visit('/');

    cy.contains('[data-testid="modulo-card"]', 'Gestión de Proyectos').click();

    cy.url().should('include', '/proyectos');
    cy.contains('h1', 'Gestión de Proyectos').should('be.visible');
    cy.contains('Esta sección está en construcción.').should('be.visible');
  });
});
