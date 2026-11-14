// Clauder App - Main JavaScript
const API_BASE_URL = '/api';
let currentUser = null;

// Initialize app
document.addEventListener('DOMContentLoaded', function() {
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

    currentUser = data;
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
          <div style="background: linear-gradient(135deg, #FF6B6B 0%, #FF8E72 100%); height: 200px; display: flex; align-items: center; justify-content: center; color: white; font-size: 3rem;">
            ${getEmojiForsRecipeType(recipe.recipeType)}
          </div>
          <div class="badge-nationality">${recipe.nationality}</div>
          <div class="badge-category">${recipe.recipeType}</div>
          <div class="card-body">
            <h5 class="card-title">${recipe.title}</h5>
            <p class="card-text text-muted" style="font-size: 0.9rem;">
              ${recipe.description.substring(0, 80)}...
            </p>
            <div class="recipe-rating">
              ${'⭐'.repeat(Math.round(recipe.ratings?.length > 0 ? (recipe.ratings.reduce((a, b) => a + b.score, 0) / recipe.ratings.length) : 0))}
              <span style="font-size: 0.9rem; color: #95A5A6;">(${recipe.ratings?.length || 0})</span>
            </div>
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

async function viewRecipe(recipeId) {
  try {
    const response = await fetch(`${API_BASE_URL}/recipes/${recipeId}`);
    const recipe = await response.json();

    const modal = new bootstrap.Modal(document.getElementById('recipeModal'));
    document.getElementById('modalRecipeTitle').textContent = recipe.title;
    document.getElementById('modalRecipeContent').innerHTML = `
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
        <div class="star-rating" onclick="rateRecipe(event, ${recipeId})">
          ${'☆'.repeat(5)}
        </div>
      </div>
    `;
    modal.show();
  } catch (error) {
    console.error('Error viewing recipe:', error);
  }
}

async function rateRecipe(event, recipeId) {
  if (event.target.classList.contains('star')) {
    const stars = event.currentTarget.querySelectorAll('.star');
    const rating = Array.from(stars).indexOf(event.target) + 1;

    try {
      const response = await fetch(`${API_BASE_URL}/ratings`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          recipeId,
          userId: currentUser.id,
          score: rating,
          comment: ''
        })
      });

      if (response.ok) {
        alert(`Grazie! Hai dato ${rating} stelle!`);
        loadRecipes();
      }
    } catch (error) {
      console.error('Error rating recipe:', error);
    }
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

// Create Recipe
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

  try {
    const response = await fetch(`${API_BASE_URL}/recipes`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        title,
        description,
        instructions,
        nationality,
        recipeType,
        servingsCount,
        prepTimeMinutes,
        cookTimeMinutes,
        createdByUserId: currentUser.id
      })
    });

    if (response.ok) {
      alert('Ricetta creata con successo!');
      // Reset form
      document.getElementById('createRecipeForm').reset();
      navigateToPage('home');
      loadRecipes();
    }
  } catch (error) {
    console.error('Error creating recipe:', error);
    alert('Errore nella creazione della ricetta');
  }
}

// User Profile
function loadUserProfile() {
  if (!currentUser) return;

  document.getElementById('profileName').textContent = currentUser.displayName || currentUser.username;
  document.getElementById('profileEmail').textContent = currentUser.email;
  document.getElementById('profileRole').textContent = currentUser.role;
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
