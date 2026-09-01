describe('Applications', () => {
  it('creates a project, an application, and an environment, then verifies the detail page', () => {
    const projectName = `Test Project App ${Date.now()}`;
    const appName = `CRM ${Date.now()}`;

    cy.visit('/proyectos');
    cy.contains('a', 'Nuevo Proyecto').click();
    cy.get('[data-testid="input-nombre"]').type(projectName);
    cy.get('[data-testid="btn-guardar"]').click();
    cy.url().should('match', /\/proyectos\/\d+$/);

    cy.get('[data-testid="btn-nueva-aplicacion"]').click();
    cy.get('[data-testid="input-nombre"]').type(appName);
    cy.get('[data-testid="btn-guardar"]').click();

    cy.url().should('match', /\/proyectos\/aplicaciones\/\d+$/);
    cy.contains('h1', appName).should('be.visible');

    cy.get('[data-testid="btn-agregar-ambiente"]').click();
    cy.get('[data-testid="modal-input-nombre"]').type('UAT');
    cy.get('[data-testid="modal-input-url"]').type('https://uat.example.com');
    cy.get('[data-testid="modal-btn-guardar"]').click();
    cy.contains('UAT').should('be.visible');

    cy.visit('/proyectos');
    cy.contains(projectName).click();
    cy.contains(appName).should('be.visible');
    cy.contains(appName).click();
    cy.url().should('match', /\/proyectos\/aplicaciones\/\d+$/);
    cy.contains('UAT').should('be.visible');
  });
});
