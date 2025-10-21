document.addEventListener("DOMContentLoaded", () => {
    const logoutBtn = document.getElementById("logoutBtn");
    if (logoutBtn) {
        logoutBtn.addEventListener("click", async () => {
            const token = sessionStorage.getItem("token");
            if (!token) return;

            await fetch("http://localhost:5289/api/auth/logout", {
                method: "POST",
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            });

            sessionStorage.clear();
            window.location.href = "index.html";
        });
    }
});
