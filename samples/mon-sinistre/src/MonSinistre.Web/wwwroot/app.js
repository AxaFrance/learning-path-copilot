const colonnes = ['numero', 'numeroContrat', 'formule', 'type', 'dateSurvenance', 'dateDeclaration', 'dansLesDelais', 'montantEstime', 'indemnisation', 'statut'];

async function chargerSinistres() {
  const reponse = await fetch('/api/sinistres');
  const sinistres = await reponse.json();
  const corps = document.getElementById('sinistres');
  corps.replaceChildren();
  for (const sinistre of sinistres) {
    const ligne = document.createElement('tr');
    for (const colonne of colonnes) {
      const cellule = document.createElement('td');
      const valeur = sinistre[colonne];
      cellule.textContent = typeof valeur === 'boolean' ? (valeur ? 'oui' : 'non') : valeur;
      ligne.appendChild(cellule);
    }
    corps.appendChild(ligne);
  }
}

document.getElementById('declaration').addEventListener('submit', async (evenement) => {
  evenement.preventDefault();
  const donnees = new FormData(evenement.target);
  const message = document.getElementById('message');
  const reponse = await fetch('/api/sinistres', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      numeroContrat: donnees.get('numeroContrat'),
      type: donnees.get('type'),
      dateSurvenance: donnees.get('dateSurvenance'),
      montantEstimeEuros: Number(donnees.get('montantEstimeEuros')),
    }),
  });
  message.textContent = reponse.ok ? 'Sinistre déclaré.' : `Erreur : ${await reponse.text()}`;
  if (reponse.ok) {
    evenement.target.reset();
    await chargerSinistres();
  }
});

chargerSinistres();
