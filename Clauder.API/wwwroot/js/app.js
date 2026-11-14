// Clauder App - Main JavaScript
const API_BASE_URL = '/api';
const APP_VERSION = '1.1.0';
let currentUser = null;
let editingRecipeId = null;

// Initialize app
document.addEventListener('DOMContentLoaded', function() {
  document.title = `Claude'r v${APP_VERSION} - Le Ricette di Claudia`;
  document.querySelectorAll('[data-app-version]').forEach(el => el.textContent = `v${APP_VERSION}`);
  loadCurrentUser();
  setupEventListeners();
  navigateToPage(getPageFromURL());
});

// Navigation
function navigateToPage(page) {
  const pages = document.querySelectorAll('[data-page]');
  pages.forEach(p => p.style.display = 'none');

  const targetPage = document.querySelector(`[data-page="${page}"]`);
  if (targetPage) {
    targetPage.style.display = 'block';
    targetPage.classList.add('animate-fade-in');
  }

  // Update bottom nav active state
  document.querySelectorAll('.bottom-nav .nav-link').forEach(link => {
    link.classList.remove('active');
  });
  document.querySelector(`[data-navigate="${page}"]`)?.classList.add('active');

  // Load page-specific data
  switch(page) {
    case 'home':
      loadRecipes();
      break;
    case 'leaderboard':
      loadLeaderboard();
      break;
    case 'profile':
      loadUserProfile();
      break;
  }

  window.history.pushState({page}, '', `?page=${page}`);
}

function getPageFromURL() {
  const params = new URLSearchParams(window.location.search);
  return params.get('page') || 'home';
}

// User Management
async function loadCurrentUser() {
  const userId = localStorage.getItem('userId');
  const userRole = localStorage.getItem('userRole');

  if (userId && userRole) {
    try {
      const response = await fetch(`${API_BASE_URL}/users/${userId}`);
      currentUser = await response.json();
      updateUIForLoggedInUser();
    } catch (error) {
      console.error('Error loading user:', error);
      showLogin();
    }
  } else {
    showLogin();
  }
}

function updateUIForLoggedInUser() {
  if (currentUser) {
    document.getElementById('userDisplayName').textContent = currentUser.displayName || currentUser.username;
    document.getElementById('loginSection').style.display = 'none';
    document.getElementById('appContent').style.display = 'block';
    document.getElementById('bottomNav').style.display = 'flex';
  }
}

function showLogin() {
  document.getElementById('loginSection').style.display = 'block';
  document.getElementById('appContent').style.display = 'none';
  document.getElementById('bottomNav').style.display = 'none';
  navigateToPage('login');
}

