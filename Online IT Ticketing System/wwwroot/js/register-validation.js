document.addEventListener("DOMContentLoaded", function () {
    const states = {
        Kerala: ["Kochi", "Thiruvananthapuram", "Kozhikode"],
        Karnataka: ["Bengaluru", "Mysuru", "Mangaluru"],
        TamilNadu: ["Chennai", "Coimbatore", "Madurai"],
    };

    const stateDropdown = document.getElementById("stateDropdown");
    const cityDropdown = document.getElementById("cityDropdown");

    // Populate states
    for (const state in states) {
        const option = document.createElement("option");
        option.value = state;
        option.textContent = state;
        stateDropdown.appendChild(option);
    }

    // Populate cities based on state selection
    stateDropdown.addEventListener("change", function () {
        const selectedState = this.value;
        cityDropdown.innerHTML = '<option value="">Select City</option>';
        if (selectedState && states[selectedState]) {
            states[selectedState].forEach((city) => {
                const option = document.createElement("option");
                option.value = city;
                option.textContent = city;
                cityDropdown.appendChild(option);
            });
        }
    });

    // Add real-time validation
    function validateField(input, validator, errorMessage) {
        const errorSpan = input.nextElementSibling;
        if (validator(input.value)) {
            input.classList.remove("is-invalid");
            input.classList.add("is-valid");
            errorSpan.textContent = "";
        } else {
            input.classList.add("is-invalid");
            input.classList.remove("is-valid");
            errorSpan.textContent = errorMessage;
        }
    }

    // Validators
    const validators = {
        firstName: (value) => /^[A-Za-z]+$/.test(value),
        lastName: (value) => /^[A-Za-z]+$/.test(value),
        phoneNumber: (value) => /^[6-9]\d{9}$/.test(value),
        email: (value) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value),
        dateOfBirth: (value) => {
            const dob = new Date(value);
            const today = new Date();
            const age = today.getFullYear() - dob.getFullYear();
            const monthDifference = today.getMonth() - dob.getMonth();
            const dayDifference = today.getDate() - dob.getDate();

            // Check if the user is at least 18 years old
            if (age > 18 || (age === 18 && (monthDifference > 0 || (monthDifference === 0 && dayDifference >= 0)))) {
                return true; // User is 18 or older
            }
            return false; // User is under 18
        },
        username: (value) => value.trim().length >= 5,
        password: (value) => value.trim().length >= 6,
        confirmPassword: (value, form) => value === form.password.value,
    };

    // Bind validation to inputs
    const form = document.getElementById("signupForm");
    form.querySelectorAll("input").forEach((input) => {
        const id = input.id;

        if (validators[id]) {
            input.addEventListener("input", () => validateField(input, validators[id], getErrorMessage(id)));
            input.addEventListener("blur", () => validateField(input, validators[id], getErrorMessage(id)));
        }
    });

    // Bind confirmPassword separately as it depends on another field
    const confirmPassword = document.getElementById("confirmPassword");
    confirmPassword.addEventListener("input", () =>
        validateField(confirmPassword, (value) => validators.confirmPassword(value, form), "Passwords do not match.")
    );

    confirmPassword.addEventListener("blur", () =>
        validateField(confirmPassword, (value) => validators.confirmPassword(value, form), "Passwords do not match.")
    );

    // Get error messages dynamically
    function getErrorMessage(field) {
        const messages = {
            firstName: "First name must contain only alphabets.",
            lastName: "Last name must contain only alphabets.",
            phoneNumber: "Enter a Valid Phone number",
            email: "Enter a valid email address.",
            dateOfBirth: "You must be at least 18 years old.",
            username: "Username must be at least 5 characters.",
            password: "Password must be at least 6 characters.",
            confirmPassword: "Passwords do not match.",
        };
        return messages[field] || "This field is required.";
    }

    // Prevent submission if any field is invalid
    form.addEventListener("submit", function (e) {
        let isValid = true;

        form.querySelectorAll("input").forEach((input) => {
            const id = input.id;
            if (validators[id]) {
                const valid = validators[id](input.value, form);
                validateField(input, validators[id], getErrorMessage(id));
                if (!valid) isValid = false;
            }
        });

        if (!isValid) {
            e.preventDefault(); // Stop form submission
        }
    });
});
