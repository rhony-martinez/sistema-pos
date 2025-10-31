﻿// ==================== VALIDACIONES GLOBALES ====================
const RESERVED_WORDS = ["SELECT", "INSERT", "UPDATE", "DELETE", "FROM", "WHERE", "GROUP BY", "HAVING", "ORDER BY", "DROP", "CREATE", "ALTER", "TRUNCATE", "EXEC", "UNION", "ALL", "AND", "OR", "NOT", "NULL", "JOIN", "INNER", "LEFT", "RIGHT", "ON", "AS", "INTO", "VALUES", "SET"];
const MAX_LENGTH = 15;
const MAX_EMAIL_LENGTH = 25;

/**
 * Muestra mensajes en el modal principal del sistema POS
 */
function showModal(message, callback) {
  const modal = document.getElementById("modal");
  const modalText = document.getElementById("modal-text");
  const modalBtn = document.getElementById("modal-btn");

  if (!modal || !modalText || !modalBtn) {
    alert(message);
    if (callback) callback();
    return;
  }

  modalText.textContent = message;
  modal.style.display = "flex";
  modalBtn.onclick = () => {
    modal.style.display = "none";
    if (callback) callback();
  };
}

/**
 * Aplica validaciones globales a todos los <input> del documento.
 */
function aplicarValidacionesGlobales() {
  document.querySelectorAll("input").forEach(input => {
    input.addEventListener("input", (e) => {
      const field = e.target;
      let value = field.value;

        //  No permitir espacio al inicio
      if (/^\s/.test(value)) {
        field.classList.add("input-error");
        field.value = value.trimStart(); // elimina espacios del inicio
        showModal("No se permiten espacios en blanco al inicio.");
        return;
      }

      //  Limitar longitud
      const limit = field.id === "correo" ? MAX_EMAIL_LENGTH : MAX_LENGTH;
      if (value.length > limit) {
        field.value = value.slice(0, limit);
        showModal(`Máximo ${limit} caracteres permitidos.`);
        return;
      }
   

        //  El correo no puede tener ningún espacio (ni al inicio, ni en medio, ni al final)
      if (field.id === "correo") {
        if (/\s/.test(value)) {
          field.classList.add("input-error");
          field.value = value.replace(/\s+/g, ""); // elimina todos los espacios
          showModal("El correo no puede contener espacios en blanco.");
          return;
        }
      }

      //  No permitir dos espacios seguidos
      if (/\s{2,}/.test(value)) {
        field.classList.add("input-error");
        field.value = value.replace(/\s{2,}/g, " ");
        showModal("No se permiten más de un espacio en blanco seguido.");
        return;
      }

      //  Bloquear espacios en campos específicos
      if (["usuId", "sedeId", "username","telefono","correo"].includes(field.id)) {
        if (/\s/.test(value)) {
          field.classList.add("input-error");
          field.value = value.replace(/\s/g, "");
          showModal("No se permiten espacios en este campo.");
          return;
        }
      }

      

      //  Bloquear caracteres especiales excepto en correo y contraseña
      if (field.id !== "correo" && field.id !== "password") {
        if (/[^a-zA-Z0-9\s]/.test(value)) {
          field.classList.add("input-error");
          field.value = value.replace(/[^a-zA-Z0-9\s]/g, "");
          showModal("Tipo de dato invalido.");
          return;
        }
      }



      //  Bloquear palabras reservadas SQL
      for (let word of RESERVED_WORDS) {
        const regex = new RegExp(`\\b${word}\\b`, "i");
        if (regex.test(value)) {
          field.classList.add("input-error");
          field.value = value.replace(regex, "");
          showModal(`No se permite usar la palabra reservada: ${word}`);
          return;
        }
      }
      
        const telefonoInput = document.getElementById("telefono");
    telefonoInput.addEventListener("input", (e) => {
        const field = e.target;
        let value = field.value;

        // Si contiene algo que no sea número → eliminarlo
        if (/[^0-9]/.test(value)) {
            field.value = value.replace(/[^0-9]/g, "");
            field.classList.add("input-error");
            showModal("Solo se permiten números en el teléfono.");
            return;
        } else {
            field.classList.remove("input-error");
        }
    });  


      field.classList.remove("input-error");
    });
  });
}