async function login() {
  const username = document.getElementById('loginUsername').value;
  const password = document.getElementById('loginPassword').value;

  if (!username || !password) {
    alert('Inserisci username e password');
    return;
  }

  try {
    const response = await fetch(`${API_BASE_URL}/users/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password })
    });

    if (!response.ok) throw new Error('Login failed');

    const data = await response.json();
    localStorage.setItem('userId', data.userId);
    localStorage.setItem('userRole', data.role);
    localStorage.setItem('userName', data.username);

    currentUser = { ...data, id: data.userId };
    updateUIForLoggedInUser();
    navigateToPage('home');
  } catch (error) {
    alert('Credenziali non valide');
    console.error('Login error:', error);
  }
}

function logout() {
  localStorage.clear();
  currentUser = null;
  showLogin();
}

// Recipe Management
async function loadRecipes(filter = {}) {
  try {
    let url = `${API_BASE_URL}/recipes`;
    const params = new URLSearchParams();
    if (filter.nationality) params.append('nationality', filter.nationality);
    if (filter.recipeType) params.append('recipeType', filter.recipeType);
    if (params.toString()) url += '?' + params.toString();

    const response = await fetch(url);
    const recipes = await response.json();

    const recipesContainer = document.getElementById('recipesList');
    if (recipes.length === 0) {
      recipesContainer.innerHTML = `
        <div class="empty-state">
          <div class="empty-state-icon">🍽️</div>
          <p>Nessuna ricetta trovata. Creane una!</p>
        </div>
      `;
      return;
    }

    recipesContainer.innerHTML = recipes.map(recipe => `
      <div class="col-md-4 col-sm-6 col-12 mb-3">
        <div class="card recipe-card h-100">
          ${renderRecipeCover(recipe)}
          <div class="badge-nationality">${recipe.nationality}</div>
          <div class="badge-category">${recipe.recipeType}</div>
          <div class="card-body">
            <h5 class="card-title">${recipe.title}</h5>
            <p class="card-text text-muted" style="font-size: 0.9rem;">
              ${recipe.description.substring(0, 80)}...
            </p>
            ${renderRatingStars(recipe)}
            <div class="recipe-info">
              <span>⏱️ ${recipe.prepTimeMinutes + recipe.cookTimeMinutes} min</span>
              <span>👨‍🍳 ${recipe.servingsCount} porzioni</span>
            </div>
            <button class="btn btn-primary btn-sm w-100 mt-3" onclick="viewRecipe(${recipe.id})">
              Visualizza
            </button>
          </div>
        </div>
      </div>
    `).join('');
  } catch (error) {
    console.error('Error loading recipes:', error);
  }
}

function getEmojiForsRecipeType(type) {
  const emojis = {
    'Primo': '🍝',
    'Secondo': '🍗',
    'Dolce': '🍰',
    'Antipasto': '🥗',
    'Contorno': '🥦',
    'Bevanda': '🥤'
  };
  return emojis[type] || '🍽️';
}

function renderRecipeCover(recipe) {
  const cover = recipe.mediaFiles?.find(m => !m.reviewId && m.mediaType === 'image');
  if (cover) {
    return `<img src="${cover.fileUrl}" alt="${recipe.title}" style="width: 100%; height: 200px; object-fit: cover;">`;
  }
  return `
    <div style="background: linear-gradient(135deg, #FF6B6B 0%, #FF8E72 100%); height: 200px; display: flex; align-items: center; justify-content: center; color: white; font-size: 3rem;">
      ${getEmojiForsRecipeType(recipe.recipeType)}
    </div>
  `;
}

function renderRecipeMedia(recipe) {
  const media = recipe.mediaFiles?.filter(m => !m.reviewId) || [];
  if (media.length === 0) return '';

  return `
    <div class="mb-3">
      ${media.map(m => m.mediaType === 'video'
        ? `<video src="${m.fileUrl}" controls class="w-100 rounded mb-2"></video>`
        : `<img src="${m.fileUrl}" alt="" class="w-100 rounded mb-2">`).join('')}
    </div>
  `;
}

function renderRatingStars(recipe) {
  const ratings = recipe.ratings || [];
  const average = ratings.length > 0 ? ratings.reduce((sum, r) => sum + r.score, 0) / ratings.length : 0;
  const roundedAverage = Math.round(average);

  const stars = [1, 2, 3, 4, 5].map(score => `
    <span class="star ${score <= roundedAverage ? 'active' : ''}" onclick="rateRecipe(${recipe.id}, ${score})">★</span>
  `).join('');

  return `
    <div class="star-rating">
      ${stars}
      <span class="text-muted small ms-2">${average.toFixed(1)} (${ratings.length})</span>
    </div>
  `;
}

async function rateRecipe(recipeId, score) {
  if (!currentUser) {
    alert('Accedi per votare');
    return;
  }

  try {
    const response = await fetch(`${API_BASE_URL}/ratings`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        recipeId,
        userId: currentUser.id,
        score,
        comment: ''
      })
    });

    if (!response.ok) throw new Error('Rating failed');

    await loadRecipes();
    if (document.getElementById('recipeModal').classList.contains('show')) {
      await viewRecipe(recipeId);
    }
  } catch (error) {
    console.error('Error rating recipe:', error);
    alert('Voto non registrato');
  }
}

function isRecipeOwner(recipe) {
  return currentUser !== null && recipe.createdByUserId === currentUser.id;
}

function renderOwnerActions(recipe) {
  if (!isRecipeOwner(recipe)) return '';

  const publishAction = recipe.isPublished
    ? `<button class="btn btn-outline-secondary" onclick="setRecipePublished(${recipe.id}, false)">📝 Metti in bozza</button>`
    : `<button class="btn btn-success" onclick="setRecipePublished(${recipe.id}, true)">🚀 Pubblica</button>`;

  return `
    <div class="d-flex flex-wrap gap-2 border-top pt-3">
      <button class="btn btn-primary" onclick="editRecipe(${recipe.id})">✏️ Modifica</button>
      ${publishAction}
      <button class="btn btn-danger" onclick="deleteRecipe(${recipe.id})">🗑️ Elimina</button>
    </div>
  `;
}

async function viewRecipe(recipeId) {
  try {
    const response = await fetch(`${API_BASE_URL}/recipes/${recipeId}`, {
      headers: { 'X-User-Id': String(currentUser?.id ?? '') }
    });
    const recipe = await response.json();

    const modal = new bootstrap.Modal(document.getElementById('recipeModal'));
    document.getElementById('modalRecipeTitle').textContent = recipe.isPublished ? recipe.title : `${recipe.title} (bozza)`;
    document.getElementById('modalRecipeContent').innerHTML = `
      ${renderRecipeMedia(recipe)}
      <div class="mb-3">
        <h6>Descrizione</h6>
        <p>${recipe.description}</p>
      </div>
      <div class="mb-3">
        <h6>Istruzioni</h6>
        <p>${recipe.instructions || 'Non disponibili'}</p>
      </div>
      <div class="mb-3">
        <h6>Ingredienti</h6>
        <ul>
          ${recipe.recipeIngredients?.map(ri => `
            <li>${ri.ingredient.name} - ${ri.quantity} ${ri.unit || ri.ingredient.unit || ''}</li>
          `).join('') || '<li>Nessun ingrediente</li>'}
        </ul>
      </div>
      <div class="mb-3">
        <h6>Valuta questa ricetta</h6>
        ${renderRatingStars(recipe)}
      </div>
      ${renderOwnerActions(recipe)}
    `;
    modal.show();
  } catch (error) {
    console.error('Error viewing recipe:', error);
  }
}

async function setRecipePublished(recipeId, publish) {
  const action = publish ? 'publish' : 'unpublish';

  try {
    const response = await fetch(`${API_BASE_URL}/recipes/${recipeId}/${action}`, {
      method: 'POST',
      headers: { 'X-User-Id': String(currentUser.id) }
    });
    if (!response.ok) throw new Error('Status change failed');

    await viewRecipe(recipeId);
    await loadRecipes();
    await loadMyRecipes();
  } catch (error) {
    console.error('Error changing recipe status:', error);
    alert('Operazione non riuscita');
  }
}

async function deleteRecipe(recipeId) {
  if (!confirm('Eliminare definitivamente questa ricetta?')) return;

  try {
    const response = await fetch(`${API_BASE_URL}/recipes/${recipeId}`, {
      method: 'DELETE',
      headers: { 'X-User-Id': String(currentUser.id) }
    });
    if (!response.ok) throw new Error('Delete failed');

    bootstrap.Modal.getInstance(document.getElementById('recipeModal'))?.hide();
    await loadRecipes();
    await loadMyRecipes();
  } catch (error) {
    console.error('Error deleting recipe:', error);
    alert('Eliminazione non riuscita');
  }
}

async function editRecipe(recipeId) {
  try {
    const response = await fetch(`${API_BASE_URL}/recipes/${recipeId}`, {
      headers: { 'X-User-Id': String(currentUser.id) }
    });
    const recipe = await response.json();

    document.getElementById('newRecipeTitle').value = recipe.title;
    document.getElementById('newRecipeDescription').value = recipe.description;
    document.getElementById('newRecipeInstructions').value = recipe.instructions ?? '';
    document.getElementById('newRecipeNationality').value = recipe.nationality;
    document.getElementById('newRecipeType').value = recipe.recipeType;
    document.getElementById('newRecipeServings').value = recipe.servingsCount;
    document.getElementById('newRecipePrepTime').value = recipe.prepTimeMinutes;
    document.getElementById('newRecipeCookTime').value = recipe.cookTimeMinutes;

    editingRecipeId = recipeId;
    document.getElementById('createRecipeBtn').textContent = '💾 Salva modifiche';
    bootstrap.Modal.getInstance(document.getElementById('recipeModal'))?.hide();
    navigateToPage('create-recipe');
  } catch (error) {
    console.error('Error loading recipe for edit:', error);
    alert('Impossibile aprire la ricetta');
  }
}

function resetRecipeForm() {
  editingRecipeId = null;
  document.getElementById('createRecipeForm').reset();
  document.getElementById('createRecipeBtn').textContent = '✨ Crea Ricetta';
}

async function loadMyRecipes() {
  if (!currentUser) return;

  try {
    const response = await fetch(`${API_BASE_URL}/recipes/mine`, {
      headers: { 'X-User-Id': String(currentUser.id) }
    });
    const recipes = await response.json();

    const container = document.getElementById('myRecipesList');
    if (recipes.length === 0) {
      container.innerHTML = '<p class="text-muted">Non hai ancora ricette.</p>';
      return;
    }

    container.innerHTML = recipes.map(recipe => `
      <div class="d-flex justify-content-between align-items-center border-top py-2">
        <div>
          <div class="fw-bold">${recipe.title}</div>
          <small class="text-muted">${recipe.isPublished ? '🟢 Pubblicata' : '📝 Bozza'}</small>
        </div>
        <button class="btn btn-outline-primary" onclick="viewRecipe(${recipe.id})">Apri</button>
      </div>
    `).join('');
  } catch (error) {
    console.error('Error loading my recipes:', error);
  }
}

// Leaderboard
async function loadLeaderboard() {
  try {
    const response = await fetch(`${API_BASE_URL}/ratings/leaderboard`);
    const leaderboard = await response.json();

    const leaderboardContainer = document.getElementById('leaderboardList');
    if (leaderboard.length === 0) {
      leaderboardContainer.innerHTML = `
        <div class="empty-state">
          <div class="empty-state-icon">🏆</div>
          <p>Nessuna ricetta ancora nel leaderboard</p>
        </div>
      `;
      return;
    }

    leaderboardContainer.innerHTML = leaderboard.map((item, index) => `
      <div class="leaderboard-item animate-slide-up">
        <div class="leaderboard-rank">#${index + 1}</div>
        <div class="leaderboard-content">
          <div class="leaderboard-title">${item.recipeTitle}</div>
          <div class="leaderboard-author">da ${item.createdBy}</div>
          <div class="leaderboard-stats">
            <div class="stat-item">
              ⭐ ${item.averageRating.toFixed(1)}
            </div>
            <div class="stat-item">
              👥 ${item.totalRatings} voti
            </div>
          </div>
        </div>
      </div>
    `).join('');
  } catch (error) {
    console.error('Error loading leaderboard:', error);
  }
}

// Create or update recipe
async function createRecipe() {
  const title = document.getElementById('newRecipeTitle').value;
  const description = document.getElementById('newRecipeDescription').value;
  const instructions = document.getElementById('newRecipeInstructions').value;
  const nationality = document.getElementById('newRecipeNationality').value;
  const recipeType = document.getElementById('newRecipeType').value;
  const servingsCount = parseInt(document.getElementById('newRecipeServings').value) || 4;
  const prepTimeMinutes = parseInt(document.getElementById('newRecipePrepTime').value) || 0;
  const cookTimeMinutes = parseInt(document.getElementById('newRecipeCookTime').value) || 0;

  if (!title || !description) {
    alert('Titolo e descrizione sono obbligatori');
    return;
  }

  const recipeData = {
    title,
    description,
    instructions,
    nationality,
    recipeType,
    servingsCount,
    prepTimeMinutes,
    cookTimeMinutes
  };
  const wasEditing = editingRecipeId !== null;

  try {
    let recipe;

    if (wasEditing) {
      const updateResponse = await fetch(`${API_BASE_URL}/recipes/${editingRecipeId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json', 'X-User-Id': String(currentUser.id) },
        body: JSON.stringify(recipeData)
      });
      if (!updateResponse.ok) throw new Error('Update failed');
      recipe = await updateResponse.json();
    } else {
      const response = await fetch(`${API_BASE_URL}/recipes`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ...recipeData, createdByUserId: currentUser.id })
      });
      if (!response.ok) throw new Error('Create failed');
      recipe = await response.json();

      const publishResponse = await fetch(`${API_BASE_URL}/recipes/${recipe.id}/publish`, {
        method: 'POST',
        headers: { 'X-User-Id': String(currentUser.id) }
      });
      if (!publishResponse.ok) throw new Error('Publish failed');
    }

    for (const file of document.getElementById('newRecipeMedia').files) {
      const mediaForm = new FormData();
      mediaForm.append('file', file);
      mediaForm.append('recipeId', recipe.id);
      mediaForm.append('uploadedByUserId', currentUser.id);
      const uploadResponse = await fetch(`${API_BASE_URL}/mediafiles/upload`, { method: 'POST', body: mediaForm });
      if (!uploadResponse.ok) throw new Error('Upload failed');
    }

    alert(wasEditing ? 'Ricetta aggiornata!' : 'Ricetta creata con successo!');
    resetRecipeForm();
    navigateToPage('home');
    loadRecipes();
  } catch (error) {
    console.error('Error saving recipe:', error);
    alert('Errore nel salvataggio della ricetta');
  }
}

