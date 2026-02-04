/**
 * validation
 */


document.addEventListener("DOMContentLoaded", function () {

    const form = document.getElementById("loaginForm");
    const nameInput = document.getElementById("name");
    const emailInput = document.getElementById("email");

    form.addEventListener("submit", function (e) {

        let isValid = true;

        // Reset previous errors
        nameInput.classList.remove("is-invalid");
        emailInput.classList.remove("is-invalid");

        // Check empty name
        if (nameInput.value.trim() === "") {
            nameInput.classList.add("is-invalid");
            isValid = false;
        }

        // Check empty email
        if (emailInput.value.trim() === "") {
            emailInput.classList.add("is-invalid");
            isValid = false;
        } else {
            // Email regex
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

            if (!emailRegex.test(emailInput.value.trim())) {
                emailInput.classList.add("is-invalid");
                isValid = false;
            }
        }

        if (!isValid) {
            e.preventDefault();
        }
    });
});



