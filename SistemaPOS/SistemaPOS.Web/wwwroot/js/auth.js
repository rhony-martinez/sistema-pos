// === LOGIN ===
document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("loginForm");
    if (!form) return; // Evita errores en páginas sin login

    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        const username = document.getElementById("usuario").value.trim();
        const password = document.getElementById("contraseña").value.trim();

        if (!username || !password) {
            alert("Por favor, completa todos los campos.");
            return;
        }

        try {
            const response = await fetch("http://localhost:5289/api/auth/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, password })
            });

            if (!response.ok) {
                alert("Credenciales incorrectas o error en el servidor.");
                return;
            }

            const data = await response.json();
            console.log("✅ Token recibido:", data.accessToken);

            // Guardar token y expiración
            sessionStorage.setItem("token", data.accessToken);
            sessionStorage.setItem("expiresAt", data.expiresAt);

            // Obtener rol desde el token
            const payload = JSON.parse(atob(data.accessToken.split(".")[1]));
            const userRole = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

            // Redirigir según el rol
            switch (userRole) {
                case "ADMIN_GENERAL":
                    window.location.href = "dashboard-adm-gral.html";
                    break;
                case "ADMIN_LOCAL":
                    window.location.href = "dashboard-adm-local.html";
                    break;
                case "CAJERO":
                    window.location.href = "gestion-caja.html";
                    break;
                default:
                    alert("Rol no reconocido.");
            }
        } catch (error) {
            console.error("❌ Error de conexión:", error);
            alert("Error de conexión con el servidor.");
        }
    });
});

// === CHECK AUTH ===
function checkAuth() {
    const token = sessionStorage.getItem("token");
    const expiresAt = sessionStorage.getItem("expiresAt");

    if (!token || !expiresAt || new Date(expiresAt) < new Date()) {
        alert("Sesión expirada. Por favor inicia sesión nuevamente.");
        sessionStorage.clear();
        window.location.href = "index.html";
        return false;
    }
    return true;
}

// === FETCH AUTORIZADO ===
async function authorizedFetch(url, options = {}) {
    const token = sessionStorage.getItem("token");
    const headers = {
        ...options.headers,
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json"
    };
    return fetch(url, { ...options, headers });
}

// === PERFIL DEL USUARIO ===
async function getUserProfile() {
    const token = sessionStorage.getItem("token");
    if (!token) return null;

    try {
        const res = await fetch("http://localhost:5289/api/Users/me", {
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            }
        });

        if (!res.ok) throw new Error("No autorizado");
        const data = await res.json();
        console.log("📋 Datos del usuario:", data);
        return data; // devuelve directamente lo que responde el backend
    } catch (err) {
        console.error("❌ Error al obtener perfil:", err);
        return null;
    }
}