// User Profile
function loadUserProfile() {
  if (!currentUser) return;

  document.getElementById('profileName').textContent = currentUser.displayName || currentUser.username;
  document.getElementById('profileEmail').textContent = currentUser.email;
  document.getElementById('profileRole').textContent = currentUser.role;
  loadMyRecipes();
}

// Event Listeners Setup
function setupEventListeners() {
  // Navigation
  document.querySelectorAll('[data-navigate]').forEach(link => {
    link.addEventListener('click', (e) => {
      e.preventDefault();
      navigateToPage(link.dataset.navigate);
    });
  });

  // Bottom nav navigation
  const bottomNavLinks = document.querySelectorAll('.bottom-nav [data-navigate]');
  bottomNavLinks.forEach(link => {
    link.addEventListener('click', (e) => {
      e.preventDefault();
      if (link.dataset.navigate === 'create-recipe') resetRecipeForm();
      navigateToPage(link.dataset.navigate);
    });
  });

  // Login
  document.getElementById('loginBtn')?.addEventListener('click', login);
  document.getElementById('loginPassword')?.addEventListener('keypress', (e) => {
    if (e.key === 'Enter') login();
  });

  // Logout
  document.getElementById('logoutBtn')?.addEventListener('click', logout);
  document.getElementById('profileLogoutBtn')?.addEventListener('click', logout);

  // Create Recipe
  document.getElementById('createRecipeBtn')?.addEventListener('click', createRecipe);

  // Filter recipes
  document.getElementById('filterNationality')?.addEventListener('change', () => {
    loadRecipes({
      nationality: document.getElementById('filterNationality').value,
      recipeType: document.getElementById('filterType').value
    });
  });

  document.getElementById('filterType')?.addEventListener('change', () => {
    loadRecipes({
      nationality: document.getElementById('filterNationality').value,
      recipeType: document.getElementById('filterType').value
    });
  });
}

// Utilities
function showNotification(message, type = 'success') {
  const alertDiv = document.createElement('div');
  alertDiv.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
  alertDiv.style.top = '20px';
  alertDiv.style.right = '20px';
  alertDiv.style.zIndex = '9999';
  alertDiv.innerHTML = `
    ${message}
    <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
  `;
  document.body.appendChild(alertDiv);

  setTimeout(() => {
    alertDiv.remove();
  }, 3000);
}
