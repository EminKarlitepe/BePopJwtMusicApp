const searchInput = document.getElementById('userSearchInput');
const resultsBox = document.getElementById('userSearchResults');
let debounce;

searchInput.addEventListener('input', function () {
    clearTimeout(debounce);
    const q = this.value.trim();
    if (q.length < 2) { resultsBox.style.display = 'none'; return; }
    debounce = setTimeout(() => fetchUsers(q), 300);
});

document.addEventListener('click', function (e) {
    if (!searchInput.contains(e.target) && !resultsBox.contains(e.target))
        resultsBox.style.display = 'none';
});

function fetchUsers(q) {
    fetch('/Account/SearchUsers?q=' + encodeURIComponent(q))
        .then(r => r.json())
        .then(data => {
            resultsBox.innerHTML = '';
            if (!data.length) {
                resultsBox.innerHTML = '<div class="user-search-empty">Kullanıcı bulunamadı.</div>';
            } else {
                data.forEach(u => {
                    const avatar = u.profileImageUrl
                        ? `background-image:url('${u.profileImageUrl}')`
                        : 'background-color:#333';
                    const sub = u.country ? u.country : '@' + u.username;
                    resultsBox.innerHTML += `
                        <a href="/Account/UserProfile?id=${u.userId}" class="user-search-item">
                            <div class="user-search-avatar" style="${avatar}"></div>
                            <div>
                                <div class="user-search-name">${u.displayName}</div>
                                <div class="user-search-sub">${sub}</div>
                            </div>
                        </a>`;
                });
            }
            resultsBox.style.display = 'block';
        });
}